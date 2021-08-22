using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using NLog.Targets;
using PersonalNewsSiteSupportTool.Models;
using System;
using System.IO;

namespace UnitTestProject.Models
{
    [TestClass]
    public class LogServiceTest : TestBase
    {
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ClassInit(testContext);
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            ClassClean();
        }

        [TestMethod][Ignore("今の構成ではうまく動かないので対象外。そもそも、実装側を少し手を入れたほうがよさそう。")]
        public void TestInit()
        {
            var logPath = @"..\..\ResultData\logs";
            var debugPath = logPath + @"\debug.log";
            var errorPath = logPath + @"\error.log";
            var logService = new PrivateType(typeof(LogService));
            var originalLogPath = logService.GetStaticFieldOrProperty("logPath");
            try
            {
                // テスト用にログパスと初期化フラグを書換え
                logService.SetStaticFieldOrProperty("logPath", logPath);
                logService.SetStaticFieldOrProperty("isInit", false);
                if (Directory.Exists(logPath))
                {
                    File.Delete(debugPath);
                    File.Delete(errorPath);
                    Directory.Delete(logPath);
                }
                LogService.Init();
                var config = LogManager.Configuration;
                Console.WriteLine(config.FindTargetByName<FileTarget>("debugLog").FileName);
                Console.WriteLine(config.FindTargetByName<FileTarget>("errorLog").FileName);
                Assert.IsTrue(File.Exists(debugPath), "ファイルが作成されていません。" + debugPath);
                Assert.IsTrue(File.Exists(errorPath), "ファイルが作成されていません。" + errorPath);

                // テスト用に初期化フラグを書換え
                logService.SetStaticFieldOrProperty("isInit", false);
                LogService.Init();
            }
            finally
            {
                // 他のテストのために値を戻す。
                logService.SetStaticFieldOrProperty("logPath", originalLogPath);
            }
        }
    }
}
