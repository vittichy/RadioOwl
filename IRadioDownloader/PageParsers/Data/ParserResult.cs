using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.PageParsers.Data
{
    public class ParserResult
    {
        /// <summary>
        /// full downloaded html source
        /// </summary>
 //       public readonly string SourceHtml;

        /// <summary>
        /// set of log messages (ussualy errors?)
        /// </summary>
        public readonly List<string> LogSet = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public readonly List<RozhlasUrl> RozhlasUrlSet = new List<RozhlasUrl>();



        public string Title { get; set; } = "PARSER_DEFAULT_TITLE";

        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }


        ///// <summary>
        ///// constructor
        ///// </summary>
        //public ParserResult(string sourceHtml)
        //{
        //    SourceHtml = sourceHtml;
        //}


        public ParserResult AddLog(string logMsg)
        {
            LogSet.Add(logMsg);
            return this;
        }


        public ParserResult AddUrl(string url, string title)
        {
            RozhlasUrlSet.Add(new RozhlasUrl() { Url = url, Title = title });
            return this;
        }
    }
}