﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PersonalNewsSiteSupportTool.Models
{
    /// <summary>
    /// 設定管理クラス
    /// </summary>
    public static class ConfigManager
    {
        /// <summary>
        /// アプリの現在の設定を保持する。
        /// </summary>
        public static Config Config { get; private set; }

        /// <summary>
        /// デフォルトの設定ファイルパス。
        /// </summary>
        private static readonly string defaultConfigPath = PathManager.GetAppPath() + @"\default.json";

        /// <summary>
        /// ユーザの設定ファイルパス。
        /// </summary>
        private static readonly string configPath = PathManager.AppDataPath + @"\config.json";

        /// <summary>
        /// 設定ファイルを読書きする際のオプション。
        /// </summary>
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
            WriteIndented = true
        };

        /// <summary>
        /// 設定読込メソッド。
        /// </summary>
        /// <returns>初期設定要フラグ</returns>
        public static bool ReloadConfig()
        {
            bool ret = false;
            // 起動時のログ出力で必ずフォルダが作成されているのでフォルダの存在判定は行わない。
            if (!File.Exists(configPath))
            {
                File.Copy(defaultConfigPath, configPath);
                ret = true;
            }
            string json = File.ReadAllText(configPath, System.Text.Encoding.UTF8);
            Config = JsonSerializer.Deserialize<Config>(json, options);

            return ret;
        }

        /// <summary>
        /// 設定保存メソッド。
        /// </summary>
        public static void SaveConfig()
        {
            var json = JsonSerializer.Serialize<Config>(Config, options);
            File.WriteAllText(configPath, json, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 設定クラスのコピーを取得。
        /// </summary>
        /// <returns></returns>
        public static Config GetCopyConfig()
        {
            string json = JsonSerializer.Serialize<Config>(Config, options);
            return JsonSerializer.Deserialize<Config>(json, options);
        }
    }
    /// <summary>
    /// 設定クラス。
    /// </summary>
    public class Config
    {
        public string Version { get; set; } = "";
        public string WatchWord { get; set; } = "";
        public bool IsRemoveWatchWord { get; set; } = true;
        public string SavePath { get; set; } = "";
        public string OutFilePrefix { get; set; } = "";
        public string OutFileSuffix { get; set; } = "";
        public string MergeFileName { get; set; } = "";
        public string NewLine { get; set; } = "\r\n";
        public string CategoryPrefix { get; set; } = "";
        public string CategorySuffix { get; set; } = "";
        public string ViaPrefix { get; set; } = "";
        public string ViaSuffix { get; set; } = "";

        public List<KeyValuePair<String, String>> Categories { get; set; }
        
        public List<KeyValuePair<String, String>> ViaList { get; set; }
    }
}
