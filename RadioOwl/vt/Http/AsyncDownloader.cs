using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace vt.Http
{
    /// <summary>
    /// jednoduche async stazeni dat z url
    /// ! vetsi pocet soubeznych spojeni je potreba nastavi rucne, napr v app.configu - connectionManagement,maxconnection="5" - default jsou pouze 2 spojeni !
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



        /// <summary>
        /// stazeni pouze casti zdrojoveho souboru (musi podporovat server na druhe strane)
        /// </summary>
        public async Task<AsyncDownloaderOutput<Stream>> GetDataRange(string url, long? rangeFrom, long? rangeTo)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Range = new RangeHeaderValue(rangeFrom, rangeTo);

                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var responseStream = await response.Content.ReadAsStreamAsync(); 
                    var output = new AsyncDownloaderOutput<Stream>(responseStream);
                    return output;
                }
            }
            catch (Exception ex)
            {
                return new AsyncDownloaderOutput<Stream>(ex);
            }
        }

    }
}

   