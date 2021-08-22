using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using NLog.Targets;
using PersonalNewsSiteSupportTool.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestProject
{
    /// <summary>
    /// UnitTest2 の概要の説明
    /// </summary>
    [TestClass]
    public class UnitTest2 : TestBase
    {
        public UnitTest2()
        {
            //
            // TODO: コンストラクター ロジックをここに追加します
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ClassInit(testContext);
        }
        //
        // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            ClassClean();
        }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            // Console.WriteLine(Environment.CurrentDirectory);
            // Console.WriteLine(PathManager.GetAppDataPath());
            List<string> exp = new List<string>
            {
                "test1"
            };
            var act = new List<string>
            {
                "test2"
            };

            AreEquals(exp, act);
        }
    }
}
