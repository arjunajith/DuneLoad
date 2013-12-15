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
using Duneload;
using YoutubeExtractor;

namespace WpfApplication1
{
    public partial class MainWindow
    {
        private bool _allowedToRun;
        private volatile bool _notstop;
        private string _source;
        private string _destination;
        private int _chunkSize;
        private Lazy<int> _contentLength;
        public int BytesWritten { get; private set; }
        public int ContentLength { get { return _contentLength.Value; } }
        public bool Done { get { return ContentLength == BytesWritten; } }
        private bool pausecheck;
        public double percl, perc;
        Uri url1;
        string fileName;
        private bool startd;
        public bool errorstays;
        public int qual;

        public MainWindow()
        {

            InitializeComponent();
            pbuttontext.Text = "pause";
            _allowedToRun = true;
            _notstop = true;
            _source = "";
            _destination = "";
            _chunkSize = 5120;
            _contentLength = new Lazy<int>(() => Convert.ToInt32(GetContentLength()));
            BytesWritten = 0;
            pausecheck = false;
            percl = 0;
            perc = 0;
            startd = false;
            qual = 360;
        }

        private void onclick(object sender, RoutedEventArgs e)
        {
            abc.Text = "";
            _notstop = true;
            pausecheck = false;

            ////DEBUG

            //if (_allowedToRun) tex1.Text = "true";
            //else tex1.Text = "false";
            //if (_notstop) tex2.Text = "true";
            //else tex2.Text = "false";
            //if (pausecheck) tex3.Text = "true";
            //else tex3.Text = "false";
            //tex4.Text = BytesWritten.ToString();


            ////END DEBUG

            try
            {
                url1 = new Uri(URL.Text);
                fileName = Locat.Text + System.IO.Path.GetFileName(url1.LocalPath);
            }
            catch
            {
                Error1 win2 = new Error1();
                win2.ShowDialog();
                return;
            }
            _source = URL.Text;
            _destination = fileName;
            percl = GetContentLength();
            if (!File.Exists(_destination))
            {
                Start();
            }
            else
            {
                Window1 Win = new Window1();
                Win.ShowDialog();

                if (Win.check == 1)
                {
                    abc.Text = "File to be overwritten";
                    File.Delete(_destination);
                    Start();
                    abc.Text = "Done" + GetContentLength().ToString();

                }
                else
                {
                    abc.Text = "Operation Aborted";
                }

            }
            abc.Text = "Done" + GetContentLength().ToString();
            //{
            //    abc.Text = "Download Complete";
            //}
        }

        private void canc(object sender, RoutedEventArgs e)
        {
            Stop();
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
                            if (!pausecheck)
                            {
                                if (BytesWritten < percl)
                                {
                                    float inMB = BytesWritten;
                                    inMB = inMB / 1048576;
                                    double totinMB = percl;
                                    totinMB = totinMB / 1048576;
                                    abc.Text = inMB.ToString("#,##0.00") + " MB of " + totinMB.ToString("#,##0.00") +" MB";
                                    perc = (BytesWritten / percl) * 100;
                                    tb2.Text = perc.ToString().Substring(0, 4) + " %";
                                    progbar.Value = perc;
                                }
                                else
                                {
                                    perc = 0;
                                    tb2.Text = "100%";
                                    progbar.Value = 0;
                                }
                            }
                            if (!_notstop)
                            {
                                URL.Text = "";
                                progbar.Value = 0;
                                abc.Text = "";
                                tb2.Text = "";
                                BytesWritten = 0;
                                pbuttontext.Text = "pause";
                                _notstop = true;
                                _source = "";
                                _allowedToRun = true;
                                fs.Close();
                                break;
                            }

                        }
                        await fs.FlushAsync();
                    }
                }
            }
            if (Done)
            {
                abc.Text = "Download Complete";
            }
        }

        public void Stop()
        {
            startd = false;
            _notstop = false;
            URL.Text = "";
            pbuttontext.Text = "pause";
            progbar.Value = 0;
            abc.Text = "";
            tb2.Text = "";
            BytesWritten = 0;
            _source = "";
            _allowedToRun = true;

        }
        private Task Start()
        {
            startd = true;
            _allowedToRun = true;
            return Start(BytesWritten);
        }
        public void Pause1()
        {
            if (startd)
            {
                _allowedToRun = false;
                pbuttontext.Text = "resume";
            }
            else
            {
                abc.Text = "No Active Downlaod";
            }
        }

        private void Pause2(object sender, RoutedEventArgs e)
        {
            if (!pausecheck && startd)
            {
                Pause1();
                pausecheck = true;
                abc.Text = "Download Paused";
            }
            else if (pausecheck && startd)
            {
                Start();
                pausecheck = false;
                pbuttontext.Text = "pause";
                abc.Text = "Resuming";
            }
            else if (!startd)
            {
                abc.Text = "No Active Download";
            }
        }

        private void yout_button_Click(object sender, RoutedEventArgs e)
        {

            _notstop = true;
            pausecheck = false;
            videoget();  
        }

        private void videoget()
        {
            string link;
            IEnumerable<VideoInfo> videoInfos;
            try
            {
                link = youtlink.Text;
                videoInfos = DownloadUrlResolver.GetDownloadUrls(link);
            }
            catch
            {
                Error1 e1 = new Error1();
                e1.ShowDialog();
                return;
            }
            quality q1 = new quality();
            id_prog.Visibility = System.Windows.Visibility.Visible;
            q1.ShowDialog();
            qual = q1.quali;

            VideoInfo video;// = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);
            try
            {
                video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == qual);
            }
            catch
            {
                tb3.Text = "Unavailable";
                video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);
            }
            id_prog.Visibility = System.Windows.Visibility.Hidden;
            tb3.Text = video.Resolution.ToString();
            fileName = Locat.Text + video.Title + ".mp4"; //System.IO.Path.GetFileName(url1.LocalPath);
            _source = video.DownloadUrl;
            _destination = fileName;
            percl = GetContentLength();
            if (!File.Exists(_destination))
            {
                Start();
            }
            else
            {
                Window1 Win = new Window1();
                Win.ShowDialog();

                if (Win.check == 1)
                {
                    abc.Text = "File to be overwritten";
                    File.Delete(_destination);
                    Start();
                    abc.Text = "Done ";

                }
                else
                {
                    abc.Text = "Operation Aborted";
                }

            }
        }

        private void about_button_Click_1(object sender, RoutedEventArgs e)
        {
            tab3.Focus();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Locat.Text = dialog.SelectedPath + "\\";
            }
        }
        //var videoDownloader = new VideoDownloader(video, System.IO.Path.Combine("D:/Downloads", video.Title + video.VideoExtension));
        //videoDownloader.DownloadProgressChanged += (sender3, args) =>
        //    {
        //      yout_progbar.Value = args.ProgressPercentage;
        //   };
        //videoDownloader.Execute();

    }
    
}