using RadioOwl.PageParsers.Base;

namespace RadioOwl.PageParsers
{
    public class Parsers
    {
        public readonly IPageParser Chain;

        public Parsers()
        {
            Chain = new RozhlasPrehrat2018PageParser(
                        new RozhlasPrehrat2017PageParser());
        }
    }
}
