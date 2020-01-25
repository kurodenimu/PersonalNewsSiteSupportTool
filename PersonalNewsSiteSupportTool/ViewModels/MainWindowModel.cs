using PersonalNewsSiteSupportTool.Behaviors;
using PersonalNewsSiteSupportTool.Models;
using PersonalNewsSiteSupportTool.Views;
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

        // TODO :本クラス内ではクリップボードのためにウィンドウへのハンドルを参照している。
        // 暫定的に他のウィンドウの直接操作も許容している。
        // この辺りをどうするべきかは別途検討する。

        public ICommand LoadedCommand { get; private set; }

        public ICommand CompleteButtonClick { get; private set; }

        public ObservableCollection<Category> Categories { get; set; }

        public ObservableCollection<InformationSource> InformationSources { get; set; }

        ClipboardWatcher clipboardWatcher = null;

        private String categoryId;

        private String newsUrl;

        private String via;

        private bool isViaEditabled;

        private String newsComment;

        private Regex newLineRegex = new Regex("\r\n|\r|\n");

        public MainWindowModel()
        {
            LoadedCommand = new CommandBase(LoadedAction);
            CompleteButtonClick = new CommandBase(CompleteAction);
        }

        private void LoadedAction()
        {
            MainWindow mainWindow = MainWindow.instance;
            mainWindow.Hide();
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

            if (CategoryId == null | "".Equals(CategoryId, StringComparison.Ordinal))
            {
                MessageBox.Show("カテゴリが選択されていません。");
            }
            else
            {
                String viaText = "";
                if (Via != null & !"".Equals(Via, StringComparison.Ordinal))
                {
                    viaText = $"（via：{Via}）";
                }

                Config config = Config.GetInsrance();
                String newLine = config.NewLine;

                File.AppendAllText($"{config.SavePath}news_{CategoryId}.txt", $"{NewsUrl}{viaText}{newLine}{newLineRegex.Replace(NewsComment, newLine)}{newLine}{newLine}");
                MainWindow.instance.Hide();
            }
        }

        private void ClipboardWatcher_DrawClipboard(object sender, EventArgs e)
        {
            MainWindow mainWindow = MainWindow.instance;

            if (Clipboard.ContainsText())
            {
                String cbText = Clipboard.GetText();
                if (cbText.StartsWith("***[", StringComparison.Ordinal))
                {

                    if (mainWindow.Visibility == Visibility.Visible)
                    {
                        mainWindow.WindowState = WindowState.Normal;
                        mainWindow.Activate();
                        if (!ShowConfirmMessage("現在の表示内容を消しても構いませんか。"))
                        {
                            return;
                        }
                    }
                    mainWindow.Show();
                    mainWindow.WindowState = WindowState.Normal;
                    CategoryId = null;
                    NewsUrl = cbText;
                    // テキストボックス欄にフォーカスがあった場合、VMの値が更新されておらず
                    // 値が変更されていない扱いとなることがあるため強制的にプロパティ変更通知を行う。
                    via = "";
                    NotifyPropertyChanged(nameof(Via));
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

                    IsViaEditabled = via == null | "".Equals(via, StringComparison.Ordinal);
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

        private void ConfigLoad()
        {
            // TODO 設定を読み込む時のメソッド。現在は固定値。
            // 設定クラス再読込
            Config config = Config.GetInsrance();
            config.ReloadConfig();

            this.Categories = new ObservableCollection<Category>();
            // カテゴリ設定
            foreach (var kvp in config.Categories)
            {
                this.Categories.Add(new Category() { Name = kvp.Value, Id = kvp.Key });
            }
            this.NotifyPropertyChanged(nameof(Categories));

            // 情報元設定
            this.InformationSources = new ObservableCollection<InformationSource>()
            {
                new InformationSource() { Name = "自分で入力", Data=""},
                new InformationSource() { Name = "はてなブックマーク", Data = "はてなブックマーク"}
            };
            this.NotifyPropertyChanged(nameof(InformationSources));
        }
    }
}
