using CrawlerLibrary.Models;
using HtmlAgilityPack;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Crawler {

    public class Crawler {
        private CrawlerContext ctx;

        public readonly BenchMarker LoopBenchMarker = new BenchMarker(100);

        private string currentHTML;
        public Page CurrentPage { get; set; }

        public int ContentTagCount { get; private set; }
        public int CurrentContentTagIndex { get; private set; }
        public int LinkTagCount { get; private set; }
        public int CurrentLinkTagIndex { get; private set; }

        public long TotalContentTagsFound { get; private set; }
        public long TotalLinkTagsFound { get; private set; }

        public int LinksCrawled { get; private set; } = 0;

        public int TotalErrors { get; private set; }

        public int NoFollows { get; private set; } = 0;
        public int NoIndex { get; private set; } = 0;

        public Crawler() {
            this.Reset();
        }

        public Page GetNextPage() {
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
            return this.ctx.Pages.SqlQuery(query).Single();
        }

        public void Start() {
            while(true) {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                try {
                    this.CurrentPage = this.GetNextPage();

                    using(DbContextTransaction scope = this.ctx.Database.BeginTransaction()) {
                        this.CrawlPage(this.CurrentPage);

                        this.CurrentPage.scanned = true;
                        ctx.Entry(this.CurrentPage).State = EntityState.Modified;
                        ctx.SaveChanges();

                        scope.Commit();
                    }

                    stopwatch.Stop();
                    this.LoopBenchMarker.Insert(stopwatch.ElapsedMilliseconds);
                } catch(Exception e) {
                    int pageID = this.CurrentPage.id;

                    this.Reset();

                    Error error = new Error() { error = e.Message + "\n" + e.StackTrace, Page_id = pageID };
                    ctx.Entry(error).State = EntityState.Added;
                    ctx.SaveChanges();
                    this.TotalErrors++;
                    //Console.WriteLine(e.Message);
                    //Console.WriteLine(e.StackTrace);
                }
                //this.LinksCrawled++;
                this.Reset();
            }
        }

        private void Reset() {
            if(this.ctx != null)
                this.ctx.Dispose();
            this.ctx = new CrawlerContext();
            this.ctx.Configuration.AutoDetectChangesEnabled = false;

            this.ContentTagCount = 0;
            this.CurrentContentTagIndex = 0;
            this.LinkTagCount = 0;
            this.CurrentLinkTagIndex = 0;
            this.CurrentPage = null;
            this.currentHTML = "";
        }

        private void CrawlPage(Page currentPage) {
            string HTML;
            using(var client = new WebClient()) {
                Uri uri = new Uri(currentPage.url);
                try {
                    this.currentHTML = client.DownloadString(uri);
                    HTML = this.currentHTML;
                } catch(WebException e) {
                    return;
                }
            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(HTML);

            // Allow search engines robots to index the page, you don’t have to add this to your pages, as it’s the default.
            bool index = true;
            // Tells the search engines robots to follow the links on the page, whether it can index it or not.
            bool follow = true;

            try {
                HtmlNode node = doc.DocumentNode.SelectSingleNode("//meta[@name='robots']");
                if(node != null) {
                    string content = node.Attributes["content"].Value;
                    if(content.ToLower().Contains("nofollow"))
                        follow = false;

                    if(content.ToLower().Contains("noindex"))
                        index = false;

                    if(content.ToLower().Contains("none")) {
                        index = false;
                        follow = false;
                    }
                }
            } catch(Exception) { }

            if(!follow)
                this.NoFollows++;

            if(!index)
                this.NoIndex++;

            if(!follow && !index)
                return;

            try {
                HtmlNode node = doc.DocumentNode.SelectSingleNode("//link[@rel='canonical']");

                if(node != null) {
                    string href = node.Attributes["href"].Value;
                    if(href != currentPage.url) {
                        this.addOrGetPage(href);
                        return;
                    }
                }
            } catch(Exception) { }

            if(index) {
                string tHTML = currentHTML;
                ISolrOperations<HTMLContent> solr = ServiceLocator.Current.GetInstance<ISolrOperations<HTMLContent>>();

                /*MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(tHTML));
                ExtractResponse extractResponse = solr.Extract(new ExtractParameters(ms, currentPage.id.ToString(), currentPage.url) {
                    AutoCommit = true,
                    Capture = "p",
                    CaptureAttributes = false,
                    //DefaultField = "text",
                    StreamType = "text/html",
                    //ExtractFormat = ExtractFormat.Text,
                    XPath = "/xhtml:html/xhtml:body/xhtml:div/descendant:node()",
                });*/

                //solr.BuildSpellCheckDictionary();
                //solr.Optimize();

                string title = doc.DocumentNode.SelectSingleNode("//title").InnerText;
                //this.updateTitle(title);

                solr.Add(
                    new HTMLContent() {
                        ID = currentPage.id.ToString(),
                        Title = title,
                        URL = currentPage.url,
                        P = this.GetContent(doc, "//p[text()]"),
                        H1 = this.GetContent(doc, "//h1[text()]"),
                        H2 = this.GetContent(doc, "//h2[text()]"),
                        H3 = this.GetContent(doc, "//h3[text()]")
                    },
                    new AddParameters() {
                        CommitWithin = 200
                    });
                solr.Commit();
            }
            /*List<Content> contentList = this.GetContent(doc);

            foreach(Content c in contentList) {
                this.ctx.Entry(c).State = EntityState.Added;
            }
            this.ctx.SaveChanges();
            this.TotalContentTagsFound += contentList.Count;*/

            if(follow) {
                /*
                List<Link> linkList = this.GetLinks(currentPage, doc);
                foreach(Link l in linkList) {
                    this.ctx.Entry(l).State = EntityState.Added;
                }
                this.ctx.SaveChanges();
                this.TotalLinkTagsFound += linkList.Count;
                */
            }

            this.LinksCrawled++;
        }

        private void updateTitle(string title) {
            using(var ctx = new CrawlerContext()) {
                ctx.Pages.Attach(this.CurrentPage);
                this.CurrentPage.title = title;
                //ctx.Entry(this.CurrentPage).State = EntityState.Modified;
                ctx.SaveChanges();
            }
        }

        private List<string> GetContent(HtmlAgilityPack.HtmlDocument doc, string XPath) {
            List<string> l = new List<string>();

            HtmlNodeCollection contentNodeCollection = doc.DocumentNode.SelectNodes(XPath);
            if(contentNodeCollection != null) {
                foreach(HtmlNode node in contentNodeCollection) {
                    string content = node.InnerText.Trim();
                    if(content.Length > 0)
                        l.Add(content);
                }
            }

            return l;
        }

        private List<Content> GetContent(HtmlAgilityPack.HtmlDocument doc) {
            List<Content> contentList = new List<Content>();

            HtmlNodeCollection contentNodeCollection = doc.DocumentNode.SelectNodes("(//h1|//h2|//h3|//h4|//h5|//h6|//p)[text()]");
            if(contentNodeCollection != null) {
                this.ContentTagCount = contentNodeCollection.Count;

                int i = 1;
                foreach(HtmlNode node in contentNodeCollection) {
                    this.CurrentContentTagIndex = i++;

                    string content = node.InnerText.Trim();

                    if(content.Length > 0) {
                        int index = 0;
                        do {
                            int max = 800;
                            int len = (content.Length - index) % max;

                            int offset = len;
                            if(!(len < max))
                                offset = content.Substring(index, len).LastIndexOf(' ');

                            string tmpContent = content.Substring(index, offset);
                            contentList.Add(new Content() {
                                page_id = this.CurrentPage.id,
                                tag = node.OriginalName.Trim(),
                                text = tmpContent
                            });

                            index += offset;
                        } while(content.Length < index && content.Length > 800);
                    }

                    /*if(content.Length > 0)
                        contentList.Add(new Content() {
                            page_id = this.CurrentPage.id,
                            tag = node.OriginalName.Trim(),
                            text = content
                        });
                    */
                }
            }

            return contentList;
        }

        private List<Link> GetLinks(Page currentPage, HtmlAgilityPack.HtmlDocument doc) {
            List<Link> linkList = new List<Link>();

            HtmlNodeCollection linkNodeCollection = doc.DocumentNode.SelectNodes("//a[@href and text()]");
            if(linkNodeCollection != null) {
                this.LinkTagCount = linkNodeCollection.Count;

                int i = 1;
                foreach(HtmlNode node in linkNodeCollection) {
                    this.CurrentLinkTagIndex = i++;

                    HtmlAttribute att = node.Attributes["href"];

                    string foundLink = att.Value;
                    string linkText = node.InnerText.Trim();

                    if(string.IsNullOrEmpty(linkText))
                        continue;

                    bool internalLink = false;
                    try {
                        foundLink = this.FixLink(this.CurrentPage.url, foundLink, ref internalLink);
                    } catch(Exception e) {
                        if(e.Message == "Skip.")
                            continue;
                        else
                            throw;
                    }

                    this.addOrGetPage(foundLink);

                    /*linkList.Add(new Link() {
                        text = linkText,
                        local = internalLink,
                        from_id = currentPage.id,
                        to_id = this.addOrGetPage(foundLink).id
                    });*/
                }
            }
            return linkList;
        }

        private string FixLink(string currentLink, string foundLink, ref bool internalLink) {
            Uri uri = new Uri(currentLink);

            if(foundLink.StartsWith("//")) {
                foundLink = uri.Scheme + "://" + uri.Authority + foundLink.Substring(1);
            } else if(foundLink.StartsWith("/")) {
                // is internal
                internalLink = true;
                foundLink = uri.GetLeftPart(UriPartial.Authority) + foundLink;
            } else if(foundLink.StartsWith("?")) {
                // is internal
                internalLink = true;
                foundLink = uri.GetLeftPart(UriPartial.Path) + foundLink;
            } else {
                throw new Exception("Skip.");
            }

            if(foundLink.Contains('#')) {
                foundLink = foundLink.Substring(0, foundLink.IndexOf('#'));
            }

            return foundLink;
        }

        private Page addOrGetPage(string foundLink) {
            using(var c = new CrawlerContext()) {
                c.Database.ExecuteSqlCommandAsync(string.Format(@"
                    declare @url varchar(500) = '{0}';

                    IF NOT EXISTS (SELECT TOP 1 * FROM Pages WHERE (url = @url))
                    BEGIN
                        INSERT INTO Pages(url, scanned)
	                    Values(@url, 0)
                    END
                ", foundLink));
            }
            return new Page();

            return this.ctx.Pages.SqlQuery(string.Format(@"
                    declare @url varchar(500) = '{0}';

                    IF NOT EXISTS (SELECT TOP 1 * FROM Pages WHERE (url = @url))
                    BEGIN
                        INSERT INTO Pages(url, scanned)
	                    OUTPUT inserted.*
                        Values(@url, 0)
                    END
                    ELSE
                    BEGIN
	                    SELECT TOP 1 * FROM Pages WHERE url = @url
                    END
                ", foundLink)).Single();

            Page foundPage = null;

            using(var tctx = new CrawlerContext()) {
                tctx.Configuration.AutoDetectChangesEnabled = false;
                try {
                    foundPage = tctx.Pages.First(x => x.url == foundLink);
                } catch(Exception) {
                    foundPage = new Page() { url = foundLink.Trim(), LastAttempt = null };

                    tctx.Entry(foundPage).State = EntityState.Added;
                    tctx.SaveChanges();
                }
            }

            return foundPage;
        }

        public static void oldStart() {
            using(var ctx = new CrawlerContext()) {
                int maxQueueItems = 100;

                BenchMarker BM = new BenchMarker(100);

                //using(var dbContextTransaction = ctx.Database.BeginTransaction()) {
                try {
                    //while(this.running)
                    while(true) {
                        try {
                            string query = @"
								UPDATE TOP (1) Pages
								SET LastAttempt = GETDATE()
								OUTPUT
									inserted.id,
									inserted.url,
									inserted.title,
									inserted.LastAttempt,
									inserted.scanned
								WHERE
									scanned = 0
									AND
									(DATEDIFF(s, '1970-01-01 00:00:00', LastAttempt) <= DATEDIFF(s, '1970-01-01 00:00:00', GETDATE()) - 60*60 OR LastAttempt IS NULL)";
                            Page page = ctx.Pages.SqlQuery(query).Single();

                            //ctx.Database.ExecuteSqlCommand("BEGIN TRAN");

                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Start();

                            //using(var scope = new TransactionScope(TransactionScopeOption.Required,
                            //    new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead })) {
                            using(DbContextTransaction scope = ctx.Database.BeginTransaction()) {
                                //Page page = ctx.Pages.SqlQuery("SELECT TOP 1 * FROM Pages WITH (HOLDLOCK, ROWLOCK) WHERE scanned = 0").Single();
                                //ctx.Database.ExecuteSqlCommand("SELECT TOP 1 * FROM Pages WITH (TABLOCKX, HOLDLOCK) WHERE scanned = 0");

                                //Page page = ctx.Pages.First(x => x.scanned == false);
                                if(page != null) {
                                    Console.WriteLine("Scanning Page: " + page.url);

                                    Crawler.oldcrawlPage(page);

                                    page.scanned = true;
                                    ctx.Entry(page).State = EntityState.Modified;
                                    ctx.SaveChanges();
                                    //ctx.Database.ExecuteSqlCommand("COMMIT TRAN");
                                    scope.Commit();
                                    //scope.Complete();
                                } else {
                                    Thread.Sleep(1000);
                                    Console.WriteLine("No more links to scan.");
                                }
                            }

                            stopwatch.Stop();

                            long lastScan = stopwatch.ElapsedMilliseconds;
                            BM.Insert(lastScan);

                            /*timeQueue.Enqueue(lastScan);

                            if(timeQueue.Count > maxQueueItems)
                                timeQueue.Dequeue();

                            long averageScanTime = timeQueue.Sum() / timeQueue.Count;*/

                            Console.WriteLine();
                            Console.WriteLine("Last scan took:\t{0} ms.", lastScan);
                            Console.WriteLine("Average scan time:\t{0} ms.", BM.AverageTime);
                            Console.WriteLine();
                        } catch(Exception e) {
                            //ctx.Database.ExecuteSqlCommand("ROLLBACK TRAN");
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                            //throw;
                        }
                    }
                } catch(Exception) { }
                //dbContextTransaction.Commit();
                /*  } catch(Exception e) {
                      dbContextTransaction.Rollback();
                  }*/
                //}
            }

            /*using(var ctx = new CrawlerContext()) {
                while(this.running) {
                    try {
                        Page page = ctx.Pages.First(x => x.scanned == false);
                        if(page != null) {
                            Console.WriteLine("Scanning Page: " + page.url);

                            this.crawlUrl(page);

                            page.scanned = true;
                            ctx.Entry(page).State = EntityState.Modified;
                            ctx.SaveChanges();
                        } else {
                            Thread.Sleep(1000);
                            Console.WriteLine("No more links to scan.");
                        }
                    } catch(Exception e) {
                        Console.WriteLine(e.StackTrace);
                        //throw;
                    }
                }
            }*/
        }

        private static void oldcrawlPage(Page currentPage) {
            using(var client = new WebClient()) {
                Uri uri = new Uri(currentPage.url);
                string HTML;
                try {
                    HTML = client.DownloadString(uri);
                } catch(WebException e) {
                    //Console.WriteLine(e.StackTrace);
                    return;
                    //throw;
                }

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(HTML);

                string title = doc.DocumentNode.SelectSingleNode("//title").InnerText;

                using(var ctx = new CrawlerContext()) {
                    ctx.Pages.Attach(currentPage);
                    currentPage.title = title;
                    //ctx.Entry(currentPage).State = EntityState.Modified;
                    ctx.SaveChanges();
                }

                HtmlNodeCollection contentNodeCollection = doc.DocumentNode.SelectNodes("(//h1|//h2|//h3|//h4|//h5|//h6|//p)[text()]");
                if(contentNodeCollection != null) {
                    Console.WriteLine("Found content tags: \t{0}", contentNodeCollection.Count);
                    using(var ctx = new CrawlerContext()) {
                        ctx.Configuration.AutoDetectChangesEnabled = false;

                        foreach(HtmlNode node in contentNodeCollection) {
                            string content = node.InnerText.Trim();

                            //Console.WriteLine("Found {0} tag", node.OriginalName.Trim());

                            if(content.Length > 0)
                                ctx.Content.Add(new Content() {
                                    page_id = currentPage.id,
                                    tag = node.OriginalName.Trim(),
                                    text = content.Trim()
                                });
                        }
                        ctx.SaveChanges();
                    }
                }

                HtmlNodeCollection linkNodeCollection = doc.DocumentNode.SelectNodes("//a[@href and text()]");
                if(linkNodeCollection != null) {
                    Console.WriteLine("Found {0} links", linkNodeCollection.Count);

                    List<Page> linkList = new List<Page>();

                    CrawlerContext ctx = new CrawlerContext();
                    ctx.Configuration.AutoDetectChangesEnabled = false;

                    int i = 1;
                    BenchMarker BM = new BenchMarker(100);
                    int entitySaveCount = 50;
                    foreach(HtmlNode node in linkNodeCollection) {
                        HtmlAttribute att = node.Attributes["href"];

                        string foundLink = att.Value;
                        string linkText = node.InnerText.Trim();

                        if(string.IsNullOrEmpty(linkText))
                            continue;

                        bool internalLink = false;

                        if(foundLink.StartsWith("//")) {
                            foundLink = uri.Scheme + "://" + foundLink.Substring(2);
                        } else if(foundLink.StartsWith("/")) {
                            // is internal
                            internalLink = true;
                            foundLink = uri.GetLeftPart(UriPartial.Authority) + foundLink;
                        } else if(foundLink.StartsWith("?")) {
                            // is internal
                            internalLink = true;
                            foundLink = uri.GetLeftPart(UriPartial.Path) + foundLink;
                        } else {
                            continue;
                        }

                        /*if (linkList.Contains(foundLink) || scannedLinks.Contains(foundLink))
                        {
                            continue;
                        }

                        linkList.Add(foundLink);
                        Console.WriteLine("Found Page: " + foundLink);
                        */
                        //Console.WriteLine("Found Link: " + foundLink);
                        Page foundPage;

                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        using(var tctx = new CrawlerContext()) {
                            tctx.Configuration.AutoDetectChangesEnabled = false;
                            try {
                                foundPage = tctx.Pages.First(x => x.url == foundLink);
                            } catch(Exception) {
                                foundPage = new Page() { url = foundLink.Trim() };

                                tctx.Entry(foundPage).State = EntityState.Added;
                                tctx.SaveChanges();
                            }
                        }

                        stopwatch.Stop();

                        long lastScan = stopwatch.ElapsedMilliseconds;
                        BM.Insert(lastScan);

                        ctx.Set<Link>().Add(new Link() {
                            text = linkText,
                            local = internalLink,
                            from_id = currentPage.id,
                            to_id = foundPage.id
                        });

                        /*ctx.Links.Add(new Link() {
                            text = linkText.Trim(),
                            local = internalLink,
                            from_id = currentPage.id,
                            to_id = foundPage.id
                        });*/

                        if(i % entitySaveCount == 0) {
                            ctx.SaveChanges();
                            ctx.Dispose();
                            ctx = new CrawlerContext();
                            ctx.Configuration.AutoDetectChangesEnabled = false;
                        }
                        i++;
                    }

                    Console.WriteLine("Avg link find: \t\t {0}ms", BM.AverageTime);

                    Stopwatch SW = new Stopwatch();
                    SW.Start();
                    if(ctx.ChangeTracker.HasChanges())
                        ctx.SaveChanges();
                    SW.Stop();

                    Console.WriteLine("Savechanges time: \t {0}ms", SW.ElapsedMilliseconds);

                    ctx.Dispose();
                }
                //ctx.Pages.AddRange(linkList);

                //ctx.SaveChanges();
            }
        }
    }
}