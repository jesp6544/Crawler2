using CrawlerLibrary.Models;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using System;
using System.Threading;

namespace Crawler
{
    internal static class Program
    {
        private static readonly DateTime StartTime = DateTime.Now;

        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs args)
            {
                Exception e = (Exception)args.ExceptionObject;
                Console.WriteLine(@"Unhandled exception: " + e);
                ISolrOperations<HTMLContent> solr = ServiceLocator.Current.GetInstance<ISolrOperations<HTMLContent>>();
                solr.Commit();
                Environment.Exit(1);
            };

            Startup.InitContainer();
            Startup.Init<HTMLContent>("http://176.23.159.28:8983/solr/new_core2");
            Crawler crawler = new Crawler();
            Thread renderThread = new Thread(() =>
            {
                Render(crawler);
            });
            renderThread.Start();
            crawler.Start();
        }

        private static void Render(Crawler crawler)
        {
            while (true)
            {
                Console.Clear();

                if (crawler == null)
                {
                    Console.WriteLine(@"Starting...");
                }
                else
                {
                    if (crawler.CurrentPage == null)
                    {
                        Console.WriteLine(@"Finding next link...");
                    }
                    else
                    {
                        Console.WriteLine(@"Scanning:             {0}", crawler.CurrentPage.url);
                        Console.WriteLine(@"content tags:         {0}/{1}", crawler.CurrentContentTagIndex, crawler.ContentTagCount);
                        Console.WriteLine(@"link tags:            {0}/{1}", crawler.CurrentLinkTagIndex, crawler.LinkTagCount);
                        if (crawler.LinksCrawled > 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine(@"Stats:");
                            Console.WriteLine(@"Pages crawled:        {0}", crawler.LinksCrawled);
                            Console.WriteLine("Average crawl time:   {0}ms", crawler.LoopBenchMarker.AverageTime);
                        }
                        Console.WriteLine(@"NoFollows:            {0}", crawler.NoFollows);
                        Console.WriteLine(@"NoIndexes:            {0}", crawler.NoIndex);

                        TimeSpan totalRunTime = DateTime.Now.Subtract(StartTime);
                        Console.WriteLine();
                        Console.WriteLine("Total time run:       {0}", totalRunTime.ToString("d':'hh':'mm':'ss"));
                        Console.WriteLine();
                        Console.Write("Total errors:         {0}", crawler.TotalErrors);
                    }
                }

                Thread.Sleep(1000 / 2);
            }
        }
    }
}