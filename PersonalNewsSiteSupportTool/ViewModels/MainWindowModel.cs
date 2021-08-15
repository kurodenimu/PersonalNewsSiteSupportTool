using Livet;
using Livet.Commands;
using Livet.Messaging;
using PersonalNewsSiteSupportTool.Behaviors;
using PersonalNewsSiteSupportTool.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace PersonalNewsSiteSupportTool.ViewModels
{
    /// <summary>
    /// MainWindowのViewModel
    /// </summary>
    class MainWindowModel : ViewModelBase
    {
        /// <summary>
        /// MainWindowがロードされたときに実行されるコマンド
        /// </summary>
        public ViewModelCommand LoadedCommand { get; private set; }

        /// <summary>
        /// 終了メニューを押下した時に実行されるコマンド
        /// </summary>
        public ViewModelCommand ExitCommand { get; private set; }

        /// <summary>
        /// 完了ボタン押下時に実行されるコマンド
        /// </summary>
        public ViewModelCommand CompleteButtonClick { get; private set; }

        /// <summary>
        /// 結合メニュー押下時に実行されるコマンド
        /// </summary>
        public ViewModelCommand CatCommand { get; private set; }

        /// <summary>
        /// 設定メニューを押下した時に実行されるコマンド
        /// </summary>
        public ViewModelCommand OpenSettingsCommand { get; private set; }

        /// <summary>
        /// カテゴリーリスト
        /// </summary>
        public ObservableCollection<Category> Categories { get; set; }

        /// <summary>
        /// 情報元リスト
        /// </summary>
        public ObservableCollection<Via> ViaList { get; set; }

        /// <summary>
        /// クリップボード監視クラス
        /// </summary>
        ClipboardWatcher clipboardWatcher;

        /// <summary>
        /// カテゴリID
        /// </summary>
        private string categoryId;

        /// <summary>
        /// ニュースURL
        /// </summary>
        private string newsUrl;

        /// <summary>
        /// 情報元
        /// </summary>
        private string via;

        /// <summary>
        /// 情報元編集可否
        /// </summary>
        private bool isViaEditabled;

        /// <summary>
        /// ニュースコメント
        /// </summary>
        private string newsComment;

        /// <summary>
        /// コンストラクタ。1回しか呼ばれないのでアプリ内の初期化を兼ねている。
        /// </summary>
        public MainWindowModel()
        {
            LogService.Init();
            LoadedCommand = new ViewModelCommand(LoadedAction);
            CompleteButtonClick = new ViewModelCommand(CompleteAction);
            CatCommand = new ViewModelCommand(Cat, CanCat);
            OpenSettingsCommand = new ViewModelCommand(OpenSettings);
            ExitCommand = new ViewModelCommand(Exit);
            ExceptionHandling.Init();
        }

        /// <summary>
        /// MainWindowがロードされた時の処理。
        /// クリップボード監視クラスの初期化をここで行うのはウィンドウハンドルが必要なため。
        /// コンフィグの読み込みがこのタイミングなのは設定の内容をMainWindowに反映させるため。
        /// </summary>
        private void LoadedAction()
        {
            var mainWindow = Application.Current.MainWindow;
            mainWindow.Hide();
            mainWindow.ShowInTaskbar = true;
            //Clipboardwatcher作成
            //ウィンドウハンドルを渡す
            clipboardWatcher = new ClipboardWatcher(new WindowInteropHelper(mainWindow).Handle);
            //クリップボード内容変更イベントに関連付け
            clipboardWatcher.UpdateClipboard += ClipboardWatcher_DrawClipboard;
            clipboardWatcher.Start();
            ConfigLoad();
        }

        /// <summary>
        /// 完了ボタン押下時の処理
        /// </summary>
        private void CompleteAction()
        {

            if (Categories.Count != 0 & (CategoryId == null | "".Equals(CategoryId, StringComparison.Ordinal)))
            {
                ShowInfoMessage("カテゴリが選択されていません。");
            }
            else
            {
                Config config = ConfigManager.Config;

                string fileName = $"{config.OutFilePrefix}{CategoryId}{config.OutFileSuffix}";
                string outText = CommonUtil.GetOutText(NewsUrl, Via, NewsComment);

                if (AppendTextFile(config.SavePath, fileName, outText))
                {
                    // 書込みが成功した時だけウィンドウを隠す。
                    Application.Current.MainWindow.Hide();
                }
            }
        }

        /// <summary>
        /// 設定メニュー押下時の処理。
        /// </summary>
        private void OpenSettings()
        {
            Messenger.Raise(new TransitionMessage(SettingsWindowViewModel.GetInstance(this), "OpenSettings"));
        }

        /// <summary>
        /// 終了メニュー押下時の処理
        /// </summary>
        public void Exit()
        {
            if (ShowConfirmMessage("終了しますか"))
            {
                clipboardWatcher.Stop();
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// 結合コマンドの実行可否
        /// </summary>
        /// <returns></returns>
        public bool CanCat()
        {
            return true;
        }

        /// <summary>
        /// 結合メニューの押下時の処理
        /// </summary>
        public void Cat()
        {
            Config config = ConfigManager.Config;
            string savePath = config.SavePath;
            string newLine = config.NewLine;
            string outText = "";
            foreach (var kvp in config.Categories)
            {
                string fullPath = $"{savePath}{config.OutFilePrefix}{kvp.Key}{config.OutFileSuffix}";
                if (File.Exists(fullPath))
                {
                    outText += $"{config.CategoryPrefix}{kvp.Value}{newLine}{newLine}";
                    outText += $"{File.ReadAllText(fullPath)}{newLine}";
                }
            }

            _ = OverwriteTextFile(savePath, config.MergeFileName, outText);
        }

        /// <summary>
        /// クリップボードに変化があった時の処理
        /// </summary>
        /// <param name="sender">イベントの送信元</param>
        /// <param name="e">イベント</param>
        private void ClipboardWatcher_DrawClipboard(object sender, EventArgs e)
        {
            var mainWindow = Application.Current.MainWindow;

            if (Clipboard.ContainsText())
            {
                String cbText = ClipboardWrapper.GetText();
                if (string.IsNullOrEmpty(cbText))
                {
                    // クリップボードからテキストが取得できなかった場合、何もせずにメソッドを終了する。
                    return;
                }
                Config config = ConfigManager.Config;
                if (cbText.StartsWith(config.WatchWord, StringComparison.Ordinal))
                {

                    if (mainWindow.Visibility == Visibility.Visible)
                    {
                        mainWindow.WindowState = WindowState.Normal;
                        if (!ShowConfirmMessage("現在の表示内容を消しても構いませんか。"))
                        {
                            return;
                        }
                    }
                    mainWindow.Show();
                    mainWindow.WindowState = WindowState.Normal;
                    if (Categories.Count == 1)
                    {
                        CategoryId = Categories[0].Id;
                    }
                    else
                    {
                        CategoryId = null;
                    }
                    if (config.IsRemoveWatchWord)
                    {
                        NewsUrl = cbText.Replace(config.WatchWord, "");
                    }
                    else
                    {
                        NewsUrl = cbText;
                    }

                    // テキストボックス欄にフォーカスがあった場合、VMの値が更新されておらず
                    // 値が変更されていない扱いとなることがあるため強制的にプロパティ変更通知を行う。
                    via = "";
                    NotifyPropertyChanged(nameof(Via));
                    isViaEditabled = true;
                    NotifyPropertyChanged(nameof(IsViaEditabled));
                    newsComment = "";
                    NotifyPropertyChanged(nameof(NewsComment));
                }
            }
        }

        /// <summary>
        /// カテゴリID
        /// </summary>
        public String CategoryId
        {
            get => categoryId;
            set => RaisePropertyChangedIfSet(ref categoryId, value);
        }

        /// <summary>
        /// ニュースURL
        /// </summary>
        public String NewsUrl
        {
            get => newsUrl;
            set => RaisePropertyChangedIfSet(ref newsUrl, value);
        }

        /// <summary>
        /// 情報元
        /// </summary>
        public String Via
        {
            get => via;
            set
            {
                RaisePropertyChangedIfSet(ref via, value);
                if (via == null | "".Equals(via, StringComparison.Ordinal))
                {
                    IsViaEditabled = true;
                }
                else
                {
                    foreach (var via in this.ViaList)
                    {
                        if (this.via == via.Data)
                        {
                            IsViaEditabled = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 情報元編集可否
        /// </summary>
        public bool IsViaEditabled
        {
            get => isViaEditabled;
            set => RaisePropertyChangedIfSet(ref isViaEditabled, value);
        }

        /// <summary>
        /// ニュースコメント
        /// </summary>
        public String NewsComment
        {
            get => newsComment;
            set => RaisePropertyChangedIfSet(ref newsComment, value);
        }

        /// <summary>
        /// 結合可否
        /// </summary>
        private bool isCatEnabled;

        /// <summary>
        /// 結合可否
        /// </summary>
        public bool IsCatEnabled
        {
            get => isCatEnabled;
            set => RaisePropertyChangedIfSet(ref isCatEnabled, value);
        }

        /// <summary>
        /// テキストファイルへの追記
        /// </summary>
        /// <param name="folder">出力するファイルのフォルダ</param>
        /// <param name="fileName">出力するファイル名</param>
        /// <param name="outText">出力する内容</param>
        /// <returns>追記の成否</returns>
        private bool AppendTextFile(string folder, string fileName, string outText)
        {
            try
            {
                File.AppendAllText($"{folder}{fileName}", outText);
            }
            catch (PathTooLongException e)
            {
                ShowErrorMessage($"出力先のファイルパスが長すぎます。\n{folder}{fileName}");
                LogService.DumpException(e);
                return false;
            }
            catch (DirectoryNotFoundException e)
            {
                ShowErrorMessage($"出力先のフォルダが見つかりませんでした。\n{folder}");
                LogService.DumpException(e);
                return false;
            }
            catch (IOException e)
            {
                ShowErrorMessage("ファイル出力時にエラーが発生しました。");
                LogService.DumpException(e);
                return false;
            }
            catch (UnauthorizedAccessException e)
            {
                ShowErrorMessage("ファイル出力時にエラーが発生しました。");
                LogService.DumpException(e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// テキストの出力（上書き）
        /// </summary>
        /// <param name="folder">出力するファイルのフォルダ</param>
        /// <param name="fileName">出力するファイル名</param>
        /// <param name="outText">出力する内容</param>
        /// <returns>出力の成否</returns>
        private bool OverwriteTextFile(string folder, string fileName, string outText)
        {
            if (File.Exists($"{folder}{fileName}"))
            {
                if (!ShowConfirmMessage("出力先にファイルがありますが上書きしますか？"))
                {
                    return false;
                }
            }
            try
            {
                File.WriteAllText($"{folder}{fileName}", outText);
            }
            catch (PathTooLongException e)
            {
                ShowErrorMessage($"出力先のファイルパスが長すぎます。\n{folder}{fileName}");
                LogService.DumpException(e);
                return false;
            }
            catch (DirectoryNotFoundException e)
            {
                ShowErrorMessage($"出力先のフォルダが見つかりませんでした。\n{folder}");
                LogService.DumpException(e);
                return false;
            }
            catch (IOException e)
            {
                ShowErrorMessage("ファイル出力時にエラーが発生しました。");
                LogService.DumpException(e);
                return false;
            }
            catch (UnauthorizedAccessException e)
            {
                ShowErrorMessage("ファイル出力時にエラーが発生しました。");
                LogService.DumpException(e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 設定読込メソッド。
        /// </summary>
        public void ConfigLoad()
        {
            // 設定を読み込む時のメソッド。
            // 設定クラス再読込
            ConfigManager.ReloadConfig();
            Config config = ConfigManager.Config;

            this.Categories = new ObservableCollection<Category>();
            // カテゴリ設定
            foreach (var kvp in config.Categories)
            {
                this.Categories.Add(new Category() { Name = kvp.Value, Id = kvp.Key });
            }
            this.NotifyPropertyChanged(nameof(Categories));

            // 情報元設定
            this.ViaList = new ObservableCollection<Via>
            {
                new Via() { Name = "自分で入力", Data = "" }
            };
            foreach (var kvp in config.ViaList)
            {
                this.ViaList.Add(new Via() { Name = kvp.Value, Data = kvp.Key });
            }
            this.NotifyPropertyChanged(nameof(ViaList));

            // 結合コマンドの有効無効
            IsCatEnabled = !string.IsNullOrEmpty(config.MergeFileName);
        }
    }
}
