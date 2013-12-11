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
        public double percl,perc;

        public MainWindow()
        {
            InitializeComponent();
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
            //client1 = new WebClient();
            
        }

        private void onclick(object sender, RoutedEventArgs e)
        {
            abc.Text = "";
            Uri url1 = new Uri(URL.Text);
            string fileName = Locat.Text + System.IO.Path.GetFileName(url1.LocalPath);
			_source = URL.Text;
			_destination = fileName;
            percl = GetContentLength();
            if (!File.Exists(_destination))
            {
                Start();
            }
            else
            {
                abc.Text = "File already exists";
            }
            if (Done)
            {
                abc.Text = "Download Complete";
            }
        }
		
        private void canc(object sender, RoutedEventArgs e)
        {
            Stop();
            //client1.CancelAsync();
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
                                abc.Text = BytesWritten.ToString();
                                perc = (BytesWritten/percl) * 100;
                                tb2.Text = perc.ToString().Substring(0,4) + " %";
                                progbar.Value = perc;
                            }
                            if (!_notstop)
                            {
                                URL.Text = "Enter URL Here";
                                progbar.Value = 0;
                                abc.Text = "";
                                tb2.Text = "";
                                BytesWritten = 0;
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
            else if (!_notstop)
            {
                File.Delete(_destination);
                _notstop = true;

            }
        }
		
		public void Stop()
        {
            _notstop = false;
        }
        private Task Start()
        {
            _allowedToRun = true;
            return Start(BytesWritten);
        }
        public void Pause1()
        {
            _allowedToRun = false;
        }

        private void Pause2(object sender, RoutedEventArgs e)
        {
            if (!pausecheck)
            {
                Pause1();
                pausecheck = true;
                abc.Text = "Download Paused";
            }
            else
            {
                Start();
                pausecheck = false;
                abc.Text = "Resuming";

            }
        }


    }
}