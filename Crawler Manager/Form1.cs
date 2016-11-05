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
            LookForUpdate();
            ProccKeeper();
        }

        private void LookForUpdate()
        {
            int timer = 1000*60;
            while (true)
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
            while (true)
            {
                foreach (Process procc in running)
                {
                    if (procc.HasExited == true)
                    {
                        //Remove this procc from running
                        StartProcesses(1);
                    }
                }
                Thread.Sleep(1000*60);
            }
        }

        private void Shutdown()
        {
            foreach (Process procc in running)
            {
                procc.Kill();
            }
        }

        private void StartProcesses(int num)
        {
            if (num >= 1 || num != running.Count)
                try
                {
                    Shutdown();
                    for (int i = 0; i < num; i++)
                    {
                        ProcessStartInfo start = new ProcessStartInfo() {FileName = "Crawler.exe"};
                        Process procc = Process.Start(start);
                        running.Add(procc);
                    }
                }
                catch (Exception)
                {
                    LogTxtBox.Text = LogTxtBox.Text + "Failed to start all processes";
                }
            if (num <= 0)
                Shutdown();
        }

        private void GoBtn_Click(object sender, EventArgs e)
        {
            try
            {
                StartProcesses(Int32.Parse(NumTxtBox.Text));
            }
            catch (Exception)
            {
                LogTxtBox.Text = "Please insert a positiv number";
            }
            
        }
    }
}
