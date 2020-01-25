using System;
using System.Collections.Generic;

namespace PersonalNewsSiteSupportTool.Models
{
    public sealed class Config
    {

        private static Config instance = new Config();

        private string savePath = "";

        private string newLine = "\r\n";

        private readonly List<KeyValuePair<String, String>> categories = new List<KeyValuePair<string, string>>();

        public string SavePath { get => savePath; }
        public string NewLine { get => newLine; }
        public List<KeyValuePair<String, String>> Categories => categories;
        public static Config GetInsrance() => instance;

        private Config()
        {
            ReloadConfig();
        }

        public void ReloadConfig()
        {
            // 外部設定化する。
            categories.Clear();
            categories.Add(new KeyValuePair<string, string>("IT", "IT"));
            categories.Add(new KeyValuePair<string, string>("その他", "その他"));
            categories.Add(new KeyValuePair<string, string>("old", "新しくないけど気になったもの"));

            savePath = @"C:\Users\pyonko\Dropbox\";
            newLine = "\n";
        }
    }
}
