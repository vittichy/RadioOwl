namespace RadioOwl.Data
{
    /// <summary>
    /// stav radku s ocuchavanymi streamy
    /// </summary>
    public enum StreamUrlRowState
    {
        None,
        // ? IsPrimaryId, - nejspis nebude potreba
        Started,
        Finnished,
        HttpError,
        Id3Ok,
        Id3Error
    }
}
