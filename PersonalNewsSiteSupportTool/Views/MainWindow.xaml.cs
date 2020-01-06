using PersonalNewsSiteSupportTool.ViewModels;
using System;
using System.IO;
using System.Windows;

namespace PersonalNewsSiteSupportTool.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance = null;

        public MainWindow()
        {
            InitializeComponent();
            instance = this;
        }
    }
}
