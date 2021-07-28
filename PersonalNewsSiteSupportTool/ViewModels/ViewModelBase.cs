using Livet;
using Livet.Messaging;
using System.Windows;

namespace PersonalNewsSiteSupportTool.ViewModels
{
    class ViewModelBase : ViewModel
    {

        /// <summary>
        /// プロパティ変更イベント時の処理
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName);
        }

        public void ShowInfoMessage(string message, string title = "情報")
        {
            ActivateWindow();
            Messenger.Raise(new InformationMessage(message, title, MessageBoxImage.Information, "Information"));
        }
        public void ShowErrorMessage(string message, string title = "エラー")
        {
            ActivateWindow();
            Messenger.Raise(new InformationMessage(message, title, MessageBoxImage.Error, "Error"));
        }
        public bool ShowConfirmMessage(string message, string title = "確認")
        {
            ActivateWindow();
            var confirmationMessage = new ConfirmationMessage(message, title, MessageBoxImage.Question, MessageBoxButton.OKCancel, "Confirm");
            Messenger.Raise(confirmationMessage);

            return confirmationMessage.Response ?? false;
        }

        private void ActivateWindow()
        {
            // アクティブなウィンドウがない場合のみ、MainWindowをアクティブにする。
            // メッセージボックスを表示するのにアクティブなウィンドウが必要なため。
            bool existActiveWindow = false;
            foreach (Window window in Application.Current.Windows)
            {
                if (window.IsActive)
                {
                    existActiveWindow = true;
                    break;
                }
            }
            if (!existActiveWindow)
            {
                Application.Current.MainWindow.Activate();
            }
        }
    }
}
