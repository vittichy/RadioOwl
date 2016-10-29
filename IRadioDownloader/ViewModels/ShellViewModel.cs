using HtmlAgilityPack;
using RadioOwl.Data;
using RadioOwl.Data.TagLibExtension;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using TagLib;
using vt.Extensions;
using vt.Http;

namespace RadioOwl.ViewModels
{
    class ShellViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Consts
        
        // typy odkazu na porad z hlavni stranky: http://hledani.rozhlas.cz/iradio/
        //
        // Prehrat:  http://prehravac.rozhlas.cz/audio/3680166                      - je u vsech poradu i tam, kde neni nabizen download, musim tedy s odkazu stahnout html playeru a z nej ziskat odkaz?
        // Stahnout: http://media.rozhlas.cz/_download/3680166.mp3                  - normalni download
        // Podcast:  http://www2.rozhlas.cz/podcast/podcast_porady.php?p_po=293     - nezajima mne

        private const string URL_BEGINNING_PLAY     = @"http://prehravac.rozhlas.cz/audio/";    // odkaz na stream - skutecnou adresu k mp3 musi vykousat z html stranky
        private const string URL_BEGINNING_DOWNLOAD = @"http://media.rozhlas.cz/_download/";    // primo odkaz na mp3

        private const string DRAG_DROP_FORMAT = "UnicodeText";

        private const string URL_IRADIO_STREAM = @"http://media.rozhlas.cz/_audio/{0}.mp3";     // url iradio streamu kdyz uz vim ID poradu

        private const int FILENAME_MAX_LEN = 150;

        #endregion


        #region Properties

        /// <summary>
        /// titulek okna
        /// </summary>
        public string Title { get { return string.Format("RadioOwl {0}", System.Reflection.Assembly.GetExecutingAssembly()?.GetName()?.Version); } }

        public ObservableCollection<FileRow> Files { get; set; }
        public FileRow SelectedRow { get; set; }

        public TotalProgress TotalProgress { get; set; }

        #endregion


        #region Constructors

        public ShellViewModel()
        {
            Files = new ObservableCollection<FileRow>();
            TotalProgress = new TotalProgress();
        }

        #endregion


        #region Methods

        public void PlayRow()
        {
            if ((SelectedRow != null) && (SelectedRow.State == FileRowState.Finnished) && !string.IsNullOrEmpty(SelectedRow.SavedFileName))
            {
                System.Diagnostics.Process.Start(SelectedRow.SavedFileName);
            }
        }


        public void DeleteRow()
        {
            if ((SelectedRow != null) && (SelectedRow.State != FileRowState.Started)) // nastartovany task by bylo nutne cancelovat, to zatim neresim
            {
                var messageBoxResult = MessageBox.Show("Opravdu smazat?", "Potvrzení", MessageBoxButton.YesNo);
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
                ProcessDropedUrl(url);
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

        #endregion


        #region Methods-Private

        private string GetPageUrl(DragEventArgs e)
        {
            if ((e != null) && (e.Data != null))
            {
                if (e.Data.GetDataPresent(DRAG_DROP_FORMAT))
                {
                    var url = (e.Data.GetData(DRAG_DROP_FORMAT) as string);
                    if (IsUrlToMp3Stream(url) || IsUrlToPage(url))
                    {
                        return url;
                    }
                }
            }
            return null;
        }


        private bool IsUrlToMp3Stream(string url)
        {
            return (!string.IsNullOrEmpty(url) && url.StartsWith(URL_BEGINNING_DOWNLOAD, StringComparison.InvariantCultureIgnoreCase));
        }


        private bool IsUrlToPage(string url)
        {
            return (!string.IsNullOrEmpty(url) && url.StartsWith(URL_BEGINNING_PLAY, StringComparison.InvariantCultureIgnoreCase));
        }


        private void ProcessDropedUrl(string url)
        {
            var fileRow = new FileRow(url);
            Files.Insert(0, fileRow);
            fileRow.AddLog("Stahuji stránku pořadu.", FileRowState.Started);

            StartDownload(fileRow, url);
        }


        private void StartDownload(FileRow fileRow, string url)
        {
            if (IsUrlToMp3Stream(url))
            {
                DownloadMp3Stream(fileRow, url);
            }
            else if (IsUrlToPage(url))
            {
                StartDownloadFromPageUrl(fileRow, url);
            }
            else
            {
                fileRow.AddLog(string.Format("Neznáme url: {0}.", url));
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

                var streamUrl = string.Format(URL_IRADIO_STREAM, fileRow.Id);
                fileRow.AddLog(string.Format("Stahuji stream: {0}", streamUrl));

                DownloadMp3Stream(fileRow, streamUrl);
            }
            catch (Exception ex)
            {
                fileRow.AddLog(string.Format("Chyba při stahování streamu: {0}.", ex.Message), FileRowState.Error);
            }
        }


        private async void DownloadMp3Stream(FileRow fileRow, string streamUrl)
        {
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
            var id3CommentTag = GetId3TagComment(data);
            var newFileNameOk = GetUniqSaveName(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), id3CommentTag, "mp3");

            using (var file = new FileStream(newFileNameOk, FileMode.Create, FileAccess.Write))
            {
                file.Write(data, 0, data.Length);
            }
            fileRow.SavedFileName = newFileNameOk;
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

        
        private static string FindAttributeValue(HtmlDocument htmlDoc, string xPathNode, string attributeName)
        {
            var node = htmlDoc.DocumentNode.SelectNodes(xPathNode).FirstOrDefault();
            if (node != null)
            {
                var att = node.Attributes.FirstOrDefault(p => p.Name == attributeName);
                if (att != null)
                {
                    return att.Value;
                }
            }
            return null;
        }


        /// <summary>
        /// pokud stahuju jen stream bez html stranky poradu, nezmam z html vykousle popisy poradu, takze bud vzdy koukat do mp3, kde to je pekne popsane
        /// zatim se zda nejvhodnejsi tag Comment
        /// ostatni tagy jsou cca takto:
        ///     Album: "Hovory (2016-10-28)"
        ///     Comment: "Hovory - 28.10.2016 02:40 - Narodni muzeum je fenomen ceske historie. Strileli po nem sovetsti vojaci v roce 1968. Dnes se do nej dobyvaji turiste, i kdyz je budova v rekonstrukci. Jake to je v takove budove delat generalniho reditele? Hostem Miroslava Dittricha byl Michal Lukes."
        ///     Copyright: "2016 Cesky rozhlas"
        ///     FirstArtist: "Hovory (28.10.2016) - Miroslav Dittrich - CRo Plus"
        ///     FirstComposer: "Miroslav Dittrich"
        ///     FirstPerformer: "Hovory (28.10.2016) - Miroslav Dittrich - CRo Plus"
        ///     JoinedArtists: "Hovory (28.10.2016) - Miroslav Dittrich - CRo Plus"
        ///     JoinedComposers: "Miroslav Dittrich"
        ///     JoinedPerformers: "Hovory (28.10.2016) - Miroslav Dittrich - CRo Plus"
        ///     Title: "Narodni muzeum je fenomen ceske historie. Strileli po nem sovetsti vojaci v roce 1968. Dnes se do nej dobyvaji turiste, i kdyz je budova v rekonstrukci. Jake to je v takove budove delat generalniho reditele? Hostem Miroslava Dittricha byl Michal Lukes."
        /// </summary>
        private string GetId3TagComment(byte[] data)
        {
            if (data?.Length > 0)
            {
                using (var memoStream = new MemoryStream(data))
                {
                    var simpleFileAbstraction = new SimpleFileAbstraction(memoStream);
                    var tagLibFile = TagLib.File.Create(simpleFileAbstraction, "taglib/mp3", ReadStyle.None);
                    if (tagLibFile?.Tag != null)
                    {
                        return tagLibFile.Tag.Comment ?? tagLibFile.Tag.Title;
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
