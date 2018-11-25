﻿using RadioOwl.PageParsers.Data;
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
        /// parent collection of all FileRows (source of datagrid)
        /// </summary>
        private readonly IList<FileRow> ParentList;

        private string _url;
        public string Url
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


        private FileRowState _state;
        public FileRowState State
        {
            get { return _state; }
            set
            {
                _state = value;
                StateColor = new SolidColorBrush(SetStateColor());
                OnPropertyChanged();
                OnPropertyChanged("StateColor");
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

        //public FileRow(IList<FileRow> parentList, StreamUrlRow streamUrlRow) : this(parentList, streamUrlRow?.Url)
        //{
        //    Id3NamePart = streamUrlRow?.Title;
        //}


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
                case FileRowState.AlreadyExists:
                    return Colors.YellowGreen;
                    //return Colors.SlateGray;
                default:
                    return Colors.Blue;
            }
        }
    }
}
