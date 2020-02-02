using System;
using System.Collections.Generic;

namespace PersonalNewsSiteSupportTool.Models
{
    public sealed class Config
    {

        private static Config instance = new Config();
        public static Config GetInsrance() => instance;

        public string SavePath { get; private set; } = "";
        public string NewLine { get; private set; } = "\r\n";
        public List<KeyValuePair<String, String>> Categories { get; } = new List<KeyValuePair<string, string>>();
        public string CategoryPrifix { get; private set; } = "";

        private Config()
        {
            ReloadConfig();
        }

        public void ReloadConfig()
        {
            // 外部設定化する。
            Categories.Clear();
            Categories.Add(new KeyValuePair<string, string>("IT", "IT"));
            Categories.Add(new KeyValuePair<string, string>("その他", "その他"));
            Categories.Add(new KeyValuePair<string, string>("old", "新しくないけど気になったもの"));

            SavePath = @"C:\Users\pyonko\Dropbox\";
            NewLine = "\n";
            CategoryPrifix = "**";
        }
    }
}
