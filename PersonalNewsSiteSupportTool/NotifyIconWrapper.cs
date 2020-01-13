namespace PersonalNewsSiteSupportTool
{
    using PersonalNewsSiteSupportTool.Models;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Forms;
    using Application = System.Windows.Application;

    public partial class NotifyIconWrapper : Control
    {

        /// <summary>
        /// NotifyIconWrapper クラス を生成、初期化します。
        /// </summary>
        public NotifyIconWrapper()
        {
            // コンポーネントの初期化
            this.InitializeComponent();

            // コンテキストメニューのイベントを設定
            this.toolStripMenuItem_Exit.Click += this.toolStripMenuItem_Exit_Click;
        }

        /// <summary>
        /// コンテナ を指定して NotifyIconWrapper クラス を生成、初期化します。
        /// </summary>
        /// <param name="container">コンテナ</param>
        public NotifyIconWrapper(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container), "タスクトレイアイコンの初期化に失敗しました。");
            }
            container.Add(this);

            this.InitializeComponent();
        }

        /// <summary>
        /// コンテキストメニュー "終了" を選択したとき呼ばれます。
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            // 現在のアプリケーションを終了
            Application.Current.Shutdown();
        }

        private void toolStripMenuItem_Cat_Click(object sender, EventArgs e)
        {
            Config config = Config.GetInsrance();
            String savePath = config.SavePath;
            String newLine = config.NewLine;
            string readText = "";
            foreach(var kvp in config.Categories)
            {
                String fullPath = $"{savePath}news_{kvp.Key}.txt";
                if (File.Exists(fullPath))
                {
                    readText = $"{readText}**{kvp.Value}{newLine}{newLine}{File.ReadAllText(fullPath)}{newLine}";
                }
            }
            File.AppendAllText($"{savePath}news.txt", readText);
        }
    }
}
