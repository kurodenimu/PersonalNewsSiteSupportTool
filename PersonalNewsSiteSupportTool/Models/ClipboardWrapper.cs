using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PersonalNewsSiteSupportTool.Models
{
    /// <summary>
    /// Clipboardのラッパークラス。
    /// </summary>
    class ClipboardWrapper
    {
        /// <summary>
        /// クリップボードのテキストを取得するメソッド。クリップボードにアクセスできない場合にリトライを行う。
        /// </summary>
        /// <returns>クリップボード内のテキスト。クリップボードにアクセスできない場合、クリップボード内がテキストじゃない場合に空文字を返却する。</returns>
        public static string GetText()
        {
            string ret = "";
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        ret = Clipboard.GetText();
                    }
                    break;
                }
                catch (System.Runtime.InteropServices.COMException e)
                {
                    // 最大10回同じログを出力するのもイマイチなので簡素なログにする。
                    LogService.ErrorLog($"Clipboard Error. Message：{e.Message} retry count {i}.");
                }
                System.Threading.Thread.Sleep(0);
            }

            return ret;
        }
    }
}
