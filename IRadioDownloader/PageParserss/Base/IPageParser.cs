using RadioOwl.PageParsers.Data;
using System.Threading.Tasks;

namespace RadioOwl.PageParsers.Base
{
    public interface IPageDecoder
    {
        Task<IPageDecoder> CanDecodeUrl(string url);

        IPageDecoder NextDecoder { get;   }

      Task<  ParserResult> Decode(string url);
    }
}
