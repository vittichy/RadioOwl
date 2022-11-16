using RadioOwl.Id3.TagLibExtension;
using System.IO;
using TagLib;

namespace RadioOwl.Id3
{
    public static class Id3Helper
    {
        /// <summary>
        /// pokud stahuju jen stream bez html stranky poradu, nezmam z html vykousle popisy poradu, takze bud vzdy koukat do mp3, kde to je pekne popsane
        /// zatim se zda nejvhodnejsi tag Comment
        /// ostatni tagy jsou cca takto:
        ///     Album: "Hovory (2016-10-28)"
        ///     Comment: "Hovory - 28.10.2016 02:40 - Narodni muzeum je fenomen ceske historie. Strileli po nem sovetsti vojaci v roce 1968. Dnes se do nej dobyvaji turiste, i kdyz je budova v rekonstrukci. Jake to je v takove budove delat generalniho reditele? Hostem Miroslava Dittricha byl Michal Lukes."
        ///     Copyright: "2016 Cesky rozhlas"
        ///     FirstArtist: "Hovory (28.10.2016) - Miroslav Dittrich - CRo Plus"
        ///     FirstComposer: "Miroslav Dittrich"
        ///     FirstPerformer: "Hovory (28.10.2016) - Miroslav Dittrich - CRo Plus"
        ///     JoinedArtists: "Hovory (28.10.2016) - Miroslav Dittrich - CRo Plus"
        ///     JoinedComposers: "Miroslav Dittrich"
        ///     JoinedPerformers: "Hovory (28.10.2016) - Miroslav Dittrich - CRo Plus"
        ///     Title: "Narodni muzeum je fenomen ceske historie. Strileli po nem sovetsti vojaci v roce 1968. Dnes se do nej dobyvaji turiste, i kdyz je budova v rekonstrukci. Jake to je v takove budove delat generalniho reditele? Hostem Miroslava Dittricha byl Michal Lukes."
        /// </summary>
        public static string GetId3TagComment(byte[] data)
        {
            if (data?.Length > 0)
            {
                using (var memoStream = new MemoryStream(data))
                {
                    return GetId3TagComment(memoStream);
                }
            }
            return null;
        }


        public static string GetId3TagComment(Stream stream)
        {
            var simpleFileAbstraction = new SimpleFileAbstraction(stream);
            var tagLibFile = TagLib.File.Create(simpleFileAbstraction, "taglib/mp3", ReadStyle.None);
            if (tagLibFile?.Tag != null)
            {
                return tagLibFile.Tag.Comment ?? tagLibFile.Tag.Title;
            }
            return null;
        }

    }
}
