using RadioOwl.PageParsers.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vt.Http;

namespace RadioOwl.PageParsers.Base
{
    public abstract class PageParserBase:IPageParser
    {
        public IPageParser NextParser { get;  }

        public PageParserBase(IPageParser nextParser)
        {
            NextParser = nextParser;
        }


        //public abstract CanParseCondition(string url);


        //public async Task<IPageParser> CanParse(string url)
        //{
        //    if (CanParseCondition(url))
        //        return this;

        //    return await CanParseByNext(url);
        //}



        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public abstract bool CanParseCondition(string url);


        public async Task<IPageParser> CanParse(string url)
        {
            if (CanParseCondition(url))
                return this;

            return await CanParseByNext(url);
        }





        protected async Task<string> DownloadHtml(string url)
        {
            var asyncDownloader = new AsyncDownloader();
            var downloaderOutput = await asyncDownloader.GetString(url);
            return downloaderOutput.DownloadOk ? downloaderOutput.Output : null;


            {
                //return new ParserResult(downloaderOutput.Output);
            }
            //            return new ParserResult(null).AddLog($"Nepodařilo se stažení hlavní stránky: '{url}'");
        }


        public abstract ParserResult ParseHtml(string html);


        public async Task<ParserResult> Parse(string url)
        {
            var html = await DownloadHtml(url);

            if (string.IsNullOrEmpty(html))
                return new ParserResult(null).AddLog($"Nepodařilo se stažení hlavní stránky: '{url}'"); // no source html - no fun
            //Parse(parserResult);
            else
                return ParseHtml(html); // try to parse
        }

        protected async Task<IPageParser> CanParseByNext(string url)
        {
            if (NextParser == null)
                return null;

            return await NextParser?.CanParse(url);
        }
    }
}
