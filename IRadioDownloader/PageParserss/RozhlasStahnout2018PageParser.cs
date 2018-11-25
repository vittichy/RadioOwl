using Dtc.Common.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using RadioOwl.PageParsers.Base;
using RadioOwl.PageParsers.Data;
using RadioOwl.Radio;
using System;
using System.Linq;
using System.Threading.Tasks;
using vt.Http;

namespace RadioOwl.PageParsers
{
    public class RozhlasStahnout2018PageDecoder : PageDecoderBase, IPageDecoder
    {
        public RozhlasStahnout2018PageDecoder(IPageDecoder next = null) : base(next) { }

        public async Task<IPageDecoder> CanDecodeUrl(string url)
        {
            if (RadioHelpers.IsUrlToPrehrat2018(url))
                return this;

            return await NextDecoder?.CanDecodeUrl(url);
        }


        public Task<ParserResult> Decode(string url)
        {
            return TryDecodeUrl(url);
        }

        private async Task<ParserResult> TryDecodeUrl(string url)
        {
           

            var asyncDownloader = new AsyncDownloader();
            var downloader = await asyncDownloader.GetString(url);
            if (downloader.DownloadOk)
            {
                 return ParsePrehrat2018Html(downloader.Output);
            }
            else
            {
                return new ParserResult().AddLog($"Nepodařilo se stažení: '{url}'");
            }
        }


        private ParserResult ParsePrehrat2018Html(string html)
        {
            var result = new ParserResult();

            // TODO zde muze byt vice url ... takze to nejak doplnit do toho gridu? a pocitat s tim, ze jich bude vice
            // - mozna jen upravit fileRow.url na list? 
            // TODO - stare musim ponechat! aby slo stahovat stare veci!
            try
            {
                // html nemusi byt validni xml, takze je potreba pro parsovani pouzit Html Agility Pack
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // get all  <script> under <head>
                var xpathNodes = htmlDoc.DocumentNode.SelectNodes(@"//head//script");

                if (xpathNodes != null && xpathNodes.Any())
                {
                    var drupalSettingsJson = xpathNodes.FirstOrDefault(p => p.InnerText.Contains("jQuery.extend(Drupal.settings"))?.InnerText;
                    if (!string.IsNullOrEmpty(drupalSettingsJson))
                    {
                        // select inner json data from <script> element
                        var json = drupalSettingsJson.RemoveStartTextTo('{').RemoveEndTextTo('}');
                        json = "{" + json + "}";

                        var jObject = JObject.Parse(json);

                        //  ajaxPageState "soundmanager2":{
                        var downloadItem = jObject.SelectToken("soundmanager2.download");
                        if (downloadItem != null)
                        {
                            foreach (JToken item in downloadItem.Children())
                            {
                                // takhle to vypada: "https://region.rozhlas.cz/sites/default/files/audios/68919bf46b77f6246089a1dd38b35bf9.mp3": "https://region.rozhlas.cz/audio-download/sites/default/files/audios/68919bf46b77f6246089a1dd38b35bf9-mp3"
                                // mp3 se da stahnout z obou url ... zatim tedy budu pouzivat ten prvni
                                var urlToken = item.ToString();
                                if (!string.IsNullOrEmpty(urlToken))
                                {
                                    var urlSet = urlToken.Split('"');
                                    if (urlSet.Count() > 2)
                                    {
                                        result.AddUrl(urlSet[1], ""); // TODO Description? atd
                                    }
                                }
                            }
                        }
                        if (!result.RozhlasUrlSet.Any())
                        {
                            result.AddLog("Chyba při parsování html - nepodařilo se dohledat seznam url z json dat.");
                        }

                        // TOD druhy zpusob
                    }
                    else
                    {
                        result.AddLog("Chyba při parsování html - nepodařilo se dohledat 'Drupal.Setings' json data.");
                    }
                }
                else
                {
                    result.AddLog("Chyba při parsování html - nepodařilo se dohledat //head//script nody.");
                }
            }
            catch (Exception ex)
            {
                result.AddLog(string.Format("ParsePrehrat2018Html error: {0}.", ex.Message));
            }

            return result;
        }
    }
}
