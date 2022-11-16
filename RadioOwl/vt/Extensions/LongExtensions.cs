using System;
using System.Globalization;
using System.Linq;

namespace vt.Extensions
{
    public static class LongExtensions
    {
        static readonly string[] SizeSuffixes = { "b", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        /// <summary>
        /// convert long into formatted number with the correct size metric
        /// alternative: http://www.danylkoweb.com/Blog/10-extremely-useful-net-extension-methods-8J
        /// </summary>
        public static string ToFileSize(this long value)
        {
            if (value < 0)
            {
                return "-" + ToFileSize(-value);
            }
            if (value == 0)
            {
                return "0" + SizeSuffixes.First();
            }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1}{1}", adjustedSize, SizeSuffixes[mag]).Replace(',', '.');
        }


        /// <summary>
        /// convert long into string format like 1.2Mb/s
        /// </summary>
        public static string ToBitesPerSecSpeed(this long value, TimeSpan duration)
        {
            if (duration.TotalSeconds > 0)
            {
                var bitesSize = value * 8;
                var bitesPerSec = bitesSize / duration.TotalSeconds;
                var result = ((long)bitesPerSec).ToFileSize() + "/s";
                return result;
            }
            return "?";
        }


        /// <summary>
        /// convert long into string format like 1.2Mb/s
        /// </summary>
        public static string ToBitesPerSecSpeed(this long value, DateTime startTime, DateTime finishTime)
        {
            return value.ToBitesPerSecSpeed(finishTime - startTime);
        }


        /// <summary>
        /// percent from total as string
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="total">total = 100%</param>
        /// <param name="formatString">format string</param>
        /// <returns></returns>
        public static string ToPercentFromTotal(this long value, long total, string formatString = "0.##")
        {
            var percent = (total > 0) ? (((float)value / total) * 100) : 0;
            var result = percent.ToString(formatString, CultureInfo.InvariantCulture) + "%";
            return result;
        }



        /// <summary>
        /// convert long into ETA string like "1.22:33"
        /// </summary>
        public static string ToETA(this long partValue, long total, TimeSpan partDuration)
        {
            if (total > 0)
            {
                var finishedPercent = (((float)partValue / total) * 100);
                var etaSeconds = (partDuration.TotalSeconds / finishedPercent) * Math.Max((100 - finishedPercent), 0);
                var eta = TimeSpan.FromSeconds(etaSeconds);

                var format = "{0:mm\\:ss}";
                if(eta.TotalDays > 1)
                {
                    format = "{0:d\\.hh\\:mm}";
                }
                else if (eta.TotalHours > 1)
                {
                    format = "{0:hh\\:mm}";
                }
                else 
                {
                    format = "{0:mm\\:ss}";
                }
                var result = string.Format(format, eta);
                return result;
            }
            return "?";
        }

    }
}
