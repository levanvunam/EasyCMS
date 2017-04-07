using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Linq;
using OpenXmlPowerTools;

namespace EzCMS.Help.Plugins.Business.Services.WordConverter.Image
{
    public class FileImageHandler : IImageHandler
    {
        private readonly Dictionary<string, byte[]> _files = new Dictionary<string, byte[]>();
        private readonly string _mediaPath;

        public FileImageHandler(string mediaPath)
        {
            _mediaPath = mediaPath;
        }

        public XElement Handle(ImageInfo imageInfo)
        {
            var extension = imageInfo.ContentType.Split('/')[1].ToLower();

            ImageFormat imageFormat = null;

            switch (extension)
            {
                case "png":
                    extension = "jpeg";
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case "bmp":
                    imageFormat = ImageFormat.Bmp;
                    break;
                case "jpeg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case "tiff":
                    imageFormat = ImageFormat.Tiff;
                    break;
            }

            if (imageFormat == null)
                return null;
            var filename = string.Format("{0}.{1}", Path.GetRandomFileName(), extension);
            var imagePath = string.Format("{0}{1}", _mediaPath, filename);

            try
            {
                using (var stream = new MemoryStream())
                {
                    imageInfo.Bitmap.Save(stream, imageFormat);
                    _files[filename] = stream.ToArray();
                }
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
                return null;
            }

            var img = new XElement(Xhtml.img, new XAttribute(NoNamespace.src, imagePath),
                                   imageInfo.ImgStyleAttribute,
                                   imageInfo.AltText != null
                                       ? new XAttribute(NoNamespace.alt, imageInfo.AltText)
                                       : null);

            return img;
        }

        /// <summary>
        /// Get files
        /// </summary>
        public Dictionary<string, byte[]> Files
        {
            get
            {
                return _files;
            }
        }
    }

}