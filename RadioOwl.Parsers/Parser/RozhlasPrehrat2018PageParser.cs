using Dtc.Common.Extensions;
//using HtmlAgilityPack;
//using Newtonsoft.Json.Linq;
//using RadioOwl.PageParsers.Base;
//using RadioOwl.PageParsers.Data;
using System;
using System.Linq;

namespace RadioOwl.Parsers.Parser
{
//    public class RozhlasPrehrat2018PageParser : PageParserBase, IPageParser
//    {
//        public RozhlasPrehrat2018PageParser(IPageParser next = null) : base(next) { }


//        /// <summary>
//        /// Odkaz 'Přehrát' verze 2018-11
//        /// http://plus.rozhlas.cz/host-galeristka-a-kuratorka-jirina-divacka-7671850?player=on#player
//        /// http://region.rozhlas.cz/malebne-vlakove-nadrazi-v-hradku-u-susice-se-dostalo-mezi-deset-nejkrasnejsich-u-7671216?player=on#player
//        /// http://radiozurnal.rozhlas.cz/pribeh-stoleti-7627378#dil=99?player=on#player
//        /// http://dvojka.rozhlas.cz/miroslav-hornicek-petatricet-skvelych-pruvanu-a-jine-povidky-7670628#dil=2?player=on#player
//        /// </summary>
//        public override bool CanParseCondition(string url)
//        {
//            return !string.IsNullOrEmpty(url)
//                            && url.Contains(@".rozhlas.cz/");
//        }


//        public override ParserResult ParseHtml(string html)
//        {
//            var parserResult = new ParserResult();
//            try
//            {
//                // html nemusi byt validni xml, takze je potreba pro parsovani pouzit Html Agility Pack
//                var htmlDoc = new HtmlDocument();
//                htmlDoc.LoadHtml(html);
                
//                // hlavni title cele stranky - tj je-li vice dilu, tak spolecny nazev - nasledne bych ho mel pouzit pro folder kam budu ukladat
//                // mozna by slo pouzit i <h1>, ale zatim beru tohle:
//                // <meta property="og:title" content="Steinar Bragi: Planina" />
//                var metaTitle= htmlDoc.DocumentNode.SelectSingleNode(@"//meta[@property='og:title']");
//                if(metaTitle != null)
//                {
//                    parserResult.MetaTitle = metaTitle.Attributes.FirstOrDefault(p => p.Name == "content")?.Value?.Trim();
//                }
//                if (string.IsNullOrEmpty(parserResult.MetaTitle))
//                {
//                    parserResult.MetaTitle = "DESCRIPTION_NOT_FOUND";
//                }

//                // hlavni description cele stranky - asi nebude potreba? 
//                // <meta property="og:description" content="Sugestivní a výborně napsaný příběh o vině a nevinnosti, hranicích lidskosti a střetnutí s chladnou krutostí islandské přírody i vlastním nitrem – dílo, které v sobě spojuje prvky psychologického thrilleru a islandské lidové pověsti. Četbu na pokračování z románu současného islandského spisovatele poslouchejte on-line po dobu jednoho týdne po odvysílání." />
//                parserResult.MetaDescription = htmlDoc.DocumentNode.SelectSingleNode(@"//meta[@property='og:description']")
//                                                        ?.Attributes
//                                                            ?.FirstOrDefault(p => p.Name == "content")
//                                                                ?.Value
//                                                                    ?.Trim();

//                // <meta property="og:site_name" content="Radiožurnál" />
//                var siteNameItem = htmlDoc.DocumentNode.SelectSingleNode(@"//meta[@property='og:site_name']");
//                if(siteNameItem != null)
//                {
//                    parserResult.MetaSiteName = siteNameItem.Attributes.FirstOrDefault(p => p.Name == "content")?.Value?.Trim();
//                }
//                else
//                {
//                    parserResult.MetaSiteName = "UNKNOWN_SITE_NAME";
//                }

//                // readme o poradu
//                var divFieldBody = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@class='field body']");
//                if(divFieldBody != null)
//                {
//                    var pTextSet = divFieldBody.ChildNodes
//                                    .Where(p => p.Name?.Trim().ToUpper() == "P")
//                                        .Select(p=> p.InnerText?.Trim())
//                                            .Where(p => !string.IsNullOrEmpty(p)) 
//                                                .ToList();
//                    parserResult.ReadMeText = string.Join(Environment.NewLine, pTextSet);
//                }

//                // jednotlive podary (dily serialu) jsou pod  <div class="sm2-playlist-wrapper">  < ul class="sm2-playlist-bd">
//                // get all  <script> under <head>
//                var headScriptSet = htmlDoc.DocumentNode.SelectNodes(@"//head//script");
//                if (headScriptSet != null && headScriptSet.Any())
//                {
//                    // < div class="sm2-playlist-wrapper">


//                    //var divSetXXX = htmlDoc.DocumentNode.SelectNodes(@"//div[@part and class='sm2-row sm2-wide']");


//                    //var divPlaylistSet = htmlDoc.DocumentNode.SelectNodes(@"//div[@class='sm2-playlist-wrapper']");


//                    var mp3AnchorSet = htmlDoc.DocumentNode.SelectNodes(@"//div[@class='sm2-playlist-wrapper']//ul//li//div//a");

//                    GetUrlsFromPlayListWrapper(mp3AnchorSet, ref parserResult);

//                    if (parserResult.RozhlasUrlSet.Any())
//                        return parserResult;

//                    //                    if (divPlaylistSet != null && divPlaylistSet.Any())
//                    //                    {
//                    //                        foreach(var divPlaylistRoot in divPlaylistSet)
//                    //                        {

//                    //                          // Get(divPlaylistRoot, ref parserResult);

//                    //                            //var divSet = divPlaylistRoot.SelectNodes(@".//div[@part and class='sm2 - row sm2 - wide']");


//                    //                            //var div1 = divPlaylistRoot.ChildNodes.FirstOrDefault(p => p.Attributes["part"] != null);

//                    ////                                ...SelectNodes(@".//div[@part]");

//                    //                        }
//                    //                    }




//                    // jiny zpusob ziskani z Drupal.settings

//                    var drupalSettingsJson = headScriptSet.FirstOrDefault(p => p.InnerText.Contains("jQuery.extend(Drupal.settings"))?.InnerText;
//                    if (!string.IsNullOrEmpty(drupalSettingsJson))
//                    {
//                        // select inner json data from <script> element
//                        var json = drupalSettingsJson.RemoveStartTextTo('{').RemoveEndTextTo('}');
//                        json = "{" + json + "}";

//                        var jObject = JObject.Parse(json);

//                        //  ajaxPageState "soundmanager2":{
//                        var downloadItem = jObject.SelectToken("soundmanager2.playtime");
//                        if (downloadItem != null)
//                        {
//                            foreach (JToken item in downloadItem.Children())
//                            {
//                                // takhle to vypada: "https://region.rozhlas.cz/sites/default/files/audios/68919bf46b77f6246089a1dd38b35bf9.mp3": "https://region.rozhlas.cz/audio-download/sites/default/files/audios/68919bf46b77f6246089a1dd38b35bf9-mp3"
//                                // mp3 se da stahnout z obou url ... zatim tedy budu pouzivat ten prvni
//                                var urlToken = item.ToString();
//                                if (!string.IsNullOrEmpty(urlToken))
//                                {
//                                    var urlSet = urlToken.Split('"');
//                                    if (urlSet.Count() > 2)
//                                    {
//                                        parserResult.AddUrl(urlSet[1], "");
//                                    }
//                                }
//                            }
//                        }

//                        // nektere 'prehrat' html stranky nemaji prehravac s json daty a mp3 url musim dohledat jinde ve strance
//                        if (!parserResult.RozhlasUrlSet.Any())
//                        {
//                            // najit prislusny div
//                            var parentDiv = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@aria-labelledby='Audio part']");
//                            // pod nim by mel byt jeden <a> s href atributem - url k mp3
//                            if (parentDiv != null)
//                            {
//                                var aHref = parentDiv.ChildNodes.FirstOrDefault(p => p.Name == "a")?.Attributes["href"]?.Value;
//                                if (!string.IsNullOrEmpty(aHref))
//                                {
//                                    parserResult.AddUrl(aHref, null);
//                                }
//                            }
//                        }

//                        // po vsechn pokusech nic nenalezeno?
//                        if (parserResult.RozhlasUrlSet.Any())
//                        {
//                            // title jen vykousnu ze stranky
//                            GetTitleFromH1(htmlDoc, ref parserResult);
//                        }
//                        else
//                        {
//                            parserResult.AddLog("Chyba při parsování html - nepodařilo se dohledat seznam url z json dat.");
//                        }
//                    }
//                    else
//                    {
//                        parserResult.AddLog("Chyba při parsování html - nepodařilo se dohledat 'Drupal.Setings' json data.");
//                    }
//                }
//                else
//                {
//                    parserResult.AddLog("Chyba při parsování html - nepodařilo se dohledat //head//script nody.");
//                }
//            }
//            catch (Exception ex)
//            {
//                parserResult.AddLog($"ParsePrehrat2018Html error: {ex.Message}.");
//            }

//            return parserResult;
//        }

//        private void GetUrlsFromPlayListWrapper(HtmlNodeCollection mp3AnchorSet, ref ParserResult parserResult)
//        {
//            /*
//                <div class="sm2-playlist-wrapper">
//                      <ul class="sm2-playlist-bd">
//                                                  <li>
//												  <div part="1" class="sm2-row sm2-wide" id="file-8490384">
//						===>  					    <a href="https://vltava.rozhlas.cz/sites/default/files/audios/8823b0fd947daa76167e9014d6ed4014.mp3?uuid=5c17536947ad0">
//												        <div class="filename" title="Steinar Bragi: Planina">
//												            <div class="filename__text" title="Steinar Bragi: Planina">1. díl: Steinar Bragi: Planina</div>
//												        </div>
//												    </a>
//												  <div class="audio-info-wrap">
//												  <span class="playlist-audio-time-to-expire">
//												  <span class="caption__desktop-only">k poslechu </span>ještě 3 dny</span>
//												  <span class="playlist-audio-length">28:14</span>
//												  </div>
//												  </div>
//												  </li>              
//             */

//            if (parserResult == null)
//                return;

//            if (mp3AnchorSet != null || mp3AnchorSet.Any())
//            {
//                foreach(var mp3A in mp3AnchorSet)
//                {
//                    // each single anchor:
//                    // <a href = "https://vltava.rozhlas.cz/sites/default/files/audios/8823b0fd947daa76167e9014d6ed4014.mp3?uuid=5c17536947ad0" >
//                    //      <div class="filename" title="Steinar Bragi: Planina">
//					//          <div class="filename__text" title="Steinar Bragi: Planina">1. díl: Steinar Bragi: Planina</div>
//					//      </div>
//					// </a>
                    
//                    var url = mp3A.Attributes["href"]?.Value;

//                    var filenameTextNode = mp3A.ChildNodes.SelectMany(p => p.ChildNodes).FirstOrDefault(p => p.Attributes.Any(a => a.Name == "class" && a.Value == "filename__text"));

//                    // verze - napr cetba, serial - vice dilu
//                    var title = filenameTextNode?.InnerHtml?.Trim();
//                    if (string.IsNullOrEmpty(title))
//                    {
//                        // verze - jen jeden dil nejakeho poradu
//                        title = mp3A?.InnerText;
//                    }

//                    parserResult.AddUrl(url, title);
//                }
//            }
//            else
//            {
//                parserResult.AddLog($"ParsePrehrat2018Html - mp3AnchorSet is null.");
//            }
//        }


//        /// <summary>
//        /// dohledani informaci o poradu z meta tagu html
//        /// </summary>
//        private void GetTitleFromH1(HtmlDocument htmlDoc, ref ParserResult parserResult)
//        {
//            // TODO description ke vsemu stejne? nebo se podari vykousat jednotlive dily?

//            var title = GetMetaTagContent(htmlDoc, @"//meta[@property='og:title']");
//            // <meta property="og:description" content="Poslechněte si oblíbené poetické texty básníka a publicisty Milana Šedivého." />
//            var description = GetMetaTagContent(htmlDoc, @"//meta[@property='og:description']");
//            // <meta property="og:site_name" content="Vltava" />
//            var siteName = GetMetaTagContent(htmlDoc, @"//meta[@property='og:site_name']");

//            parserResult.RozhlasUrlSet.ForEach(
//                p => 
//                {
//                    p.Title = title;
////                    p.Description = description;
////                    p.SiteName = siteName;
//                }
//            );
//        }


//        private string GetMetaTagContent(HtmlDocument htmlDoc, string xPath)
//        {
//            var xpathNodes = htmlDoc.DocumentNode.SelectNodes(xPath);
//            var contentAttribute = xpathNodes?.FirstOrDefault()?.Attributes["content"]?.Value;

//            // dencode char such &nbsp; as well (https://stackoverflow.com/questions/6665488/htmlagilitypack-and-htmldecode)
//            var deEntitized = HtmlEntity.DeEntitize(contentAttribute);
//            return deEntitized;
//        }


//        private HtmlNode GetNodeWithAttributeValue(HtmlNodeCollection htmlNodes, string attName, string attValue)
//        {
//            if (htmlNodes != null)
//                return null;

//            //foreach(var node in htmlNodes)
//            //{
//            //    if(node != null && node.Attributes.Any(p => p.Name == attName && p.Value == attValue))
//            //    {
//            //        return node;
//            //    }
//            //}

//            return htmlNodes.FirstOrDefault(p => p.Attributes.Any(a => a.Name == attName && a.Value == attValue));



//            //return null;
//        }
//    }
}
