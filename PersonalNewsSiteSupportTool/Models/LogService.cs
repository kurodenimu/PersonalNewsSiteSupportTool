using NLog;
using System;

namespace PersonalNewsSiteSupportTool.Models
{
    public static class LogService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static bool isInit = false;

        private static readonly bool isDebug =
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

        public static void DebugLog(String msg)
        {
            if (isDebug)
            {
                logger.Debug(msg);
            }
        }
    }
}
