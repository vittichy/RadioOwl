using System.Linq;
using System.Collections.ObjectModel;
using vt.Extensions;

namespace RadioOwl.Data
{
    /// <summary>
    /// trida pro drzeni total progressu vsech downloadu
    /// </summary>
    public class TotalProgress : PropertyChangedBase
    {
        private string _bytesReceived;
        public string BytesReceived
        {
            get { return _bytesReceived; }
            set
            {
                _bytesReceived = value;
                OnPropertyChanged();
            }
        }


        internal void UpdateProgress(ObservableCollection<FileRow> files)
        {
            BytesReceived = files.Sum(p => p.BytesReceived).ToFileSize();
        }
    }
}
