using System.Diagnostics;

namespace RadioOwl.PageParsers.Data
{
    [DebuggerDisplay("Url:{Url} | Title:{Title}")]
    public class RozhlasUrl
    {
        public string Url;
        //public string Description;
        private string title;

        public string Title
        {
            get => title;
            set => title = value;
        }
        //        public string SiteName;
    }
}
