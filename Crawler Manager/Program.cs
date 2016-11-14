using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crawler_Manager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] Args)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Args.Length>=1)
            try
            {
                if (int.Parse(Args.First()) > 0)
                {
                    Application.Run(new Form1(int.Parse(Args.First())));
                    //MessageBox.Show("Run cmd line here");
                }
            }
            catch (Exception) { }
            
            else
            {
                Application.Run(new Form1());
            }
        }
    }
}
