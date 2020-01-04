using PersonalNewsSiteSupportTool.Behaviors;
using PersonalNewsSiteSupportTool.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Interop;

namespace PersonalNewsSiteSupportTool.ViewModels
{
    class MainWindowModel : ViewModelBase
    {

        // TODO :本クラス内ではクリップボードのためにウィンドウへのハンドルを参照している。
        // 暫定的に他のウィンドウの直接操作も許容している。
        // この辺りをどうするべきかは別途検討する。

        public ICommand LoadedCommand { get; private set; }

        public ICommand CompleteButtonClick { get; set; }

        public TriggerAction<Window> WindowClosingAction { get; private set; }

        public ObservableCollection<Category> Categories { get; set; }

        ClipboardWatcher clipboardWatcher = null;

        private String categoryId;

        private String newsComment;

        private String savePath = "";

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

            if (CategoryId == null | "".Equals(CategoryId))
            {
                MessageBox.Show("カテゴリが選択されていません。");
            }
            else
            {
                File.AppendAllText(savePath + "news_" + CategoryId + ".txt", NewsComment + "\r\n\r\n");
                MainWindow.instance.Hide();
            }
        }

        private void ClipboardWatcher_DrawClipboard(object sender, EventArgs e)
        {
            MainWindow mainWindow = MainWindow.instance;

            if (Clipboard.ContainsText())
            {
                String cbText = Clipboard.GetText();
                if (cbText.StartsWith("***["))
                {
                    mainWindow.Show();
                    mainWindow.WindowState = WindowState.Normal;
                    newsComment = cbText + "\r\n";
                    this.NotifyPropertyChanged("NewsComment");
                    categoryId = null;
                    this.NotifyPropertyChanged("CategoryId");
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
                    this.NotifyPropertyChanged("CategoryId");
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
                    this.NotifyPropertyChanged("NewsComment");
                }
            }
        }

        private void ConfigLoad()
        {
            // TODO 設定を読み込んだ時のメソッド。現在は固定値。
            this.Categories = new ObservableCollection<Category>() {
                new Category() { Name = "IT", Id = "IT"},
                new Category() { Name = "その他", Id  = "その他"},
                new Category() { Name = "old", Id = "old"}
            };
            this.NotifyPropertyChanged("Categories");
            savePath = @"C:\Users\pyonko\Dropbox\";
        }
    }
}
