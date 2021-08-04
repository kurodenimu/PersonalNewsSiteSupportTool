using System;
using System.Threading.Tasks;
using System.Windows;

namespace PersonalNewsSiteSupportTool.Models
{
    public static class ExceptionHandling
    {
        private static bool initFlag = false;

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

        private static void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMember = GetTargetSiteName(e.Exception);
            string errorMessage = e.Exception.Message;
            string message = string.Format(@"UnhandledException occurred：{0}. ErrorMessage：{1}, Type：{2}\r\nStackTrace：{3}",
                                      errorMember, errorMessage, e.Exception.GetType().FullName, e.Exception.StackTrace);
            LogService.ErrorLog(message);
        }

        private static void App_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            string errorMember = GetTargetSiteName(e.Exception);
            string errorMessage = e.Exception.Message;
            string message = string.Format(@"Exception occurred：{0}. ErrorMessage：{1}, Type：{2}\r\nStackTrace：{3}",
                                      errorMember, errorMessage, e.Exception.GetType().FullName, e.Exception.StackTrace);
            LogService.ErrorLog(message);
        }

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

        private static void App_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            string errorMember = GetTargetSiteName(e.Exception);
            string errorMessage = e.Exception.Message;
            string message = string.Format(@"UnobservedTaskException occurred：{0}. ErrorMessage：{1}, Type：{2}\r\nStackTrace：{3}",
                                      errorMember, errorMessage, e.Exception.GetType().FullName, e.Exception.StackTrace);
            LogService.ErrorLog(message);
        }

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
