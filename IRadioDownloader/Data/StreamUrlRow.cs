using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace RadioOwl.Data
{
    /// <summary>
    /// trida radku url streamu pro form prohledavani okolnich ID
    /// </summary>
    public class StreamUrlRow : PropertyChangedBase
    {
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

        private StreamUrlRowState _state;
        public StreamUrlRowState State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged();
                SetStateColor();
            }
        }


        private Brush _stateColor;
        /// <summary>
        /// barva se ve WPF binduje na Brush
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
      

        public StreamUrlRow(string id, string url)
        {
            LogList = new List<string>();
            Id = id;
            Url = url;
            State = StreamUrlRowState.None;
        }


        public void AddLog(string log)
        {
            LogList.Add(log);
            LogListIndex = LogList.Count - 1;
        }


        public void AddLog(string log, StreamUrlRowState state)
        {
            AddLog(log);
            State = state;
        }
        

        private void SetStateColor()
        {
            Color color;
            switch (State)
            {
                //case StreamUrlRowState.IsPrimaryId:
                //    color = Colors.Silver;
                //    break;
                case StreamUrlRowState.Started:
                    color = Colors.Orange;
                    break;
                case StreamUrlRowState.Finnished:
                case StreamUrlRowState.Id3Ok:
                    color = Colors.LightGreen;
                    break;
                case StreamUrlRowState.HttpError:
                    color = Colors.Plum;
                    break;
                case StreamUrlRowState.Id3Error:
                    color = Colors.LightPink;
                    break;
                default:
                    color = Colors.Blue;
                    break;
            }
            StateColor = new SolidColorBrush(color);
        }
    }
}

