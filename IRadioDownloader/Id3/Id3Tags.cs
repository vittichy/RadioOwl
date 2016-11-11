using RadioOwl.Id3.TagLibExtension;
using System.IO;
using TagLib;

namespace RadioOwl.Id3
{
    public class Id3Tags
    {
        public readonly string Album;           // "Hovory (2016-10-28)"
        public readonly string Comment;         // "Hovory - 28.10.2016 02:40 - Narodni muzeum je fenomen ceske historie. Strileli po nem sovetsti vojaci v roce 1968. Dnes se do nej dobyvaji turiste, i kdyz je budova v rekonstrukci. Jake to je v takove budove delat generalniho reditele? Hostem Miroslava Dittricha byl Michal Lukes."
        public readonly string Copyright;       // "2016 Cesky rozhlas"
        // Obsolete     string FirstArtist;     // "Hovory (28.10.2016) - Miroslav Dittrich - CRo Plus"
        public readonly string FirstComposer;   // "Miroslav Dittrich"
        public readonly string FirstPerformer;  // "Hovory (28.10.2016) - Miroslav Dittrich - CRo Plus"
        // Obsolete     string JoinedArtists;   // "Hovory (28.10.2016) - Miroslav Dittrich - CRo Plus"
        public readonly string JoinedComposers; // "Miroslav Dittrich"
        public readonly string JoinedPerformers;// "Hovory (28.10.2016) - Miroslav Dittrich - CRo Plus"
        public readonly string Title;           // "Narodni muzeum je fenomen ceske historie. Strileli po nem sovetsti vojaci v roce 1968. Dnes se do nej dobyvaji turiste, i kdyz je budova v rekonstrukci. Jake to je v takove budove delat generalniho reditele? Hostem Miroslava Dittricha byl Michal Lukes." 

        public Id3Tags(Stream stream)
        {
            var simpleFileAbstraction = new SimpleFileAbstraction(stream);
            var tagLibFile = TagLib.File.Create(simpleFileAbstraction, "taglib/mp3", ReadStyle.None);
            if (tagLibFile?.Tag != null)
            {
                Album = tagLibFile.Tag.Album;
                Comment = tagLibFile.Tag.Comment;
                Copyright = tagLibFile.Tag.Copyright;
                FirstComposer = tagLibFile.Tag.FirstComposer;
                FirstPerformer = tagLibFile.Tag.FirstPerformer;
                JoinedComposers = tagLibFile.Tag.JoinedComposers;
                JoinedPerformers = tagLibFile.Tag.JoinedPerformers;
                Title = tagLibFile.Tag.Title;
            }
        }

        public Id3Tags(byte[] data) : this(new MemoryStream(data))
        {
        }
    }
}
