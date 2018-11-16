using Dtc.Common.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using RadioOwl.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RadioOwl.Radio
{
    public class RadioHtmlParser
    {
        public Prehrat2018ParseResult ParsePrehrat2018Html(string html)
        {
            var result = new Prehrat2018ParseResult();

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
                                    if(urlSet.Count() > 2)
                                    {
                                        result.Urls.Add(urlSet[1]);
                                    }
                                }
                            }
                        }
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
    }
}
