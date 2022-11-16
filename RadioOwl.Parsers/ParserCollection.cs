using RadioOwl.Parsers.Parser;
using RadioOwl.Parsers.Parser.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RadioOwl.Parsers
{
    /// <summary>
    /// Seznam dostupných parserů
    /// </summary>
    public class ParserCollection
    {
        /// <summary>
        /// Seznam dostupných parserů
        /// </summary>
        public readonly List<IPageParser> Parsers
                                = new List<IPageParser>()
                                {
                                    new MujRozhlas2022Parser(),
                                };

        /// <summary>
        /// Vrací parser vhodný pro url
        /// </summary>
        public List<IPageParser> FindParser(string url)
        {
            return Parsers.Where(p => p.CanParse(url)).ToList();
        }

        /// <summary>
        /// Existuje parser pro url?
        /// </summary>
        public bool ExistsParser(string url)
        {
            return FindParser(url).Any();
        }
    }
}   