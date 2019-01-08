using NUnit.Framework;
using RadioOwl.PageParsers;
using System.Linq;
using System.Threading.Tasks;

namespace RadioOwlTests.PageParsers
{
    [TestFixture]
    public class RozhlasPrehrat2018PageParserTests : TestBase
    {
        [Test]
        public async Task CanParseTests()
        {
            var parser = new RozhlasPrehrat2018PageParser();

            var result = await parser.CanParse(null);
            Assert.IsNull(result);

            result = parser.CanParse(string.Empty).Result;
            Assert.IsNull(result);

            result = parser.CanParse("xxxxxxxxxxx").Result;
            Assert.IsNull(result);

            // ok urls
            var urls = new string[]
            {
                @"http://plus.rozhlas.cz/host-galeristka-a-kuratorka-jirina-divacka-7671850?player=on#player",
                @"http://region.rozhlas.cz/malebne-vlakove-nadrazi-v-hradku-u-susice-se-dostalo-mezi-deset-nejkrasnejsich-u-7671216?player=on#player",
                @"http://radiozurnal.rozhlas.cz/pribeh-stoleti-7627378#dil=99?player=on#player",
                @"http://dvojka.rozhlas.cz/miroslav-hornicek-petatricet-skvelych-pruvanu-a-jine-povidky-7670628#dil=2?player=on#player",
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
        public void ParseHtmlTests2018()
        {
            var html = GetEmbeddedResource("Prehrat2018_01.html");

            var parserResult = new RozhlasPrehrat2018PageParser().ParseHtml(html);

            Assert.IsNotNull(parserResult);
            Assert.IsNotNull(parserResult.LogSet);
            Assert.IsNotNull(parserResult.RozhlasUrlSet);
            Assert.AreEqual(0, parserResult.LogSet.Count);
            Assert.AreEqual(2, parserResult.RozhlasUrlSet.Count);
        }



        [Test]
        public void ParseHtmlTests2019_1()
        {
            // serial vice dilu na jedne strance
            var html = GetEmbeddedResource("Prehrat2018-Steinar_Bragi-Planina-Vltava.html");

            var parserResult = new RozhlasPrehrat2018PageParser().ParseHtml(html);

            Assert.IsNotNull(parserResult);
            Assert.IsNotNull(parserResult.LogSet);
            Assert.IsNotNull(parserResult.RozhlasUrlSet);
            Assert.AreEqual(0, parserResult.LogSet.Count);
            Assert.AreEqual(5, parserResult.RozhlasUrlSet.Count);
            Assert.IsTrue(parserResult.RozhlasUrlSet.All(p => !string.IsNullOrEmpty(p.Url)));
            Assert.IsTrue(parserResult.RozhlasUrlSet.All(p => !string.IsNullOrEmpty(p.Title)));

            html = GetEmbeddedResource("Prehrat2019-PJORourke Jak se dnes chovat ve spolecnosti.html");

            parserResult = new RozhlasPrehrat2018PageParser().ParseHtml(html);

            Assert.IsNotNull(parserResult);
            Assert.IsNotNull(parserResult.LogSet);
            Assert.IsNotNull(parserResult.RozhlasUrlSet);
            Assert.AreEqual(0, parserResult.LogSet.Count);
            Assert.AreEqual(4, parserResult.RozhlasUrlSet.Count);
            Assert.IsTrue(parserResult.RozhlasUrlSet.All(p => !string.IsNullOrEmpty(p.Url)));
            Assert.IsTrue(parserResult.RozhlasUrlSet.All(p => !string.IsNullOrEmpty(p.Title)));

        }


        [Test]
        public void ParseHtmlTests2019_2()
        {
            // porad bez vice dilu
            var html = GetEmbeddedResource("Prehrat2019-Nosorozec severni bily.html");

            var parserResult = new RozhlasPrehrat2018PageParser().ParseHtml(html);

            Assert.IsNotNull(parserResult);
            Assert.IsNotNull(parserResult.LogSet);
            Assert.IsNotNull(parserResult.RozhlasUrlSet);
            Assert.AreEqual(0, parserResult.LogSet.Count);
            Assert.AreEqual(1, parserResult.RozhlasUrlSet.Count);
            Assert.IsTrue(parserResult.RozhlasUrlSet.All(p => !string.IsNullOrEmpty(p.Url)));
            Assert.IsTrue(parserResult.RozhlasUrlSet.All(p => !string.IsNullOrEmpty(p.Title)));
        }


        [Test]
        public void ParseHtmlTests2019_3()
        {
            // porad bez vice dilu
            var html = GetEmbeddedResource("Prehrat2019-A.html");

            var parserResult = new RozhlasPrehrat2018PageParser().ParseHtml(html);

            Assert.IsNotNull(parserResult);
            Assert.IsNotNull(parserResult.LogSet);
            Assert.IsNotNull(parserResult.RozhlasUrlSet);
            Assert.AreEqual(0, parserResult.LogSet.Count);
            Assert.AreEqual(1, parserResult.RozhlasUrlSet.Count);
            Assert.IsTrue(parserResult.RozhlasUrlSet.All(p => !string.IsNullOrEmpty(p.Url)));
            Assert.IsTrue(parserResult.RozhlasUrlSet.All(p => !string.IsNullOrEmpty(p.Title)));
        }


        [Test]
        public void ParseHtmlTests2019_4_Radiozurnal()
        {
            // TODO - tohle zatim neumim - stranka obsahuje pouze odkazy na dalsi html ne na konkretni mp3 :-(
            var html = GetEmbeddedResource("Prehrat2019-C-Radiozurnal.html");

            var parserResult = new RozhlasPrehrat2018PageParser().ParseHtml(html);

            Assert.IsNotNull(parserResult);
            Assert.IsNotEmpty(parserResult.MetaDescription);
            Assert.IsNotEmpty(parserResult.MetaSiteName);
            Assert.IsNotEmpty(parserResult.MetaTitle);
            //Assert.IsNotNull(parserResult.LogSet);
            //Assert.IsNotNull(parserResult.RozhlasUrlSet);
            //Assert.AreEqual(0, parserResult.LogSet.Count);
            //Assert.AreEqual(1, parserResult.RozhlasUrlSet.Count);
            //Assert.IsTrue(parserResult.RozhlasUrlSet.All(p => !string.IsNullOrEmpty(p.Url)));
            //Assert.IsTrue(parserResult.RozhlasUrlSet.All(p => !string.IsNullOrEmpty(p.Title)));
        }


        [Test]
        public void ParseHtmlTests2019_5()
        {
            // porad bez vice dilu
            var html = GetEmbeddedResource("Prehrat2019-SoudceZMilosti.html");

            var parserResult = new RozhlasPrehrat2018PageParser().ParseHtml(html);

            Assert.IsNotNull(parserResult);
            Assert.IsNotNull(parserResult.LogSet);
            Assert.IsNotNull(parserResult.RozhlasUrlSet);
            Assert.AreEqual(0, parserResult.LogSet.Count);
            Assert.AreEqual(7, parserResult.RozhlasUrlSet.Count);
            Assert.IsTrue(parserResult.RozhlasUrlSet.All(p => !string.IsNullOrEmpty(p.Url)));
            Assert.IsTrue(parserResult.RozhlasUrlSet.All(p => !string.IsNullOrEmpty(p.Title)));

            // vsechny title a url by meli byt rozdilne
            Assert.AreEqual(parserResult.RozhlasUrlSet.Count(), parserResult.RozhlasUrlSet.Select(p => p.Title).Distinct().Count());
            Assert.AreEqual(parserResult.RozhlasUrlSet.Count(), parserResult.RozhlasUrlSet.Select(p => p.Url).Distinct().Count());
        }

    }
}