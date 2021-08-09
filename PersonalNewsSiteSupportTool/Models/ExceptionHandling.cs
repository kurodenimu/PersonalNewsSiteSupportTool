using System;
using System.Threading.Tasks;
using System.Windows;

namespace PersonalNewsSiteSupportTool.Models
{
    /// <summary>
    /// 例外ハンドリングクラス。
    /// </summary>
    public static class ExceptionHandling
    {
        /// <summary>
        /// 初期化フラグ。
        /// </summary>
        private static bool initFlag = false;

        /// <summary>
        /// 初期化メソッド。
        /// </summary>
        public static void Init()
        {
            if (!initFlag)
            {
                // 例外発生時
                AppDomain.CurrentDomain.FirstChanceException += App_FirstChanceException;
                // 未処理例外(UIスレッド)
                Application.Current.DispatcherUnhandledException += App_DispatcherUnhandledException;
                // 未処理例外(全体)
                AppDomain.CurrentDomain.UnhandledException += App_UnhandledException;
                // バックグラウンドタスクで発生した例外
                TaskScheduler.UnobservedTaskException += App_UnobservedTaskException;
                initFlag = true;
            }
        }

        /// <summary>
        ///  未処理例外発生時のメソッド。
        /// </summary>
        /// <param name="sender">イベントを発生させたオブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private static void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMember = GetTargetSiteName(e.Exception);
            string errorMessage = e.Exception.Message;
            string message = string.Format(@"UnhandledException occurred：{0}. ErrorMessage：{1}, Type：{2}\r\nStackTrace：{3}",
                                      errorMember, errorMessage, e.Exception.GetType().FullName, e.Exception.StackTrace);
            LogService.ErrorLog(message);
        }

        /// <summary>
        /// マネージコード内で発生した例外が発生した時にトラップするメソッド。
        /// </summary>
        /// <param name="sender">イベントを発生させたオブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private static void App_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            string errorMember = GetTargetSiteName(e.Exception);
            string errorMessage = e.Exception.Message;
            string message = string.Format(@"Exception occurred：{0}. ErrorMessage：{1}, Type：{2}\r\nStackTrace：{3}",
                                      errorMember, errorMessage, e.Exception.GetType().FullName, e.Exception.StackTrace);
            LogService.ErrorLog(message);
        }

        /// <summary>
        /// 未処理例外発生時（アプリ全体）のメソッド
        /// </summary>
        /// <param name="sender">イベントを発生させたオブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private static void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string errorMember;
            string errorMessage;
            string message;

            if (!(e.ExceptionObject is Exception exception))
            {
                message = "Unhandled Exception occurred. But not System.Exception." + e.ToString();
            }
            else
            {
                errorMember = GetTargetSiteName(exception);
                errorMessage = exception.Message;
                message = string.Format(@"Unhandled Exception occurred：{0}. ErrorMessage：{1}, Type：{2}\r\nStackTrace：{3}",
                                          errorMember, errorMessage, exception.GetType().FullName, exception.StackTrace);
            }
            LogService.ErrorLog(message);
        }

        /// <summary>
        /// バックグラウンドタスクで発生した未処理例外を処理するメソッド。
        /// </summary>
        /// <param name="sender">イベントを発生させたオブジェクト</param>
        /// <param name="e">発生したイベント</param>
        private static void App_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            string errorMember = GetTargetSiteName(e.Exception);
            string errorMessage = e.Exception.Message;
            string message = string.Format(@"UnobservedTaskException occurred：{0}. ErrorMessage：{1}, Type：{2}\r\nStackTrace：{3}",
                                      errorMember, errorMessage, e.Exception.GetType().FullName, e.Exception.StackTrace);
            LogService.ErrorLog(message);
        }

        /// <summary>
        /// Exceptionが発生したメソッドの名前を取得。
        /// </summary>
        /// <param name="exception">例外</param>
        /// <returns>メソッド名。取得できない場合は空文字を返却。</returns>
        private static string GetTargetSiteName(Exception exception)
        {
            string ret = "";
            if (exception.TargetSite != null)
            {
                ret = exception.TargetSite.Name;
            }

            return ret;
        }
    }
}
