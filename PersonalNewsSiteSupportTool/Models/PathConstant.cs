using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalNewsSiteSupportTool.Models
{
    public static class PathConstant
    {
        public static readonly string APP_DATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            + @"\PersonalNewsSiteSupportTool";

        public static readonly string APP_PATH = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
    }
}
