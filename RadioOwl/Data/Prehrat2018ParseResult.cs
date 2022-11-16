using System.Collections.Generic;

namespace RadioOwl.Data
{
    public class Prehrat2018ParseResult
    {
        /// <summary>
        /// list of parsed urls
        /// </summary>
        public List<string> Urls = new List<string>();

        /// <summary>
        /// error log
        /// </summary>
        public List<string> Log = new List<string>();

        /// <summary>
        /// title form html page
        /// </summary>
        public string Title;

        /// <summary>
        /// description
        /// </summary>
        public string Description;

        /// <summary>
        /// site - Vltava, Hvezda :-) atd
        /// </summary>
        public string SiteName;
    }
}
