using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PersonalNewsSiteSupportTool.Models
{
    public sealed class ConfigManager
    {

        public static Config Config { get; private set; }

        private static readonly string defaultConfigPath = PathConstant.APP_PATH + @"\config.json";

        private static readonly string configPath = PathConstant.APP_DATA_PATH + @"\config.json";

        private static readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
            WriteIndented = true
        };

        private ConfigManager()
        {
            
        }

        public static void ReloadConfig()
        {
            // 起動時のログ出力で必ずフォルダが作成されているのでフォルダの存在判定は行わない。
            if (!File.Exists(configPath))
            {
                File.Copy(defaultConfigPath, configPath);
            }
            string json = File.ReadAllText(configPath, System.Text.Encoding.UTF8);
            Config = JsonSerializer.Deserialize<Config>(json, options);
        }

        public static void SaveConfig()
        {
            var json = JsonSerializer.Serialize<Config>(Config, options);
            File.WriteAllText(configPath, json, System.Text.Encoding.UTF8);
        }

        public static Config GetCopyConfig()
        {
            string json = JsonSerializer.Serialize<Config>(Config, options);
            return JsonSerializer.Deserialize<Config>(json, options);
        }
    }
    public class Config
    {
        public string Version { get; set; } = "";
        public string WatchWord { get; set; } = "";
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
        
        public List<KeyValuePair<String, String>> InformationSources { get; set; }
    }
}
