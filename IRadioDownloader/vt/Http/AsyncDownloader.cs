using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace vt.Http
{
    /// <summary>
    /// jednoduche async stazeni dat z url
    /// </summary>
    public class AsyncDownloader
    {
        public async Task<AsyncDownloaderOutput<byte[]>> GetData(string url, Action<DownloadProgressChangedEventArgs> report = null)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadProgressChanged += (s, e) => report?.Invoke(e);

                    var data = await webClient.DownloadDataTaskAsync(new Uri(url));
                    var output = new AsyncDownloaderOutput<byte[]>(data);
                    return output;
                }
            }
            catch (Exception ex)
            {
                return new AsyncDownloaderOutput<byte[]>(ex);
            }
        }


        public async Task<AsyncDownloaderOutput<string>> GetString(string url, Action<DownloadProgressChangedEventArgs> report = null)
        {
            var data = await GetData(url, report);
            if (data.DownloadOk)
            {
                var result = Encoding.UTF8.GetString(data?.Output);
                return new AsyncDownloaderOutput<string>(result);
            }
            return new AsyncDownloaderOutput<string>(data.Exception);
        }
    }
}
   