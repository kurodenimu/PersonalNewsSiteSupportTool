using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PersonalNewsSiteSupportTool.Models
{
    class ClipboardWrapper
    {
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
