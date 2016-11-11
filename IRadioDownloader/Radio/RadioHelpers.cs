using System;
using System.Linq;
using vt.Extensions;

namespace RadioOwl.Radio
{
    public static class RadioHelpers
    {
        #region Consts

        // typy odkazu na porad z hlavni stranky: http://hledani.rozhlas.cz/iradio/
        //
        // Prehrat:  http://prehravac.rozhlas.cz/audio/3680166                      - je u vsech poradu i tam, kde neni nabizen download, musim tedy s odkazu stahnout html playeru a z nej ziskat odkaz?
        // Stahnout: http://media.rozhlas.cz/_download/3680166.mp3                  - normalni download
        // Podcast:  http://www2.rozhlas.cz/podcast/podcast_porady.php?p_po=293     - nezajima mne

        private const string URL_BEGINNING_PLAY = @"http://prehravac.rozhlas.cz/audio/";        // odkaz na stream - skutecnou adresu k mp3 musi vykousat z html stranky
        private const string URL_BEGINNING_DOWNLOAD = @"http://media.rozhlas.cz/_download/";    // primo odkaz na mp3

        private const string URL_BEGINNING_MP3_AUDIO = @"http://media.rozhlas.cz/_audio/";      // url iradio streamu kdyz uz vim ID poradu

        // mms://audio.proglas.cz/audioarchiv/stream/audio_51117.wma
        //
        // wma - pouze format playlistu, viz http://www.linuxquestions.org/questions/linux-software-2/wget-download-streaming-media-620376/
        //
        // private const string URL_BEGINNING_PROGLAS = @"mms://audio.proglas.cz/audioarchiv/stream/";     // odkaz z audioarchivu proglas.cz

        // mms:// - mozne neco viz ManagedBASS na gitu?
        // http://www.bass.radio42.com/help/html/b8b8a713-7af4-465e-a612-1acd769d4639.htm

        private const string URL_IRADIO_MP3_DOWNLOAD_URL = URL_BEGINNING_MP3_AUDIO + @"{0}.mp3";     // url iradio streamu kdyz uz vim ID poradu


        #endregion

        public static string GetIRadioMp3Url(string id)
        {
            return string.Format(URL_IRADIO_MP3_DOWNLOAD_URL, id);
        }


        /// <summary>
        /// jedna se o URL, ktere umim zpracovat?
        /// </summary>
        public static bool IsUrlToIRadio(string url)
        {
            return (IsUrlToIRadioDownload(url) || IsUrlToIRadioPlayPage(url));
        }


        public static bool IsUrlToIRadioDownload(string url)
        {
            return (!string.IsNullOrEmpty(url) 
                    && (url.StartsWith(URL_BEGINNING_DOWNLOAD, StringComparison.InvariantCultureIgnoreCase) 
                        || 
                        url.StartsWith(URL_BEGINNING_MP3_AUDIO, StringComparison.InvariantCultureIgnoreCase))
                    );
        }


        public static bool IsUrlToIRadioPlayPage(string url)
        {
            return (!string.IsNullOrEmpty(url) && url.StartsWith(URL_BEGINNING_PLAY, StringComparison.InvariantCultureIgnoreCase));
        }


        public static string GetStreamIdFromUrl(string url)
        {
            if (IsUrlToIRadioDownload(url))
            {
                return GetStreamIdFromIRadioDownloadUrl(url);
            }
            if (IsUrlToIRadioPlayPage(url))
            {
                return GetStreamIdFromIRadioPlayUrl(url);
            }
            return null;
        }


        /// <summary>
        /// ziskani ID streamu z url
        /// napr:
        /// http://prehravac.rozhlas.cz/audio/3686290/embed?iframe=true&width=545&height=550
        /// http://prehravac.rozhlas.cz/audio/3741710
        /// </summary>
        /// <param name="url">url z IRadio</param>
        /// <returns>ID streamu</returns>
        private static string GetStreamIdFromIRadioPlayUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var id = url.RemoveStartText(URL_BEGINNING_PLAY);
                if (!string.IsNullOrEmpty(id) && id.Contains('/'))
                {
                    id = id.SusbstringToChar('/');
                }
                return id;
            }
            return null;
        }


        private static string GetStreamIdFromIRadioDownloadUrl(string url)
        {
            return url.SusbstringFromLastChar('/').SusbstringToChar('.').Trim();
        }
                
    }
}
