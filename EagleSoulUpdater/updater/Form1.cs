using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace updater
{
    public partial class Form1 : Form
    {
        public static int counter = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("EagleSoul.exe");
            foreach(Process lm in processes)
            {
                lm.Kill();
            }

            timer1.Start();
            timer1.Enabled = true;
        }

        private void recreateDLL2()
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                wc.DownloadFileAsync(new Uri("http://66.70.154.61/EagleSoulLauncherUpdate/languages.JSON"), Application.StartupPath + "\\languages.JSON");
            }
        }

        private void recreateDLL3()
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                wc.DownloadFileAsync(new Uri("http://66.70.154.61/EagleSoulLauncherUpdate/EagleSoulLauncher.dll"), Application.StartupPath + "\\EagleSoulLauncher.dll");
            }
        }


        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;         
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            counter++;
            if (counter == 3)
            {
                MessageBox.Show("Update finished!");
                Application.Exit();
                Process.Start(Application.StartupPath + "\\EagleSoul Launcher.exe");
            }
            else if (counter == 2)
            {
                recreateDLL2();
            }
            else if (counter == 1)
            {
                recreateDLL3();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                wc.DownloadFileAsync(new Uri("http://66.70.154.61/EagleSoulLauncherUpdate/EagleSoul Launcher.exe"), Application.StartupPath + "\\EagleSoul Launcher.exe");
            }
        }
    }
}
