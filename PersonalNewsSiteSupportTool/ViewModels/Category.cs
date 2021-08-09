using System;

namespace PersonalNewsSiteSupportTool.ViewModels
{
    /// <summary>
    /// カテゴリクラス
    /// </summary>
    public class Category
    {
        /// <summary>
        /// カテゴリの表示名
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// カテゴリのID。出力ファイルに使用する。
        /// </summary>
        public String Id { get; set; }
    }
}
