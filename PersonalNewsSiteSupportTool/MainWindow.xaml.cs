using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Interop;

namespace PersonalNewsSiteSupportTool
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        ClipboardWatcher clipboardWatcher = null;

        private String savePath = "";

        public ObservableCollection<Category> Categories { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            this.Categories = new ObservableCollection<Category>() {
                new Category() { Name = "IT", Id = "IT"},
                new Category() { Name = "その他", Id  = "その他"},
                new Category() { Name = "old", Id = "old"}
            };
            combobox1.ItemsSource = this.Categories;
            savePath = "C:\\Users\\pyonko\\Dropbox\\";
        }

        //アプリ起動直後
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            //Clipboardwatcher作成
            //ウィンドウハンドルを渡す
            // clipboardWatcher = new ClipboardWatcher(new WindowInteropHelper(new MainWindow()).Handle);
            clipboardWatcher = new ClipboardWatcher(new WindowInteropHelper(this).Handle);
            //クリップボード内容変更イベントに関連付け
            clipboardWatcher.DrawClipboard += ClipboardWatcher_DrawClipboard;
            clipboardWatcher.Start();
        }

        private String categoryId;

        public String CategoryId
        {
            get
            {
                return categoryId;
            }
            set
            {
                if (categoryId != value)
                {
                    categoryId = value;
                    this.NotifyPropertyChanged("CategiryId");
                }
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            this.Hide();
        }

        private void ClipboardWatcher_DrawClipboard(object sender, EventArgs e)
        {


            if (Clipboard.ContainsText())
            {
                String cbText = Clipboard.GetText();
                if (cbText.StartsWith("***["))
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                    textbox1.Text = cbText;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show((String)combobox1.SelectedValue);
            if (combobox1.SelectedValue == null | "".Equals(combobox1.SelectedValue))
            {
                MessageBox.Show("カテゴリが選択されていません。");
            }
            else
            {
                File.AppendAllText(savePath + "news_" + combobox1.SelectedValue + ".txt", textbox1.Text + "\r\n\r\n");
                this.Hide();
            }
        }
    }
}
