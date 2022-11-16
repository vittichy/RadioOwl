namespace RadioOwl.Parsers.Data.Factory
{
    /// <summary>
    /// Factory pro RadioData - hlavní master řádek datagridu
    /// </summary>
    public class RadioDataFactory
    {
        /// <summary>
        /// Vytvoření instance RadioData
        /// </summary>
        /// <param name="url">Url</param>
        public RadioData Create(string url)
        {
            var radioData = new RadioData()
            {
                Url = url,
            };
            return radioData;
        }
    }
}
