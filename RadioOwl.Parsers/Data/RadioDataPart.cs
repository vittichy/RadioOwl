using Dtc.Common.Extensions;
using RadioOwl.Parsers.Data.Factory;
using RadioOwl.Parsers.Data.Helper;
using RadioOwl.Parsers.Data.PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Windows.Media;

namespace RadioOwl.Parsers.Data
{
    /// <summary>
    /// Jedna část pořadu 
    /// </summary>
    public class RadioDataPart : PropertyChangedBase
    {
        /// <summary>
        /// Hlavička pořadu
        /// </summary>
        public readonly RadioData RadioData;

        private int? _partNo;
        /// <summary>
        /// Číslo pořadu
        /// </summary>
        public int? PartNo
        {
            get { return _partNo; }
            set
            {
                _partNo = value;
                OnPropertyChanged();
            }
        }

        private string _title;
        /// <summary>
        /// Titulek pořadu
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        /// <summary>
        /// Popis pořadu
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
                DescriptionSingleLine = Description?.RemoveWhitespace();
            }
        }

        private string _descriptionSingleLine;
        /// <summary>
        /// Popis pořadu jako jedna řádka (pro datagrid)
        /// </summary>
        public string DescriptionSingleLine
        {
            get { return _descriptionSingleLine; }
            private set
            {
                _descriptionSingleLine = value;
                OnPropertyChanged();
            }
        }

        private string _urlMp3;
        /// <summary>
        /// Url 
        /// </summary>
        public string UrlMp3
        {
            get { return _urlMp3; }
            set
            {
                _urlMp3 = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Existuje <see cref="UrlMp3"/>
        /// </summary>
        public bool UrlMp3Exists { get { return !string.IsNullOrEmpty(UrlMp3); } }

        private string _fileName;
        /// <summary>
        /// Filename pořadu pro uložení na disk
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        private bool _saved;
        /// <summary>
        /// Uloženo?
        /// </summary>
        public bool Saved
        {
            get { return _saved; }
            set
            {
                _saved = value;
                OnPropertyChanged();
            }
        }

        private int _progressPercentage;
        /// <summary>
        /// Progres stahování v %
        /// </summary>
        public int ProgressPercentage
        {
            get { return _progressPercentage; }
            set
            {
                _progressPercentage = value;
                OnPropertyChanged();
                ProgressPercentageAsText = RadioDataHelper.ProgressText(ProgressPercentage, BytesReceived);
                // reportovat změnu do master záznamu RadioData
                RadioData.PartChanged();
            }
        }

        private string _progressPercentageAsText;
        /// <summary>
        /// Progres stahování v % jako text pro datagrid
        /// </summary>
        public string ProgressPercentageAsText
        {
            get { return _progressPercentageAsText; }
            private set
            {
                _progressPercentageAsText = value;
                OnPropertyChanged();
            }
        }

        private long _bytesReceived;
        /// <summary>
        /// Stažená část souboru
        /// </summary>
        public long BytesReceived
        {
            get { return _bytesReceived; }
            set
            {
                _bytesReceived = value;
                OnPropertyChanged();
                // reportovat změnu do master záznamu RadioData
                RadioData.PartChanged();
            }
        }

        private long _totalBytesToReceive;
        /// <summary>
        /// Celková velikost souboru ke stažení
        /// </summary>
        public long TotalBytesToReceive
        {
            get { return _totalBytesToReceive; }
            set
            {
                _totalBytesToReceive = value;
                OnPropertyChanged();
                RadioData.PartChanged(); // reportovat změnu do master záznamu RadioData
            }
        }

        private RadioDataPartState _state;
        /// <summary>
        /// Stav záznamu viz <see cref="RadioDataPartState"/>
        /// </summary>
        public RadioDataPartState State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
                StateColor = new RadioDataPartStateHelper().ToBrush(State);
                RadioData.PartChanged(); // reportovat změnu do master záznamu RadioData
            }
        }

        private Brush _stateColor;
        /// <summary>
        /// Barva dle stavu řádku <see cref="State"/>
        /// </summary>
        public Brush StateColor
        {
            get { return _stateColor; }
            private set
            {
                _stateColor = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Kostruktor
        /// </summary>
        public RadioDataPart(RadioData radioData)
        {
            RadioData = radioData ?? throw new ArgumentNullException(nameof(radioData));
        }

        /// <summary>
        /// Přidání logu
        /// </summary>
        public void AddLog(string msg, EventLevel eventLevel = EventLevel.Informational)
        {
            var radioLog = new RadioLogFactory().Create(msg, PartNo, eventLevel);
            //LogSet.Add(radioLog);
            //LogSetSelectedIndex = LogSet.Count() - 1;
            RadioData.LogSet.Add(radioLog); 
        }

        public void AddLogError(string msg)
        {
            AddLog(msg, EventLevel.Error);
        }
        public void AddLogWarning(string msg)
        {
            AddLog(msg, EventLevel.Warning);
        }
    }
}
