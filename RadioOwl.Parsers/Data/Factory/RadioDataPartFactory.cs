namespace RadioOwl.Parsers.Data.Factory
{
    public class RadioDataPartFactory
    {
       public RadioDataPart Create (RadioData radioData, int? partNo, string partTitle, string partDescription, string partUrl)
        {
            var radioDataPart = new RadioDataPart(radioData)
            {
                PartNo = partNo,
                Title = partTitle,
                Description = partDescription,
                UrlMp3 = partUrl
            };
            radioData.PartSet.Add(radioDataPart);
            radioDataPart.AddLog($"Init. (#{partNo} {partUrl})");
            return radioDataPart;
        }
    }
}
