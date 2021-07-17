using Livet;
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

        private MainWindowModel mainWindow;

        public SettingsWindowViewModel(MainWindowModel viewModel)
        {
            mainWindow = viewModel;
        }

        // This method would be called from View, when ContentRendered event was raised.
        public void Initialize()
        {
            NewLines = new ObservableCollection<NewLineItem>();
            NewLines.Add(new NewLineItem("CRLF", "\r\n"));
            NewLines.Add(new NewLineItem("LF", "\n"));
            NewLines.Add(new NewLineItem("CR", "\r"));
            this.NotifyPropertyChanged(nameof(NewLines));

            beforeConfig = ConfigManager.getCopyConfig();

            var config = ConfigManager.config;
            WatchWord = config.WatchWord;
            SavePath = config.SavePath;
            OutFilePrefix = config.OutFilePrefix;
            OutFileSuffix = config.OutFileSuffix;
            NewLineCode = config.NewLine;
            CategoryPrefix = config.CategoryPrefix;
            CategorySuffix = config.CategorySuffix;
            ViaPrefix = config.ViaPrefix;
            ViaSuffix = config.ViaSuffix;

            this.Categories = new ObservableCollection<Category>();
            // カテゴリ設定
            foreach (var kvp in config.Categories)
            {
                this.Categories.Add(new Category() { Name = kvp.Value, Id = kvp.Key });
            }
            this.NotifyPropertyChanged(nameof(Categories));

            // 情報元設定
            this.InformationSources = new ObservableCollection<InformationSource>();
            foreach (var kvp in config.InformationSources)
            {
                this.InformationSources.Add(new InformationSource() { Name = kvp.Value, Data = kvp.Key });
            }
            this.NotifyPropertyChanged(nameof(InformationSources));
        }


        private ViewModelCommand _SaveCommand;

        public ViewModelCommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new ViewModelCommand(SaveDo);
                }
                return _SaveCommand;
            }
        }

        public void SaveDo()
        {
            // 詰替え
            var config = ConfigManager.config;
            config.WatchWord = WatchWord;
            config.SavePath = SavePath;
            config.OutFilePrefix = OutFilePrefix;
            config.OutFileSuffix = OutFileSuffix;
            config.NewLine = NewLineCode;
            config.CategoryPrefix = CategoryPrefix;
            config.CategorySuffix = CategorySuffix;
            config.ViaPrefix = ViaPrefix;
            config.ViaSuffix = ViaSuffix;
            // カテゴリ
            config.Categories.Clear();
            foreach (var category in this.Categories)
            {
                config.Categories.Add(new KeyValuePair<string, string>(category.Id, category.Name));
            }
            // 情報元
            config.InformationSources.Clear();
            foreach (var informationSource in this.InformationSources)
            {
                config.InformationSources.Add(new KeyValuePair<string, string>(informationSource.Data, informationSource.Name));
            }

            if (Equals(ConfigManager.config, beforeConfig))
            {
                // 処理なし
            }
            else
            {
                // ここでファイルに保存する。
                ConfigManager.SaveConfig();
            }
            mainWindow.ConfigLoad();
            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }

    }
}
