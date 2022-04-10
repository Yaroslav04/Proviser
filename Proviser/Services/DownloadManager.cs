using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Proviser.Services
{
    public class DownloadManager
    {
        public delegate void AccountHandler(string message);
        public event AccountHandler Notify;
        //https://dsa.court.gov.ua/open_data_files/91509/513/8faabdb91244be394947eb26f2153a1f.csv

        public async Task Download(string _url, string _fileName)
        {
            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                await client.DownloadFileTaskAsync(new Uri(_url), _fileName);
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Notify?.Invoke($"Скачано: {e.ProgressPercentage}");
        }
    }
}


