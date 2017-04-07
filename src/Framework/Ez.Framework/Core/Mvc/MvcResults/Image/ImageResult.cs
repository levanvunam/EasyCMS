using Simple.ImageResizer;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Ez.Framework.Core.Mvc.MvcResults.Image
{
    /// <summary>
    /// MVC image result
    /// </summary>
    public class ImageResult : FilePathResult
    {
        private readonly string _filePath;
        private readonly int _width;
        private readonly int _height;

        public ImageResult(string filePath, int width = 0, int height = 0) :
            base(filePath, string.Format("image/{0}",
                filePath.FileExtensionForContentType()))
        {
            _filePath = filePath;
            _width = width;
            _height = height;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            string resizedFilePath = GetResizedImagePath(_filePath, _width, _height);
            response.SetDefaultImageHeaders(resizedFilePath);
            WriteFileToResponse(resizedFilePath, response);
        }

        private static void WriteFileToResponse(string filePath, HttpResponseBase response)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                const int bufferLength = 65536;
                var buffer = new byte[bufferLength];

                while (true)
                {
                    int bytesRead = fs.Read(buffer, 0, bufferLength);

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    response.OutputStream.Write(buffer, 0, bytesRead);
                }
            }
        }

        private static string GetResizedImagePath(string filepath, int width, int height)
        {
            string resizedPath = filepath;

            if (width > 0 || height > 0)
            {
                resizedPath = filepath.GetPathForResizedImage(width, height);

                if (!Directory.Exists(resizedPath))
                    Directory.CreateDirectory(new FileInfo(resizedPath).DirectoryName);

                var originalFile = new FileInfo(filepath);
                var resizedFile = new FileInfo(resizedPath);
                if (originalFile.LastWriteTimeUtc > resizedFile.LastWriteTimeUtc)
                {
                    File.Delete(resizedPath);
                }

                if (!File.Exists(resizedPath))
                {
                    var imageResizer = new ImageResizer(filepath);
                    if (width > 0 && height > 0)
                    {
                        imageResizer.Resize(width, height, ImageEncoding.Jpg90);
                    }
                    else if (width > 0)
                    {
                        imageResizer.Resize(width, ImageEncoding.Jpg90);
                    }
                    imageResizer.SaveToFile(resizedPath);
                    imageResizer.Dispose();
                }
            }
            return resizedPath;
        }
    }
}