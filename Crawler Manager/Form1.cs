using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
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
        Thread updateThread;
        Thread proccThread;
        private int screenX;
        private int screenY;
        private int screenXCount = 1;

        public Form1()
        {
            updateThread = new Thread(LookForUpdate);
            proccThread = new Thread(ProccWatcher);
            InitializeComponent();
            screenX = Screen.PrimaryScreen.Bounds.Width;
            screenY = Screen.PrimaryScreen.Bounds.Height;
            updateThread.Start();
            proccThread.Start();

        }

        private void Download()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFile(new Uri("Our hosting/dll.dll"), @"dll.dll");
            //Continue when file finished
        }

        private void LookForUpdate()
        {
            int timer = 1000*60;
            while (false) //true to enable
            {
                /*
                if(Some code to check for new dll){
                Shutdown();
                //delete old dll
                Download();
                GoBtn_Click(null,null);}
                Thread.Sleep(timer);
                */
            }
        }

        private void ScreenOrder()
        {
            int windowHeight = 255;
                int yMax = screenY / windowHeight ;
                screenXCount =  running.Count / yMax + 1;
            int i = 0;
            foreach (Process procc in running)
            {
                MoveWindow(procc.MainWindowHandle, 
                    (i / yMax) * (screenX / screenXCount), 
                    (i % yMax) * windowHeight, 
                    screenX / screenXCount, 
                    windowHeight, true);
                i++;
            }
            
        }

        private void ProccWatcher()
        {
            while (true)
            {
                try
                {
                    int cpu = 0;
                    long mem = 0;
                    ScreenOrder();
                    int i = 0;
                    foreach (Process procc in running)
                    {
                        procc.Refresh();
                        mem += (procc.PrivateMemorySize64/1024/1024);
                        //LogTxtBox.Text += (procc.WorkingSet64 / 1024) + Environment.NewLine;
                        if (procc.HasExited || !procc.Responding) //|| procc.ExitCode != 0
                        {
                            try
                            {
                                LogTxtBox.Text += "A process has shutdown with error code: " + procc.ExitCode + Environment.NewLine;

                            }
                            catch (Exception)
                            {
                            }
                            //running.Remove(procc);
                            if (!procc.HasExited)
                            procc.Kill();
                            StartProcesses(1 + running.Count);
                            //cpu += procc.tot
                        }
                    }
                    MemLabel.Text = mem.ToString();

                }
                catch (Exception e)
                {
                    //LogTxtBox.Text += "fuck";
                }
                Thread.Sleep(1000*2);
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
                        LogTxtBox.Text = LogTxtBox.Text + "Error in closure of processes" + Environment.NewLine;
                    }
                }
            }
        }

        private void StartProcesses(int num)
        {
            if (num > 0 && num != running.Count)
                try
                {
                    if (num < running.Count)
                        Shutdown(running.Count - num);
                    else
                    {
                        int toStart = num - running.Count;
                        for (int i = 0; i < toStart; i++)
                        {
                            Process procc = Process.Start(@"Crawler.exe");
                            running.Add(procc);
                        }
                    }
                }
                catch (Exception)
                {
                    LogTxtBox.Text = LogTxtBox.Text + " Failed to start all processes" + Environment.NewLine;
                }
            else if (num <= 0)
                Shutdown(running.Count);
        }
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);


        private void GoBtn_Click(object sender, EventArgs e)
        {
            try
            {
                StartProcesses(int.Parse(NumTxtBox.Text));
            }
            catch (Exception)
            {
                LogTxtBox.Text = "Please insert a positiv number" + Environment.NewLine;
            }

        }



        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            updateThread.Abort();
            proccThread.Abort();
            foreach (Process procc in running)
            {
                try
                {
                    procc.Kill();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
