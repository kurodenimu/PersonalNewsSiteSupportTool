using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersonalNewsSiteSupportTool.Models;

namespace UnitTestProject
{
    [TestClass]
    public class CommonUtilTest : TestBase
    {
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ClassInit();
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            ClassClean();
        }
        [TestMethod]
        public void TestGetOutText()
        {
            // テスト用にConfig設定
            var config = new Config
            {
                NewLine = "\n",
                ViaPrefix = "（via：",
                ViaSuffix = "）"
            };
            SetConfig(config);

            // ファイルから読み込んだ設定で呼び出すケース
            Assert.AreEqual("url\n（via：via）\ncomment\n\n", CommonUtil.GetOutText("url", "via", "comment"));

            // 設定ウィンドウから変更した設定で呼び出すケース
            var config2 = ConfigManager.GetCopyConfig();
            config2.NewLine = "\r\n";
            Assert.AreEqual("url\r\ncomment\r\n\r\n", CommonUtil.GetOutText("url", null, "comment", config2));
            // viaが空文字のケース
            Assert.AreEqual("url\r\ncomment\r\n\r\n", CommonUtil.GetOutText("url", "", "comment", config2));
        }
        [TestMethod]
        public void TestValidateFileName()
        {
            // 正常ケースとファイルに使用できない各文字でFalseが返ってくることの確認
            Assert.IsTrue(CommonUtil.ValidateFileName("testok"));
            Assert.IsFalse(CommonUtil.ValidateFileName("\\"));
            Assert.IsFalse(CommonUtil.ValidateFileName("/"));
            Assert.IsFalse(CommonUtil.ValidateFileName(":"));
            Assert.IsFalse(CommonUtil.ValidateFileName("*"));
            Assert.IsFalse(CommonUtil.ValidateFileName("?"));
            Assert.IsFalse(CommonUtil.ValidateFileName("\""));
            Assert.IsFalse(CommonUtil.ValidateFileName("<"));
            Assert.IsFalse(CommonUtil.ValidateFileName(">"));
            Assert.IsFalse(CommonUtil.ValidateFileName("|"));
        }
    }
}
