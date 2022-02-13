using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersonalNewsSiteSupportTool.Models;
using System;
using System.IO;
using System.Collections.Generic;

namespace UnitTestProject.Models
{
    [TestClass]
    public class ConfigManagerTest :TestBase
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

        [TestMethod]
        public void TestReloadConfig_NoFile()
        {
            var configManager = new PrivateType(typeof(ConfigManager));
            try
            {
                // テスト用に設定ファイルのパスを書換え
                string noFilePath = "noconfig.json";
                configManager.SetStaticFieldOrProperty("configPath", noFilePath);
                if (File.Exists(noFilePath))
                {
                    // 前のテストの不備などでファイルが残っていた場合を考慮して削除。
                    File.Delete(noFilePath);
                }

                ConfigManager.ReloadConfig();

                Assert.IsTrue(File.Exists(noFilePath), "ファイルがコピーできていません。");
                // 設定ファイルの読込結果確認。代表して改行コードのみ確認。
                Assert.AreEqual("\n", ConfigManager.Config.NewLine, "読み込んだ設定ファイルの改行コードの値が誤っています。");
                File.Delete(noFilePath);
            }
            finally
            {
                // 他のテストのために設定を戻す。
                configManager.SetStaticFieldOrProperty("configPath", TEST_CONFIG_PATH);
            }
        }

        [TestMethod]
        public void TestReloadConfig_ExistFile()
        {
            var configManager = new PrivateType(typeof(ConfigManager));
            try
            {
                // テスト用に設定ファイルのパスを書換え
                string loadFilePath = @"..\..\TestData\loadtest.json";
                configManager.SetStaticFieldOrProperty("configPath", loadFilePath);

                ConfigManager.ReloadConfig();

                // 設定ファイルの読込結果確認。
                var expConfig = new Config
                {
                      WatchWord = "1",
                      IsRemoveWatchWord = false,
                      SavePath = "2",
                      OutFilePrefix = "3",
                      OutFileSuffix = "4",
                      NewLine = "5",
                      CategoryPrefix = "6",
                      CategorySuffix = "7",
                      ViaPrefix = "8",
                      ViaSuffix = "9",
                      MergeFileName = "10",
                      Categories = new List<KeyValuePair<string, string>>
                      {
                          new KeyValuePair<string, string>("Category1", "カテゴリ１")
                      },
                      ViaList = new List<KeyValuePair<string, string>>
                      {
                          new KeyValuePair<string, string>("Via1", "情報元１")
                      }
                };
                AreEquals(expConfig, ConfigManager.Config);
            }
            finally
            {
                // 他のテストのために設定を戻す。
                configManager.SetStaticFieldOrProperty("configPath", TEST_CONFIG_PATH);
            }
        }

        [TestMethod]
        public void TestSaveConfig()
        {
            var configManager = new PrivateType(typeof(ConfigManager));
            try
            {
                // テスト用に設定ファイルのパスを書換え（読込）
                string loadFilePath = @"..\..\TestData\loadtest.json";
                configManager.SetStaticFieldOrProperty("configPath", loadFilePath);
                ConfigManager.ReloadConfig();
                Console.WriteLine("reloaded");
                // テスト用に設定ファイルのパスを書換え（保存先）
                string saveFilePath = @"..\..\ResultData\save.json";
                configManager.SetStaticFieldOrProperty("configPath", saveFilePath);
                ConfigManager.SaveConfig();
                Console.WriteLine("saved");

                AssertFile(loadFilePath, saveFilePath);
            }
            finally
            {
                // 他のテストのために設定を戻す。
                configManager.SetStaticFieldOrProperty("configPath", TEST_CONFIG_PATH);
            }
        }


    }
}
