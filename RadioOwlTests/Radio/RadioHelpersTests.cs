using NUnit.Framework;
using RadioOwl.Data;
using RadioOwl.Radio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwlTests.Radio
{

    [TestFixture]
    public class RadioHelpersTests : TestBase
    {
        [Test]
        public void IsUrlToPrehrat2018_Tests()
        {
            var result = RadioHelpers.IsUrlToPrehrat2018(null);
            Assert.IsFalse(result);

            result = RadioHelpers.IsUrlToPrehrat2018(string.Empty);
            Assert.IsFalse(result);

            result = RadioHelpers.IsUrlToPrehrat2018("xxxx");
            Assert.IsFalse(result);

            // ok urls
            var urls = new string[]
            {
                @"http://plus.rozhlas.cz/host-galeristka-a-kuratorka-jirina-divacka-7671850?player=on#player",
                @"http://region.rozhlas.cz/malebne-vlakove-nadrazi-v-hradku-u-susice-se-dostalo-mezi-deset-nejkrasnejsich-u-7671216?player=on#player",
                @"http://radiozurnal.rozhlas.cz/pribeh-stoleti-7627378#dil=99?player=on#player",
                @"http://dvojka.rozhlas.cz/miroslav-hornicek-petatricet-skvelych-pruvanu-a-jine-povidky-7670628#dil=2?player=on#player",
            };
            urls.ToList().ForEach(p => { Assert.IsTrue(RadioHelpers.IsUrlToPrehrat2018(p)); });

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
            urls.ToList().ForEach(p => { Assert.IsFalse(RadioHelpers.IsUrlToPrehrat2018(p)); });
        }


        [Test]
        public void ParsePrehrat2018Html_Tests1()
        {
            var html = GetEmbeddedResource("Prehrat2018_01.html");
            var result = new RadioHtmlParser().ParsePrehrat2018Html(html);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Log);
            Assert.IsNotNull(result.Urls);
            Assert.AreEqual(0, result.Log.Count);
            Assert.AreEqual(2, result.Urls.Count);
            Assert.IsFalse(string.IsNullOrEmpty(result.Title));
        }


        [Test]
        public void ParsePrehrat2018Html_Tests2()
        {
            var html = GetEmbeddedResource("Prehrat2018_02.html"); // pozor Prehrat2018_02 ma nevalidny cestinu! takze netestuj title atd
            var result = new RadioHtmlParser().ParsePrehrat2018Html(html);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Log);
            Assert.IsNotNull(result.Urls);
            Assert.AreEqual(0, result.Log.Count);
            Assert.AreEqual(5, result.Urls.Count);
            Assert.IsFalse(string.IsNullOrEmpty(result.Title));
        }


        [Test]
        public void ParsePrehrat2018Html_Tests3()
        {
            var html = GetEmbeddedResource("Prehrat2018_03.html");
            var result = new RadioHtmlParser().ParsePrehrat2018Html(html);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Log);
            Assert.IsNotNull(result.Urls);
            Assert.AreEqual(0, result.Log.Count);
            Assert.AreEqual(5, result.Urls.Count);
            Assert.AreEqual("BáSnění Milana Šedivého", result.Title);
        }


        [Test]
        public void ParsePrehrat2018Html_Tests4()
        {
            var html = GetEmbeddedResource("Prehrat2018_04.html");
            var result = new RadioHtmlParser().ParsePrehrat2018Html(html);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Log);
            Assert.IsNotNull(result.Urls);
            Assert.AreEqual(0, result.Log.Count);
            Assert.AreEqual(7, result.Urls.Count);
            Assert.AreEqual("Lucie Výborná: Mise Afghánistán", result.Title);
        }


        [Test]
        public void ParsePrehrat2018Html_Tests5()
        {
            // html je z odkazu 'Prehrat', ale neobsahuje odkazu v jsonu
            var html = GetEmbeddedResource("Prehrat2018_05.html");
            var result = new RadioHtmlParser().ParsePrehrat2018Html(html);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Log);
            Assert.IsNotNull(result.Urls);
            Assert.AreEqual(0, result.Log.Count);
            Assert.AreEqual(1, result.Urls.Count);
            //Assert.AreEqual("BáSnění Milana Šedivého", result.Title);
        }

    }
}
