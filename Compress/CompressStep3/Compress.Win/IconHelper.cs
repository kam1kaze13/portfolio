using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Win
{
    public class IconHelper
    {
        public static Icon GetIcon(string path)
        {
            string filePath = Path.GetTempPath() + path;

            var fileStream = File.Create(filePath);

            var icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);

            fileStream.Close();

            return icon;
        }
    }
}
