using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PersonalNewsSiteSupportTool.Models
{
    public sealed class ConfigManager
    {

        public static Config Config { get; private set; }

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
            Config = JsonSerializer.Deserialize<Config>(json, options);
        }

        public static void SaveConfig()
        {
            var json = JsonSerializer.Serialize<Config>(Config, options);
            File.WriteAllText(@".\config.json", json, System.Text.Encoding.UTF8);
        }

        public static Config GetCopyConfig()
        {
            string json = JsonSerializer.Serialize<Config>(Config, options);
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

        public List<KeyValuePair<String, String>> Categories { get; set; }
        
        public List<KeyValuePair<String, String>> InformationSources { get; set; }
    }
}
