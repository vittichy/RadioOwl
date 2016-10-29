using System.IO;

namespace RadioOwl.Data.TagLibExtension
{
    /// <summary>
    /// pomocna pro napojeni TagLib na stream
    /// </summary>
    public class SimpleFile
    {
        public readonly string Name;
        public readonly Stream Stream;
        public SimpleFile(string name, Stream stream)
        {
            this.Name = name;
            this.Stream = stream;
        }
    }
}
