using CrawlerLibrary.Models;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crawler {

    internal static class Program {
        private static readonly DateTime startTime = DateTime.Now;

        private static void Main() {
            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs args)
            {
                Exception e = (Exception)args.ExceptionObject;
                Console.WriteLine("Unhandled exception: " + e);
                Environment.Exit(1);
            };

            Startup.InitContainer();
            Startup.Init<HTMLContent>("http://176.23.159.28:8983/solr/new_core2");

            /*ISolrOperations<HTMLContent> solr = ServiceLocator.Current.GetInstance<ISolrOperations<HTMLContent>>();
            var results = solr.Query(
                new SolrQueryByField("text", "Martin"), new QueryOptions {
                    Highlight = new HighlightingParameters {
                        Fields = new[] { "text" },
                    }
                });

            Console.WriteLine("dfsdf");
            Console.WriteLine(results.Count);
            */

            /*foreach(var h in results.Highlights[results[0].Title]) {
                Console.WriteLine("{0}: {1}", h.Key, string.Join(", ", h.Value.ToArray()));
            }*/

            //Console.ReadLine();

            //return;

            /*string currentHTML;

            using(var client = new WebClient()) {
                Uri uri = new Uri("https://en.wikipedia.org/wiki/Dependency_injection");d
                try {
                    currentHTML = client.DownloadString(uri);
                    //HTML = client.DownloadString(uri);
                } catch(WebException e) {
                    return;
                }
            }

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(currentHTML));

            //var solr = ServiceLocator.Current.GetInstance();
            ISolrOperations<HTMLContent> solr = ServiceLocator.Current.GetInstance<ISolrOperations<HTMLContent>>();
            ExtractResponse extractResponse = solr.Extract(new ExtractParameters(ms, "9001", "Wikipedia main") {
                AutoCommit = true,
                Capture = "p",
                CaptureAttributes = false,
                DefaultField = "text",
                ExtractFormat = ExtractFormat.Text,
                XPath = "/xhtml:html/xhtml:body/xhtml:div/descendant:node()"
            });

            solr.Commit();

            return;*/

            Crawler crawler = new Crawler();
            Thread renderThread = new Thread(() => {
                Program.render(crawler);
            });
            renderThread.Start();

            crawler.Start();
        }

        private static void render(Crawler crawler) {
            while(true) {
                Console.Clear();

                if(crawler == null) {
                    Console.WriteLine("Starting...");
                } else {
                    if(crawler.CurrentPage == null) {
                        Console.WriteLine("Finding next link...");
                    } else {
                        Console.WriteLine("Scanning:             {0}", crawler.CurrentPage.url);
                        Console.WriteLine("content tags:         {0}/{1}", crawler.CurrentContentTagIndex, crawler.ContentTagCount);
                        Console.WriteLine("link tags:            {0}/{1}", crawler.CurrentLinkTagIndex, crawler.LinkTagCount);
                        if(crawler.LinksCrawled > 0) {
                            Console.WriteLine();
                            Console.WriteLine("Stats:");
                            Console.WriteLine("Pages crawled:        {0}", crawler.LinksCrawled);
                            //Console.WriteLine("Total links:          {0}", crawler.TotalLinkTagsFound);
                            //Console.WriteLine("Total content tags:   {0}", crawler.TotalContentTagsFound);
                            Console.WriteLine("Average crawl time:   {0}ms", crawler.LoopBenchMarker.AverageTime);
                        }
                        Console.WriteLine("NoFollows:            {0}", crawler.NoFollows);
                        Console.WriteLine("NoIndexes:            {0}", crawler.NoIndex);

                        TimeSpan totalRunTime = DateTime.Now.Subtract(Program.startTime);
                        Console.WriteLine();
                        string totalRunTimeNoMili = totalRunTime.ToString();
                        Console.WriteLine("Total time run:       {0}", totalRunTime.ToString("d':'hh':'mm':'ss"));
                        Console.WriteLine();
                        Console.Write("Total errors:         {0}", crawler.TotalErrors);
                    }
                }

                Thread.Sleep(1000 / 10);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /*[STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }*/
    }
}