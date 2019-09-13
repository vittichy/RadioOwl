using Dtc.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.PageParsers.Data
{
    [DebuggerDisplay("{DebuggerDisplay}")]
     // | RozhlasUrlSet:{RozhlasUrlSet?.Count() ?? -1} | LogSet:{LogSet?.Count() ?? -1 | ...")]
    public class ParserResult
    {
        private string DebuggerDisplay
        {
            get { return $"MetaTitle:{MetaTitle.Left(20)} | MetaDescription:{MetaDescription.Left(20)} | RozhlasUrlSet:{RozhlasUrlSet?.Count}  | LogSet:{LogSet?.Count} "; }
        }


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

                  
        public string MetaTitle { get; set; }


        public string MetaDescription { get; set; }

        /// <summary>
        /// site name (Vlatava, Radiozurnal etc)
        /// </summary>
        public string MetaSiteName { get; set; }

        /// <summary>
        /// text for readme file
        /// </summary>
        public string ReadMeText { get; set; }


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