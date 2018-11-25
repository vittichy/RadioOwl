using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.PageParsers.Data
{
    public class ParserResult
    {
        public List<RozhlasUrl> RozhlasUrlSet = new List<RozhlasUrl>();

        public List<string> LogSet = new List<string>();


        public ParserResult AddLog(string logMsg)
        {
            LogSet.Add(logMsg);
            return this;
        }

        public ParserResult AddUrl(string url, string description)
        {
            RozhlasUrlSet.Add(new RozhlasUrl() { Url = url, Description = description });
            return this;
        }
    }
}
