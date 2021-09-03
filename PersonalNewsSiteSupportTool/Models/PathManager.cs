using PersonalNewsSiteSupportTool.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalNewsSiteSupportTool.Models
{
    /// <summary>
    /// パスを管理するクラス。
    /// </summary>
    public static class PathManager
    {
        /// <summary>
        /// exeの格納されているパス
        /// </summary>
        private static readonly string APP_PATH = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');

        public static string AppDataPath { get; private set; } = 
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\PersonalNewsSiteSupportTool";

        public static string GetAppPath()
        {
            return APP_PATH;
        }

        public static string GetFullPath(string folder, string filename)
        {
            if (folder.EndsWith("\\"))
            {
                return folder + filename;
            }
            else
            {
                return folder + "\\" + filename;
            }
        }
    }
}
