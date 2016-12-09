using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Crawler_Manager
{
    public partial class Form1 : Form
    {
        private readonly List<Process> _running = new List<Process>();
        private readonly Thread _proccThread;
        private readonly int _screenX;
        private readonly int _screenY;
        private int _screenXCount = 1;

        public Form1()
        {
            _proccThread = new Thread(ProccWatcher);
            InitializeComponent();
            _screenX = Screen.PrimaryScreen.Bounds.Width;
            _screenY = Screen.PrimaryScreen.Bounds.Height;
            _proccThread.Start();
        }

        public Form1(int count)
        {
            StartProcesses(count);
        }

        private void ScreenOrder()
        {
            int windowHeight = 255;
            int yMax = _screenY / windowHeight;
            _screenXCount = _running.Count / yMax + 1;
            int i = 0;
            foreach (var procc in _running)
            {
                MoveWindow(procc.MainWindowHandle,
                    (i / yMax) * (_screenX / _screenXCount),
                    (i % yMax) * windowHeight,
                    _screenX / _screenXCount,
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
                    long mem = 0;
                    foreach (var procc in _running)
                    {
                        try
                        {
                            if (procc.ExitCode == 0)
                                continue;
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                        if (!procc.HasExited && procc.Responding)
                            continue;
                        LogTxtBox.Text += @"A process has shutdown with error code" + Environment.NewLine;
                        _running.Remove(procc);
                        if (!procc.HasExited)
                            procc.Kill();
                        StartProcesses(1 + _running.Count);
                        procc.Refresh();
                        mem += (procc.PrivateMemorySize64 / 1024 / 1024);
                    }
                    MemLabel.Text = mem.ToString();
                    ScreenOrder();
                }
                catch (Exception)
                {
                    // ignored
                }
                Thread.Sleep(100);
            }
        }

        private void Shutdown(int num)
        {
            foreach (var procc in _running)
            {
                if (num > 0)
                {
                    try
                    {
                        procc.CloseMainWindow();
                        num--;
                        _running.Remove(procc);
                    }
                    catch (Exception)
                    {
                        LogTxtBox.Text = LogTxtBox.Text + @"Error in closure of processes" + Environment.NewLine;
                    }
                }
            }
        }

        private void StartProcesses(int num)
        {
            if (num > 0 && num != _running.Count)
                try
                {
                    if (num < _running.Count)
                        Shutdown(_running.Count - num);
                    else
                    {
                        int toStart = num - _running.Count;
                        for (int i = 0; i < toStart; i++)
                        {
                            Process procc = Process.Start(@"Crawler.exe");
                            _running.Add(procc);
                        }
                    }
                }
                catch (Exception)
                {
                    LogTxtBox.Text = LogTxtBox.Text + @"Failed to start all processes" + Environment.NewLine;
                }
            else if (num <= 0)
                Shutdown(_running.Count);
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        private void GoBtn_Click(object sender, EventArgs e)
        {
            try
            {
                StartProcesses(int.Parse(NumTxtBox.Text));
            }
            catch (Exception)
            {
                LogTxtBox.Text = @"Please insert a positiv number" + Environment.NewLine;
            }
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            _proccThread.Abort();
            foreach (var procc in _running)
            {
                try
                {
                    procc.Kill();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}