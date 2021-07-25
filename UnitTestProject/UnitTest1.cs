using System;
using System.Diagnostics;
using System.Windows;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RM.Friendly.WPFStandardControls;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {

        private Process process;
        private WindowsAppFriend app;
        private MainWindowTestDriver driver;

        [TestInitialize]
        public void Initialize()
        {
            this.process = Process.Start(@"D:\Develop\VisualStudio\PersonalNewsSiteSupportTool\PersonalNewsSiteSupportTool\bin\Debug\PersonalNewsSiteSupportTool.exe");
            this.app = new WindowsAppFriend(this.process);
            this.driver = new MainWindowTestDriver(this.app.Type<Application>().Current.MainWindow);
            app.Type("System.Windows.Application").Current.MainWindow.Show();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.app.Dispose();
            this.process.Kill();
        }

        [TestMethod]
        public void TestMethod1()
        {

            

            var button = this.driver.LogicalTree.ByType("System.Windows.Controls.Button");

            new WPFButtonBase(button.Single()).EmulateClick();
        }
    }

    public class MainWindowTestDriver
    {

        public WPFComboBox Category { get; }

        public WPFTextBox Text { get; }

        public WPFButtonBase CompleteButton { get; }

        public IWPFDependencyObjectCollection<DependencyObject> LogicalTree { get; }

        public MainWindowTestDriver(dynamic window)
        {
            var w = new WindowControl(window);

            this.LogicalTree = w.LogicalTree();

            

            this.Category = new WPFComboBox(this.LogicalTree.ByBinding("Category").Single());
            this.Text = new WPFTextBox(this.LogicalTree.ByType("System.Windows.Controls.TextBox").Single());
            this.CompleteButton = new WPFButtonBase(this.LogicalTree.ByType("System.Windows.Controls.Button").Single());


        }

    }
}
