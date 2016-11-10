using Caliburn.Micro;
using RadioOwl.Data;
using RadioOwl.Id3;
using RadioOwl.Radio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using vt.Extensions;
using vt.Http;

namespace RadioOwl.ViewModels
{
    class SniffAroundViewModel : Screen 
    {
        #region Properties

        private readonly int StreamCount = 10; // TODO presunouf do nastaveni?


        public ObservableCollection<StreamUrlRow> UrlStreams { get; set; }

        public StreamUrlRow SelectedRow { get; set; }

        /// <summary>
        /// titulek okna
        /// </summary>
        public string Title { get { return "Očuchat okolní ID"; } }


        private IList _selectedRows;
        public IList SelectedRows
        {
            get { return _selectedRows; }
            set
            {
                _selectedRows = value;
                NotifyOfPropertyChange(() => SelectedRows);
            }
        }

        #endregion


        #region Constructors

        public SniffAroundViewModel(string startUrlStream)
        {
            UrlStreams = PrepareStreamUrls(startUrlStream);
        }

        #endregion


        #region Methods

        /// <summary>
        /// zaridi form a hned vraci string vysledek
        /// </summary>
        public static List<StreamUrlRow> ExecuteModal(string primaryUrlStream)
        {
            try
            {
                var instance = new SniffAroundViewModel(primaryUrlStream);
                var dialogResult = new WindowManager().ShowDialog(instance);
                return (dialogResult.HasValue && dialogResult.Value) ? instance.SelectedRows?.OfType<StreamUrlRow>().ToList() : null;
            }
            catch (Exception ex)
            {
                var exceptionMessages = ex.GetExceptionMessages().ToCrLfText();
                MessageBox.Show(exceptionMessages, string.Format("Chyba při instancování '{0}'.", typeof(SniffAroundViewModel).FullName));
                throw;
            }
        }


        public void ButtonOk()
        {
            TryClose(true);
        }

        #endregion


        #region Methods-Private

        /// <summary>
        /// inicializace radku pro ID v rozsahu +-StreamCount
        /// </summary>
        /// <param name="primaryUrlStream">url s primarnim ID</param>
        private ObservableCollection<StreamUrlRow> PrepareStreamUrls(string primaryUrlStream)
        {
            var result = new ObservableCollection<StreamUrlRow>();

            var primaryStreamId = RadioHelpers.GetStreamIdFromUrl(primaryUrlStream);

            if (!string.IsNullOrEmpty(primaryStreamId))
            {
                for (int i = -StreamCount; i < StreamCount; i++)
                {
                    var id = primaryStreamId.IncrementStringNumber(i);
                    var radioMp3 = RadioHelpers.GetIRadioMp3Url(id);
                    var row = new StreamUrlRow(id, radioMp3);
                    //row.State = (id == primaryStreamId) ? StreamUrlRowState.IsPrimaryId : StreamUrlRowState.None;
                    result.Add(row);
                    DownloadMp3Stream(row);
                }
            }
            else
            {
                MessageBox.Show(string.Format("Nepodařilo se zjistit ID streamu. {0}", primaryUrlStream));
                TryClose();
            }
            return result;
        }


        /// <summary>
        /// stahne zacatek souboru
        /// </summary>
        private async Task DownloadMp3Stream(StreamUrlRow row)
        {
            row.AddLog(string.Format("Stahování streamu: {0}.", row.Url), StreamUrlRowState.Started);
            var output = await new AsyncDownloader().GetDataRange(row.Url, 0, 4096);
            if (output.DownloadOk)
            {
                row.AddLog("Stahování Ok.", StreamUrlRowState.Finnished);
                GetId3(row, output);
            }
            else
            {
                row.AddLog(string.Format("Chyba při stahování streamu: {0}.", output.Exception?.Message), StreamUrlRowState.HttpError);
            }
        }


        /// <summary>
        /// ze stazeneho streamu vykousam Id3 tagy do radku
        /// </summary>
        private static void GetId3(StreamUrlRow row, AsyncDownloaderOutput<Stream> downloaderOutput)
        {
            row.Title = Id3Helper.GetId3TagComment(downloaderOutput.Output);
            if (!string.IsNullOrEmpty(row.Title))
            {
                row.AddLog("Id3 ok.", StreamUrlRowState.Id3Ok);
            }
            else
            {
                row.AddLog("Id3 se nepodařilo zjistit.", StreamUrlRowState.Id3Error);
            }
        }

        #endregion
    }
}
