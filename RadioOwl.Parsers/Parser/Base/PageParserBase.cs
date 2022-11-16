using Dtc.Http.Http;
using RadioOwl.Parsers.Data;
using RadioOwl.Parsers.Parser.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RadioOwl.Parsers.Parser.Base
{
    /// <summary>
    /// Zaklad pro parsery stranek + http utility
    /// </summary>
    public abstract class PageParserBase : IPageParser
    {
        /// <inheritdoc/>
        public abstract int Version { get; }

        /// <inheritdoc/>
        public abstract string[] ParseUrls { get; }

        /// <inheritdoc/>
        public bool CanParse(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            return ParseUrls.Any(p => url.ToLower().Contains(p.ToLower()));
        }
    
        public abstract Task<bool> ParseAsync(RadioData radioData);

        /// <summary>
        /// Jednoduche stazeni url do stringu
        /// </summary>
        protected async Task<string> DownloadHtmlAsync(string url)
        {
            var asyncDownloader = new AsyncDownloader();
            var downloaderOutput = await asyncDownloader.GetString(url);
            return downloaderOutput.DownloadOk ? downloaderOutput.Output : null;
        }

        /// <summary>
        /// Jednoduche stazeni url do stringu
        /// </summary>
        protected async Task DownloadHtmlAsync(RadioData radioData)
        {
            radioData.HtmlPage = await DownloadHtmlAsync(radioData.Url);
        }
    }
}
