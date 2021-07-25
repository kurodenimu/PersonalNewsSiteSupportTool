using NLog;
using System;

namespace PersonalNewsSiteSupportTool.Models
{
    public static class LogService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static bool isInit = false;

#pragma warning disable CA1802 // Use literals where appropriate
        private static readonly bool isDebug =
#pragma warning restore CA1802 // Use literals where appropriate
#if DEBUG
            true;
#else
            false;
#endif

        public static void Init()
        {
            if (!isInit)
            {
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

        public static void Final()
        {
            logger.Info("End App.");
            LogManager.Flush();
        }

        public static void ErrorLog(String msg)
        {
            logger.Error(msg);
        }

        public static void DumpException(Exception e)
        {
            string msg = string.Format(@"Exception occurred : {0}. ErrorMessage：{1}, StackTrace：{2}",
                e.GetType().Name, e.Message, e.StackTrace);
            ErrorLog(msg);
        }

        public static void DebugLog(String msg)
        {
            if (isDebug)
            {
                logger.Debug(msg);
            }
        }
    }
}
