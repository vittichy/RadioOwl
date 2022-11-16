//using Dtc.Common.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using RadioOwl.Data;
using System;
using System.Linq;

namespace RadioOwl.Radio
{
    public class RadioHtmlParser
    {
        public Prehrat2018ParseResult ParsePrehrat2018Html(string html)
        {
            var result = new Prehrat2018ParseResult();

            try
            {
                // html nemusi byt validni xml, takze je potreba pro parsovani pouzit Html Agility Pack
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // title jen vykousnu ze stranky
                GetTitleFromH1(htmlDoc, ref result);
                                                                      
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
                                    if(urlSet.Count() > 2)
                                    {
                                        result.Urls.Add(urlSet[1]);
                                    }
                                }
                            }
                        }

                        // nektere 'prehrat' html stranky nemaji prehravac s json daty a mp3 url musim dohledat jinde ve strance
                        if (!result.Urls.Any())
                        {
                            // najit prislusny div
                            var parentDiv = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@aria-labelledby='Audio part']");
                            // pod nim by mel byt jeden <a> s href atributem - url k mp3
                            if(parentDiv != null)
                            {
                                var aHref = parentDiv.ChildNodes.FirstOrDefault(p => p.Name == "a")?.Attributes["href"]?.Value;
                                if (!string.IsNullOrEmpty(aHref))
                                {
                                    result.Urls.Add(aHref);
                                }
                            }
                        }

                        // po vsechn pokusech nic nenalezeno?
                        if (!result.Urls.Any())
                        {
                            result.Log.Add("Chyba při parsování html - nepodařilo se dohledat seznam url z json dat.");
                        }
                    }
                    else
                    {
                        result.Log.Add("Chyba při parsování html - nepodařilo se dohledat 'Drupal.Setings' json data.");
                    }
                }
                else
                {
                    result.Log.Add("Chyba při parsování html - nepodařilo se dohledat //head//script nody.");
                }
            }
            catch (Exception ex)
            {
                result.Log.Add(string.Format("ParsePrehrat2018Html error: {0}.", ex.Message));
            }

            return result;
        }

                 
        /// <summary>
        /// dohledani informaci o poradu z meta tagu html
        /// </summary>
        private void GetTitleFromH1(HtmlDocument htmlDoc, ref Prehrat2018ParseResult prehrat2018ParseResult)
        {
            prehrat2018ParseResult.Title = GetMetaTagContent(htmlDoc, @"//meta[@property='og:title']");
            // <meta property="og:description" content="Poslechněte si oblíbené poetické texty básníka a publicisty Milana Šedivého." />
            prehrat2018ParseResult.Description = GetMetaTagContent(htmlDoc, @"//meta[@property='og:description']");
            // <meta property="og:site_name" content="Vltava" />
            prehrat2018ParseResult.SiteName = GetMetaTagContent(htmlDoc, @"//meta[@property='og:site_name']");
        }


        private string GetMetaTagContent(HtmlDocument htmlDoc, string xPath)
        {
            var xpathNodes = htmlDoc.DocumentNode.SelectNodes(xPath);
            var contentAttribute = xpathNodes?.FirstOrDefault()?.Attributes["content"]?.Value;
            return contentAttribute;
        }
    }
}
