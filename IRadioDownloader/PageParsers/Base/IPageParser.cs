using RadioOwl.PageParsers.Data;
using System.Threading.Tasks;

namespace RadioOwl.PageParsers.Base
{
    public interface IPageParser
    {
        /// <summary>
        /// next parser in chain
        /// </summary>
        IPageParser NextParser { get; }

        /// <summary>
        /// can i parse this url?
        /// </summary>
        Task<IPageParser> CanParse(string url);

        /// <summary>
        /// parse from url
        /// </summary>
        Task<ParserResult> Parse(string url);







         ParserResult ParseHtml(string html);
    }
}
