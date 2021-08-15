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
    /// <summary>
    /// 設定ウィンドウのViewModel
    /// </summary>
    class SettingsWindowViewModel : ViewModelBase
    {
        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).
        /// <summary>
        /// 変更前の設定
        /// </summary>
        private Config beforeConfig;

        /// <summary>
        /// 初期化フラグ
        /// 設定ウィンドウが初期化モードで開かれたかどうか。
        /// 初回設定時に入力必須チェックを迂回するのに使用。
        /// ウィンドウがロードされたらfalseにする。
        /// </summary>
        private bool isInit;

        /// <summary>
        /// ウィンドウをクローズする際にチェックを行うかどうか。
        /// 初回設定時に未保存でウィンドウを閉じるのを防ぐ。
        /// </summary>
        private bool isCloseCheck;

        /// <summary>
        /// 保存有無
        /// </summary>
        private bool isSaved;

        /// <summary>
        /// 監視文言
        /// </summary>
        private string watchWord;

        /// <summary>
        /// 監視文言
        /// </summary>
        public string WatchWord
        {
            get => watchWord;
            set {
                if (string.IsNullOrEmpty(value) && !isInit)
                {
                    ShowErrorMessage("監視する単語は入力必須です。");
                }
                RaisePropertyChangedIfSet(ref watchWord, value);
            }
        }

        /// <summary>
        /// 監視文言の除去有無
        /// </summary>
        private bool isRemoveWatchWord;

        /// <summary>
        /// 監視文言の除去有無
        /// </summary>
        public bool IsRemoveWatchWord
        {
            get => isRemoveWatchWord;
            set => RaisePropertyChangedIfSet(ref isRemoveWatchWord, value);
        }

        /// <summary>
        /// 保存先パス
        /// </summary>
        private string savePath;

        /// <summary>
        /// 保存先パス
        /// </summary>
        public string SavePath
        {
            get => savePath;
            set
            {
                if (string.IsNullOrEmpty(value) && !isInit)
                {
                    RaisePropertyChangedIfSet(ref savePath, value);
                }
            }
        }

        /// <summary>
        /// 出力ファイル接頭辞
        /// </summary>
        private string outFilePrefix;

        /// <summary>
        /// 出力ファイル接頭辞
        /// </summary>
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

        /// <summary>
        /// 出力ファイル接尾辞
        /// </summary>
        private string outFileSuffix;

        /// <summary>
        /// 出力ファイル接尾辞
        /// </summary>
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

        /// <summary>
        /// 結合ファイル名
        /// </summary>
        private string mergeFileName;

        /// <summary>
        /// 結合ファイル名
        /// </summary>
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

        /// <summary>
        /// 改行
        /// </summary>
        private NewLineItem newLine;

        /// <summary>
        /// 改行
        /// </summary>
        public NewLineItem NewLine
        {
            get => newLine;
            set => RaisePropertyChangedIfSet(ref newLine, value);
        }

        /// <summary>
        /// 改行コード
        /// </summary>
        private string newLineCode;

        /// <summary>
        /// 改行コード
        /// </summary>
        public string NewLineCode
        {
            get => newLineCode;
            set => RaisePropertyChangedIfSet(ref newLineCode, value);
        }

        /// <summary>
        /// カテゴリ接頭辞
        /// </summary>
        private string categoryPrefix;

        /// <summary>
        /// カテゴリ接頭辞
        /// </summary>
        public string CategoryPrefix
        {
            get => categoryPrefix;
            set => RaisePropertyChangedIfSet(ref categoryPrefix, value);
        }

        /// <summary>
        /// カテゴリ接尾辞
        /// </summary>
        private string categorySuffix;

        /// <summary>
        /// カテゴリ接尾辞
        /// </summary>
        public string CategorySuffix
        {
            get => categorySuffix;
            set => RaisePropertyChangedIfSet(ref categorySuffix, value);
        }

        /// <summary>
        /// 情報元接頭辞
        /// </summary>
        private string viaPrefix;

        /// <summary>
        /// 情報元接頭辞
        /// </summary>
        public string ViaPrefix
        {
            get => viaPrefix;
            set => RaisePropertyChangedIfSet(ref viaPrefix, value);
        }

        /// <summary>
        /// 情報接尾辞
        /// </summary>
        private string viaSuffix;

        /// <summary>
        /// 情報接尾辞
        /// </summary>
        public string ViaSuffix
        {
            get => viaSuffix;
            set => RaisePropertyChangedIfSet(ref viaSuffix, value);
        }

        /// <summary>
        /// カテゴリリスト
        /// </summary>
        public ObservableCollection<Category> Categories { get; set; }

        /// <summary>
        /// 情報元リスト
        /// </summary>
        public ObservableCollection<Via> ViaList { get; set; }

        /// <summary>
        /// 改行クラス
        /// </summary>
        public class NewLineItem
        {
            /// <summary>
            /// 改行コード表示名
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 改行コード
            /// </summary>
            public string Code { get; set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="name">改行コード表示名</param>
            /// <param name="code">改行コード</param>
            public NewLineItem(string name, string code)
            {
                this.Name = name;
                this.Code = code;
            }
        }

        /// <summary>
        /// 改行リスト
        /// </summary>
        public ObservableCollection<NewLineItem> NewLines { get; set; }

        /// <summary>
        /// 親ウィンドウVM
        /// </summary>
        private readonly MainWindowModel mainWindow;

        /// <summary>
        /// 設定ウィンドウを閉じれるかどうか。
        /// </summary>
        public bool CanClose 
        {
            get
            {
                if (isCloseCheck)
                {
                    return isSaved;
                }
                return true;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="viewModel">親ウィンドウ</param>
        private SettingsWindowViewModel(MainWindowModel viewModel, bool isInit = false)
        {
            mainWindow = viewModel;
            // コマンドの設定
            SaveCommand = new ViewModelCommand(SaveDo);
            CancelCommand = new ViewModelCommand(Cancel);
            PreviewCommand = new ViewModelCommand(Preview);
            this.isInit = isInit;
            this.isCloseCheck = isInit;
        }

        /// <summary>
        /// 自クラスインスタンス
        /// </summary>
        private static SettingsWindowViewModel instance = null;

        /// <summary>
        /// インスタンス取得
        /// </summary>
        /// <param name="viewModel">親ウィンドウVM</param>
        /// <returns>インスタンス</returns>
        public static SettingsWindowViewModel GetInstance(MainWindowModel viewModel, bool isInit = false)
        {
            if(instance == null)
            {
                instance = new SettingsWindowViewModel(viewModel, isInit);
            }
            else
            {
                instance.isInit = isInit;
            }
            return instance;
        }

        // This method would be called from View, when ContentRendered event was raised.
        /// <summary>
        /// 初期化
        /// </summary>
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
            this.ViaList = new ObservableCollection<Via>();
            foreach (var kvp in config.ViaList)
            {
                this.ViaList.Add(new Via() { Name = kvp.Value, Data = kvp.Key });
            }
            this.NotifyPropertyChanged(nameof(ViaList));
            isInit = false;
            isSaved = false;
        }

        /// <summary>
        /// 保存ボタン押下時のコマンド
        /// </summary>
        public ViewModelCommand SaveCommand { get; private set; }

        /// <summary>
        /// 保存ボタン押下時の処理
        /// </summary>
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
            isSaved = true;
            this.NotifyPropertyChanged("CanClose");
            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }

        /// <summary>
        /// キャンセルボタン押下時のコマンド
        /// </summary>
        public ViewModelCommand CancelCommand { get; private set; }

        /// <summary>
        /// キャンセルボタン押下時の処理
        /// </summary>
        public void Cancel()
        {
            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }

        /// <summary>
        /// プレビューボタン押下時のコマンド
        /// </summary>
        public ViewModelCommand PreviewCommand { get; private set; }

        /// <summary>
        /// プレビューボタン押下時の処理
        /// </summary>
        public void Preview()
        {
            var config = ConfigManager.GetCopyConfig();
            UpdateConfig(config);
            Messenger.Raise(new TransitionMessage(new OutTextPreviewViewModel(config), "Preview"));
        }

        /// <summary>
        /// 引数に渡された設定を入力された内容で更新する。
        /// </summary>
        /// <param name="config">更新対象の設定クラス</param>
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
            config.ViaList.Clear();
            foreach (var via in this.ViaList)
            {
                config.ViaList.Add(new KeyValuePair<string, string>(via.Data, via.Name));
            }
        }

        /// <summary>
        /// チェック処理
        /// </summary>
        /// <returns></returns>
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
