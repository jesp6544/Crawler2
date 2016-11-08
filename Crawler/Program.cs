using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crawler {

    internal static class Program {
        private static readonly DateTime startTime = DateTime.Now;

        private static void Main() {
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
                            Console.WriteLine("Total links:          {0}", crawler.TotalLinkTagsFound);
                            Console.WriteLine("Total content tags:   {0}", crawler.TotalContentTagsFound);
                            Console.WriteLine("Average crawl time:   {0}ms", crawler.LoopBenchMarker.AverageTime);
                        }

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