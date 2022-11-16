using Dtc.Common.Extensions;
using Dtc.Html.Html;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using RadioOwl.Parsers.Data;
using RadioOwl.Parsers.Data.Factory;
using RadioOwl.Parsers.Parser.Base;
using RadioOwl.Parsers.Parser.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RadioOwl.Parsers.Parser
{
    /// <summary>
    /// Parser pro stránku https://www.mujrozhlas.cz/ 
    /// - verze 2022 nektere url starym parserem nejdou nacist, napr: https://www.mujrozhlas.cz/podvecerni-cteni/jan-hornicek-carostrelec-historicka-detektivka-severskeho-strihu-ze-zasnezenych
    /// 
    /// Postup:
    /// 1) stahnout html
    /// 2) dohledat 
    /// </summary>
    public class MujRozhlas2022Parser : PageParserBase
    {
        /// <inheritdoc/>
        public override int Version => 1;

        /// <inheritdoc/>
        public override string[] ParseUrls { get { return new string[] { "mujrozhlas.cz" }; } }

        /// <inheritdoc/>
        public override async Task<bool> ParseAsync(RadioData radioData)
        {
            await DownloadHtmlAsync(radioData);

            try
            {
                // html nemusi byt validni xml, takze je potreba pro parsovani pouzit Html Agility Pack
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(radioData.HtmlPage);

                // RID nelze zjistit pouze z url poradu, napr 'https://www.mujrozhlas.cz/lide/martin-c-putna' zadne RID nevraci!
                var rid = GetRID(radioData.HtmlPage);
                if (string.IsNullOrEmpty(rid))
                    throw new Exception("Nepodařilo se dohledat RID.");
                radioData.AddLog($"Dohledáno RID: {rid}");

                // načtení hlavního popisu k pořadu
                var detailDescription = htmlDoc.DocumentNode.SelectSingleNode(@".//div[@class='b-detail__description']//p").InnerText;
                radioData.DetailDescription = HtmlEntity.DeEntitize(detailDescription);

                // zde asi jedine misto, kde zjistim pocet epizod?
                // dohledat <script> pod <div> kde je to jako kus JS zdrojaku s definovanou JSON promennou, kterou z toho zkusim vykousnou
                var mujRozhlas2020SiteInfo = GetContentSerialAllParts(htmlDoc);

                if (string.IsNullOrEmpty(mujRozhlas2020SiteInfo?.SiteEntityBundle))
                {
                    radioData.AddLog($"Nedohledáno SiteEntityBundle:'{mujRozhlas2020SiteInfo.ContentId}");
                }
                radioData.AddLog($"Dohledáno SiteEntityBundle:'{mujRozhlas2020SiteInfo.SiteEntityBundle}', ContentSerialAllParts:'{mujRozhlas2020SiteInfo.ContentSerialAllParts}'");

                SiteInfoToRadioData(radioData, mujRozhlas2020SiteInfo);

                switch (mujRozhlas2020SiteInfo.SiteEntityBundle)
                {
                    case "episode":
                        // neni serial, jen jednodilny porad, ContentSerialAllParts je null
                        await ParseEpisodeAsync(radioData, mujRozhlas2020SiteInfo);
                        break;
                    case "show":
                        // napr Spirituala https://www.mujrozhlas.cz/spirituala - stranka obsahuje jednotlive dily 
                        await ParseShowBundleAsync(radioData, mujRozhlas2020SiteInfo, rid);
                        break;
                    case "serial":
                        // napr cetba s hvezdickou: https://www.mujrozhlas.cz/cetba-s-hvezdickou/zenska-na-1000deg-drsna-i-humorna-zpoved-prezidentske-vnucky-z-islandu
                        await ParseSerialAsync(radioData, rid, mujRozhlas2020SiteInfo);
                        break;
                    default:
                        throw new Exception($"Unknown SiteEntityBundle:'{mujRozhlas2020SiteInfo.SiteEntityBundle}'");
                }

                radioData.ImageUrl = TryFindImage(htmlDoc);

                return true;
            }
            catch (Exception ex)
            {
                radioData.AddLogError($"ParseAsync error: {ex.Message}.");
                return false;
            }
        }

        /// <summary>
        /// Zkusím dohledat obrazek k pořadu
        /// </summary>
        private string TryFindImage(HtmlDocument htmlDoc)
        {
            var imgNode = htmlDoc.DocumentNode.SelectSingleNode(@".//figure[@class='b-detail__img']//img");
            var imgUrl = imgNode?.Attributes["src"]?.Value;
            return string.IsNullOrEmpty(imgUrl) ? null : $"https://www.mujrozhlas.cz{imgUrl}";
        }

        private async Task ParseShowBundleAsync(RadioData radioData, MujRozhlas2020SiteInfo mujRozhlas2020SiteInfo, string rid)
        {
            // ziskat data pro seznam dalsich epizod poradu - stejny postup jako kdyz dam nacist dalsi na strance poradu
            // https://www.mujrozhlas.cz/ajax/ajax_list/show?page=1&size=9&id=show-577096&rid=577096
            var pageUrl = $@"https://www.mujrozhlas.cz/ajax/ajax_list/show?page=1&size=9&id=show-{rid}&rid={rid}";
            var jsonDataForPart = await DownloadHtmlAsync(pageUrl);
            var jsonDataForPartDecoded = System.Net.WebUtility.HtmlDecode(jsonDataForPart);
            var JsonForPart = JObject.Parse(jsonDataForPartDecoded);
            var jsonContetnForPart = JsonForPart.SelectToken($"$.snippets.show-{rid}.content");
            // zde mam html data pro nove nactene porady
            var jsonContetnForPartX = jsonContetnForPart?.Value<string>();
            // z nich musim vykousnout jednotlive uuid pro jednotlive mp3 dily
            var partHtml = new HtmlDocument();
            partHtml.LoadHtml(jsonContetnForPartX);
            // jedna se o seznam <article> tagu
            var articlePartSet = partHtml.DocumentNode.SelectNodes(@"//article[@class='b-episode']")?.ToList();
            if (articlePartSet == null)
                throw new Exception($"Article set  - nepodařilo se dohledat //article[@class='b-episode'].");
            if (articlePartSet == null)
                throw new Exception($"Article set - je prázdný.");
            // zajima mne data-entry atribut u jednotlivych <article>
            foreach (var articleTag in articlePartSet)
            {
                var dataEntryValue = articleTag
                                        .Attributes
                                            ?.FirstOrDefault(p => p.Name == "data-entry")
                                                ?.Value;

                var dataEntryValueEnc = System.Net.WebUtility.HtmlDecode(dataEntryValue);
                if (string.IsNullOrEmpty(dataEntryValueEnc))
                    throw new Exception($"Part  - hodnota dataEntryValueEnc je prázdná.");

                var jsonDataEntry = JObject.Parse(dataEntryValueEnc);

                // hlavni ID dílu seriálu
                var partUuid = jsonDataEntry.SelectToken("$.uuid")?.Value<string>();
            }
        }

        private async Task ParseSerialAsync(RadioData radioData, string rid, MujRozhlas2020SiteInfo mujRozhlas2020SiteInfo)
        {
            if (!mujRozhlas2020SiteInfo.ContentSerialAllParts.HasValue)
                throw new Exception($"Nepodařilo se dohledat ContentSerialAllParts, pro SiteEntityBundle:'{mujRozhlas2020SiteInfo.SiteEntityBundle}'.");

            var allPartsHtml = await GetShowPartsAllAsync(rid);
            if (allPartsHtml == null)
                throw new Exception($"Nepodařilo se dohledat díly pořadu. RID={rid},SiteEntityBundle:'{mujRozhlas2020SiteInfo.SiteEntityBundle}'.");
            if (!allPartsHtml.Any())
                throw new Exception($"Neexistují díly pořadu. RID={rid},SiteEntityBundle:'{mujRozhlas2020SiteInfo.SiteEntityBundle}'.");

            // ID jednotlivých částí
            var partUuidSet = GetPartIdAll(allPartsHtml);

            // z ID jednotlivých částí už mohu přes API stáhnout JSON s finalními daty pro konkrétní část
            foreach (var partUuid in partUuidSet)
            {
                await GetAudioLink(radioData, partUuid);
            }
        }

        /// <summary>
        /// Momentalne to vypada, ze vsechny dily poradu pujdou stahnout pres 1 dotaz, tak jako to dela web stranka pri kliknuti
        /// na "Dalsi dily" - tam se ajax dotaz vola s parametry page=1 size=9 (na strance je zobrazeno prvnich 9 poradu), timhle
        /// se donacte zbytek.
        /// Pokusne zjistuju, ze pri zaslani page=0, size=99 api neprotestuje a zasle vsechny dily najednou - bohuzel jako html stranku.
        /// </summary>
        internal async Task<HtmlNodeCollection> GetShowPartsAsync(string rid, int page, int size)
        {
            var ajaxListUrl = $@"https://www.mujrozhlas.cz/ajax/ajax_list/serial?page={page}&size={size}&id=serial-{rid}&rid={rid}";

            var ajaxList = await DownloadHtmlAsync(ajaxListUrl);
            var ajaxListDecoded = System.Net.WebUtility.HtmlDecode(ajaxList);
            var ajaxListJson = JObject.Parse(ajaxListDecoded);
            var contentTag = ajaxListJson.SelectToken($"$.snippets.serial-{rid}.content")?.Value<string>();
            var contentTagDecodede = System.Net.WebUtility.HtmlDecode(contentTag);

            // selector pro vyber tagu jednotlivych dilu
            var articleTagSelector = @".//article[@class='b-episode']";

            var html = new HtmlDocument();
            html.LoadHtml(contentTagDecodede);
            var htmlArticleSet = html.DocumentNode.SelectNodes(articleTagSelector);
            return htmlArticleSet;
        }

        /// <summary>
        /// Stahne vsechny (prvnich 99) dilu poradu
        /// </summary>
        internal async Task<HtmlNodeCollection> GetShowPartsAllAsync(string rid)
        {
            return await GetShowPartsAsync(rid, 0, 99);
        }

        /// <summary>
        /// Z html casti pro dil poradu se pokusi vykousnout ID jednotlive casti
        /// </summary>
        internal string GetPartId(HtmlNode articleTag)
        {
            var dataEntryAttributeHtml = articleTag.GetAttributeValue("data-entry", string.Empty);
            if (string.IsNullOrWhiteSpace(dataEntryAttributeHtml))
                throw new Exception($"Chyba při parsování html - 'data-entry' is empty.");

            var dataEntryAttributeValue = System.Net.WebUtility.HtmlDecode(dataEntryAttributeHtml);

            var jsonDataEntry = JObject.Parse(dataEntryAttributeValue);

            // hlavni ID dílu seriálu
            var partUuid = jsonDataEntry.SelectToken("$.uuid")?.Value<string>();
            return partUuid;
        }

        /// <summary>
        /// Z html casti pro dil poradu se pokusi vykousnout ID jednotlive casti
        /// </summary>
        internal List<string> GetPartIdAll(HtmlNodeCollection htmlCollection)
        {
            return htmlCollection.Select(p => GetPartId(p)).ToList();
        }

        private async Task GetAudioLink(RadioData radioData, string partUuid)
        {
            var episodeJson = await ApiMujRozhlasGetEpisodesJsonAsync(partUuid);

            // guid
            var partDataId = episodeJson.SelectToken("$.data.id")?.Value<string>();
            // melo by byt episode?
            var partDataType = episodeJson.SelectToken("$.data.type")?.Value<string>();
            // 
            var partNo = episodeJson.SelectToken("$.data.attributes.part")?.Value<int>();
            // 
            var partTitle = episodeJson.SelectToken("$.data.attributes.title")?.Value<string>();
            //
            var partShortTitle = episodeJson.SelectToken("$.data.attributes.shortTitle")?.Value<string>();
            // 
            var partDescriptionHtml = episodeJson.SelectToken("$.data.attributes.description")?.Value<string>();
            var partDescription = new HtmlHelper().StripHtmlTags(partDescriptionHtml);

            // samotne url k mp3
            var partAudioLink = episodeJson.SelectToken("$.data.attributes.audioLinks[0].url")?.Value<string>();

            new RadioDataPartFactory().Create(radioData, partNo, partTitle, partDescription, partAudioLink);
        }

        /// <summary>
        /// Stáhne pře API json data ke konkrétnímu pořadu
        /// </summary>
        private async Task<JObject> ApiMujRozhlasGetEpisodesJsonAsync(string partUuid)
        {
            //priklad: https://api.mujrozhlas.cz/episodes/6d34cb00-fb37-3aaf-8037-6c2c5ba0deb0

            var urlPart = $@"https://api.mujrozhlas.cz/episodes/{partUuid}";
            var urlPartInfo = await DownloadHtmlAsync(urlPart);

            // unescapovat unicode znaky typu "\u003Cp\u003E\u010cetbu na pokra\u010dov\u00e1n\u00ed ze..."
            // viz https://stackoverflow.com/questions/9303257/how-to-decode-a-unicode-character-in-a-string
            var urlPartInfoUnescaped = System.Text.RegularExpressions.Regex.Unescape(urlPartInfo);

            // faq dotazy na json: https://www.newtonsoft.com/json/help/html/QueryJsonSelectTokenJsonPath.htm
            var partJson = JObject.Parse(urlPartInfoUnescaped);
            return partJson;
        }

        private static void SiteInfoToRadioData(RadioData radioData, MujRozhlas2020SiteInfo mujRozhlas2020SiteInfo)
        {
            radioData.SiteDocumentPath = mujRozhlas2020SiteInfo.SiteDocumentPath;
            radioData.SiteEntityBundle = mujRozhlas2020SiteInfo.SiteEntityBundle;
            radioData.SiteEntityLabel = mujRozhlas2020SiteInfo.SiteEntityLabel;
            radioData.ContentSerialAllParts = mujRozhlas2020SiteInfo.ContentSerialAllParts;
            radioData.ContentId = mujRozhlas2020SiteInfo.ContentId;
        }

        private async Task ParseEpisodeAsync(RadioData radioData, MujRozhlas2020SiteInfo mujRozhlas2020SiteInfo)
        {
            if (radioData is null) throw new ArgumentNullException(nameof(radioData));
            if (mujRozhlas2020SiteInfo is null) throw new ArgumentNullException(nameof(mujRozhlas2020SiteInfo));

            if (mujRozhlas2020SiteInfo.SiteEntityBundle != "episode")
                throw new NotSupportedException("Pouze pro typ pořadu bundle=episode!");

            var episodeJsonUrl = $@"https://api.mujrozhlas.cz/episodes/{mujRozhlas2020SiteInfo.ContentId}";

            var episodeJsonCode = await DownloadHtmlAsync(episodeJsonUrl);

            // viz https://stackoverflow.com/questions/9303257/how-to-decode-a-unicode-character-in-a-string
            // var urlPartInfoUnescaped = System.Text.RegularExpressions.Regex.Unescape(urlPartInfo);

            var episodeJson = JObject.Parse(episodeJsonCode);

            if (episodeJson != null)
            {
                //  var episodeMp3 = episodeJson.SelectToken("$.data.audioLinks[0].url")?.Value<string>();

                var partNo = 0; // jen jedn epizoda  
                // 
                var partTitle = episodeJson.SelectToken("$.data.attributes.title")?.Value<string>();
                //
                var partShortTitle = episodeJson.SelectToken("$.data.attributes.shortTitle")?.Value<string>();
                // 
                var partDescriptionHtml = episodeJson.SelectToken("$.data.attributes.description")?.Value<string>();
                var partDescription = new HtmlHelper().StripHtmlTags(partDescriptionHtml);
                // samotne url k mp3
                var partAudioLink = episodeJson.SelectToken("$.data.attributes.audioLinks[0].url")?.Value<string>();

                new RadioDataPartFactory().Create(radioData, partNo, partTitle, partDescription, partAudioLink);
            }
        }

        /// <summary>
        /// zde asi jedine misto, kde zjistim pocet epizod?
        /// dohledat <script> pod <div> kde je to jako kus JS zdrojaku s definovanou JSON promennou, kterou z toho zkusim vykousnou
        /// </summary>
        private MujRozhlas2020SiteInfo GetContentSerialAllParts(HtmlDocument htmlDoc)
        {
            var divPageMain = htmlDoc.DocumentNode.SelectNodes(@".//div[@class='page__main']//script");
            if (divPageMain == null)
                throw new Exception("Chyba při parsování html - nepodařilo se dohledat seznam 'div[@class='page__main']'.");
            if (!divPageMain.Any())
                throw new Exception("Chyba při parsování html - podařilo se dohledat seznam 'article[@class='b-episode']' - seznam je však prázdný.");

            var pageMainScript = divPageMain.FirstOrDefault().InnerHtml?.SubstrFromToChar('{', '}');

            var mainJson = System.Net.WebUtility.HtmlDecode(pageMainScript);
            var jMainJson = JObject.Parse(mainJson);

            var mujRozhlas2020SiteInfo = new MujRozhlas2020SiteInfo
            {
                ContentSerialAllParts = jMainJson.SelectToken("contentSerialAllParts")?.Value<int>(),
                SiteEntityBundle = jMainJson.SelectToken("siteEntityBundle")?.Value<string>(),
                SiteEntityLabel = jMainJson.SelectToken("siteEntityLabel")?.Value<string>(),
                SiteDocumentPath = jMainJson.SelectToken("siteDocumentPath")?.Value<string>(),
                ContentId = jMainJson.SelectToken("contentId")?.Value<string>()
            };

            return mujRozhlas2020SiteInfo;
        }

        /// <summary>
        /// Potrebuju zjistit RID identifaktor poradu 
        /// - bohuzel nejde vykousnout z nejakeho jsonu atd, musim dohledat v parametru ajax volani z cele html stranky
        /// 
        /// napr:
        /// <input type="checkbox" class="checkbox__control" id="" name="" checked="checked" data-ajax="/ajax/ajax_list_redraw/serial?size=9&amp;id=serial-1239859&amp;rid=1239859">
        /// <a href="https://www.mujrozhlas.cz/ajax/ajax_list/serial?page=1&amp;size=9&amp;id=serial-1239859&amp;rid=1239859" class="more-link__link ajax">
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string GetRID(string html)
        {
            if (string.IsNullOrEmpty(html))
                return null;
            // někde je problém s crlf a pod
            html = html.Replace("\n", "").Replace("\r", "");
            var htmlParts = html.Split(new string[] { "rid=" }, StringSplitOptions.RemoveEmptyEntries);
            if (htmlParts.Length > 1)
            {
                // prvni token je zacatek html, ten mne nezajima
                for (int i = 1; i < htmlParts.Length; i++)
                {
                    // var rid = htmlParts[i]?.Trim()?.SubstrTo('"')?.Trim();

                    var rid = ReadNumber(htmlParts[i]);
                    if (!string.IsNullOrEmpty(rid))
                        return rid;
                }
            }
            return null;
        }

        private string ReadNumber(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;
            var result = string.Empty;
            for (var i = 0; i < s.Length; i++)
            {
                if (!char.IsDigit(s[i]))
                    break;
                result += s[i];
            }
            return result;
        }
    }
}
