using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace vt.Extensions.HtmlAgilityPack
{
    public static class HtmlNodeExtensions
    {

        /// <summary>
        /// Dohleda sub-node daneho typu (jmena)
        /// </summary>
        public static List<HtmlNode> GetSubNodesOfName(this HtmlNode instance, string htmlTagName)
        {
            var result = new List<HtmlNode>();

            if (instance != null)
            {
                foreach (var child in instance.ChildNodes)
                {
                    if (HtmlNameCompare(htmlTagName, child.Name))
                    {
                        result.Add(child);
                    }
                    var subChilds = child.GetSubNodesOfName(htmlTagName);
                    result.AddRange(subChilds);
                }
            }
            return result;
        }


        /// <summary>
        /// Vraci atribut HtmlNode dle jmena
        /// </summary>
        public static HtmlAttribute GetAttributeByName(this HtmlNode instance, string attributeName)
        {
            return (instance != null) ? instance.Attributes.FirstOrDefault(p => HtmlNameCompare(p.Name, attributeName)) : null;
        }


        /// <summary>
        /// Vraci hodnotu atributu HtmlNode dle jmena
        /// </summary>
        public static string GetAttributeValueByName(this HtmlNode instance, string attributeName)
        {
            return instance.GetAttributeByName(attributeName)?.Value;
        }


        /// <summary>
        /// porovnani jmen HTML tagu - velikost nehraje roli
        /// </summary>
        private static bool HtmlNameCompare(string name1, string name2)
        {
            return (string.Compare(name1, name2, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
    }







}
