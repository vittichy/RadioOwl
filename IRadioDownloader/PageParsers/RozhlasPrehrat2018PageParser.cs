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
    public class RozhlasPrehrat2018PageParser : PageParserBase, IPageParser
    {
        public RozhlasPrehrat2018PageParser(IPageParser next = null) : base(next) { }


        /// <summary>
        /// Odkaz 'Přehrát' verze 2018-11
        /// http://plus.rozhlas.cz/host-galeristka-a-kuratorka-jirina-divacka-7671850?player=on#player
        /// http://region.rozhlas.cz/malebne-vlakove-nadrazi-v-hradku-u-susice-se-dostalo-mezi-deset-nejkrasnejsich-u-7671216?player=on#player
        /// http://radiozurnal.rozhlas.cz/pribeh-stoleti-7627378#dil=99?player=on#player
        /// http://dvojka.rozhlas.cz/miroslav-hornicek-petatricet-skvelych-pruvanu-a-jine-povidky-7670628#dil=2?player=on#player
        /// </summary>
        public override bool CanParseCondition(string url)
        {
            return !string.IsNullOrEmpty(url)
                            && url.StartsWith(@"http://", StringComparison.InvariantCultureIgnoreCase)
                            && url.Contains(@".rozhlas.cz/")
                            && url.EndsWith(@"?player=on#player", StringComparison.InvariantCultureIgnoreCase);
        }


        //public async Task<ParserResult> Parse(string url)
        //{
        //    var html = await DownloadHtml(url);

        //    if (string.IsNullOrEmpty(html))
        //        return new ParserResult(null).AddLog($"Nepodařilo se stažení hlavní stránky: '{url}'"); // no source html - no fun
        //    //Parse(parserResult);
        //    else
        //        return ParseHtml(html); // try to parse
        //}


        //        private async Task<string> DownloadHtml(string url)
        //        {
        //            var asyncDownloader = new AsyncDownloader();
        //            var downloaderOutput = await asyncDownloader.GetString(url);
        //            return downloaderOutput.DownloadOk ? downloaderOutput.Output : null;


        //            {
        //                //return new ParserResult(downloaderOutput.Output);
        //            }
        ////            return new ParserResult(null).AddLog($"Nepodařilo se stažení hlavní stránky: '{url}'");
        //        }


        //private async Task<ParserResult> TryDecodeUrl(string url)
        //{
        //    var asyncDownloader = new AsyncDownloader();
        //    var downloader = await asyncDownloader.GetString(url);
        //    if (downloader.DownloadOk)
        //    {
        //        return Parse(downloader.Output);
        //    }
        //    else
        //    {
        //        return new ParserResult().AddLog($"Nepodařilo se stažení: '{url}'");
        //    }
        //}


        public override ParserResult ParseHtml(string html)
        {
            var parserResult = new ParserResult(html);
            try
            {
                // html nemusi byt validni xml, takze je potreba pro parsovani pouzit Html Agility Pack
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(parserResult.SourceHtml);

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
                        var downloadItem = jObject.SelectToken("soundmanager2.playtime");
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
                                        parserResult.AddUrl(urlSet[1], "");
                                    }
                                }
                            }
                        }

                        // nektere 'prehrat' html stranky nemaji prehravac s json daty a mp3 url musim dohledat jinde ve strance
                        if (!parserResult.RozhlasUrlSet.Any())
                        {
                            // najit prislusny div
                            var parentDiv = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@aria-labelledby='Audio part']");
                            // pod nim by mel byt jeden <a> s href atributem - url k mp3
                            if (parentDiv != null)
                            {
                                var aHref = parentDiv.ChildNodes.FirstOrDefault(p => p.Name == "a")?.Attributes["href"]?.Value;
                                if (!string.IsNullOrEmpty(aHref))
                                {
                                    parserResult.AddUrl(aHref, null);
                                }
                            }
                        }

                        // po vsechn pokusech nic nenalezeno?
                        if (parserResult.RozhlasUrlSet.Any())
                        {
                            // title jen vykousnu ze stranky
                            GetTitleFromH1(htmlDoc, ref parserResult);
                        }
                        else
                        {
                            parserResult.AddLog("Chyba při parsování html - nepodařilo se dohledat seznam url z json dat.");
                        }
                    }
                    else
                    {
                        parserResult.AddLog("Chyba při parsování html - nepodařilo se dohledat 'Drupal.Setings' json data.");
                    }
                }
                else
                {
                    parserResult.AddLog("Chyba při parsování html - nepodařilo se dohledat //head//script nody.");
                }
            }
            catch (Exception ex)
            {
                parserResult.AddLog($"ParsePrehrat2018Html error: {ex.Message}.");
            }

            return parserResult;
        }


        /// <summary>
        /// dohledani informaci o poradu z meta tagu html
        /// </summary>
        private void GetTitleFromH1(HtmlDocument htmlDoc, ref ParserResult parserResult)
        {
            // TODO description ke vsemu stejne? nebo se podari vykousat jednotlive dily?

            var title = GetMetaTagContent(htmlDoc, @"//meta[@property='og:title']");
            // <meta property="og:description" content="Poslechněte si oblíbené poetické texty básníka a publicisty Milana Šedivého." />
            var description = GetMetaTagContent(htmlDoc, @"//meta[@property='og:description']");
            // <meta property="og:site_name" content="Vltava" />
            var siteName = GetMetaTagContent(htmlDoc, @"//meta[@property='og:site_name']");

            parserResult.RozhlasUrlSet.ForEach(
                p => 
                {
                    p.Title = title;
                    p.Description = description;
                    p.SiteName = siteName;
                }
            );
        }


        private string GetMetaTagContent(HtmlDocument htmlDoc, string xPath)
        {
            var xpathNodes = htmlDoc.DocumentNode.SelectNodes(xPath);
            var contentAttribute = xpathNodes?.FirstOrDefault()?.Attributes["content"]?.Value;
            return contentAttribute;
        }
    }
}
