using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using EagleSoulLauncher;

namespace EagleSoul_Launcher
{
    public partial class Main : Form
    {
        
        public PictureBox loadingGif;
        public static ArrayList filesToDownload = new ArrayList();
        public static ArrayList missingFiles = new ArrayList();
        public static ArrayList existingFiles = new ArrayList();
        public static ArrayList bytesOfFiles = new ArrayList();
        public static ArrayList needToDownload = new ArrayList();
        public static ArrayList folderPath = new ArrayList();
        public static ArrayList missingFolders = new ArrayList();
        public static string fileName;
        public static int fileCounter = 0;
        public static int downloadedFileCounter = 0;

        public WebClient wclient = new WebClient();
     

        

        public Main()
        {            
            InitializeComponent();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        }



        private void setUserLanguage()
        {
            string info = wclient.DownloadString("https://ipinfo.io/json");
            var JSONObject = JObject.Parse(info);
            string country = JSONObject["country"].ToString();

            string languages = File.ReadAllText(Application.StartupPath + "\\languages.JSON");
            var languageJSON = JObject.Parse(languages);

          
            label2.Text = languageJSON[country][0]["label2"].ToString();
            label3.Text = languageJSON[country][0]["label3"].ToString();

            button1.Text = languageJSON[country][0]["button1"].ToString();
            button2.Text = languageJSON[country][0]["button2"].ToString();

        }

        private void LoadingFinished(object sender, EventArgs e)
        {
            string serverVersion = wclient.DownloadString("http://66.70.154.61/EagleSoulLauncherUpdate/version.ini");
            if (EagleSoul_Launcher.Properties.Settings.Default.version != serverVersion)
            {
                MessageBox.Show("New update available!");              
                Process.Start(Application.StartupPath + "\\EagleSoulLauncherUpdater.exe");
                Application.Exit();
                return;
            }


            label7.Text = "Version: " + EagleSoul_Launcher.Properties.Settings.Default.version;

            setUserLanguage();


            this.FormBorderStyle = FormBorderStyle.None;
            loadingGif = new PictureBox();
            loadingGif.Size = new Size(836, 466);
            loadingGif.Location = new Point(0, 0);
            loadingGif.Image = EagleSoul_Launcher.Properties.Resources.eaglesoulBig;
            this.Controls.Add(loadingGif);
            timer1.Start();
            timer1.Enabled = true;

          
            label2.Parent = pictureBox5;
            label3.Parent = pictureBox5;
            label8.Parent = pictureBox5;

            label2.Location = new Point(300, 135);
            label3.Location = new Point(300, 180);
            label8.Location = new Point(314, 410);


            //textBox1.Enabled = false;
            //textBox2.Enabled = false;

            //button1.Enabled = false;
            //button2.Enabled = false;

          //  gettingsFiles();
        }


        private void gettingsFiles()
        {

          

            label6.Text = "Status: Getting file list...";
            wclient.DownloadFile("http://66.70.154.61/EagleSoul/files.txt", Application.StartupPath + "\\files.txt");
            var table = File.ReadAllLines(Application.StartupPath + "\\files.txt");
            foreach (string fileName2 in table)
            {
                if (fileName2 != "")
                {
                    filesToDownload.Add(fileName2);
                }
            }
            getFolders();
        }

        //step 1.5 get the folders and create the missing ones

        private void getFolders()
        {
            label6.Text = "Status: Getting folder list...";
            wclient.DownloadFile("http://66.70.154.61/EagleSoul/folders.xml", Application.StartupPath + "\\folders.xml");
            var folder = File.ReadAllLines(Application.StartupPath + "\\folders.xml");
            foreach (string folderName in folder)
            {
                if (folderName != "")
                {
                    folderPath.Add(folderName);
                }
            }
            getMissingFolder();
        }

        private void getMissingFolder()
        {

            label6.Text = "Status: Check missing folders...";
            foreach (string folderName in folderPath)
            {
                if (folderName != "")
                {
                    if (!Directory.Exists(Application.StartupPath + "\\" + folderName))
                    {
                        missingFolders.Add(folderName);
                    }
                }
            }
            createMissingFolders();
        }

        private void createMissingFolders()
        {

            label6.Text = "Status: Create missing folders...";
            foreach (string missingFolderName in missingFolders)
            {
                if (missingFolderName != "")
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\" + missingFolderName);
                }
            }
            checkNotExistsFiles();
        }

        //step 2 check if already some file exists and add the not existing files to a new array

        private void checkNotExistsFiles()
        {

            label6.Text = "Status: Check missing files...";
            foreach (string file in filesToDownload)
            {
                // MessageBox.Show(Application.StartupPath + "/" + file);
                if (file != "")
                {
                    // MessageBox.Show(Application.StartupPath + "\\" + file);
                    if (!File.Exists(Application.StartupPath + "\\" + file))
                    {
                        //  MessageBox.Show(file);
                        missingFiles.Add(file);
                    }
                }
            }
            FileBytes();
        }



        //step 3 getting file bytes

        private void FileBytes()
        {

            label6.Text = "Status: Getting file bytes...";
            wclient.DownloadFile("http://66.70.154.61/EagleSoul/bytes.xml", Application.StartupPath + "\\bytes.xml");
            var table = File.ReadAllLines(Application.StartupPath + "\\bytes.xml");
            foreach (string byteS in table)
            {
                if (byteS != "")
                {
                    bytesOfFiles.Add(byteS);
                }
            }
            byteChecker();
        }

        //step 4 check the bytes of the existing files


        private void byteChecker()
        {

            label6.Text = "Status: Check bytes of existing files...";
            foreach (string fileBytes in filesToDownload)
            {
                if (File.Exists(Application.StartupPath + "\\" + fileBytes))
                {
                    if (FileSystem.FileLen(fileBytes).ToString() != bytesOfFiles[0].ToString())
                    {
                        needToDownload.Add(fileBytes);
                        bytesOfFiles.RemoveAt(0);
                    }
                    else
                    {
                        bytesOfFiles.RemoveAt(0);
                    }
                }
                else
                {
                    needToDownload.Add(fileBytes);
                    bytesOfFiles.RemoveAt(0);
                }
            }
             fileCount();
           // downloader
        }

        //step 4.5 counting files

        private void fileCount()
        {

            foreach (string file in needToDownload)
            {
               // fileCounter++;
                downloadedFileCounter++;
            }
            downloader();
        }

        //step 5 Download the missing Files

        private void downloader()
        {

            if (fileCounter < downloadedFileCounter)
            {
                label6.Text = "Status: Downloading missing files...";
                using (WebClient wc = new WebClient())
                {
                    label1.Text = "Files: " + fileCounter + " / " + downloadedFileCounter;
                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                    //   MessageBox.Show(needToDownload[0].ToString());
                    label4.Text = needToDownload[0].ToString();
                    wc.DownloadFileAsync(new Uri("http://66.70.154.61/EagleSoul/" + needToDownload[0].ToString()), Application.StartupPath + "\\" + needToDownload[0].ToString());
                }
            }
            else if (fileCounter >= downloadedFileCounter)
            {
                label1.Text = "Download Finished!";
                label5.Location = new Point(353, 411);
                label5.Text = "";
                label6.Text = "";
                label4.Text = "";
                progressBar1.Value = 100;
                button2.Visible = true;


                //textBox1.Enabled = true;
                //textBox2.Enabled = true;

                //button1.Enabled = true;
                //button2.Enabled = true;


                button1.Visible = false;
                button2.Visible = false;

                textBox1.Visible = false;
                textBox2.Visible = false;

                label2.Visible = false;
                label3.Visible = false;

                button3.Visible = true;


                filesToDownload.Clear();
                missingFiles.Clear();
                existingFiles.Clear();
                bytesOfFiles.Clear();
                needToDownload.Clear();
                folderPath.Clear();
                missingFolders.Clear();
            }
            else
            {
                MessageBox.Show("Error!");
                return;
            }
        }




        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            long totalbytes = e.TotalBytesToReceive / 1024 / 1024;
            long totalbytesKB = e.TotalBytesToReceive / 1024;
            long bytes = e.BytesReceived / 1024 / 1024;
            long gbbytes = e.BytesReceived / 1024 / 1024 / 1024;
            long totalbytesGB = e.TotalBytesToReceive / 1024 / 1024 / 1024;
            long bytesKB = e.BytesReceived / 1024;
            if (e.BytesReceived >= 999)
            {
                label5.Text = bytes.ToString() + " / " + totalbytes.ToString() + " MB ";
            }
            else if (e.BytesReceived < 999)
            {
                label5.Text = bytesKB.ToString() + " / " + totalbytesKB.ToString() + " KB ";
            }
            else if (e.BytesReceived >= 9999)
            {
                label5.Text = gbbytes.ToString() + " / " + totalbytesGB.ToString() + " GB ";
            }
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {


            needToDownload.RemoveAt(0);
            fileCounter++;

            if (fileCounter < downloadedFileCounter)
            {
                downloader();
            }
            else
            {
                label5.Visible = false;
                label4.Visible = false;
                label6.Visible = false;
                label1.Text = "Download finished!";
                label5.Location = new Point(353, 411);
                progressBar1.Value = 100;
                button2.Visible = true;

                //textBox1.Enabled = true;
                //textBox2.Enabled = true;

                //button1.Enabled = true;
                //button2.Enabled = true;

                button1.Visible = false;
                button2.Visible = false;

                textBox1.Visible = false;
                textBox2.Visible = false;

                label2.Visible = false;
                label3.Visible = false;

                button3.Visible = true;




                filesToDownload.Clear();
                missingFiles.Clear();
                existingFiles.Clear();
                bytesOfFiles.Clear();
                needToDownload.Clear();
                folderPath.Clear();
                missingFolders.Clear();

            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {               
            timer1.Stop();
            timer1.Enabled = false;
            this.Controls.Remove(loadingGif);
            this.FormBorderStyle = FormBorderStyle.Sizable;

            pictureBox1.Visible = true;
            pictureBox2.Visible = true;
            pictureBox3.Visible = true;
            pictureBox4.Visible = true;
            pictureBox5.Visible = true;

          
            label2.Visible = true;
            label3.Visible = true;

            //  label1.Visible = true;
            //  label4.Visible = true;
            //   label5.Visible = true;
            //    label6.Visible = true;
            //   progressBar1.Visible = true;
            label8.Visible = true;

            textBox1.Visible = true;
            textBox2.Visible = true;

            button1.Visible = true;
            button2.Visible = true;

     
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.facebook.com/Eaglesoulgamer");
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.youtube.com/channel/UCqVyR56F_64Trdr3R5fs4YQ");
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Process.Start("https://invite.gg/eaglesoul");
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + "\\Client.exe");
        }

        private void button1_Click(object sender, EventArgs e)  //login process
        {
            //button1.Visible = false;
            //button2.Visible = false;

            //textBox1.Visible = false;
            //textBox2.Visible = false;

            //label2.Visible = false;
            //label3.Visible = false;

            //button3.Visible = true;

            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Please enter username!");
            }
            else
            {
               
                if (textBox1.Text == "" || textBox2.Text == "")
                {
                    MessageBox.Show("Please enter username!");
                }
                else
                {
                    string checkLogin = DbConnector.login(textBox1.Text,textBox2.Text);                 
                    if (checkLogin == "1")
                    {
                        MessageBox.Show("Successfully logedin!");


                        button1.Visible = false;
                        button2.Visible = false;

                        textBox1.Visible = false;
                        textBox2.Visible = false;

                        label2.Visible = false;
                        label3.Visible = false;


                        label1.Visible = true;
                        label4.Visible = true;
                        label5.Visible = true;
                        label6.Visible = true;
                        progressBar1.Visible = true;


                        gettingsFiles();


                    }
                    else
                    {
                        MessageBox.Show("Something went wrong!");
                    }
                }
            }
          
        }


      
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Please enter username!");
            }
            else
            {
                          
                string checkRegister = DbConnector.register(textBox1.Text, textBox2.Text);
                if (checkRegister == "1")
                {
                    MessageBox.Show("Successfully registered!");
                    button1.Visible = false;
                    button2.Visible = false;

                    textBox1.Visible = false;
                    textBox2.Visible = false;

                    label2.Visible = false;
                    label3.Visible = false;


                    label1.Visible = true;
                    label4.Visible = true;
                    label5.Visible = true;
                    label6.Visible = true;
                    progressBar1.Visible = true;

                    gettingsFiles();

                }
                else
                {
                    MessageBox.Show("Something went wrong!");
                }
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }
    }
}
