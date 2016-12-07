using CrawlerLibrary.Models;
using HtmlAgilityPack;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Crawler
{
    public class Crawler
    {
        private CrawlerContext ctx;

        public readonly BenchMarker LoopBenchMarker = new BenchMarker(100);

        private string _currentHtml;
        public Page CurrentPage { get; set; }

        public int ContentTagCount { get; private set; }
        public int CurrentContentTagIndex { get; private set; }
        public int LinkTagCount { get; private set; }
        public int CurrentLinkTagIndex { get; private set; }

        private long TotalLinkTagsFound { get; set; }

        public int LinksCrawled { get; private set; }

        public int TotalErrors { get; private set; }

        public int NoFollows { get; private set; }
        public int NoIndex { get; private set; }

        public Crawler()
        {
            Reset();
        }

        public Page GetNextPage()
        {
            string query = @"
				UPDATE TOP (1) Pages
				SET LastAttempt = GETDATE()
				OUTPUT
					inserted.id,
					inserted.url,
					inserted.title,
					inserted.LastAttempt,
					inserted.scanned
				FROM Pages
				LEFT JOIN (
					SELECT Errors.Page_id, COUNT(Errors.id) AS ErrorCount
					FROM Errors
					GROUP BY Page_id
				) AS E ON E.Page_id = Pages.id

				WHERE
					Pages.scanned = 0
					AND
					(DATEDIFF(s, '1970-01-01 00:00:00', LastAttempt) <= DATEDIFF(s, '1970-01-01 00:00:00', GETDATE()) - 60*60 OR LastAttempt IS NULL)
					AND (E.ErrorCount < 2 OR E.ErrorCount IS NULL)
				";
            return ctx.Pages.SqlQuery(query).Single();
        }

        public void Start()
        {
            while (true)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                try
                {
                    CurrentPage = GetNextPage();

                    using (DbContextTransaction scope = ctx.Database.BeginTransaction())
                    {
                        CrawlPage(CurrentPage);

                        CurrentPage.scanned = true;
                        ctx.Entry(CurrentPage).State = EntityState.Modified;
                        ctx.SaveChanges();

                        scope.Commit();
                    }

                    stopwatch.Stop();
                    LoopBenchMarker.Insert(stopwatch.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    int pageId = CurrentPage.id;

                    Reset();

                    Error error = new Error { error = e.Message + "\n" + e.StackTrace, Page_id = pageId };
                    ctx.Entry(error).State = EntityState.Added;
                    ctx.SaveChanges();
                    TotalErrors++;
                }
                Reset();
            }
        }

        private void Reset()
        {
            if (ctx != null)
                ctx.Dispose();
            ctx = new CrawlerContext();
            ctx.Configuration.AutoDetectChangesEnabled = false;

            ContentTagCount = 0;
            CurrentContentTagIndex = 0;
            LinkTagCount = 0;
            CurrentLinkTagIndex = 0;
            CurrentPage = null;
            _currentHtml = "";
        }

        private void CrawlPage(Page currentPage)
        {
            string html;
            using (var client = new WebClient())
            {
                Uri uri = new Uri(currentPage.url);
                try
                {
                    _currentHtml = client.DownloadString(uri);
                    html = _currentHtml;
                }
                catch (WebException)
                {
                    return;
                }
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Allow search engines robots to index the page, you don’t have to add this to your pages, as it’s the default.
            bool index = true;
            // Tells the search engines robots to follow the links on the page, whether it can index it or not.
            bool follow = true;

            try
            {
                HtmlNode node = doc.DocumentNode.SelectSingleNode("//meta[@name='robots']");
                if (node != null)
                {
                    string content = node.Attributes["content"].Value;
                    if (content.ToLower().Contains("nofollow"))
                        follow = false;

                    if (content.ToLower().Contains("noindex"))
                        index = false;

                    if (content.ToLower().Contains("none"))
                    {
                        index = false;
                        follow = false;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            if (!follow)
                NoFollows++;

            if (!index)
                NoIndex++;

            if (!follow && !index)
                return;

            try
            {
                HtmlNode node = doc.DocumentNode.SelectSingleNode("//link[@rel='canonical']");

                if (node != null)
                {
                    string href = node.Attributes["href"].Value;
                    if (href != currentPage.url)
                    {
                        AddOrGetPage(href);
                        return;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            if (index)
            {
                ISolrOperations<HTMLContent> solr = ServiceLocator.Current.GetInstance<ISolrOperations<HTMLContent>>();
                string title = doc.DocumentNode.SelectSingleNode("//title").InnerText;

                solr.Add(
                    new HTMLContent
                    {
                        ID = currentPage.id.ToString(),
                        Title = title,
                        URL = currentPage.url,
                        P = GetContent(doc, "//p[text()]"),
                        H1 = GetContent(doc, "//h1[text()]"),
                        H2 = GetContent(doc, "//h2[text()]"),
                        H3 = GetContent(doc, "//h3[text()]")
                    },
                    new AddParameters
                    {
                        CommitWithin = 5000
                    });
            }

            if (follow)
            {
                List<Link> linkList = GetLinks(doc);
                foreach (Link l in linkList)
                {
                    ctx.Entry(l).State = EntityState.Added;
                }
                ctx.SaveChanges();
                TotalLinkTagsFound += linkList.Count;
            }

            LinksCrawled++;
        }

        private List<string> GetContent(HtmlDocument doc, string XPath)
        {
            List<string> l = new List<string>();
            HtmlNodeCollection contentNodeCollection = doc.DocumentNode.SelectNodes(XPath);
            if (contentNodeCollection != null)
            {
                foreach (HtmlNode node in contentNodeCollection)
                {
                    string content = node.InnerText.Trim();
                    if (content.Length > 0)
                        l.Add(content);
                }
            }
            return l;
        }

        private List<Content> GetContent(HtmlDocument doc)
        {
            List<Content> contentList = new List<Content>();

            HtmlNodeCollection contentNodeCollection = doc.DocumentNode.SelectNodes("(//h1|//h2|//h3|//h4|//h5|//h6|//p)[text()]");
            if (contentNodeCollection != null)
            {
                ContentTagCount = contentNodeCollection.Count;
                int i = 1;
                foreach (HtmlNode node in contentNodeCollection)
                {
                    CurrentContentTagIndex = i++;
                    string content = node.InnerText.Trim();
                    if (content.Length > 0)
                    {
                        int index = 0;
                        do
                        {
                            int max = 800;
                            int len = (content.Length - index) % max;

                            int offset = len;
                            if (!(len < max))
                                offset = content.Substring(index, len).LastIndexOf(' ');

                            string tmpContent = content.Substring(index, offset);
                            contentList.Add(new Content
                            {
                                page_id = CurrentPage.id,
                                tag = node.OriginalName.Trim(),
                                text = tmpContent
                            });

                            index += offset;
                        } while (content.Length < index && content.Length > 800);
                    }
                }
            }

            return contentList;
        }

        private List<Link> GetLinks(HtmlDocument doc)
        {
            List<Link> linkList = new List<Link>();

            HtmlNodeCollection linkNodeCollection = doc.DocumentNode.SelectNodes("//a[@href and text()]");
            if (linkNodeCollection != null)
            {
                LinkTagCount = linkNodeCollection.Count;

                int i = 1;
                foreach (HtmlNode node in linkNodeCollection)
                {
                    CurrentLinkTagIndex = i++;

                    HtmlAttribute att = node.Attributes["href"];

                    string foundLink = att.Value;
                    string linkText = node.InnerText.Trim();

                    if (string.IsNullOrEmpty(linkText))
                        continue;

                    bool internalLink = false;
                    try
                    {
                        foundLink = FixLink(CurrentPage.url, foundLink, ref internalLink);
                    }
                    catch (Exception e)
                    {
                        if (e.Message == "Skip.")
                            continue;
                        throw;
                    }
                    AddOrGetPage(foundLink);
                }
            }
            return linkList;
        }

        private string FixLink(string currentLink, string foundLink, ref bool internalLink)
        {
            Uri uri = new Uri(currentLink);

            if (foundLink.StartsWith("//"))
            {
                foundLink = uri.Scheme + "://" + uri.Authority + foundLink.Substring(1);
            }
            else if (foundLink.StartsWith("/"))
            {
                // is internal
                internalLink = true;
                foundLink = uri.GetLeftPart(UriPartial.Authority) + foundLink;
            }
            else if (foundLink.StartsWith("?"))
            {
                // is internal
                internalLink = true;
                foundLink = uri.GetLeftPart(UriPartial.Path) + foundLink;
            }
            else
            {
                throw new Exception("Skip.");
            }

            if (foundLink.Contains('#'))
            {
                foundLink = foundLink.Substring(0, foundLink.IndexOf('#'));
            }

            return foundLink;
        }

        private Page AddOrGetPage(string foundLink)
        {
            using (var c = new CrawlerContext())
            {
                c.Database.ExecuteSqlCommand(string.Format(@"
					declare @url varchar(500) = '{0}';

					IF NOT EXISTS (SELECT TOP 1 * FROM Pages WHERE (url = @url))
					BEGIN
						INSERT INTO Pages(url, scanned)
						Values(@url, 0)
					END
				", foundLink));
            }
            return new Page();
        }
    }
}