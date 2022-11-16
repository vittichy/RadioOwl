using Dtc.Common.Extensions;
//using HtmlAgilityPack;
//using Newtonsoft.Json.Linq;
//using RadioOwl.PageParsers.Base;
//using RadioOwl.PageParsers.Data;
using System;
using System.Linq;

namespace RadioOwl.Parsers.Parser
{
//    /// <summary>
//    /// odkazy s ID na konci url:
//    /// https://prehravac.rozhlas.cz/audio/4020655
//    /// ID vsak nevykousavam z url jako drive, ale otevru html kde ho vycucnu z meta tagu spolecne s dalsimi udaji
//    /// </summary>
//    public class RozhlasPrehrat2017PageParser : PageParserBase, IPageParser
//    {
//        public RozhlasPrehrat2017PageParser(IPageParser next = null) : base(next) { }

//        private const string URL_BEGINNING_PREHRAVAC = @"http://prehravac.rozhlas.cz/audio/";       // odkaz na stream - skutecnou adresu k mp3 musi vykousat z html stranky

//        private const string URL_BEGINNING_MP3_AUDIO = @"http://media.rozhlas.cz/_audio/";          // url iradio streamu kdyz uz vim ID poradu

//        private const string URL_IRADIO_MP3_DOWNLOAD_URL = URL_BEGINNING_MP3_AUDIO + @"{0}.mp3";    // url iradio streamu kdyz uz vim ID poradu


//        /// <summary>
//        /// Odkaz 'Přehrát' puvodni verze, odkazy typu:
//        /// http://prehravac.rozhlas.cz/audio/4020142
//        /// </summary>
//        public override bool CanParseCondition(string url)
//        {
//            url = TrimProtocolFromUrl(url);
//            return (!string.IsNullOrEmpty(url)
//                    && url.StartsWith(@"prehravac.rozhlas.cz/audio/", StringComparison.InvariantCultureIgnoreCase));
//        }


//        public override ParserResult ParseHtml(string html)
//        {
//            var parserResult = new ParserResult();
//            try
//            {
//                // html nemusi byt validni xml, takze je potreba pro parsovani pouzit Html Agility Pack
//                var htmlDoc = new HtmlDocument();
//                htmlDoc.LoadHtml(html);

//                // get all  <script> under <head>
//                var xpathNodes = htmlDoc.DocumentNode.SelectNodes(@"//head//script");
//                if (xpathNodes != null && xpathNodes.Any())
//                {
//                    var croplayerJson = xpathNodes.FirstOrDefault(p => p.InnerText.Contains("croplayer"))?.InnerText;
//                    if (!string.IsNullOrEmpty(croplayerJson))
//                    {
//                        // select inner json data from <script> element
//                        var json = croplayerJson.RemoveStartTextTo('{').RemoveEndTextTo('}');
//                        json = "{" + json + "}";

//                        var jObject = JObject.Parse(json);

//                        // main id for download mp3
//                        var trackId = GetJsonAttributeValue(jObject, "track");
//                        if (!string.IsNullOrEmpty(trackId))
//                        {
//                            var fullMp3Url = GetIRadioMp3Url(trackId);
//                            parserResult.AddUrl(fullMp3Url, "");

////                            parserResult.RozhlasUrlSet[0].SiteName = GetJsonAttributeValue(jObject, "station");
//                            parserResult.RozhlasUrlSet[0].Title = GetMetaTagContent(htmlDoc, @"//meta[@name='og:title']");
////                            parserResult.RozhlasUrlSet[0].Description = GetMetaTagContent(htmlDoc, @"//meta[@name='description']");
//                        }
//                        else
//                        {
//                            parserResult.AddLog("Chyba při parsování html - nepodařilo se dohledat trackID v json datech.");
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


//        private string GetMetaTagContent(HtmlDocument htmlDoc, string xPath)
//        {
//            var xpathNodes = htmlDoc.DocumentNode.SelectNodes(xPath);
//            var contentAttribute = xpathNodes?.FirstOrDefault()?.Attributes["content"]?.Value;

//            // dencode char such &nbsp; as well (https://stackoverflow.com/questions/6665488/htmlagilitypack-and-htmldecode)
//            var deEntitized = HtmlEntity.DeEntitize(contentAttribute);
//            return deEntitized;
//        }


//        string GetIRadioMp3Url(string id)
//        {
//            return string.Format(URL_IRADIO_MP3_DOWNLOAD_URL, id);
//        }


//        string GetJsonAttributeValue(JObject jObject, string attributeValue)
//        {
//            var jToken = jObject.SelectToken(attributeValue);
//            if (jToken is JValue jValue)
//            {
//                var value = jValue.Value?.ToString();
//                return value;
//            }
//            return null;
//        }
    //}
}