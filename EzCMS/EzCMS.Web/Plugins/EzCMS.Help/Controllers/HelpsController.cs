using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Logging;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Models.BodyTemplates.HelpServices;
using EzCMS.Core.Models.SlideInHelps.HelpServices;
using EzCMS.Core.Models.WordConverters;
using EzCMS.Help.Plugins.Business.Models.WordConverter;
using EzCMS.Help.Plugins.Business.Services.Helps;
using EzCMS.Help.Plugins.Business.Services.WordConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace EzCMS.Help.Controllers
{
    [WebApiAuthentication]
    public class HelpsController : ApiController
    {
        private readonly IHelpService _helpService;

        public HelpsController(IHelpService helpService)
        {
            _helpService = helpService;
        }

        #region Slide In Helps

        [HttpGet]
        public List<SlideInHelpResponseModel> LoadSlideInHelps(string lastUpdatingTime)
        {
            DateTime lastUpdating;

            if (DateTime.TryParse(lastUpdatingTime, out lastUpdating))
            {
                return _helpService.GetSlideInHelps(lastUpdating);
            }
            return _helpService.GetSlideInHelps(null);
        }

        #endregion

        #region Body Templates

        [HttpGet]
        public BodyTemplateResponseModel SearchBodyTemplates(string keyword, int? pageIndex, int? pageSize)
        {
            return _helpService.GetBodyTemplates(keyword, pageIndex, pageSize);
        }

        [HttpGet]
        public BodyTemplateDownloadModel DownloadBodyTemplate(int id)
        {
            return _helpService.DownloadBodyTemplate(id);
        }

        #endregion

        #region Word Converter

        [HttpPost]
        public HtmlResult WordToHtml(WordToHtmlRequest model)
        {
            try
            {
                var wordConverterService = HostContainer.GetInstance<IWordConverterService>();
                using (var stream = new MemoryStream(model.Docx))
                {
                    var result = wordConverterService.ToHtml(stream.ToArray(), model.MediaPath);
                    result.Html = HttpUtility.HtmlEncode(result.Html);
                    return result;
                }
            }
            catch (Exception exception)
            {
                var logger = HostContainer.GetInstance<ILogger>();
                logger.Error(exception);
                throw;
            }
        }

        [HttpPost]
        public HttpResponseMessage HtmlToWord(HtmlToWordRequest model)
        {
            var wordConverterService = HostContainer.GetInstance<IWordConverterService>();
            var data = wordConverterService.ToDocx(model.Html, model.Files);
            var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(data) };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }

        #endregion
    }
}