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
        public List<KeyValuePair<String, String>> InformationSources { get; } = new List<KeyValuePair<string, string>>();
        public string CategoryPrifix { get; private set; } = "";
        public string CategoryPostfix { get; private set; } = "";
        public string ViaPrifix { get; private set; } = "";
        public string ViaPostfix { get; private set; } = "";
        public string OutFilePrifix { get; private set; } = "";
        public string OutFilePostfix { get; private set; } = "";
        public string WatchWord { get; private set; } = "";

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

            InformationSources.Clear();
            InformationSources.Add(new KeyValuePair<string, string>("", "自分で入力"));
            InformationSources.Add(new KeyValuePair<string, string>("はてなブックマーク", "はてなブックマーク"));

            SavePath = @"C:\Users\pyonko\Dropbox\";
            NewLine = "\n";
            CategoryPrifix = "**";
            ViaPrifix = "（via：";
            ViaPostfix = "）";
            OutFilePrifix = "news_";
            OutFilePostfix = ".txt";
            WatchWord = "***[";
        }
    }
}
