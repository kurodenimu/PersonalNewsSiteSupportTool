using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using PersonalNewsSiteSupportTool.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PersonalNewsSiteSupportTool.ViewModels
{
    /// <summary>
    /// テキスト出力プレビューのViewModel
    /// </summary>
    public class OutTextPreviewViewModel : ViewModel
    {
        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).

        /// <summary>
        /// 出力するテキストの内容
        /// </summary>
        private string outText;

        /// <summary>
        /// 出力するテキストの内容
        /// </summary>
        public string OutText
        {
            get => outText;
            set => RaisePropertyChangedIfSet(ref outText, value);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="config">プレビューに使用する設定</param>
        public OutTextPreviewViewModel(Config config)
        {
            OutText = CommonUtil.GetOutText("<a href=\"https://sample.com/sample.html\">Sample Url</a>", "Sample Information Source", "Sample Comment.", config);
        }
    }
}
