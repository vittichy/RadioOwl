using RadioOwl.Parsers.Data;
using System.Collections;
using System.Threading.Tasks;

namespace RadioOwl.Parsers.Parser.Interfaces
{
    /// <summary>
    /// Interface pro parsery 
    /// </summary>
    public interface IPageParser 
    {   
        /// <summary>
        /// Verze parseru
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Url ke zpracování
        /// </summary>
        string[] ParseUrls { get; }

        /// <summary>
        /// Umim parsovat zaslané url?
        /// </summary>
        bool CanParse(string url);

        /// <summary>
        /// Parsování dat z url odkazu
        /// </summary>
        Task<bool> ParseAsync(RadioData radioData);
    }
}
