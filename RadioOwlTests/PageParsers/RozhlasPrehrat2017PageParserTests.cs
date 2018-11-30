using NUnit.Framework;
using RadioOwl.PageParsers;
using System.Linq;
using System.Threading.Tasks;

namespace RadioOwlTests.PageParsers
{
    [TestFixture]
    public class RozhlasPrehrat2017PageParserTests : TestBase
    {
        [Test]
        public async Task CanParseTests()
        {
            var parser = new RozhlasPrehrat2017PageParser();

            var result = await parser.CanParse(null);
            Assert.IsNull(result);

            result = parser.CanParse(string.Empty).Result;
            Assert.IsNull(result);

            result = parser.CanParse("xxxxxxxxxxx").Result;
            Assert.IsNull(result);

            // ok urls
            var urls = new string[]
            {
                @"https://prehravac.rozhlas.cz/audio/4020655",
            };
            urls.ToList().ForEach(p => { Assert.IsNotNull(parser.CanParse(p).Result); });

            // bad urls
            urls = new string[]
            {
                null,
                string.Empty,
                "A",
                @"google.com",
                @"http://google.com",
                @"https://google.com",
                @"https://plus.rozhlas.cz/audio-download/sites/default/files/audios/307c2f0f817ffd1b8032aa157c8559d7-mp3",
                @"https://api.rozhlas.cz/data/v2/podcast/show/6946964.rss"
            };
            urls.ToList().ForEach(async p => { Assert.IsNull(parser.CanParse(p).Result); });
        }


        [Test]
        public void ParseHtmlTests1()
        {
            var html = GetEmbeddedResource("Prehrat2017-Radio_Junior_(archiv_-_Klub_Radia_Junior).html");

            var parserResul = new RozhlasPrehrat2017PageParser().ParseHtml(html);

            Assert.IsNotNull(parserResul);
            Assert.IsNotNull(parserResul.LogSet);
            Assert.IsNotNull(parserResul.RozhlasUrlSet);
            Assert.AreEqual(0, parserResul.LogSet.Count);
            Assert.AreEqual(1, parserResul.RozhlasUrlSet.Count);
        }
    }
}