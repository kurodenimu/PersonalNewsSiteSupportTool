using Livet;
using Livet.Messaging;
using System.Windows;

namespace PersonalNewsSiteSupportTool.ViewModels
{
    /// <summary>
    /// ViewModelの基底クラス
    /// </summary>
    class ViewModelBase : ViewModel
    {

        /// <summary>
        /// プロパティ変更イベント時の処理
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// インフォメーションのメッセージボックス表示。
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="title">表示するタイトル</param>
        public void ShowInfoMessage(string message, string title = "情報")
        {
            ActivateWindow();
            Messenger.Raise(new InformationMessage(message, title, MessageBoxImage.Information, "Information"));
        }

        /// <summary>
        /// エラーのメッセージボックス表示
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="title">表示するタイトル</param>
        public void ShowErrorMessage(string message, string title = "エラー")
        {
            ActivateWindow();
            Messenger.Raise(new InformationMessage(message, title, MessageBoxImage.Error, "Error"));
        }

        /// <summary>
        /// 確認のメッセージボックス表示
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="title">表示するタイトル</param>
        /// <returns>メッセージボックスでOKを押したかどうか。OK押下時にtrue。</returns>
        public bool ShowConfirmMessage(string message, string title = "確認")
        {
            ActivateWindow();
            var confirmationMessage = new ConfirmationMessage(message, title, MessageBoxImage.Question, MessageBoxButton.OKCancel, "Confirm");
            Messenger.Raise(confirmationMessage);

            return confirmationMessage.Response ?? false;
        }

        /// <summary>
        /// アクティブウィンドウの制御
        /// アクティブなウィンドウがない場合のみ、MainWindowをアクティブにする。
        /// メッセージボックスを表示するのにアクティブなウィンドウが必要なため。
        /// </summary>
        private void ActivateWindow()
        {
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
