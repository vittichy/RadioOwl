using Dtc.Common.Extensions;

namespace RadioOwl.Parsers.Data.Helper
{
    public class RadioDataHelper
    {
        public static string ProgressText(long progressPercentage, long bytesReceived)
        {
            return progressPercentage == 0 ? "0%" : string.Format("{0}%  {1}", progressPercentage, bytesReceived.ToFileSize());
        }
    }
}
