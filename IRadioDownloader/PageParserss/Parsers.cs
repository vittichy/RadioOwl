using RadioOwl.PageParsers.Base;

namespace RadioOwl.PageParsers
{
    public class Parsers
    {
        public readonly IPageDecoder Chain;

        public Parsers()
        {
            Chain = new RozhlasPrehrat2018PageDecoder(
                        new RozhlasStahnout2018PageDecoder());
        }
    }
}
