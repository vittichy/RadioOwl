﻿//using Dtc.Common.Extensions;
using RadioOwl.Data;
//using RadioOwl.PageParsers;
//using RadioOwl.PageParsers.Data;
//using RadioOwl.Radio;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using vt.Http;

namespace RadioOwl.ViewModels
{
    /// <summary>
    /// TODO
    /// - rozumny format filenamu 
    /// - mozna doplnoval id3?
    /// - nektere odkazy jsou na WAV ne MP3: http://pardubice.rozhlas.cz/zaver-zivota-v-ustavu-ne-prectete-si-pribehy-tri-krehkych-bojovniku-7692592?player=on#player
    /// 
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    class ShellViewModel : Caliburn.Micro.PropertyChangedBase
    {
        private const string DRAG_DROP_FORMAT = "UnicodeText";

        #region Properties

        //private readonly Parsers parsers = new Parsers();

        /// <summary>
        /// titulek okna
        /// </summary>
        public string Title { get { return string.Format("RadioOwl {0}", System.Reflection.Assembly.GetExecutingAssembly()?.GetName()?.Version); } }

        public ObservableCollection<FileRow> Files { get; set; }

        private FileRow _selectedRow;

        public FileRow SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                _selectedRow = value;
                NotifyOfPropertyChange(() => SelectedRow);
                NotifyOfPropertyChange(() => CanPlayRow);
                NotifyOfPropertyChange(() => CanDeleteRow);
            }
        }
        
        public TotalProgress TotalProgress { get; set; }

        /// <summary>
        /// selected radek je Finnished a ulozen na disk
        /// </summary>
        public bool SelectedRowIsSaved { get { return (SelectedRow != null) && (SelectedRow.State == FileRowState.Finnished) && !string.IsNullOrEmpty(SelectedRow.FileName); } }

        #endregion


        #region Properties-CanExecute

        public bool CanPlayRow { get { return SelectedRowIsSaved; } }

        public bool CanDeleteRow { get { return (SelectedRow != null); } }

        #endregion


        #region Constructors

        public ShellViewModel()
        {
            Files = new ObservableCollection<FileRow>();
            TotalProgress = new TotalProgress();
        }

        #endregion


        #region Methods

        /// <summary>
        /// akce prehrani aktualniho zaznamu
        /// </summary>
        public void PlayRow()
        {
            if (CanPlayRow)
            {
                System.Diagnostics.Process.Start(SelectedRow.FileName);
            }
        }


        /// <summary>
        /// smazani aktualniho radku
        /// </summary>
        public void DeleteRow()
        {
            if (CanDeleteRow) // nastartovany task by bylo nutne cancelovat, to zatim neresim
            {
                var messageBoxResult = MessageBox.Show("Opravdu smazat?", "Potvrzení", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Files.Remove(SelectedRow);
                }
            }
        }


        public void EventDrop(DragEventArgs e)
        {
            // nefunguje-li drag-drop nezoufej a pust visual studio jako uzivatel a ne pod adminem!
            var url = GetPageUrl(e);
            ProcessUrl(url);
        }


        public void EventPreviewDragOver(DragEventArgs e)
        {
            //var url = GetPageUrl(e);
            //var parser = parsers.Chain.CanParse(url);
            //e.Effects = (parser != null) ? DragDropEffects.Copy : DragDropEffects.None;
            //e.Handled = true;
        }


        /// <summary>
        /// akce Načíst z URL
        /// </summary>
        public void OpenUrl()
        {
            var url = InputBoxViewModel.ExecuteModal("Načíst z URL", "URL:");
            ProcessUrl(url);
        }

        #endregion


        #region Methods-Private

        /// <summary>
        /// v DragEventArgs dokaze najit zpracovatelne Url
        /// </summary>
        private string GetPageUrl(DragEventArgs e)
        {
            if ((e != null) && (e.Data != null))
            {
                if (e.Data.GetDataPresent(DRAG_DROP_FORMAT))
                {
                    var url = (e.Data.GetData(DRAG_DROP_FORMAT) as string);
                    //if (RadioHelpers.IsUrlToIRadio(url)) 
                    //{
                    //    return url;
                    //}
                }
            }
            return null;
        }


        /// <summary>
        /// zahaji zpracovani url
        /// </summary>
        private void ProcessUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;

            var fileRow = new FileRow(Files, url);
            Files.Add(fileRow);
            fileRow.AddLog("Stahuji stránku pořadu.", FileRowState.Started);

            ProcessUrl(fileRow);
        }


        private async void ProcessUrl(FileRow fileRow)
        {
            if (fileRow == null)
                return;

            //var decoder = await parsers.Chain.CanParse(fileRow.UrlPage);
            //if(decoder != null)
            //{
            //    var decoderResult = await decoder.Parse(fileRow.UrlPage);
            //    if (decoderResult != null)
            //    {
            //        if (!decoderResult.LogSet.Any())
            //        {
            //            //if (decoderResult.RozhlasUrlSet.Any())
            //            //    ProcessParserResult(fileRow, decoderResult);
            //            //else
            //            //    fileRow.AddLog($"Dekoder '{decoder?.GetType()?.FullName}' nevrátil seznam mp3 url pro: {fileRow.UrlPage}");
            //        }
            //        else
            //        {
            //            decoderResult.LogSet.ForEach(p => fileRow.AddLog(p));
            //        }
            //    }
            //    else
            //    {
            //        fileRow.AddLog($"Dekoder '{decoder?.GetType()?.FullName}' nevrátil žádná data pro url: {fileRow.UrlPage}");
            //    }
            //}
            //else
            //{
            //    fileRow.AddLog($"Nepodařilo se dohledat dekoder pro url: {fileRow.UrlPage}.");
            //}
        }


        //private void ProcessParserResult(FileRow mainFileRow, ParserResult decoderResult)
        //{
        //    if (mainFileRow == null || decoderResult == null || !decoderResult.RozhlasUrlSet.Any())
        //        return;

        //    var isMultipart = (decoderResult.RozhlasUrlSet.Count > 1);

        //    // first or main download link
        //    DownloadMp3StreamStart(decoderResult, decoderResult.RozhlasUrlSet[0], mainFileRow);

        //    // next download links... (more part on one html page)
        //    if (isMultipart)
        //    {
        //        for (int i = 1; i < decoderResult.RozhlasUrlSet.Count; i++)
        //        {
        //            var fileRow = new FileRow(Files, mainFileRow.UrlPage);
        //            Files.Add(fileRow);

        //            DownloadMp3StreamStart(decoderResult, decoderResult.RozhlasUrlSet[i], fileRow);
        //        }
        //    }
        ////}


        //private void DownloadMp3StreamStart(ParserResult decoderResult, RozhlasUrl rozhlasUrl, FileRow fileRow /*bool isMultipart,*/  /*int? partNo, string logMessage*/)
        //{
        //    if (fileRow == null || rozhlasUrl == null || decoderResult == null)
        //        return;

        //    // cela skupina poradu - pokdu jich je na strance odkazovano vice
        //    fileRow.MetaSiteName = decoderResult.MetaSiteName;
        //    fileRow.MetaTitle = decoderResult.MetaTitle;
        //    fileRow.MetaDescription = decoderResult.MetaDescription;

        //    // pro jednotlivy porad
        //    fileRow.UrlMp3Download = rozhlasUrl.Url;
        //    fileRow.MetaSubTitle = rozhlasUrl.Title;

        //    // ted uz mohu dopocitat filename pro ulozeni
        //    GetFileName(fileRow);
        //    if (File.Exists(fileRow.FileName))
        //    {
        //        fileRow.Progress = 100;
        //        fileRow.AddLog(string.Format("Soubor již existuje: {0}.", fileRow.FileName), FileRowState.AlreadyExists);
        //    }
        //    else
        //        DownloadMp3Stream(fileRow);
        //}


        //private async void DownloadMp3Stream(FileRow fileRow)
        //{
        //    fileRow.AddLog(string.Format("Zahájení stahování streamu: {0}", fileRow.UrlPage));

        //    var asyncDownloader = new AsyncDownloader();
        //    var output = await asyncDownloader.GetData(fileRow.UrlMp3Download,
        //                                               p =>
        //                                               {
        //                                                   fileRow.Progress = p.ProgressPercentage;
        //                                                   fileRow.BytesReceived = p.BytesReceived;
        //                                                   TotalProgress.UpdateProgress(Files);
        //                                               });
        //    if (output.DownloadOk)
        //        SaveMp3(fileRow, output.Output);
        //    else
        //        fileRow.AddLog(string.Format("Chyba při stahování streamu: {0}.", output.Exception?.Message), FileRowState.Error);
        //}


        private void SaveMp3(FileRow fileRow, byte[] data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileRow.FileName));

            using (var file = new FileStream(fileRow.FileName, FileMode.Create, FileAccess.Write))
            {
                file.Write(data, 0, data.Length);
            }
            fileRow.Saved = true;
            fileRow.AddLog(string.Format("Uložen soubor: {0}", fileRow.FileName), FileRowState.Finnished);
        }


        /// <summary>
        /// sestrojeni filename - jiz nemohu spolehat na ID3 tagy, nove rozhlas mp3 soubory je neobsahujou :-/
        /// </summary>
        private void GetFileName(FileRow fileRow)
        {
            // filename - nejdrive vezmu title samotneho dilu poradu, neni-li tak hromadny title, neni-li tak vygeneruju nejake tmp jmeno
            var fileName = GetFilenameSafeString(fileRow.MetaSubTitle);
            if(string.IsNullOrWhiteSpace(fileName))
            {
                fileName = GetFilenameSafeString(fileRow.MetaTitle);
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = $"RADIOOWL_DOWNLOAD_{Guid.NewGuid()}";
                }
            }

            // pouzit hromadny titul (pri vice-dilnem downloadu) jako folder?
            if (!string.Equals(fileRow.MetaTitle, fileRow.MetaSubTitle))
            {
                fileName = Path.Combine(GetFilenameSafeString(fileRow.MetaTitle), fileName);
            }

            // pouzit jmeno stanice jako folder?
            if (!string.IsNullOrWhiteSpace(fileRow.MetaSiteName))
                fileName = Path.Combine(GetFilenameSafeString(fileRow.MetaSiteName), fileName);

            // pridej root adresar
            fileRow.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), fileName) + ".mp3";
        }


        /// <summary>
        /// ensure suitable characters & max length of path (max is abou 248-250 chars)
        /// </summary>
        private string GetFilenameSafeString(string filename)
        {
            return null; // string.Join("_", (filename?.Trim() ?? string.Empty).Split(Path.GetInvalidFileNameChars())).Left(60);
        }


        /// <summary>
        /// pokusim se zjistit unikatni jmeno
        /// </summary>
        private string GetUniqSaveName(string rootPath, string filePath, string extension)
        {
            var fileName = Path.Combine(rootPath, filePath);

            var fullFileName = string.Empty;
            var attemptNo = 0;
            do
            {
                var duplicateExtension = (attemptNo++ > 0) ? $"_{attemptNo}" : null;
                fullFileName = $"{fileName}{duplicateExtension}.{extension}";
            } while (File.Exists(fullFileName));

            return fullFileName;
        }

        #endregion
    }
}
