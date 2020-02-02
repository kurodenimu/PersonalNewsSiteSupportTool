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
            Application.Current.MainWindow.Activate();
            Messenger.Raise(new InformationMessage(message, title, MessageBoxImage.Information, "Information"));
        }
        public void ShowErrorMessage(string message, string title = "エラー")
        {
            Application.Current.MainWindow.Activate();
            Messenger.Raise(new InformationMessage(message, title, MessageBoxImage.Error, "Error"));
        }
        public bool ShowConfirmMessage(string message, string title = "確認")
        {
            Application.Current.MainWindow.Activate();
            var confirmationMessage = new ConfirmationMessage(message, title, MessageBoxImage.Question, MessageBoxButton.OKCancel, "Confirm");
            Messenger.Raise(confirmationMessage);

            return confirmationMessage.Response ?? false;
        }
    }
}
