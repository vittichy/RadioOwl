using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.Parsers.Parser.Data
{
    public class MujRozhlas2020SiteInfo
    {
        /// <summary>
        /// Zajima mne ted hodnota 'serial' - napr pro vicedilne cteni, poradi jako Spirituala atd maji 'show', to zatim neumim rozparsovat
        /// napr:
        ///     "siteEntityBundle":"serial"
        ///     "siteEntityBundle":"show"
        /// </summary>
        public string SiteEntityBundle;

        /// <summary>
        /// Podit dilu poradu, u "siteEntityBundle":"serial", u jinych typu je prazdny
        /// napr:
        ///     "contentSerialAllParts":"8"
        /// </summary>
        public int? ContentSerialAllParts;

        /// <summary>
        /// Popisek
        /// napr:
        ///     "siteEntityLabel":"Spiritu\u00e1l"
        ///     "siteEntityLabel":"Frankenstein. Hororov\u00fd p\u0159\u00edb\u011bh um\u011bl\u00e9ho tvora, pod jeho\u017e d\u011bsivou podobou se skr\u00fdvaj\u00ed lidsk\u00e9 city"
        /// </summary>
        public string SiteEntityLabel;

        /// <summary>
        /// 'Cesta' dokumentu - zkusim vyuzit k ukladani souboru
        /// siteDocumentPath": "\/spirituala"
        /// "siteDocumentPath":"\/cetba-na-pokracovani-na-dvojce\/frankenstein-hororovy-pribeh-umeleho-tvora-pod-jehoz-desivou-podobou"
        /// </summary>
        public string SiteDocumentPath;

        ///// <summary>
        ///// Popisek
        ///// napr:
        /////     "siteDocumentTitle":"Spiritu\u00e1la \u2022 mujRozhlas"
        /////     "siteDocumentTitle":"Frankenstein. Hororov\u00fd p\u0159\u00edb\u011bh um\u011bl\u00e9ho tvora, pod jeho\u017e d\u011bsivou podobou se skr\u00fdvaj\u00ed lidsk\u00e9 city \u2022 mujRozhlas"
        ///// </summary>
        //public string SiteDocumentTitle;

        /// <summary>
        /// 
        /// </summary>
        public string ContentId;
    }
}
