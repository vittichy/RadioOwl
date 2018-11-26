using NUnit.Framework;
using RadioOwl.Data;
using RadioOwl.PageParsers;
using RadioOwl.Radio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwlTests.PageParsers
{
    [TestFixture]
    public class RozhlasPrehrat2018PageParserTests : TestBase
    {
        [Test]
        public async Task IsUrlToPrehrat2018Tests()
        {
            var parser = new RozhlasPrehrat2018PageParser();


            //var result2 = await parser.CanParse(null);
            //var xxx = result2 != null;


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

        // RozhlasStahnout2018PageDecoder





        [Test]
        public void IsUrlToPrehrat2018TestsWWWWWWWWWWWW()
        {
            var html = GetEmbeddedResource("Prehrat2018_01.html");

            var parser = new RozhlasPrehrat2018PageParser().ParseHtml(html);


            //var result = parser.Decode()
            //    //RadioHtmlParser().ParsePrehrat2018Html(html);

            //Assert.IsNotNull(result);
            //Assert.IsNotNull(result.Log);
            //Assert.IsNotNull(result.Urls);
            //Assert.AreEqual(0, result.Log.Count);
            //Assert.AreEqual(2, result.Urls.Count);
            //Assert.IsFalse(string.IsNullOrEmpty(result.Title));

        }
    }
}