using System;

namespace vt.Http
{
    /// <summary>
    /// vystup od AsyncDownloader - vysledek + pripadna exception
    /// </summary>
    public class AsyncDownloaderOutput<T> where T : class
    {
        public readonly Exception Exception;
        public readonly T Output;
        public bool DownloadOk { get { return ((Exception == null) && (Output != null)); } }

        public AsyncDownloaderOutput(T output) 
        {
            Output = output;
        }

        public AsyncDownloaderOutput(Exception exception)
        {
            Exception = exception;
        }
    }
}
