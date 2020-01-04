using System.ComponentModel;

namespace PersonalNewsSiteSupportTool.ViewModels
{
    class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        ///  プロパティ変更イベント
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティ変更イベント時の処理
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
