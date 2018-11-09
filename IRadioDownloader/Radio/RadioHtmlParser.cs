using Dtc.Common.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using RadioOwl.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.Radio
{
    public class RadioHtmlParser
    {


        ///// <summary>
        ///// dohledani url k mp3 streamu na vltava like odkazu
        ///// </summary>
        //private static HtmlNode VltavaPageFindStreamUrl(HtmlDocument htmlDoc, string xPathNode)
        //{
        //    var xpathNodes = htmlDoc.DocumentNode.SelectNodes(xPathNode);
        //    if (xpathNodes != null)
        //    {
        //        // html tagy dohledany pres xPath - staci mi ten prvni ;-) vic ji ted ani neni
        //        var htmlNode = xpathNodes.FirstOrDefault();
        //        if (htmlNode != null)
        //        {
        //            // mel by mit par urovni pod sebou <a> kde mne zajima jeho href
        //            var childNodeOfType = htmlNode.GetSubNodesOfName("A").FirstOrDefault();
        //            return childNodeOfType;
        //        }
        //    }
        //    return null;
        //}


//        var vltavaAnchorNode = VltavaPageFindStreamUrl(htmlDoc, @"//div[@class='sm2-playlist-wrapper']");




        public void ParsePrehrat2018Html(string html, ref FileRow fileRow)
        {
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


                        //  ajaxPageState
                        // "soundmanager2":{

                        var downloadItem = jObject.SelectToken("soundmanager2.download");

                        // https://region.rozhlas.cz/sites/default/files/audios/ee77510acabbc23b7ecbac29f5a6abe7.mp3
                        // https://region.rozhlas.cz/audio-download/sites/default/files/audios/ee77510acabbc23b7ecbac29f5a6abe7.mp3

                        foreach (JToken item in downloadItem.Children()) //.Children())
                        {
                            var a = item.ToString();
                        }

                    }
                    else
                    {
                        fileRow.AddLog("Chyba při parsování html - nepodařilo se dohledat 'Drupal.Setings' json data.", FileRowState.Error);
                    }
                }
                else
                {
                    fileRow.AddLog("Chyba při parsování html - nepodařilo se dohledat //head//script nody.", FileRowState.Error);
                }



                //var vltavaAnchorNode = VltavaPageFindStreamUrl(htmlDoc, @"//div[@class='sm2-playlist-wrapper']");
                //if (vltavaAnchorNode != null)
                //{
                //    // povedlo se dohledat <a>, vykousnu href atribut a je to
                //    fileRow.Url = vltavaAnchorNode.GetAttributeValueByName("HREF");
                //    // ID v tomhle pripade nemam
                //    fileRow.Id = "?";
                //    // title jen vykousnu ze stranky
                //    fileRow.Title = VltavaPageFindTitle(htmlDoc, @"//meta[@property='og:title']");
                //}

                //if (string.IsNullOrEmpty(fileRow.Url))
                //{
                //    fileRow.AddLog("Chyba při parsování stránky pořadu - nepodařilo se dohledat URL streamu.", FileRowState.Error);
                //    return;
                //}
            }
            catch (Exception ex)
            {
                fileRow.AddLog(string.Format("ParsePrehrat2018Html error: {0}.", ex.Message), FileRowState.Error);
            }
        }
    }
}
