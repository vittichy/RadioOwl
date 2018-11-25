using RadioOwl.PageParsers.Base;
using RadioOwl.PageParsers.Data;
using System.Threading.Tasks;

namespace RadioOwl.PageParsers
{
    public class RozhlasPrehrat2018PageDecoder : PageDecoderBase, IPageDecoder
    {
        public RozhlasPrehrat2018PageDecoder(IPageDecoder next = null) : base(next) { }

        public Task< IPageDecoder> CanDecodeUrl(string url)
        {
            if (false)
            {
             //   return new Task<IPageDecoder>( this);
            }
            return NextDecoder?.CanDecodeUrl(url);
        }

        public async  Task<ParserResult> Decode(string url)
        {
            return   new ParserResult();
        }
    }
}
