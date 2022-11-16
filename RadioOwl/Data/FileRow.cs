using System.Collections.Generic;
using System.Windows.Media;
using vt.Extensions;

namespace RadioOwl.Data
{
    /// <summary>
    /// trida radku downloadu
    /// </summary>
    public class FileRow :  PropertyChangedBase
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IList<FileRow> ParentList;

        private string _urlPage;
        /// <summary>
        /// main html page with links (for parsing)
        /// </summary>
        public string UrlPage
        {
            get { return _urlPage; }
            set
            {
                _urlPage = value;
                OnPropertyChanged();
            }
        }

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


        private FileRowState _state;
        public FileRowState State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged();

                StateColor = new SolidColorBrush(SetColorByState());
            }
        }


        private Brush _stateColor;
        /// <summary>
        /// barva se ve WPF binduje na Brush!
        /// </summary>
        public Brush StateColor
        {
            get { return _stateColor; }
            set
            {
                _stateColor = value;
                OnPropertyChanged();
            }
        }


        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged();
                OnPropertyChanged("ProgressPercent");
            }
        }


        private long _bytesReceived;
        public long BytesReceived
        {
            get { return _bytesReceived; }
            set
            {
                _bytesReceived = value;
                OnPropertyChanged();
                OnPropertyChanged("ProgressPercent");
            }
        }


        public string ProgressPercent
        {
            get { return string.Format("{0}%  {1}", Progress, BytesReceived.ToFileSize()); }
        }


        private List<string> _logList;
        public List<string> LogList
        {
            get { return _logList; }
            set
            {
                _logList = value;
                OnPropertyChanged();
            }
        }


        private int _logListIndex;
        public int LogListIndex
        {
            get { return _logListIndex; }
            set
            {
                _logListIndex = value;
                OnPropertyChanged();
            }
        }

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


        public FileRow(IList<FileRow> parentList, string urlPage)
        {
            ParentList = parentList;
            State = FileRowState.Started;
            LogList = new List<string>();
            UrlPage = urlPage;
        }



        public void AddLog(string log)
        {
            LogList.Add(log);
            LogListIndex = LogList.Count - 1;
        }


        public void AddLog(string log, FileRowState fileRowState)
        {
            AddLog(log);
            State = fileRowState;
        }

        
        private Color SetColorByState()
        {
            switch (State)   
            {
                case FileRowState.Started:
                    return Colors.Orange;
                case FileRowState.Finnished:
                    return Colors.LightGreen;
                case FileRowState.Error:
                    return Colors.Red;
                case FileRowState.AlreadyExists:
                    return Colors.Yellow;

                default:
                    return Colors.Blue;
            }
        }
    }
}
