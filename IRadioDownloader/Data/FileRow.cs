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

        private string _url;
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                OnPropertyChanged();
            }
        }

        private string _id;
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


        private string _album;
        public string Album
        {
            get { return _album; }
            set
            {
                _album = value;
                OnPropertyChanged();
            }
        }


        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
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
                StateColor = new SolidColorBrush(SetStateColor());
                OnPropertyChanged();
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

        private string _savedFileName;
        public string SavedFileName
        {
            get { return _savedFileName; }
            set
            {
                _savedFileName = value;
                OnPropertyChanged();
            }
        }


        public FileRow(IList<FileRow> parentList, string url)
        {
            ParentList = parentList;
            State = FileRowState.Started;
            LogList = new List<string>();
            Url = url;
        }

        public FileRow(IList<FileRow> parentList, StreamUrlRow streamUrlRow) : this(parentList, streamUrlRow?.Url)
        {
            Title = streamUrlRow?.Title;
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


        
        private Color SetStateColor()
        {
            switch (State)   
            {
                case FileRowState.Started:
                    return Colors.Orange;
                case FileRowState.Finnished:
                    return Colors.LightGreen;
                case FileRowState.Error:
                    return Colors.Red;
                default:
                    return Colors.Blue;
            }
        }
    }
}
