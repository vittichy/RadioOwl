using System.IO;

namespace RadioOwl.Id3.TagLibExtension
{
    /// <summary>
    /// pomocna pro napojeni TagLib na stream
    /// </summary>
    public class SimpleFileAbstraction : TagLib.File.IFileAbstraction
    {
        private readonly SimpleFile simpleFile;

        public SimpleFileAbstraction(Stream stream) : this(new SimpleFile(null, stream))
        {
        }
        public SimpleFileAbstraction(string name, Stream stream) : this(new SimpleFile(name, stream))
        {
        }

        public SimpleFileAbstraction(SimpleFile simpleFile)
        {
            this.simpleFile = simpleFile;
        }

        public string Name
        {
            get { return simpleFile.Name; }
        }

        public System.IO.Stream ReadStream
        {
            get { return simpleFile.Stream; }
        }

        public System.IO.Stream WriteStream
        {
            get { return simpleFile.Stream; }
        }

        public void CloseStream(System.IO.Stream stream)
        {
            stream.Position = 0;
        }
    }
}
