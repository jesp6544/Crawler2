using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crawler_Manager
{
    public partial class Form1 : Form
    {
        int number = 0;
        List<Process> running = new List<Process>();

        public Form1()
        {
            InitializeComponent();
            Thread updateThread = new Thread(LookForUpdate);
            Thread proccThread = new Thread(ProccKeeper);
        }

        private void LookForUpdate()
        {
            int timer = 1000*60;
            while (false) //true to enable
            {
                /*
                if(Some code to check for new dll){
                Some code to download new dll
                Shutdown();
                //Some code to replace dll
                GoBtn_Click(null,null);}
                Thread.Sleep(timer);
                */
            }
        }

        private void ProccKeeper()
        {
            while (false)  //Enable when working
            {
                try
                {
                    if (running.First().Threads != null)
                    {
                        foreach (Process procc in running)
                        {
                            if (procc.HasExited == true)
                            {
                                LogTxtBox.Text = LogTxtBox.Text + procc.StandardError;
                                //Remove this procc from running
                                StartProcesses(1);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Thread.Sleep(1000*10);
                }

                Thread.Sleep(1000*60);
            }
        }

        private void Shutdown(int num)
        {
            foreach (Process procc in running)
            {
                if (num > 0)
                {
                    try
                    {
                        procc.CloseMainWindow();
                        num--;
                        running.Remove(procc);
                    }
                    catch (Exception)
                    {
                        LogTxtBox.Text = LogTxtBox.Text + "\n Error in closure of processes";
                    }
                    
                }
            }
        }

        private void StartProcesses(int num)
        {
            if (num > 0 && num != running.Count)
                try
                {
                    if (num <running.Count)
                    Shutdown(running.Count - num);
                    int toStart = num - running.Count;
                    for (int i = 0; i < toStart; i++)
                    {
                        //ProcessStartInfo start = new ProcessStartInfo() { = "C:\Users\Post\Source\Repos\Crawler2\Crawler\bin\Release"};
                        Process procc = Process.Start(@"Crawler.exe");
                        running.Add(procc);
                    }
                }
                catch (Exception)
                {
                    LogTxtBox.Text = LogTxtBox.Text + "\n Failed to start all processes";
                }
            else if (num <= 0)
                Shutdown(running.Count);
        }

        private void GoBtn_Click(object sender, EventArgs e)
        {
            try
            {
               StartProcesses(int.Parse(NumTxtBox.Text));
            }
            catch (Exception)
            {
                LogTxtBox.Text = "Please insert a positiv number";
            }
            
        }
    }
}
