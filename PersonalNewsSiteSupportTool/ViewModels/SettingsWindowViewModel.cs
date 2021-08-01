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
using System.IO;
using System.Linq;
using System.Text;

namespace PersonalNewsSiteSupportTool.ViewModels
{
    class SettingsWindowViewModel : ViewModelBase
    {
        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).
        private Config beforeConfig;

        private string watchWord;

        public string WatchWord
        {
            get => watchWord;
            set {
                if (string.IsNullOrEmpty(value))
                {
                    ShowErrorMessage("監視する単語は入力必須です。");
                }
                RaisePropertyChangedIfSet(ref watchWord, value);
            }
        }


        private bool isRemoveWatchWord;

        public bool IsRemoveWatchWord
        {
            get => isRemoveWatchWord;
            set => RaisePropertyChangedIfSet(ref isRemoveWatchWord, value);
        }

        private string savePath;

        public string SavePath
        {
            get => savePath;
            set => RaisePropertyChangedIfSet(ref savePath, value);
        }


        private string outFilePrefix;

        public string OutFilePrefix
        {
            get => outFilePrefix;
            set
            {
                if(!CommonUtil.ValidateFileName(value))
                {
                    ShowErrorMessage($"出力ファイル名先頭にファイルに含められない文字列({CommonUtil.CANNOT_USED_FILE_NAME})が含まれています。");
                }
                RaisePropertyChangedIfSet(ref outFilePrefix, value);
            }
        }


        private string outFileSuffix;

        public string OutFileSuffix
        {
            get => outFileSuffix;
            set
            {
                if (!CommonUtil.ValidateFileName(value))
                {
                    ShowErrorMessage($"出力ファイル名末尾にファイルに含められない文字列({CommonUtil.CANNOT_USED_FILE_NAME})が含まれています。");
                }
                RaisePropertyChangedIfSet(ref outFileSuffix, value);
            }
        }


        private string mergeFileName;

        public string MergeFileName
        {
            get => mergeFileName;
            set
            {
                if (!CommonUtil.ValidateFileName(value))
                {
                    ShowErrorMessage($"結合ファイル名にファイルに含められない文字列({CommonUtil.CANNOT_USED_FILE_NAME})が含まれています。");
                }
                RaisePropertyChangedIfSet(ref mergeFileName, value);
            }
        }

        private NewLineItem newLine;

        public NewLineItem NewLine
        {
            get => newLine;
            set => RaisePropertyChangedIfSet(ref newLine, value);
        }


        private string newLineCode;

        public string NewLineCode
        {
            get => newLineCode;
            set => RaisePropertyChangedIfSet(ref newLineCode, value);
        }


        private string categoryPrefix;

        public string CategoryPrefix
        {
            get => categoryPrefix;
            set => RaisePropertyChangedIfSet(ref categoryPrefix, value);
        }


        private string categorySuffix;

        public string CategorySuffix
        {
            get => categorySuffix;
            set => RaisePropertyChangedIfSet(ref categorySuffix, value);
        }


        private string viaPrefix;

        public string ViaPrefix
        {
            get => viaPrefix;
            set => RaisePropertyChangedIfSet(ref viaPrefix, value);
        }


        private string viaSuffix;

        public string ViaSuffix
        {
            get => viaSuffix;
            set => RaisePropertyChangedIfSet(ref viaSuffix, value);
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

        private readonly MainWindowModel mainWindow;

        private SettingsWindowViewModel(MainWindowModel viewModel)
        {
            mainWindow = viewModel;
        }

        private static SettingsWindowViewModel instance = null;

        public static SettingsWindowViewModel GetInstance(MainWindowModel viewModel)
        {
            if(instance == null)
            {
                instance = new SettingsWindowViewModel(viewModel);
            }
            return instance;
        }

        // This method would be called from View, when ContentRendered event was raised.
        public void Initialize()
        {
            NewLines = new ObservableCollection<NewLineItem>
            {
                new NewLineItem("CRLF", "\r\n"),
                new NewLineItem("LF", "\n"),
                new NewLineItem("CR", "\r")
            };
            this.NotifyPropertyChanged(nameof(NewLines));

            beforeConfig = ConfigManager.GetCopyConfig();

            var config = ConfigManager.Config;
            WatchWord = config.WatchWord;
            IsRemoveWatchWord = config.IsRemoveWatchWord;
            SavePath = config.SavePath;
            OutFilePrefix = config.OutFilePrefix;
            OutFileSuffix = config.OutFileSuffix;
            MergeFileName = config.MergeFileName;
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

        private ViewModelCommand saveCommand;

        public ViewModelCommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new ViewModelCommand(SaveDo);
                }
                return saveCommand;
            }
        }

        public void SaveDo()
        {
            if (!Validate())
            {
                return;
            }
            UpdateConfig(ConfigManager.Config);

            if (Equals(ConfigManager.Config, beforeConfig))
            {
                // 処理なし
            }
            else
            {
                // ここでファイルに保存する。
                ConfigManager.SaveConfig();
                mainWindow.ConfigLoad();
            }
            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }


        private ViewModelCommand cancelCommand;

        public ViewModelCommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                {
                    cancelCommand = new ViewModelCommand(Cancel);
                }
                return cancelCommand;
            }
        }

        public void Cancel()
        {
            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }

        private ViewModelCommand previewCommand;

        public ViewModelCommand PreviewCommand
        {
            get
            {
                if (previewCommand == null)
                {
                    previewCommand = new ViewModelCommand(Preview);
                }
                return previewCommand;
            }
        }

        public void Preview()
        {
            var config = ConfigManager.GetCopyConfig();
            UpdateConfig(config);
            Messenger.Raise(new TransitionMessage(new OutTextPreviewViewModel(config), "Preview"));
        }

        private void UpdateConfig(Config config)
        {
            config.WatchWord = WatchWord;
            config.IsRemoveWatchWord = IsRemoveWatchWord;
            config.SavePath = SavePath;
            config.OutFilePrefix = OutFilePrefix;
            config.OutFileSuffix = OutFileSuffix;
            config.MergeFileName = MergeFileName;
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
        }

        private bool Validate()
        {
            // 監視する単語
            if (string.IsNullOrEmpty(WatchWord))
            {
                ShowErrorMessage("監視する単語は入力必須です。");
                return false;
            }
            // 出力ファイル名
            if (!CommonUtil.ValidateFileName(OutFilePrefix))
            {
                ShowErrorMessage($"出力ファイル名先頭にファイルに含められない文字列({CommonUtil.CANNOT_USED_FILE_NAME})が含まれています。");
                return false;
            }
            if (!CommonUtil.ValidateFileName(OutFileSuffix))
            {
                ShowErrorMessage($"出力ファイル名末尾にファイルに含められない文字列({CommonUtil.CANNOT_USED_FILE_NAME})が含まれています。");
                return false;
            }
            // 保存先
            if (!Directory.Exists(savePath))
            {
                ShowErrorMessage("保存先フォルダが存在しません。");
                return false;
            }
            // カテゴリ
            var hashset = new HashSet<string>();
            foreach (var category in this.Categories)
            {
                // ID重複チェック
                if (!hashset.Add(category.Id))
                {
                    ShowErrorMessage($"カテゴリのID、{category.Id}が重複しています。");
                    return false;
                }
            }
            return true;
        }
    }
}
