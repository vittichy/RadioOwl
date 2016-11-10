using System;
using System.IO;
using System.Linq;

namespace vt.Extensions
{
    public static class StringExtension
    {

        /// <summary>
        /// remove text at beginning of string
        /// </summary>
        public static string RemoveStartText(this string value, string startWith)
        {
            if ((value != null) && !string.IsNullOrEmpty(startWith))
            {
                if (value.StartsWith(startWith, StringComparison.InvariantCultureIgnoreCase))
                {
                    value = value.Substring(startWith.Length, value.Length - startWith.Length);
                }
            }
            return value;
        }


        public static string TrimToMaxLen(this string value, int maxLen, string endString = null)
        {
            if (!string.IsNullOrEmpty(value) && (value.Length > maxLen))
            {
                maxLen = string.IsNullOrEmpty(endString) ? maxLen : (maxLen - endString.Length);
                if (maxLen <= 0)
                {
                    value = string.IsNullOrEmpty(endString) ? string.Empty : endString;
                }
                else
                {
                    value = value.Remove(maxLen);
                    if (!string.IsNullOrEmpty(endString))
                    {
                        value += endString;
                    }
                }
            }
            return value;
        }


        /// <summary>
        /// porovan obsah stringu na fox pro bool vyraz .T. nebo .F.
        /// </summary>
        public static bool IsFoxProBool(this string value, bool compareBool)
        {
            var compareFoxBool = compareBool ? ".T." : ".F.";
            var result = (string.IsNullOrEmpty(value) || (value.Trim().ToUpper() == compareFoxBool));
            return result;
        }


        public static string SusbstringToChar(this string value, char ch)
        {
            int chIndex = value.IndexOf(ch);
            if (chIndex > 0)
            {
                return value.Substring(0, chIndex);
            }
            return string.Empty;
        }


        public static string SusbstringFromChar(this string value, char ch)
        {
            int chIndex = value.IndexOf(ch);
            if ((chIndex > 0) && ((chIndex + 1) < value.Length))
            {
                return value.Substring(chIndex + 1, value.Length - chIndex - 1);
            }
            return string.Empty;
        }


        public static string SusbstringFromLastChar(this string value, char ch)
        {
            int chIndex = value.LastIndexOf(ch);
            if ((chIndex > 0) && ((chIndex + 1) < value.Length))
            {
                return value.Substring(chIndex + 1, value.Length - chIndex - 1);
            }
            return string.Empty;
        }

        public static Tuple<string, string> SplitBy(this string value, char ch)
        {
            var tuple = new Tuple<string, string>(value.SusbstringToChar(ch), value.SusbstringFromChar(ch));
            return tuple;
        }


        public static string ReplaceInvalidFilenameChar(this string value, char charToReplace = '_')
        {
            var invalidChars = Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars());
            foreach(var invalid in invalidChars)
            {
                value = value.Replace(invalid, charToReplace);
            }
            return value;
        }



        public static string IncrementStringNumber(this string value, int i)
        {
            int number;
            if(int.TryParse(value, out number))
            {
                number += i;
                var format = string.Format("D{0}", value.Length);
                var result = number.ToString(format);
                return result;
            }
            return null;
        }

    }
}
