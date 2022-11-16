namespace RadioOwl.Parsers.Data
{
    /// <summary>
    /// Stav řádku s downloadem
    /// </summary>
    public enum RadioDataPartState
    {
        None,

        /// <summary>
        /// Parsování web stránky pořadu
        /// </summary>
        Parse,

        /// <summary>
        /// Start stahování
        /// </summary>
        Started,

        /// <summary>
        /// Stažený soubor již existuje
        /// </summary>
        FileAlreadyExists,

        /// <summary>
        /// Ukončeno
        /// </summary>
        Finnished,

        /// <summary>
        /// Ukončeno s chybou
        /// </summary>
        Error,
    }
}
