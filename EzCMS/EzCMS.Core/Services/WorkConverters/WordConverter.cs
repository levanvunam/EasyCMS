using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Routing;
using CsQuery;
using Ez.Framework.Core.IoC;
using Ez.Framework.Utilities.Web;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Models.WordConverters;
using EzCMS.Core.Services.SiteSettings;

namespace EzCMS.Core.Services.WorkConverters
{
    public class WordConverter : IWordConverter
    {
        private readonly ISiteSettingService _siteSettingService;

        public WordConverter()
        {
            _siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
        }

        /// <summary>
        /// Convert to html
        /// </summary>
        /// <param name="serverMediaFolder"></param>
        /// <param name="imageUrlPrefix"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ToHtml(string serverMediaFolder, string imageUrlPrefix, Stream input)
        {
            using (var stream = new MemoryStream())
            {
                imageUrlPrefix = imageUrlPrefix.Replace("\\", "/");
                input.CopyTo(stream);
                var data = stream.ToArray();
                var helpServiceSetting = _siteSettingService.LoadSetting<EzCMSHelpConfigurationSetting>();
                var result = WebUtilities.SendApiRequest<HtmlResult>(
                    helpServiceSetting.HelpServiceUrl, helpServiceSetting.AuthorizeCode,
                    "api/Helps/WordToHtml",
                    HttpMethod.Post, new RouteValueDictionary(new
                    {
                        docx = data,
                        mediaPath = imageUrlPrefix
                    }));

                result.Html = HttpUtility.HtmlDecode(result.Html);
                if (!Directory.Exists(serverMediaFolder))
                {
                    Directory.CreateDirectory(serverMediaFolder);
                }
                if (result.Files != null)
                {
                    foreach (var file in result.Files)
                    {
                        File.WriteAllBytes(Path.Combine(serverMediaFolder, file.Key), file.Value);
                    }
                }

                return result.Html;
            }
        }

        /// <summary>
        /// Convert to docx
        /// </summary>
        /// <param name="serverMediaFolder"></param>
        /// <param name="imageUrlPrefix"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public byte[] ToDocx(string serverMediaFolder, string imageUrlPrefix, string content)
        {
            if (!Directory.Exists(serverMediaFolder))
            {
                Directory.CreateDirectory(serverMediaFolder);
            }
            var baseUrl = "";
            if (HttpContext.Current != null)
            {
                var url = HttpContext.Current.Request.Url;
                baseUrl = url.IsDefaultPort
                    ? string.Format("{0}://{1}", url.Scheme, url.Host)
                    : string.Format("{0}://{1}:{2}", url.Scheme, url.Host, url.Port);
            }
            var images = new Dictionary<string, byte[]>();
            CQ dom = content;
            var imgs = dom["img"];
            foreach (var img in imgs)
            {
                string src;
                if (img.TryGetAttribute("src", out src))
                {
                    if (src.StartsWith(imageUrlPrefix))
                    {
                        var imagePath = Path.Combine(serverMediaFolder, src.Substring(imageUrlPrefix.Length));
                        if (File.Exists(imagePath))
                        {
                            images[src] = File.ReadAllBytes(imagePath);
                        }
                    }
                    else if (src.StartsWith("/"))
                    {
                        img.SetAttribute("src", baseUrl + src);
                    }
                }
            }

            var helpServiceSetting = _siteSettingService.LoadSetting<EzCMSHelpConfigurationSetting>();
            var responseMessage = WebUtilities.SendApiRequest<byte[]>(
                helpServiceSetting.HelpServiceUrl, helpServiceSetting.AuthorizeCode,
                "api/Helps/HtmlToWord",
                HttpMethod.Post, new RouteValueDictionary(new
                {
                    Html = content,
                    Files = images
                }));
            return responseMessage;
        }

        /// <summary>
        /// Convert to body
        /// </summary>
        /// <param name="serverMediaFolder"></param>
        /// <param name="imageUrlPrefix"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ToBody(string serverMediaFolder, string imageUrlPrefix, Stream input)
        {
            var page = ToHtml(serverMediaFolder, imageUrlPrefix, input);
            var begin = page.IndexOf("<body>", StringComparison.Ordinal);
            if (begin < 0)
            {
                return null;
            }
            begin += "<body>".Length;
            var end = page.IndexOf("</body>", begin, StringComparison.Ordinal);
            if (end < 0)
            {
                return null;
            }
            return page.Substring(begin, end - begin);
        }

        /// <summary>
        /// Convert docx to html by service
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string ConvertDocxToHtmlByService(int pageId, byte[] content)
        {
            var attachmentPath = _siteSettingService.GetSetting<string>(SettingNames.WordDocUploadsFolderPhysicalPath);
            var serverMediaFolder =
                HttpContext.Current.Server.MapPath(Path.Combine(attachmentPath,
                    pageId.ToString(CultureInfo.InvariantCulture)));
            var pageIdMediaRelativePath = Path.Combine(attachmentPath, pageId.ToString(CultureInfo.InvariantCulture)) +
                                          "/";

            return ToHtml(serverMediaFolder, pageIdMediaRelativePath, new MemoryStream(content));
        }

        /// <summary>
        /// Convert html to docx by service
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        public byte[] ConvertHtmlToDocxByService(int pageId, string html)
        {
            var attachmentPath = _siteSettingService.GetSetting<string>(SettingNames.WordDocUploadsFolderPhysicalPath);
            var serverMediaFolder =
                HttpContext.Current.Server.MapPath(Path.Combine(attachmentPath,
                    pageId.ToString(CultureInfo.InvariantCulture)));

            var pageIdMediaRelativePath = Path.Combine(attachmentPath, pageId.ToString(CultureInfo.InvariantCulture)) +
                                          "/";

            return ToDocx(serverMediaFolder, pageIdMediaRelativePath, html);
        }
    }
}