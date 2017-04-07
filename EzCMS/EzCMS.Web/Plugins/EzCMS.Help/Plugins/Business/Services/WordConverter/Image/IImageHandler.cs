using System.Collections.Generic;
using System.Xml.Linq;
using OpenXmlPowerTools;

namespace EzCMS.Help.Plugins.Business.Services.WordConverter.Image
{
    public interface IImageHandler
    {
        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="imageInfo"></param>
        /// <returns></returns>
        XElement Handle(ImageInfo imageInfo);

        /// <summary>
        /// Get files
        /// </summary>
        Dictionary<string, byte[]> Files { get; }
    }
}
