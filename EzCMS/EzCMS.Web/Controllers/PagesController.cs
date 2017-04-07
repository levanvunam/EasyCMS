using Ez.Framework.Utilities;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Exceptions.Sites;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Mvc.ViewEngines.ViewResult;
using EzCMS.Core.Services.Pages;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Entity.Core.Enums;
using System;
using System.Net;
using System.Web.Mvc;

namespace EzCMS.Web.Controllers
{
    public class PagesController : ClientController
    {
        private readonly IPageService _pageService;
        private readonly ISiteSettingService _siteSettingService;

        public PagesController(IPageService pageService, ISiteSettingService siteSettingService)
        {
            _pageService = pageService;
            _siteSettingService = siteSettingService;
        }

        /// <summary>
        /// Get/set current page status code
        /// </summary>
        public int StatusCode
        {
            get
            {
                int statusCode;
                try
                {
                    statusCode = (int)TempData[EzCMSContants.CurrentPageStatusCode];
                }
                catch (Exception)
                {
                    statusCode = (int)HttpStatusCode.OK;
                }

                return statusCode;
            }
            set
            {
                TempData[EzCMSContants.CurrentPageStatusCode] = value;
            }
        }

        public ActionResult Index(string url, bool showDraft = false)
        {
            /*
             * check if url invalid or not found (file not found)
             * check if url contain '/' character at the end of url
             */
            if (!string.IsNullOrEmpty(url) && !url.ToUrlString().Equals(url) && !url.EndsWith("/"))
            {
                throw new EzCMSNotFoundException(System.Web.HttpContext.Current.Request.RawUrl);
            }

            var model = _pageService.RenderContent(url, showDraft);

            switch (model.ResponseCode)
            {
                case PageEnums.PageResponseCode.FileTemplateRedirect:
                    var routeValues = new
                    {
                        controller = model.FileTemplateModel.Controller,
                        action = model.FileTemplateModel.Action,
                        area = model.FileTemplateModel.Area
                    };
                    return new MVCTransferResult(routeValues, model.FileTemplateModel.Parameters);
                case PageEnums.PageResponseCode.RequiredSSL:
                    if (HttpContext.Request.Url != null)
                    {
                        var secureUrl = HttpContext.Request.Url.AbsoluteUri.Replace("http", "https");
                        return Redirect(secureUrl);
                    }
                    break;
                case PageEnums.PageResponseCode.Redirect301:
                    StatusCode = (int)HttpStatusCode.Moved;
                    var redirectUrl = string.Format("~/{0}", model.Redirect301Url).ToAbsoluteUrl();
                    return Redirect(redirectUrl);
            }

            switch (StatusCode)
            {
                case (int)HttpStatusCode.Moved:
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Moved;
                    HttpContext.Response.TrySkipIisCustomErrors = true;
                    break;
                case (int)HttpStatusCode.NotFound:

                    HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    HttpContext.Response.TrySkipIisCustomErrors = true;
                    break;
            }

            return View(model);
        }

        #region Methods

        [HttpPost]
        public JsonResult SetupPageTracking(int id)
        {
            return Json(_pageService.SetupPageTracking(id));
        }

        #endregion

        /// <summary>
        /// The style file for editor
        /// </summary>
        /// <returns></returns>
        public ActionResult EditorStyleFile()
        {
            var model = _siteSettingService.LoadSetting<EditorSetting>();

            var mime = System.Web.MimeMapping.GetMimeMapping(".css");
            return Content(model.StyleContent, mime);
        }

        /// <summary>
        /// The script file for editor
        /// </summary>
        /// <returns></returns>
        public ActionResult EditorScriptFile()
        {
            var model = _siteSettingService.LoadSetting<EditorSetting>();

            var mime = System.Web.MimeMapping.GetMimeMapping(".js");
            return Content(model.ScriptContent, mime);
        }
    }
}
