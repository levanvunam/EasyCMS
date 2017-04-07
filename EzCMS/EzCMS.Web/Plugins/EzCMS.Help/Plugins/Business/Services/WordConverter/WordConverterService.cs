using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Models.WordConverters;
using EzCMS.Help.Plugins.Business.Models.WordConverter;
using EzCMS.Help.Plugins.Business.Services.WordConverter.HtmlConverter;
using EzCMS.Help.Plugins.Business.Services.WordConverter.Image;
using EzCMS.Help.Plugins.Mvc.Extensions;
using NotesFor.HtmlToOpenXml;
using System.Collections.Generic;
using System.IO;

namespace EzCMS.Help.Plugins.Business.Services.WordConverter
{
    public class WordConverterService : IWordConverterService
    {
        /// <summary>
        /// Convert word to html
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mediaPath"></param>
        /// <returns></returns>
        public HtmlResult ToHtml(byte[] input, string mediaPath)
        {
            var result = new HtmlResult();
            var imageHandler = mediaPath == null ? new Base64ImageHandler() : new FileImageHandler(mediaPath) as IImageHandler;
            using (var stream = new MemoryStream(input))
            {
                using (var doc = WordprocessingDocument.Open(stream, true))
                {
                    var settings = new HtmlConverterSettings
                    {
                        PageTitle = ""
                    };
                    var htmlConverterService = HostContainer.GetInstance<IHtmlConverterService>();
                    var html = htmlConverterService.ConvertToHtml(doc, settings, imageHandler.Handle);
                    result.Html = html.ToStringNewLineOnAttributes();
                }
            }
            result.Files = imageHandler.Files;
            return result;
        }

        /// <summary>
        /// Convert html to word
        /// </summary>
        /// <param name="content"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public byte[] ToDocx(string content, Dictionary<string, byte[]> files)
        {
            using (var generatedDocument = new MemoryStream())
            {
                using (var package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document))
                {
                    var mainPart = package.MainDocumentPart;
                    if (mainPart == null)
                    {
                        mainPart = package.AddMainDocumentPart();
                        new Document(new Body()).Save(mainPart);
                    }

                    var converter = new NotesFor.HtmlToOpenXml.HtmlConverter(mainPart);

                    var imageProvider = new MemoryImageProvider(files);
                    converter.ImageProcessing = ImageProcessing.ManualProvisioning;
                    converter.ProvisionImage += imageProvider.Get;

                    var body = mainPart.Document.Body;

                    var paragraphs = converter.Parse(content);
                    for (int i = 0; i < paragraphs.Count; i++)
                    {
                        body.Append(paragraphs[i]);
                    }

                    mainPart.Document.Save();
                }
                return generatedDocument.ToArray();
            }
        }

    }
}
