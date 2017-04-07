using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Linq;
using OpenXmlPowerTools;

namespace EzCMS.Help.Plugins.Business.Services.WordConverter.Image
{
    public class Base64ImageHandler : IImageHandler
    {
        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="imageInfo"></param>
        /// <returns></returns>
        public XElement Handle(ImageInfo imageInfo)
        {
            var extension = imageInfo.ContentType.Split('/')[1].ToLower();

            ImageFormat imageFormat;

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
                default:
                    imageFormat = ImageFormat.Jpeg;
                    break;
            }
            using (var stream = new MemoryStream())
            {
                imageInfo.Bitmap.Save(stream, imageFormat);
                string base64 = Convert.ToBase64String(stream.ToArray());
                var img = new XElement(Xhtml.img, new XAttribute(NoNamespace.src, "data:image/" + extension + ";base64," + base64),
                                   imageInfo.ImgStyleAttribute,
                                   imageInfo.AltText != null
                                       ? new XAttribute(NoNamespace.alt, imageInfo.AltText)
                                       : null);

                return img;
            }
        }

        /// <summary>
        /// Get files
        /// </summary>
        public Dictionary<string, byte[]> Files
        {
            get
            {
                return null;
            }
        }
    }
}