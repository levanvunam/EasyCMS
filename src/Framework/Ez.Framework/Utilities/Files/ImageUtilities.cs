using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Ez.Framework.Utilities.Files
{
    /// <summary>
    /// Image utilities
    /// </summary>
    public class ImageUtilities
    {
        public static void DeleteFile(string folder, string fileName)
        {
            folder = HttpContext.Current.Server.MapPath(folder);
            var path = folder + fileName;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static Bitmap ConvertImageFromBase64(string imageDataUri)
        {
            var base64Data = Regex.Match(imageDataUri, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var binData = Convert.FromBase64String(base64Data);
            using (var stream = new MemoryStream(binData))
            {
                var image = new Bitmap(stream);
                return image;
            }
        }

        public static void SaveImageFromBase64String(string url, string path, ImageFormat format)
        {
            var base64Data = Regex.Match(url, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var binData = Convert.FromBase64String(base64Data);
            using (var stream = new MemoryStream(binData))
            {
                var image = Image.FromStream(stream);
                image.Save(path, format);
            }
        }
    }
}
