using HtmlAgilityPack;
using RadioOwl.Data;
using RadioOwl.Id3;
using RadioOwl.Radio;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using vt.Extensions;
using vt.Http;

namespace RadioOwl.ViewModels
{
    class ShellViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Consts
        
        private const string DRAG_DROP_FORMAT = "UnicodeText";
        private const int FILENAME_MAX_LEN = 150;

        #endregion


        #region Properties

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
                NotifyOfPropertyChange(() => CanSniffAround);
                NotifyOfPropertyChange(() => CanPlayRow);
                NotifyOfPropertyChange(() => CanDeleteRow);
            }
        }
        
        public TotalProgress TotalProgress { get; set; }

        /// <summary>
        /// selected radek je Finnished a ulozen na disk
        /// </summary>
        public bool SelectedRowIsSaved { get { return ((SelectedRow != null) && (SelectedRow.State == FileRowState.Finnished) && !string.IsNullOrEmpty(SelectedRow.SavedFileName)); } }

        #endregion


        #region Properties-CanExecute

        /// <summary>
        /// can execute akce pro metodu SniffAround() - v caliburnu to lze resit nejen metodou, ale i pres takovouhle property, coz ma vyhodu v tom, ze ji lze odpalit v NotifyOfPropertyChange!
        /// </summary>
        public bool CanSniffAround { get { return (SelectedRow != null); } }

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
                System.Diagnostics.Process.Start(SelectedRow.SavedFileName);
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
            if (!string.IsNullOrEmpty(url))
            {
                ProcessUrl(url);
            }
        }


        public void EventPreviewDragOver(DragEventArgs e)
        {
            var url = GetPageUrl(e);
            if (!string.IsNullOrEmpty(url))
            {
                e.Handled = true;
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }


        /// <summary>
        /// akce Načíst z URL
        /// </summary>
        public void OpenUrl()
        {
            var url = InputBoxViewModel.ExecuteModal("Načíst z URL", "URL:");
            if (!string.IsNullOrEmpty(url))
            {
                if (RadioHelpers.IsUrlToIRadio(url))
                {
                    ProcessUrl(url);
                }
                else
                {
                    MessageBox.Show("Neznámý typ URL.", "Chyba");
                }
            }
        }


        /// <summary>
        /// spusteni formulare s prohledanim okolnich ID (Očuchat okolní ID)
        /// </summary>
        public void SniffAround()
        {
            if (CanSniffAround)
            {
                var urlStreams = SniffAroundViewModel.ExecuteModal(SelectedRow?.Url);
                urlStreams?.ForEach(p => ProcessUrl(p));
            }
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
                    if (RadioHelpers.IsUrlToIRadio(url)) 
                    {
                        return url;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// zahaji zpracovani url
        /// </summary>
        private void ProcessUrl(StreamUrlRow streamUrlRow)
        {
            var fileRow = new FileRow(streamUrlRow);
            ProcessUrlRow(fileRow, fileRow.Url);
        }


        /// <summary>
        /// zahaji zpracovani url
        /// </summary>
        private void ProcessUrl(string url)
        {
            var fileRow = new FileRow(url);
            ProcessUrlRow(fileRow, url);
        }


        /// <summary>
        /// zahaji zpracovani radku pro dotazenou url
        /// </summary>
        private void ProcessUrlRow(FileRow fileRow, string url)
        {
            Files.Insert(0, fileRow);
            fileRow.AddLog("Stahuji stránku pořadu.", FileRowState.Started);

            StartDownload(fileRow, url);
        }


        private void StartDownload(FileRow fileRow, string url)
        {
            if (RadioHelpers.IsUrlToIRadioDownload(url))
            {
                DownloadMp3Stream(fileRow, url);
            }
            else if (RadioHelpers.IsUrlToIRadioPlayPage(url))
            {
                StartDownloadFromPageUrl(fileRow, url);
            }
            else
            {
                fileRow.AddLog(string.Format("Neznámé url: {0}.", url));
            }
        }


        private async void StartDownloadFromPageUrl(FileRow fileRow, string url)
        {
            var asyncDownloader = new AsyncDownloader();
            var output = await asyncDownloader.GetString(url);
            if (output.DownloadOk)
            {
                ParseIRadioHtmlPage(fileRow, output.Output);
            }
            else
            {
                fileRow.AddLog(string.Format("Chyba při stahování stránku pořadu: {0}.", output.Exception?.Message));
            }
        }

        
        /// <summary>
        /// rozparsovani html stranky poradu
        /// </summary>
        private void ParseIRadioHtmlPage(FileRow fileRow, string html) 
        {
            try
            {
                // html nemusi byt validni xml, takze je potreba pro parsovani pouzit Html Agility Pack, viz http://htmlagilitypack.codeplex.com/
                // http://www.c-sharpcorner.com/UploadFile/9b86d4/getting-started-with-html-agility-pack/
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // <div id="player-track" class="player uniplayer" data-mode="audio" data-type="ondemand" data-autostart="1" data-id="3696063" data-event_label="Glosa [3696063]" data-duration="99" data-primary="html5" data-debug="1"></div>
                fileRow.Id = FindAttributeValue(htmlDoc, @"//div[@id='player-track']", "data-id");

                // <meta name="og:title" content="Dvojka - Glosa (01.09.2016 06:22)">
                fileRow.StreamName = FindAttributeValue(htmlDoc, @"//meta[@name='og:title']", "content");

                // <p title="Marie Procházková: Paralínek (1/6). Paralínek se učí létat. Malý ptáček Paralínek bydlí v hnízdě ...
                fileRow.Title = FindAttributeValue(htmlDoc, @"//body//div/p[@title]", "title");

                if (string.IsNullOrEmpty(fileRow.Id))
                {
                    fileRow.AddLog("Chyba při parsování stránky pořadu - nepodařilo se dohledat ID streamu.", FileRowState.Error);
                    return;
                }
                if (string.IsNullOrEmpty(fileRow.StreamName))
                {
                    fileRow.AddLog("Chyba při parsování stránky pořadu - nepodařilo se dohledat TITLE pořadu.", FileRowState.Error);
                    return;
                }

                var streamUrl = RadioHelpers.GetIRadioMp3Url(fileRow.Id);
                DownloadMp3Stream(fileRow, streamUrl);
            }
            catch (Exception ex)
            {
                fileRow.AddLog(string.Format("Chyba při stahování streamu: {0}.", ex.Message), FileRowState.Error);
            }
        }


        private async void DownloadMp3Stream(FileRow fileRow, string streamUrl)
        {
            fileRow.AddLog(string.Format("Stahuji stream: {0}", streamUrl));

            var asyncDownloader = new AsyncDownloader();
            var output = await asyncDownloader.GetData(streamUrl,
                                                       p =>
                                                       { 
                                                           fileRow.Progress = p.ProgressPercentage;
                                                           fileRow.BytesReceived = p.BytesReceived;
                                                           TotalProgress.UpdateProgress(Files); 
                                                       });
            if (output.DownloadOk)
            {
                SaveMp3(fileRow, output.Output);
            }
            else
            {
                fileRow.AddLog(string.Format("Chyba při stahování streamu: {0}.", output.Exception?.Message), FileRowState.Error);
            }
        }


        private void SaveMp3(FileRow fileRow, byte[] data) 
        {
            var id3CommentTag = Id3Helper.GetId3TagComment(data);
            var newFileNameOk = GetUniqSaveName(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), id3CommentTag, "mp3");

            using (var file = new FileStream(newFileNameOk, FileMode.Create, FileAccess.Write))
            {
                file.Write(data, 0, data.Length);
            }
            fileRow.SavedFileName = newFileNameOk;
            if (string.IsNullOrEmpty(fileRow.Title))
            {
                fileRow.Title = id3CommentTag;
            }
            fileRow.AddLog(string.Format("Uložen soubor: {0}", newFileNameOk), FileRowState.Finnished);
        }


        /// <summary>
        /// pokusim se zjistit unikatni jmeno
        /// </summary>
        private string GetUniqSaveName(string path, string id3CommentTag, string extension)
        {
            
            var fileName = (id3CommentTag ?? "_").ReplaceInvalidFilenameChar().TrimToMaxLen(FILENAME_MAX_LEN);
            string fullPathFilename;
            var attemptNo = 0;
            do
            {
                var duplicateExtension = (attemptNo++ > 0) ? string.Format("_{0}", attemptNo) : null;
                var fullFileName = string.Format("{0}{1}.{2}", fileName, duplicateExtension, extension);
                fullPathFilename = Path.Combine(path, fullFileName);
            } while (System.IO.File.Exists(fullPathFilename));

            return fullPathFilename;
        }


        /// <summary>
        /// zkousi dohledat atribut od html elemetu dohledaneho pres xPath
        /// </summary>
        /// <param name="htmlDoc">html dokument</param>
        /// <param name="xPathNode">xPath pro dohledani elementu</param>
        /// <param name="attributeName">jmeno hledaneho atributu</param>
        /// <returns>hodnota hledaneho atributu</returns>
        private static string FindAttributeValue(HtmlDocument htmlDoc, string xPathNode, string attributeName)
        {
            var xpathNodes = htmlDoc.DocumentNode.SelectNodes(xPathNode);
            if (xpathNodes != null)
            {
                var node = xpathNodes.FirstOrDefault();
                if (node != null)
                {
                    var att = node.Attributes.FirstOrDefault(p => p.Name == attributeName);
                    if (att != null)
                    {
                        return att.Value;
                    }
                }
            }
            return null;
        }
               
        #endregion
    }
}
