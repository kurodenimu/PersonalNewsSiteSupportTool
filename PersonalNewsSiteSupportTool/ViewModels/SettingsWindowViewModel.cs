﻿using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using PersonalNewsSiteSupportTool.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PersonalNewsSiteSupportTool.ViewModels
{
    class SettingsWindowViewModel : ViewModelBase
    {
        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).
        private Config beforeConfig;

        private string _WatchWord;

        public string WatchWord
        {
            get => _WatchWord;
            set => RaisePropertyChangedIfSet(ref _WatchWord, value);
        }

        private string _SavePath;

        public string SavePath
        {
            get => _SavePath;
            set => RaisePropertyChangedIfSet(ref _SavePath, value);
        }


        private string _OutFilePrefix;

        public string OutFilePrefix
        {
            get => _OutFilePrefix;
            set => RaisePropertyChangedIfSet(ref _OutFilePrefix, value);
        }


        private string _OutFileSuffix;

        public string OutFileSuffix
        {
            get => _OutFileSuffix;
            set => RaisePropertyChangedIfSet(ref _OutFileSuffix, value);
        }


        private NewLineItem _NewLine;

        public NewLineItem NewLine
        {
            get => _NewLine;
            set => RaisePropertyChangedIfSet(ref _NewLine, value);
        }


        private string _NewLineCode;

        public string NewLineCode
        {
            get => _NewLineCode;
            set => RaisePropertyChangedIfSet(ref _NewLineCode, value);
        }


        private string _CategoryPrefix;

        public string CategoryPrefix
        {
            get => _CategoryPrefix;
            set => RaisePropertyChangedIfSet(ref _CategoryPrefix, value);
        }


        private string _CategorySuffix;

        public string CategorySuffix
        {
            get => _CategorySuffix;
            set => RaisePropertyChangedIfSet(ref _CategorySuffix, value);
        }


        private string _ViaPrefix;

        public string ViaPrefix
        {
            get => _ViaPrefix;
            set => RaisePropertyChangedIfSet(ref _ViaPrefix, value);
        }


        private string _ViaSuffix;

        public string ViaSuffix
        {
            get => _ViaSuffix;
            set => RaisePropertyChangedIfSet(ref _ViaSuffix, value);
        }

        public ObservableCollection<Category> Categories { get; set; }

        public ObservableCollection<InformationSource> InformationSources { get; set; }

        public class NewLineItem
        {
            public string Name { get; set; }

            public string Code { get; set; }

            public NewLineItem(string name, string code)
            {
                this.Name = name;
                this.Code = code;
            }
        }

        public ObservableCollection<NewLineItem> NewLines { get; set; }

        // This method would be called from View, when ContentRendered event was raised.
        public void Initialize()
        {
            NewLines = new ObservableCollection<NewLineItem>();
            NewLines.Add(new NewLineItem("CRLF", "\r\n"));
            NewLines.Add(new NewLineItem("LF", "\n"));
            NewLines.Add(new NewLineItem("CR", "\r"));
            this.NotifyPropertyChanged(nameof(NewLines));

            beforeConfig = ConfigManager.getCopyConfig();
            WatchWord = beforeConfig.WatchWord;
            SavePath = beforeConfig.SavePath;
            OutFilePrefix = beforeConfig.OutFilePrefix;
            OutFileSuffix = beforeConfig.OutFileSuffix;
            NewLineCode = beforeConfig.NewLine;
            CategoryPrefix = beforeConfig.CategoryPrefix;
            CategorySuffix = beforeConfig.CategorySuffix;
            ViaPrefix = beforeConfig.ViaPrefix;
            ViaSuffix = beforeConfig.ViaSuffix;

            this.Categories = new ObservableCollection<Category>();
            // カテゴリ設定
            foreach (var kvp in beforeConfig.Categories)
            {
                this.Categories.Add(new Category() { Name = kvp.Value, Id = kvp.Key });
            }
            this.NotifyPropertyChanged(nameof(Categories));

            // 情報元設定
            this.InformationSources = new ObservableCollection<InformationSource>();
            foreach (var kvp in beforeConfig.InformationSources)
            {
                this.InformationSources.Add(new InformationSource() { Name = kvp.Value, Data = kvp.Key });
            }
            this.NotifyPropertyChanged(nameof(InformationSources));
        }
    }
}