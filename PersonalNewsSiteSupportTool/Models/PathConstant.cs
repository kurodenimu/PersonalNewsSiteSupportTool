using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalNewsSiteSupportTool.Models
{
    /// <summary>
    /// パスに関する定数を所持するクラス。
    /// </summary>
    public static class PathConstant
    {
        /// <summary>
        /// RoamingのApplicationDataフォルダのパス
        /// </summary>
        public static readonly string APP_DATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            + @"\PersonalNewsSiteSupportTool";

        /// <summary>
        /// exeの格納されているパス
        /// </summary>
        public static readonly string APP_PATH = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
    }
}
