using HtmlAgilityPack;
using RadioOwl.Data;
using RadioOwl.Id3;
using RadioOwl.Radio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using vt.Extensions;
using vt.Extensions.HtmlAgilityPack;
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
        public bool CanSniffAround { get { return ((SelectedRow != null) && !string.IsNullOrEmpty(SelectedRow.Id)); } }

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
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
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
            // TODO zrusit?

            //if (CanSniffAround)
            //{
            //    var urlStreams = SniffAroundViewModel.ExecuteModal(SelectedRow?.Id);
            //    urlStreams?.ForEach(p => ProcessUrl(p));
            //}
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
        /// zahaji zpracovani url - volani ze SniffView se seznamem vybranych ID
        /// </summary>
        private void ProcessUrl(StreamUrlRow streamUrlRow)
        {
            //var fileRow = new FileRow(Files, streamUrlRow);
            //ProcessUrlRow(fileRow, fileRow.Url);
        }


        /// <summary>
        /// zahaji zpracovani url
        /// </summary>
        private void ProcessUrl(string url)
        {
            var fileRow = new FileRow(Files, url);
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
                StartDownloadFromDownloadUrl(fileRow, url);
            }
            else if (RadioHelpers.IsUrlToIRadioPlayPage(url))
            {
                StartDownloadFromStreamUrl(fileRow, url);
            }
            else if (RadioHelpers.IsUrlToVltavaPlay(url))
            {
                StartDownloadFromVltavaPlayUrl(fileRow, url);
            }
            else if (RadioHelpers.IsUrlToPrehrat2018(url))
            {
                StartDownloadFromPrehrat2018Url(fileRow, url);
            }
            else
            {
                fileRow.AddLog(string.Format("Neznámé url: {0}.", url));
            }
        }


        private async void StartDownloadFromPrehrat2018Url(FileRow fileRow, string url)
        {
            var asyncDownloader = new AsyncDownloader();
            var downloader = await asyncDownloader.GetString(url);
            if (downloader.DownloadOk)
            {
                var parserResult = new RadioHtmlParser().ParsePrehrat2018Html(downloader.Output);

                if(!parserResult.Log.Any())
                {
                    if (parserResult.Urls.Any())
                    {
                        // no errors - go to download - process all parsed urls (and use existing FileRow for first url)

                        // first file
                        fileRow.Url = parserResult.Urls[0];
                        DownloadMp3Stream(fileRow);

                        // jen jeden soubor nebo vice?
                        if (parserResult.Urls.Count() > 1)
                        {
                            fileRow.GroupOrder = 1;

                            // second and next files ... if exists
                            for (int i = 1; i < parserResult.Urls.Count; i++)
                            {
                                var fr = new FileRow(Files, parserResult.Urls[i])
                                {
                                    GroupOrder = i + 1
                                };
                                Files.Add(fr);
                                DownloadMp3Stream(fr);
                            }
                        }
                    }
                }
                else
                {
                    // chyby pri parsovani - jen prekopiruji
                    parserResult.Log.ForEach(p => fileRow.AddLog(p, FileRowState.Error));
                }
            }
            else
            {
                fileRow.AddLog(string.Format("Chyba při stahování stránku pořadu: {0}.", downloader.Exception?.Message));
            }
        }

        
        /// <summary>
        /// drag-dropnute url z javascript playeru z webu Vltavy (zatim) - je nutne stahnout html a dohledat link na mp3 stream
        /// </summary>
        private async void StartDownloadFromVltavaPlayUrl(FileRow fileRow, string url)
        {
            var asyncDownloader = new AsyncDownloader();
            var downloader = await asyncDownloader.GetString(url);
            if (downloader.DownloadOk)
            {
                ParseVltavaStyleHtmlPage(fileRow, downloader.Output);
                if (!string.IsNullOrEmpty(fileRow.Url))
                {
                    DownloadMp3Stream(fileRow);
                }
            }
            else
            {
                fileRow.AddLog(string.Format("Chyba při stahování stránku pořadu: {0}.", downloader.Exception?.Message));
            }
        }


        private void StartDownloadFromDownloadUrl(FileRow fileRow, string url)
        {
            fileRow.Url = url;
            fileRow.Id = RadioHelpers.GetStreamIdFromUrl(fileRow.Url);
            DownloadMp3Stream(fileRow);
        }


        private void StartDownloadFromStreamUrl(FileRow fileRow, string streamUrl)
        {
            fileRow.Id = RadioHelpers.GetStreamIdFromUrl(streamUrl);
            if (!string.IsNullOrEmpty(fileRow.Id))
            {
                fileRow.Url = RadioHelpers.GetIRadioMp3Url(fileRow.Id);
                DownloadMp3Stream(fileRow);
            }
            else
            {
                fileRow.AddLog(string.Format("Nepodařilo se zjistit ID streamu (url:{0}).", streamUrl), FileRowState.Error);
            }
        }


        /// <summary>
        /// rozparsovani html stranky poradu
        /// </summary>
        private void ParseVltavaStyleHtmlPage(FileRow fileRow, string html)
        {
            try
            {
                // html nemusi byt validni xml, takze je potreba pro parsovani pouzit Html Agility Pack, viz http://htmlagilitypack.codeplex.com/
                // http://www.c-sharpcorner.com/UploadFile/9b86d4/getting-started-with-html-agility-pack/
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var vltavaAnchorNode = VltavaPageFindStreamUrl(htmlDoc, @"//div[@class='sm2-playlist-wrapper']");
                if(vltavaAnchorNode != null)
                {
                    // povedlo se dohledat <a>, vykousnu href atribut a je to
                    fileRow.Url = vltavaAnchorNode.GetAttributeValueByName("HREF");
                    // ID v tomhle pripade nemam
                    fileRow.Id = "?";
                    // title jen vykousnu ze stranky
                    fileRow.Title = VltavaPageFindTitle(htmlDoc, @"//meta[@property='og:title']");
                }

                if (string.IsNullOrEmpty(fileRow.Url))
                {
                    fileRow.AddLog("Chyba při parsování stránky pořadu - nepodařilo se dohledat URL streamu.", FileRowState.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                fileRow.AddLog(string.Format("Chyba při stahování streamu: {0}.", ex.Message), FileRowState.Error);
            }
        }


        private async void DownloadMp3Stream(FileRow fileRow)
        {
            fileRow.AddLog(string.Format("Stahuji stream: {0}", fileRow.Url));

            GetId3Tags(fileRow);

            var asyncDownloader = new AsyncDownloader();
            var output = await asyncDownloader.GetData(fileRow.Url,
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


        /// <summary>
        /// dohledam Id3 tagy se stahovaneho streamu - jelikoz nedokazu nakouknout na rozstahovana data z AsyncDownloader.GetData(), musim jeste pred samotnym
        /// stazenim pouzit tuto fci. Stahuje se jen Range z url, tak to snad nebude takova zatez ac se jedna o duplicitni data :-/
        /// </summary>
        private async void GetId3Tags(FileRow fileRow)
        {
            var data = await new AsyncDownloader().GetDataRange(fileRow.Url, 0, 4096);
            if (data.DownloadOk)
            {
                var id3Tags = new Id3Tags(data.Output);
                // prepisovat jen je-li naplneno - napr ted u vltava strane ID3 tagy nejsou :-/
                if (!string.IsNullOrEmpty(id3Tags.Title))
                {
                    fileRow.Title = id3Tags.Title;
                }
                if (!string.IsNullOrEmpty(id3Tags.Album))
                {
                    fileRow.Album = id3Tags.Album;
                }
                fileRow.AddLog("Id3 tagy ok.");
            }
            else
            {
                fileRow.AddLog(string.Format("Nepodařilo se načíst Id3 tagy ze streamu: {0}.", fileRow.Url));
            }
        }


        private void SaveMp3(FileRow fileRow, byte[] data)
        {
            var fNameId3 = GetFileNameFromId3Tags(fileRow, data);
            var newFileNameOk = GetUniqSaveName(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), fNameId3, "mp3");

            using (var file = new FileStream(newFileNameOk, FileMode.Create, FileAccess.Write))
            {
                file.Write(data, 0, data.Length);
            }
            fileRow.SavedFileName = newFileNameOk;
            fileRow.AddLog(string.Format("Uložen soubor: {0}", newFileNameOk), FileRowState.Finnished);
        }


        /// <summary>
        /// sestrojeni filename z id3 tagu - pokud by v row jeste nebyly, doplnim je
        /// </summary>
        private static string GetFileNameFromId3Tags(FileRow fileRow, byte[] data)
        {
            if(string.IsNullOrEmpty(fileRow.Album) && string.IsNullOrEmpty(fileRow.Title))
            {
                var id3Tags = new Id3Tags(data);
                fileRow.Title = id3Tags.Title;
                fileRow.Album = id3Tags.Album;
            }
            return string.Format("{0}-{1}", fileRow.Album, fileRow.Title);
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
            } while (File.Exists(fullPathFilename));

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
        

        /// <summary>
        /// dohledani url k mp3 streamu na vltava like odkazu
        /// </summary>
        private static HtmlNode VltavaPageFindStreamUrl(HtmlDocument htmlDoc, string xPathNode)
        {
            var xpathNodes = htmlDoc.DocumentNode.SelectNodes(xPathNode);
            if (xpathNodes != null)
            {
                // html tagy dohledany pres xPath - staci mi ten prvni ;-) vic ji ted ani neni
                var htmlNode = xpathNodes.FirstOrDefault();
                if (htmlNode != null)
                {
                    // mel by mit par urovni pod sebou <a> kde mne zajima jeho href
                    var childNodeOfType = htmlNode.GetSubNodesOfName("A").FirstOrDefault();
                    return childNodeOfType;
                }
            }
            return null;
        }


        /// <summary>
        /// dohledani title k mp3 streamu na vltava like odkazu - trochu tim supluju ID3 tagy, ktere ted v stazenem souboru neni
        /// </summary>
        private static string VltavaPageFindTitle(HtmlDocument htmlDoc, string xPathNode)
        {
            var xpathNodes = htmlDoc.DocumentNode.SelectNodes(xPathNode);
            if (xpathNodes != null)
            {
                var htmlNode = xpathNodes.FirstOrDefault();
                if (htmlNode != null)
                {
                    var contentValue = htmlNode.GetAttributeValueByName("content");
                    return contentValue;
                }
            }
            return null;
        }

        #endregion
    }
}
