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
    class MainWindowModel : ViewModelBase
    {

        public ViewModelCommand LoadedCommand { get; private set; }

        public ViewModelCommand CompleteButtonClick { get; private set; }

        public ViewModelCommand OpenSettingsCommand { get; private set; }

        public ObservableCollection<Category> Categories { get; set; }

        public ObservableCollection<InformationSource> InformationSources { get; set; }

        ClipboardWatcher clipboardWatcher;

        private string categoryId;

        private string newsUrl;

        private string via;

        private bool isViaEditabled;

        private string newsComment;

        public MainWindowModel()
        {
            LogService.Init();
            LoadedCommand = new ViewModelCommand(LoadedAction);
            CompleteButtonClick = new ViewModelCommand(CompleteAction);
            OpenSettingsCommand = new ViewModelCommand(OpenSettings);
            ExceptionHandling.Init();
        }

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

        private void OpenSettings()
        {
            Messenger.Raise(new TransitionMessage(SettingsWindowViewModel.GetInstance(this), "OpenSettings"));
        }

        private ViewModelCommand exitCommand;

        public ViewModelCommand ExitCommand
        {
            get
            {
                if (exitCommand == null)
                {
                    exitCommand = new ViewModelCommand(Exit, CanExit);
                }
                return exitCommand;
            }
        }

        public bool CanExit()
        {
            return true;
        }

        public void Exit()
        {
            if (ShowConfirmMessage("終了しますか"))
            {
                clipboardWatcher.Stop();
                Application.Current.Shutdown();
            }
        }


        private ViewModelCommand catCommand;

        public ViewModelCommand CatCommand
        {
            get
            {
                if (catCommand == null)
                {
                    catCommand = new ViewModelCommand(Cat, CanCat);
                }
                return catCommand;
            }
        }

        public bool CanCat()
        {
            return true;
        }

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

        public String CategoryId
        {
            get => categoryId;
            set
            {
                if (categoryId != value)
                {
                    categoryId = value;
                    this.NotifyPropertyChanged(nameof(CategoryId));
                }
            }
        }

        public String NewsUrl
        {
            get => newsUrl;
            set
            {
                if (newsUrl != value)
                {
                    newsUrl = value;
                    this.NotifyPropertyChanged(nameof(NewsUrl));
                }
            }
        }

        public String Via
        {
            get => via;
            set
            {
                if (via != value)
                {
                    via = value;
                    this.NotifyPropertyChanged(nameof(Via));

                    if (via == null | "".Equals(via, StringComparison.Ordinal))
                    {
                        IsViaEditabled = true;
                    }
                    else
                    {
                        foreach (var informationSource in this.InformationSources) {
                            if (via == informationSource.Data)
                            {
                                IsViaEditabled = false;
                            }
                        }
                    }
                }
            }
        }

        public bool IsViaEditabled
        {
            get => isViaEditabled;
            set
            {
                if (isViaEditabled != value)
                {
                    isViaEditabled = value;
                    this.NotifyPropertyChanged(nameof(IsViaEditabled));
                }
            }
        }

        public String NewsComment
        {
            get => newsComment;
            set
            {
                if (newsComment != value)
                {
                    newsComment = value;
                    this.NotifyPropertyChanged(nameof(NewsComment));
                }
            }
        }

        private bool isCatEnabled;

        public bool IsCatEnabled
        {
            get => isCatEnabled;
            set => RaisePropertyChangedIfSet(ref isCatEnabled, value);
        }


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
            this.InformationSources = new ObservableCollection<InformationSource>
            {
                new InformationSource() { Name = "自分で入力", Data = "" }
            };
            foreach (var kvp in config.InformationSources)
            {
                this.InformationSources.Add(new InformationSource() { Name = kvp.Value, Data = kvp.Key });
            }
            this.NotifyPropertyChanged(nameof(InformationSources));

            // 結合コマンドの有効無効
            IsCatEnabled = !string.IsNullOrEmpty(config.MergeFileName);
        }
    }
}
