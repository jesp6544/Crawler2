using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crawler {

    internal static class Program {

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

                if (crawler == null) {
                    Console.WriteLine("Starting...");
                } else {
                    if (crawler.CurrentPage == null) {
                        Console.WriteLine("Finding next link...");
                    } else {
                        Console.WriteLine("Scanning:\n{0}", crawler.CurrentPage.url);
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