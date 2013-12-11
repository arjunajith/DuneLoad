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
using System.IO;


namespace WpfApplication1
{
    public partial class MainWindow
    {
       // public WebClient client1;
        FileDownload down1;

        public MainWindow()
        {
            InitializeComponent();
            //client1 = new WebClient();
            
        }

        private void onclick(object sender, RoutedEventArgs e)
        {
            Uri url1 = new Uri(URL.Text);
            string fileName = Locat.Text + System.IO.Path.GetFileName(url1.LocalPath);
            down1 = new FileDownload(URL.Text, fileName, 100);
            
            abc.Text = down1.GetContentLength().ToString();

            //DownloadFileAsync method.
            //client1.DownloadFileCompleted += (sender2, e2) =>
            //    {
            //        if (e2.Cancelled)
            //        {
            //            progbar.Value = 0;
            //            abc.Text = "Download Aborted by User";
            //        }
            //        else
            //            abc.Text = "Download Complete!";
            //    };
            //client1.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback); 
            //client1.DownloadFileAsync(url1 , fileName);

        }
        //public void DownloadCompleteCallBack(object sender, DownloadDataCompletedEventArgs e)
        //{ }
        //public void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        //{
        //   // textb_temp.Text = e.ProgressPercentage.ToString();
        //    progbar.Minimum = 0;
        //    progbar.Maximum = 100;
        //    progbar.Value = e.ProgressPercentage;
        //}

        private void canc(object sender, RoutedEventArgs e)
        {
            down1.Stop();
            //client1.CancelAsync();
        }

    }

    public class FileDownload
    {
        private volatile bool _allowedToRun;
        private volatile bool _notstop;
        private string _source;
        private string _destination;
        private int _chunkSize;
        private Lazy<int> _contentLength;
        public int BytesWritten { get; private set; }
        public int ContentLength { get { return _contentLength.Value; } }
        public bool Done { get { return ContentLength == BytesWritten; } }
        public FileDownload(string source, string destination, int chunkSize)
        {
            _allowedToRun = true;
            _notstop = true;
            _source = source;
            _destination = destination;
            _chunkSize = chunkSize;
            _contentLength = new Lazy<int>(() => Convert.ToInt32(GetContentLength()));
            BytesWritten = 0;
        }
        public long GetContentLength()
        {
            var request = (HttpWebRequest)WebRequest.Create(_source);
            request.Method = "HEAD";
            using (var response = request.GetResponse())
                return response.ContentLength;
        }
        private async Task Start(int range)
        {
            if (!_allowedToRun)
                throw new InvalidOperationException();
            var request = (HttpWebRequest)WebRequest.Create(_source);
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            request.AddRange(range);
            using (var response = await request.GetResponseAsync())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var fs = new FileStream(_destination, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        while (_allowedToRun && _notstop)
                        {
                            var buffer = new byte[_chunkSize];
                            var bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                            if (bytesRead == 0) break;
                            await fs.WriteAsync(buffer, 0, bytesRead);
                            BytesWritten += bytesRead;
                            if (!_notstop)
                            {
                                break;
                            }
                        }
                        
                        await fs.FlushAsync();
                    }
                }
            }
        }
        public void Stop()
        {
            _notstop = false;
        }
        public Task Start()
        {
            _allowedToRun = true;
            return Start(BytesWritten);
        }
        public void Pause()
        {
            _allowedToRun = false;
        }
    }

}
