namespace RadioOwl.Radio
{
    public static class RadioHelpers
    {
        /// <summary>
        /// jedna se o jeden z typu URL, ktere umim zpracovat?
        /// </summary>
        public static bool IsUrlToIRadio(string url)
        {
            // url se porad meni - takze beru vse a uvidime zda se poradi nejake mp3 url
            return (!string.IsNullOrEmpty(url) && url.StartsWith("http"));
        }
    }
}
