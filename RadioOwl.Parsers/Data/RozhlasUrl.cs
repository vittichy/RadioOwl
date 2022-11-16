using System.Diagnostics;

namespace RadioOwl.Parsers.Data
{
    /// <summary>
    /// Vyparsované url a titulek
    /// </summary>
    [DebuggerDisplay("Url:{Url} | Title:{Title}")]
    public class RozhlasUrl
    {
        public string Title { get; set; }
        
        public string Url { get; set; }
    }
}
