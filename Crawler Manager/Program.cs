using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crawler_Manager {

    internal static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] Args) {
            MessageBox.Show("\"" + Application.ExecutablePath + "\" 5");

            //RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", true);
            //rkApp.SetValue("Krawler Manager", "\"" + Application.ExecutablePath + "\" %5", RegistryValueKind.String);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if(Args.Length >= 1)
                try {
                    if(int.Parse(Args.First()) > 0) {
                        Application.Run(new Form1(int.Parse(Args.First())));
                        //MessageBox.Show("Run cmd line here");
                    }
                } catch(Exception) { } else {
            }
            Application.Run(new Form1());
        }
    }
}