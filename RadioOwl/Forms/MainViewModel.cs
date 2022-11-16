using Caliburn.Micro;
using Dtc.Http;
using Dtc.Http.Http;
using RadioOwl.Helpers;
using RadioOwl.Parsers;
using RadioOwl.Parsers.Data;
using RadioOwl.Parsers.Data.Factory;
using RadioOwl.Parsers.Parser.Interfaces;
using RadioOwl.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RadioOwl.Forms
{
    /// <summary>
    /// ViewModel hlavniho okna aplikace
    /// </summary>
    public class MainViewModel : PropertyChangedBase
    {
        /// <summary>
        /// Seznam dostupných parserů
        /// </summary>
        private readonly ParserCollection _parsers = new ParserCollection();

        private string _viewTitle;
        /// <summary>
        /// Titulek okna
        /// </summary>
        public string ViewTitle
        {
            get { return _viewTitle; }
            set
            {
                _viewTitle = value;
                NotifyOfPropertyChange();
            }
        } 

        /// <summary>
        /// Hlavní set s pořady (master)
        /// </summary>
        public ObservableCollection<RadioData> RadioDataSet { get; set; } = new ObservableCollection<RadioData>();

        private RadioData _radioDataSelected;
        /// <summary>
        /// Selected pořad (v master datagridu)
        /// </summary>
        public RadioData RadioDataSelected
        {
            get { return _radioDataSelected; }
            set
            {
                _radioDataSelected = value;
                NotifyOfPropertyChange();
// TODO RefreshLogSet(value);
            }
        }


        /// <summary>
        /// Konstruktor
        /// </summary>
        public MainViewModel()
        {
            ViewTitle = $"RadioOwl {Assembly.GetExecutingAssembly().GetName()?.Version?.Major}";
        }





        /// <summary>
        /// Drag-drop preview event
        /// </summary>
        public void EventPreviewDragOver(DragEventArgs e)
        {
            var url = e.GetUnicodeData();
            var parserExists = _parsers.ExistsParser(url);
            e.Effects = parserExists ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>
        /// Drag-drop event
        /// </summary>
        public void EventDrop(DragEventArgs e)
        {
            // nefunguje-li drag-drop nezoufej a pust visual studio jako uzivatel a ne pod adminem!
            var url = e.GetUnicodeData();
            ProcessUrl(url);
        }

        /// <summary>
        /// Event obdržení focusu na master datagridu --------- TODO nevypada, ze by event prichazel?
        /// </summary>
        public void DataGridMainGotFocus()
        {
            //            RefreshLogSet(RadioDataSelected);
        }


        public void DataGridMainMouseDoubleClick()
        {
            //            RefreshLogSet(RadioDataSelected);
        }

        




        /// <summary>
        /// Zahaji zpracovani url
        /// </summary>
        private void ProcessUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            // založení nového stahování
            var radioData = new RadioDataFactory().Create(url);
            RadioDataSet.Insert(0, radioData);
            RadioDataSelected = radioData;
            DataGridMainGotFocus();

            ProcessRadioData(radioData);
        }

        /// <summary>
        /// Zahájení zpracování RadioData řádku
        /// </summary>
        private async void ProcessRadioData(RadioData radioData)
        {
            if (radioData == null)
                return;

            // dohledání vhodného parseru stránky
            var parserSet = _parsers.FindParser(radioData.Url);
            if (!parserSet.Any())
            {
                radioData.AddLogError($"Nepodařilo se dohledat parser pro url: {radioData.Url}.");
                return;
            }

            // zkusím použít parser a rozpasovat
            var parseOk = await TryParser(radioData, parserSet);
            if (parseOk)
            {
                radioData.AddLog("Parser ok.");
                await DownloadRadioDataAsync(radioData);
            }
            else
            {
                radioData.AddLogError("Parser error.");
            }
        }

        private async Task<bool> TryParser(RadioData radioData, List<IPageParser> parserSet)
        {
            foreach (var parser in parserSet)
            {
                var parseOk = await parser.ParseAsync(radioData);
                // beru prvni parser kteremu se povedlo parsovani
                if (parseOk)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Zpracovani rozparsovanych dat - tj zde bych jiz mel znat odkazy na finalni mp3 a ty stahnu, ulozim, porezim filename, mp3id tagy atd
        /// </summary>
        private async Task DownloadRadioDataAsync(RadioData radioData)
        {
            radioData.PartSet
                            .Where(p => p.UrlMp3Exists)
                                .ToList()
                                    .ForEach(async p => await ProcessDataPartAsync(p));
            await SaveImageAsync(radioData);
            SaveReadMe(radioData);
        }

        /// <summary>
        /// Zpracování jedné části pořadu - zde už musím mít rozparsovanou url
        /// </summary>
        private async Task ProcessDataPartAsync(RadioDataPart radioDataPart)
        {
            radioDataPart.State = RadioDataPartState.Started;
            radioDataPart.FileName = new FileHelper().GenerateMp3FileName(radioDataPart);

            // soubor jeste neexistuje?
            if (!File.Exists(radioDataPart.FileName))
            {
                radioDataPart.State = RadioDataPartState.Started;
                await DownloadPartAsync(radioDataPart);
            }
            else
            {
                radioDataPart.State = RadioDataPartState.FileAlreadyExists;
                radioDataPart.AddLogWarning($"Soubor již existuje: {radioDataPart.FileName}.");
            }
        }

        private async Task DownloadPartAsync(RadioDataPart radioDataPart)
        {
            radioDataPart.AddLog($"Zahájení stahování: '{radioDataPart.UrlMp3}'");

            var asyncDownloader = new AsyncDownloader();
            var output = await asyncDownloader.GetData(radioDataPart.UrlMp3,
                                                       p =>
                                                       {
                                                           radioDataPart.ProgressPercentage = p.ProgressPercentage;
                                                           radioDataPart.BytesReceived = p.BytesReceived;
                                                           radioDataPart.TotalBytesToReceive = p.TotalBytesToReceive;
                                                       });
            if (output.DownloadOk)
            {
                SaveDataPart(radioDataPart, output.Output);
                SavePartReadMe(radioDataPart);
                radioDataPart.State = RadioDataPartState.Finnished;
            }
            else
            {
                radioDataPart.AddLogError(string.Format("Chyba při stahování streamu: {0}.", output.Exception?.Message));
                radioDataPart.State = RadioDataPartState.Error;
            }
        }

        /// <summary>
        /// Uloží txt soubor ke konkrétnímu dílu <see cref="RadioDataPart"/>
        /// </summary>
        private void SavePartReadMe(RadioDataPart radioDataPart)
        {
            var readmeFilename = new FileHelper().GenerateReadmeFilename(radioDataPart);

            var readme = new StringBuilder();
            readme.AppendLine(radioDataPart?.RadioData.SiteEntityLabel);
            readme.AppendLine($"{radioDataPart.RadioData?.ContentSerialAllParts}/{radioDataPart.RadioData?.PartSet?.Count()} {radioDataPart.Title}");
            readme.AppendLine(radioDataPart.Description);
            File.WriteAllText(readmeFilename, readme.ToString());
        }

        private void SaveReadMe(RadioData radioData)
        {
            var readmeFilename = new FileHelper().GenerateReadmeFilename(radioData);
            new FileHelper().EnsureDirectoryCreated(readmeFilename);

            var readme = new StringBuilder();
            readme.AppendLine(radioData.SiteEntityLabel);
            readme.AppendLine(radioData.DetailDescription);
            File.WriteAllText(readmeFilename, readme.ToString());
        }

        private async Task SaveImageAsync(RadioData radioData)
        {
            if (!string.IsNullOrEmpty(radioData.ImageUrl)) 
            {
                var imageFilename = new FileHelper().GenerateImageFilename(radioData);
                if (!File.Exists(imageFilename))
                {
                    await DownloadImageAsync(radioData.ImageUrl, imageFilename);
                } 
            }
        }

        /// <summary>
        /// Stažení image k pořadu
        /// </summary>
        private async Task DownloadImageAsync(string imageUrl, string imageFilename)
        {
            if (!string.IsNullOrEmpty(imageUrl) && !File.Exists(imageFilename))
            {
                var asyncDownloader = new AsyncDownloader();
                var output = await asyncDownloader.GetData(imageUrl);
                if (output.DownloadOk)
                {
                    new FileHelper().EnsureDirectoryCreated(imageFilename);
                    using (var file = new FileStream(imageFilename, FileMode.Create, FileAccess.Write))
                    {
                        file.Write(output.Output, 0, output.Output.Length);
                    }
                }
            }
        }

        /// <summary>
        /// Uložení <see cref="RadioDataPart"/>
        /// </summary>
        /// <param name="radioDataPart">RadioDaraPart</param>
        /// <param name="data">Mp3 data k ulozeni</param>
        private void SaveDataPart(RadioDataPart radioDataPart, byte[] data)
        {
            new FileHelper().EnsureDirectoryCreated(radioDataPart.FileName);

            using (var file = new FileStream(radioDataPart.FileName, FileMode.Create, FileAccess.Write))
            {
                file.Write(data, 0, data.Length);
            }
            radioDataPart.Saved = true;
            radioDataPart.AddLog($"Uložen soubor: {radioDataPart.FileName}");
        }

        /// <summary>
        /// akce Načíst z URL
        /// </summary>
        public void OpenUrl()
        {
            // TODO zatim jen test stazeni - nahrada dropu
            // NEJDE ??? var url = @"https://www.mujrozhlas.cz/cetba-na-pokracovani/woody-allen-mimochodem-szirave-sebeironicka-vtipna-autobiografie";

            //var url = @"https://www.mujrozhlas.cz/cetba-na-pokracovani/marcel-pagnol-jak-voni-tymian";
            //ProcessUrl(url);

            var url = InputBoxViewModel.ExecuteModal("Načíst z URL", "URL:");
            ProcessUrl(url);

        }

        /// <summary>
        /// Update squirrel app
        /// </summary>
        public void UpdateApp()
        {
            //using Squirrel.UpdateManager upManager = new Squirrel.UpdateManager(@"d:\vt\dev\!squirrel_out")
            //{

            //};
        }
        
        /// <summary>
        /// Informace o aplikaci
        /// </summary>
        public void AppInfo()
        {
            var appInfo = $"App version:?{Environment.NewLine}";
            var parsersInfo = _parsers.Parsers.Select(p => $"{p.GetType().Name} (v:{p.Version} url:{string.Join(",", p.ParseUrls)})");
            var result = $"{appInfo}{Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, parsersInfo)}";
            MessageBox.Show(result, "App info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
