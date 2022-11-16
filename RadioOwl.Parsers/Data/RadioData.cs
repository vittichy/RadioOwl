using Dtc.Common.Extensions;
using RadioOwl.Parsers.Data.Factory;
using RadioOwl.Parsers.Data.Helper;
using RadioOwl.Parsers.Data.PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Windows.Media;

namespace RadioOwl.Parsers.Data
{
    /// <summary>
    /// Hlavní data o pořadu - má pod sebou jeden nebo více částí viz <see cref="RadioDataPart"/>
    /// </summary>
    public class RadioData : PropertyChangedBase
    {
        /// <summary>
        /// Html kod pro url
        /// </summary>
        public string HtmlPage;

        /// <summary>
        /// Části - jendotlivé díly pořadu a pod
        /// </summary>
        public ObservableCollection<RadioDataPart> PartSet { get; set; } = new ObservableCollection<RadioDataPart>();

        /// <summary>
        /// Seznam log hlášení - z jednotlivých částí přidávám hlášení i sem, takže by to měl být komplet seznam
        /// </summary>
        public ObservableCollection<RadioLog> LogSet { get; set; } = new ObservableCollection<RadioLog>();

        private int _logSetSelectedIndex;
        /// <summary>
        /// SelectedIndex z seznamu logů - jen pro pěkné zobrazování v lookupu logu v datagridu
        /// </summary>
        public int LogSetSelectedIndex
        {
            get { return _logSetSelectedIndex; }
            set
            {
                _logSetSelectedIndex = value;
                OnPropertyChanged();
            }
        }

        private string _url;
        /// <summary>
        /// Hlavní url (to co bylo dotaženo přes drag-drop)
        /// </summary>
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Url s obrázkem k pořadu
        /// </summary>
        public string ImageUrl;

        /// <summary>
        /// Celková velikost všech částí
        /// </summary>
        public long TotalBytesToReceive { get { return PartSet.Sum(p => p.TotalBytesToReceive); } }

        /// <summary>
        /// Celková velikost všech částí - jako text (123MB a pod)
        /// </summary>
        public string TotalBytesToReceiveAsText { get { return TotalBytesToReceive.ToFileSize(); } }

        /// <summary>
        /// Celková velikost ze všech stažených částí
        /// </summary>
        public long BytesReceived { get { return PartSet.Sum(p => p.BytesReceived); } }

        /// <summary>
        /// Celková velikost ze všech stažených částí
        /// </summary>
        public string BytesReceivedAsText { get { return BytesReceived.ToFileSize(); } }

        /// <summary>
        /// Celkový total progres ze všech částí v %
        /// </summary>
        public long ProgressPercentage
        {
            get
            {
                if (TotalBytesToReceive == 0)
                {
                    return 0;
                }
                return BytesReceived == 0 ? 0 : (long)Math.Round(((decimal)BytesReceived / (decimal)TotalBytesToReceive) * 100, 0);
            }
        }

        /// <summary>
        /// Celkový total progres ze všech částí v % jako text
        /// </summary>
        public string ProgressPercentageAsText { get { return RadioDataHelper.ProgressText(ProgressPercentage, BytesReceived); } }

        /// <summary>
        /// Detailní popis pořadu
        /// </summary>
        public string DetailDescription;
























        ///// <summary>
        ///// 
        ///// </summary>
        //public List<RadioLog> LogWithPartsSet { get; set; } = new List<RadioLog>();






        private string _siteEntityBundle;
        /// <summary>
        /// 
        /// </summary>
        public string SiteEntityBundle
        {
            get { return _siteEntityBundle; }
            set
            {
                _siteEntityBundle = value;
                OnPropertyChanged();
            }
        }
        private string _siteEntityLabel;
        /// <summary>
        /// 
        /// </summary>
        public string SiteEntityLabel
        {
            get { return _siteEntityLabel; }
            set
            {
                _siteEntityLabel = value;
                OnPropertyChanged();
            }
        }
        private string _siteDocumentPath;
        /// <summary>
        /// 
        /// </summary>
        public string SiteDocumentPath
        {
            get { return _siteDocumentPath; }
            set
            {
                _siteDocumentPath = value;
                OnPropertyChanged();
            }
        }


        private int? _contentSerialAllParts;
        /// <summary>
        /// Počet dílů zjištěný parsováním
        /// </summary>
        public int? ContentSerialAllParts
        {
            get { return _contentSerialAllParts; }
            set
            {
                _contentSerialAllParts = value;
                OnPropertyChanged();
            }
        }








        private string _contentId;
        /// <summary>
        /// 
        /// </summary>
        public string ContentId
        {
            get { return _contentId; }
            set
            {
                _contentId = value;
                OnPropertyChanged();
            }
        }

















      

        ///// <summary>
        ///// TODO nepujde volat rovnou z jednotliv7ych casti?
        ///// </summary>
        //public void UpdateTotalProgress()
        //{
        //    //OnPropertyChanged(() => TotalBytesToReceive);
        //    //OnPropertyChanged(() => TotalBytesToReceiveAsText);
        //    //OnPropertyChanged(() => BytesReceived);
        //    //OnPropertyChanged(() => BytesReceivedAsText);
        //   OnPropertyChanged(() => ProgressPercentage);
        //    OnPropertyChanged(() => ProgressPercentageAsText);

        //    //   TotalBytesToReceive = PartSet.Sum(p => p.TotalBytesToReceive);
        //}





        ///// <summary>
        ///// 
        ///// </summary>
        //public void OnStatePropertyChanged()
        //{
        //    OnPropertyChanged(() => TotalBytesToReceive);
        //    OnPropertyChanged(() => TotalBytesToReceiveAsText);
        //    OnPropertyChanged(() => State);
        //    OnPropertyChanged(() => StateColor);
        //}


        /// <summary>
        /// Signál z <see cref="RadioDataPart"/>, že na něm došlo k nějaké změně
        /// </summary>
        public void PartChanged()
        {
            OnPropertyChanged(() => ProgressPercentage);
            OnPropertyChanged(() => ProgressPercentageAsText);
            OnPropertyChanged(() => TotalBytesToReceive);
            OnPropertyChanged(() => TotalBytesToReceiveAsText);
            OnPropertyChanged(() => State);
            OnPropertyChanged(() => StateColor);
        }


        /// <summary>
        /// Stav záznamu
        /// </summary>
        public RadioDataPartState State
        {
            get
            {
                // žádné díly pořadu?
                if (PartSet == null || !PartSet.Any())
                {
                    return RadioDataPartState.None;
                }
                // všechny záznamy stejný State? - nemusi byt nutne vse stazene, pocet dilu nemusi odpovidat rozparsovanemu poctu (viz ContentSerialAllParts)
                var groups = PartSet.GroupBy(p => p.State);
                if (groups.Count() == 1)
                {
                    return groups.First().Key;
                }
                // nějaky error
                if (PartSet.Any(p => p.State == RadioDataPartState.Error))
                {
                    return RadioDataPartState.Error;
                }
                // něco probíhá
                if (PartSet.Any(p => p.State == RadioDataPartState.Started))
                {
                    return RadioDataPartState.Started;
                }
                return RadioDataPartState.None;
            }          
        }

        /// <summary>
        /// Barva dle stavu řádku
        /// </summary>
        public Brush StateColor => new RadioDataPartStateHelper().ToBrush(State);





























        private string _urlMp3Download;
        /// <summary>
        /// url for mp3 file
        /// </summary>
        public string UrlMp3Download
        {
            get { return _urlMp3Download; }
            set
            {
                _urlMp3Download = value;
                OnPropertyChanged();
            }
        }

        //private int? _urlMp3DownloadNo = null;
        //public int? UrlMp3DownloadNo
        //{
        //    get { return _urlMp3DownloadNo; }
        //    set
        //    {
        //        _urlMp3DownloadNo = value;
        //        OnPropertyChanged();
        //    }
        //}

        private string _id;
        // TODO delete?
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }


        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }


        private string _readMeFileName;
        public string ReadMeFileName
        {
            get { return _readMeFileName; }
            set
            {
                _readMeFileName = value;
                OnPropertyChanged();
            }
        }


        //private string _id3Name;
        //public string Id3Name
        //{
        //    get { return _id3Name; }
        //    set
        //    {
        //        _id3Name = value;
        //        OnPropertyChanged();
        //    }
        //}


        //private string _id3NamePart;
        //public string Id3NamePart
        //{
        //    get { return _id3NamePart; }
        //    set
        //    {
        //        _id3NamePart = value;
        //        OnPropertyChanged();
        //    }
        //}


        //private string _id3NameSite;
        //public string Id3NameSite
        //{
        //    get { return _id3NameSite; }
        //    set
        //    {
        //        _id3NameSite = value;
        //        OnPropertyChanged();
        //    }
        //}


        //private FileRowState _state;
        //public FileRowState State
        //{
        //    get => _state;
        //    set
        //    {
        //        _state = value;
        //        // StateColor = new SolidColorBrush(SetStateColor());
        //        OnPropertyChanged();
        //        OnPropertyChanged("StateColor");
        //    }
        //}


        //      private Brush _stateColor;
        /// <summary>
        ///// barva se ve WPF binduje na Brush!
        ///// </summary>
        //public Brush StateColor
        //{
        //    get
        //    {
        //        Color color;
        //        switch (State)
        //        {
        //            case FileRowState.Parse:
        //                color = Colors.Yellow;
        //                break;
        //            case FileRowState.Started:
        //                color = Colors.Orange;
        //                break;
        //            case FileRowState.Finnished:
        //                color = Colors.LightGreen;
        //                break;
        //            case FileRowState.Error:
        //                color = Colors.Red;
        //                break;
        //            case FileRowState.FileAlreadyExists:
        //                color = Colors.YellowGreen;
        //                break;
        //            default:
        //                color = Colors.Blue;
        //                break;
        //        }
        //        return new SolidColorBrush(color);




        //        //return _state;
        //    }
        //    // get { return _stateColor; }
        //    //set
        //    //{
        //    //    _stateColor = value;
        //    //    OnPropertyChanged();
        //    //}
        //}


        //private int _progress;
        //public int Progress
        //{
        //    get { return _progress; }
        //    set
        //    {
        //        _progress = value;
        //        OnPropertyChanged();
        //        OnPropertyChanged("ProgressPercent");
        //    }
        //}


        //private long _bytesReceived;
        //public long BytesReceived
        //{
        //    get { return _bytesReceived; }
        //    set
        //    {
        //        _bytesReceived = value;
        //        OnPropertyChanged();
        //        OnPropertyChanged("ProgressPercent");
        //    }
        //}


        //public string ProgressPercent
        //{
        //    get { return string.Format("{0}%  {1}", Progress, BytesReceived.ToFileSize()); }
        //}


        //private List<string> _logList;
        //public List<string> LogList
        //{
        //    get { return _logList; }
        //    set
        //    {
        //        _logList = value;
        //        OnPropertyChanged();
        //    }
        //}


        //private int _logListIndex;
        //public int LogListIndex
        //{
        //    get { return _logListIndex; }
        //    set
        //    {
        //        _logListIndex = value;
        //        OnPropertyChanged();
        //    }
        //}

        private bool _saved;
        public bool Saved
        {
            get { return _saved; }
            set
            {
                _saved = value;
                OnPropertyChanged();
            }
        }






        private string _metaSiteName;
        public string MetaSiteName
        {
            get { return _metaSiteName; }
            set
            {
                _metaSiteName = value;
                OnPropertyChanged();
            }
        }

        private string _metaTitle;
        public string MetaTitle
        {
            get { return _metaTitle; }
            set
            {
                _metaTitle = value;
                OnPropertyChanged();
            }
        }

        private string _metaDescription;
        public string MetaDescription
        {
            get { return _metaDescription; }
            set
            {
                _metaDescription = value;
                OnPropertyChanged();
            }
        }

        private string _metaSubTitle;
        public string MetaSubTitle
        {
            get { return _metaSubTitle; }
            set
            {
                _metaSubTitle = value;
                OnPropertyChanged();
            }
        }

        private string _readMeText;
        public string ReadMeText
        {
            get { return _readMeText; }
            set
            {
                _readMeText = value;
                OnPropertyChanged();
            }
        }












        //   public RadioData(/*IList<FileRow> parentList,*/ string url)
        //   {
        // //      ParentList = parentList;
        //       State = FileRowState.Started;
        ////       LogList = new List<string>();
        //       Url = url;
        //   }

        //public FileRow(IList<FileRow> parentList, StreamUrlRow streamUrlRow) : this(parentList, streamUrlRow?.Url)
        //{
        //    Id3NamePart = streamUrlRow?.Title;
        //}


        /// <summary>
        /// Přidá záznam do logu
        /// </summary>
        public void AddLog(string msg, EventLevel eventLevel = EventLevel.Informational)
        {
            var radioLog = new RadioLogFactory().Create(msg, eventLevel);
            LogSet.Add(radioLog);
            LogSetSelectedIndex = LogSet.Count();
        }

        public void AddLogError(string msg)
        {
            AddLog(msg, EventLevel.Error);
        }














        //public void AddLog(string log)
        //{
        //    AddLog(log);
        //}


        
        //private Color SetStateColor()
        //{
        //    switch (State)   
        //    {
        //        case FileRowState.Parse:
        //            return Colors.Yellow;
        //        case FileRowState.Started:
        //            return Colors.Orange;
        //        case FileRowState.Finnished:
        //            return Colors.LightGreen;
        //        case FileRowState.Error:
        //            return Colors.Red;
        //        case FileRowState.FileAlreadyExists:
        //            return Colors.YellowGreen;
        //            //return Colors.SlateGray;
        //        default:
        //            return Colors.Blue;
        //    }
        //}










    }
}
