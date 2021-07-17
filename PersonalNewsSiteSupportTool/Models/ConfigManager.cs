using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PersonalNewsSiteSupportTool.Models
{
    public sealed class ConfigManager
    {

        public static Config config { get; private set; }

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
            string json = File.ReadAllText(@".\config.json", System.Text.Encoding.UTF8);
            config = JsonSerializer.Deserialize<Config>(json, options);
        }

        public static void SaveConfig()
        {
            var json = JsonSerializer.Serialize<Config>(config, options);
            File.WriteAllText(@".\config.json", json, System.Text.Encoding.UTF8);
        }

        public static Config getCopyConfig()
        {
            string json = JsonSerializer.Serialize<Config>(config, options);
            return JsonSerializer.Deserialize<Config>(json, options);
        }
    }
    public class Config
    {
        public string WatchWord { get; set; } = "";
        public string SavePath { get; set; } = "";
        public string OutFilePrefix { get; set; } = "";
        public string OutFileSuffix { get; set; } = "";
        public string NewLine { get; set; } = "\r\n";
        public string CategoryPrefix { get; set; } = "";
        public string CategorySuffix { get; set; } = "";
        public string ViaPrefix { get; set; } = "";
        public string ViaSuffix { get; set; } = "";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:コレクション プロパティは読み取り専用でなければなりません", Justification = "<保留中>")]
        public List<KeyValuePair<String, String>> Categories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:コレクション プロパティは読み取り専用でなければなりません", Justification = "<保留中>")]
        public List<KeyValuePair<String, String>> InformationSources { get; set; }
    }
}
