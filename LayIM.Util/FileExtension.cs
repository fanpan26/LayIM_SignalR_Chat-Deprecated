using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayIM.Util
{
    public class FileExtension
    {
        public static readonly string[] Images = { "png", "jpg", "jpeg","gif" };
        public static readonly string[] Files = { "ppt", "pptx", "doc", "docx", "xlsx", "txt", "rar", "zip" };
        public static bool isImage(string ext)
        {
            if (ext == null) { return false; }
            return Images.Contains(ext.Substring(1, ext.Length - 1).ToLowerInvariant());
        }

        public static bool isFile(string ext)
        {
            if (ext == null) { return false; }
            return Files.Contains(ext.ToLowerInvariant());
        }
    }
}
