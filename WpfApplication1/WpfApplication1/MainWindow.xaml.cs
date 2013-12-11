using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Net;
using System.ComponentModel;

namespace WpfApplication1
{
    public partial class MainWindow
    {
        public WebClient client1;

        public MainWindow()
        {
            InitializeComponent();
            client1 = new WebClient();
        }

        private void onclick(object sender, RoutedEventArgs e)
        {
            Uri url1 = new Uri(URL.Text);
            string fileName = Locat.Text + System.IO.Path.GetFileName(url1.LocalPath);
            //abc.Text = fileName;
            client1.DownloadFileCompleted += (sender2, e2) =>
                {
                    if (e2.Cancelled)
                    {
                        progbar.Value = 0;
                        abc.Text = "Download Aborted by User";
                    }
                    else
                        abc.Text = "Download Complete!";
                    //if (!String.IsNullOrWhiteSpace(e2.Error.ToString()) || e2.Error.ToString().Trim().Length != 0)
                     //   textb_temp.Text = e2.Error.ToString();
                };
            client1.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback); 
            client1.DownloadFileAsync(url1 , fileName);
        }
        public void DownloadCompleteCallBack(object sender, DownloadDataCompletedEventArgs e)
        { }
        public void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
           // textb_temp.Text = e.ProgressPercentage.ToString();
            progbar.Minimum = 0;
            progbar.Maximum = 100;
            progbar.Value = e.ProgressPercentage;
        }

        private void canc(object sender, RoutedEventArgs e)
        {
            client1.CancelAsync();
        }

    }
}
