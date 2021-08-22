using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersonalNewsSiteSupportTool.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnitTestProject
{
    [TestClass]
    public class TestBase
    {

        protected const string TEST_CONFIG_PATH = @"..\..\TestData\comfig.json";

        protected static void ClassInit()
        {
            // アプリケーションデータパスを書換え
            var pathManager = new PrivateType(typeof(PathManager));
            pathManager.SetStaticFieldOrProperty("AppDataPath", Environment.CurrentDirectory);
            // 設定ファイルのパスを書換え
            var configManager = new PrivateType(typeof(ConfigManager));
            configManager.SetStaticFieldOrProperty("configPath", TEST_CONFIG_PATH);
            // 初期化処理
            LogService.Init();
        }

        protected static void ClassClean()
        {
            LogService.Final();
        }

        /// <summary>
        /// 設定ファイルを書き換える。
        /// </summary>
        /// <param name="config">書き換える内容</param>
        protected static void SetConfig(Config config)
        {
            var privateType = new PrivateType(typeof(ConfigManager));
            privateType.SetStaticFieldOrProperty("Config", config);
        }

        protected static void AreEquals(Object expected, Object actual, string msg = "")
        {
            AreEquals(expected, actual, msg, "\nexp：\n" + expected.ToString() + "\nact：\n" + actual.ToString());
        }

        private static void AreEquals(Object expected, Object actual, string msg, string objStrings)
        {
            var type = expected.GetType();
            if (!type.Equals(actual.GetType()))
            {
                Assert.Fail("型が異なります。想定している型は" + expected.GetType().FullName +
                    "ですが実際は" + actual.GetType().FullName + "です。" + objStrings);
            }
            if (type.GetInterfaces().Any(t => t.IsConstructedGenericType &&
                          t.GetGenericTypeDefinition() == typeof(IEquatable<>)))
            {
                expected.Equals(actual);
            }
            else if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>)) {
                    var expList = (IList)expected;
                    var actList = (IList)actual;
                    if (expList.Count != actList.Count)
                    {
                        Assert.Fail(msg + "リストのサイズが異なります。想定している数は" + expList.Count +
                            "ですが、実際は" + actList.Count + "です。" + objStrings);
                    }
                    for (int i = 0; i < expList.Count; i++)
                    {
                        AreEquals(expList[i], actList[i], msg, objStrings);
                    }
                }
                else if (type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                {
                    var keyProp = type.GetProperty("Key");
                    var valProp = type.GetProperty("Value");
                    AreEquals(keyProp.GetValue(expected), keyProp.GetValue(expected), msg, objStrings);
                    AreEquals(valProp.GetValue(expected), valProp.GetValue(expected), msg, objStrings);
                }
                else
                {
                    Assert.Fail("本メソッドで検査できない型です。実装を追加してください。" + type.FullName);
                }
            }
            else
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach(var field in fields)
                {
                    AreEquals(field.GetValue(expected), field.GetValue(actual), msg, objStrings);
                }
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach(var prop in properties)
                {
                    AreEquals(prop.GetValue(expected), prop.GetValue(actual), msg, objStrings);
                }
            }
        }

        protected static void AssertFile(string expFilePath, string actFilePath, string msg = "")
        {
            CollectionAssert.AreEqual(System.IO.File.ReadAllBytes(expFilePath), System.IO.File.ReadAllBytes(actFilePath), msg);
        }
    }
}
