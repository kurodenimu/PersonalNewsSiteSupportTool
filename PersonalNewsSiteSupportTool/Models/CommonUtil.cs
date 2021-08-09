using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PersonalNewsSiteSupportTool.Models
{
    /// <summary>
    /// 共通ユーティリティ。メソッドが増えるときはクラスの分割を検討すること。
    /// </summary>
    static class CommonUtil
    {
        /// <summary>
        /// 改行コードの正規表現オブジェクト
        /// </summary>
        private static readonly Regex newLineRegex = new Regex("\r\n|\r|\n");

        /// <summary>
        /// ファイル出力用のテキストを取得する。
        /// </summary>
        /// <param name="newsUrl">ニュースのURLを含むテキスト</param>
        /// <param name="via">情報元のテキスト</param>
        /// <param name="newsComment">ニュースへのコメント</param>
        /// <param name="config">設定クラス。省略した場合、現在の設定を使用する。</param>
        /// <returns></returns>
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

        /// <summary>
        /// ファイルに使用できない文字列。メッセージ等表示用。
        /// </summary>
        public const string CANNOT_USED_FILE_NAME = @"\/:*?""<>|";
        /// <summary>
        /// 引数のテキストにファイルに使用できない文字列を使用しているかチェックする。
        /// </summary>
        /// <param name="target">チェック対象文字列</param>
        /// <returns>チェック結果。ファイルに使用できない文字が含まれる場合にFalse、含まれない場合にTrueを返却する。</returns>
        public static bool ValidateFileName(string target)
        {
            return !Regex.IsMatch(target, @"[\\/:*?""<>|]");
        }
    }
}
