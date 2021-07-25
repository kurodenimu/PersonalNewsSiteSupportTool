using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PersonalNewsSiteSupportTool.Models
{
    static class CommonUtil
    {
        private static readonly Regex newLineRegex = new Regex("\r\n|\r|\n");

        public static string GetOutText(string newsUrl, string via, String newsComment, Config config = null)
        {
            if (config == null)
            {
                config = ConfigManager.Config;
            }
            string newLine = config.NewLine;

            String viaText = "";
            if (via != null & !"".Equals(via, StringComparison.Ordinal))
            {
                viaText = $"{config.ViaPrefix}{via}{config.ViaSuffix}{newLine}";
            }

            return $"{newsUrl}{newLine}{viaText}{newLineRegex.Replace(newsComment, newLine)}{newLine}{newLine}";
        }
    }
}
