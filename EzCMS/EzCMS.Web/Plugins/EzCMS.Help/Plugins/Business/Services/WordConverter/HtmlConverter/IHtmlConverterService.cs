using DocumentFormat.OpenXml.Packaging;
using Ez.Framework.Core.IoC.Attributes;
using OpenXmlPowerTools;
using System;
using System.Xml.Linq;
using HtmlConverterSettings = EzCMS.Help.Plugins.Business.Models.WordConverter.HtmlConverterSettings;

namespace EzCMS.Help.Plugins.Business.Services.WordConverter.HtmlConverter
{
    [Register(Lifetime.PerInstance)]
    public interface IHtmlConverterService
    {
        /// <summary>
        /// Convert to html
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="htmlConverterSettings"></param>
        /// <returns></returns>
        XElement ConvertToHtml(WmlDocument doc, HtmlConverterSettings htmlConverterSettings);

        /// <summary>
        /// Convert to html
        /// </summary>
        /// <param name="wordDoc"></param>
        /// <param name="htmlConverterSettings"></param>
        /// <returns></returns>
        XElement ConvertToHtml(WordprocessingDocument wordDoc, HtmlConverterSettings htmlConverterSettings);

        /// <summary>
        /// Convert to html
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="htmlConverterSettings"></param>
        /// <param name="imageHandler"></param>
        /// <returns></returns>
        XElement ConvertToHtml(WmlDocument doc, HtmlConverterSettings htmlConverterSettings,
            Func<ImageInfo, XElement> imageHandler);


        /// <summary>
        /// Convert to html
        /// </summary>
        /// <param name="wordDoc"></param>
        /// <param name="htmlConverterSettings"></param>
        /// <param name="imageHandler"></param>
        /// <returns></returns>
        XElement ConvertToHtml(WordprocessingDocument wordDoc,
            HtmlConverterSettings htmlConverterSettings, Func<ImageInfo, XElement> imageHandler);
    }
}
