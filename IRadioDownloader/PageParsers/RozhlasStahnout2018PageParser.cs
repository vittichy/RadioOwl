using Dtc.Common.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using RadioOwl.PageParsers.Base;
using RadioOwl.PageParsers.Data;
using RadioOwl.Radio;
using System;
using System.Linq;
using System.Threading.Tasks;
using vt.Http;

namespace RadioOwl.PageParsers
{
    public class RozhlasStahnout2018PageParser : PageParserBase, IPageParser
    {
        public RozhlasStahnout2018PageParser(IPageParser next = null) : base(next) { }

        //public Task<IPageParser> CanParse(string url)
        //{
        //    if (false)
        //    {
        //        //   return new Task<IPageDecoder>( this);
        //    }
        //    return NextParser?.CanParse(url);
        //}

        public override ParserResult ParseHtml(string html)
        {
            return new ParserResult("TODO");
        }




        public override bool CanParseCondition(string url)
        {
            return false;
        }

        //public async Task<ParserResult> Parse(string url)
        //{
        //    return new ParserResult("TODO");
        //}


    }
}
