using NLog;
using System;
using System.IO;

namespace PersonalNewsSiteSupportTool.Models
{
    /// <summary>
    /// ログ管理クラス。
    /// </summary>
    public static class LogService
    {
        /// <summary>
        /// NLogのLogger。
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// ログの出力先。
        /// </summary>
        private static readonly string logPath = PathManager.GetAppDataPath() + @"\logs";

        /// <summary>
        /// 初期化フラグ。
        /// </summary>
        private static bool isInit = false;

        /// <summary>
        /// デバッグモードかどうかを所持。
        /// <para>定数にしても動作はするが、構成を切り替えた際に利用箇所にて動作不能コードと表示されるため、読取専用変数としている。
        /// また、定数にするよう警告が出るため抑制している。</para>
        /// </summary>
#pragma warning disable CA1802 // Use literals where appropriate
        private static readonly bool isDebug =
#pragma warning restore CA1802 // Use literals where appropriate
#if DEBUG
            true;
#else
            false;
#endif

        /// <summary>
        /// 初期化メソッド
        /// </summary>
        public static void Init()
        {
            if (!isInit)
            {
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                if (!isDebug)
                {
                    var config = LogManager.Configuration;
                    config.RemoveTarget("debugLog");
                }
                logger.Info("Start app.");
                LogManager.Flush();
                isInit = true;
            }
        }

        /// <summary>
        /// 終了メソッド。
        /// </summary>
        public static void Final()
        {
            logger.Info("End App.");
            LogManager.Flush();
        }

        /// <summary>
        /// エラーログ出力メソッド
        /// </summary>
        /// <param name="msg"></param>
        public static void ErrorLog(String msg)
        {
            logger.Error(msg);
        }

        /// <summary>
        /// 例外出力メソッド
        /// </summary>
        /// <param name="e"></param>
        public static void DumpException(Exception e)
        {
            string msg = string.Format(@"Exception occurred : {0}. ErrorMessage：{1}, StackTrace：{2}",
                e.GetType().Name, e.Message, e.StackTrace);
            ErrorLog(msg);
        }

        /// <summary>
        /// デバッグログ出力メソッド。
        /// </summary>
        /// <param name="msg"></param>
        public static void DebugLog(String msg)
        {
            if (isDebug)
            {
                logger.Debug(msg);
            }
        }
    }
}
