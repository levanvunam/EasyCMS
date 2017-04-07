using Ez.Framework.Core.Enums;
using Ez.Framework.Core.Resources;
using Ez.Framework.IoC;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.SiteSettings;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Services.Widgets;
using RazorEngine.Templating;
using System;
using System.Web;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Framework.Mvc.ViewEngines.Razor
{
    public abstract class WebViewPage : WebViewPage<object>
    {

    }
    public class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private IWidgetService _widgetService;
        private WebViewPageBase _webViewPageBase;
        protected ResourceScriptManager Script { get; private set; }
        protected ResourceStyleManager Style { get; private set; }

        public override void InitHelpers()
        {
            base.InitHelpers();

            _webViewPageBase = new WebViewPageBase();
            _widgetService = HostContainer.GetInstance<IWidgetService>();
            Script = new ResourceScriptManager(this, Html);
            Style = new ResourceStyleManager(this, Html);
        }

        public override void Execute()
        {
        }

        #region Multi Languages Helpers

        /// <summary>
        /// Get translated value by key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string TFormat(string key, params object[] parameters)
        {
            return _webViewPageBase.TFormat(key, parameters);
        }

        /// <summary>
        /// Get translated value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string T(string key)
        {
            return _webViewPageBase.T(key);
        }

        /// <summary>
        /// Get translated value by key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string T(string key, string defaultValue)
        {
            return _webViewPageBase.T(key, defaultValue);
        }

        /// <summary>
        /// Render a tag with text in multi language
        /// </summary>
        /// <param name="htmlTagName">Tag name to render</param>
        /// <param name="textKey">Key of the text</param>
        /// <returns>Rendered Tag</returns>
        public IHtmlString MText(HtmlTag htmlTagName, string textKey, object htmlAttributes = null)
        {
            return _webViewPageBase.MText(htmlTagName, textKey, null, htmlAttributes);
        }

        /// <summary>
        /// Render a tag with text in multi language
        /// </summary>
        /// <param name="htmlTagName">Tag name to render</param>
        /// <param name="textKey">Key of the text</param>
        /// <param name="defaultValue">Default value of the text</param>
        /// <param name="htmlAttributes"></param>
        /// <returns>Rendered Tag</returns>
        public IHtmlString MText(HtmlTag htmlTagName, string textKey, string defaultValue, object htmlAttributes = null)
        {
            return _webViewPageBase.MText(htmlTagName, textKey, defaultValue, htmlAttributes);
        }

        /// <summary>
        /// Render a tag with text in multi language
        /// </summary>
        /// <param name="htmlTagName">Tag name to render</param>
        /// <param name="textKey">Key of the text</param>
        /// <param name="defaultValue">Default value of the text</param>
        /// <param name="htmlAttributes">Html attributes for the tag</param>
        /// <param name="parameters">Parameters for the string</param>
        /// <returns>Rendered Tag</returns>
        public IHtmlString MText(HtmlTag htmlTagName,
            string textKey,
            string defaultValue,
            object htmlAttributes,
            object[] parameters)
        {
            return _webViewPageBase.MText(htmlTagName, textKey, defaultValue, htmlAttributes, parameters);
        }
        #endregion

        #region Setting Helpers

        /// <summary>
        /// Load setting by key and parse to type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T SValue<T>(string key)
        {
            return _webViewPageBase.SValue<T>(key);
        }

        /// <summary>
        /// Load complex setting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SValue<T>() where T : ComplexSetting
        {
            return _webViewPageBase.SValue<T>();
        }

        #endregion

        #region Message Helpers

        /// <summary>
        /// Show status message after page refresh
        /// </summary>
        /// <returns></returns>
        public MvcHtmlString ShowStatusMessage()
        {
            return _webViewPageBase.ShowStatusMessage();
        }

        #endregion

        #region Image Helpers

        public string Thumbnail(string filePath, int width = 0, int height = 0)
        {
            return Html.Raw(Url.Action("Thumbnail", "Media", new { area = "Admin", path = filePath, w = width, h = height })).ToString();
        }
        #endregion

        #region Widget Render

        /// <summary>
        /// Render widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public HtmlString RenderWidget(string widget)
        {
            return new HtmlString(_widgetService.Render(widget));
        }
        #endregion

        #region TimeZone & Culture Helpers

        /// <summary>
        /// Get current timezone of user
        /// </summary>
        public TimeZoneInfo CurrentTimeZone
        {
            get
            {
                return _webViewPageBase.CurrentTimeZone;
            }
        }

        /// <summary>
        /// Get label for string
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public string LabelForTime(string label)
        {
            return _webViewPageBase.LabelForTime(label);
        }

        /// <summary>
        /// Convert current date time to local date time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DateTime? ToLocalDateTime(DateTime? dateTime)
        {
            return _webViewPageBase.ToLocalDateTime(dateTime);
        }

        /// <summary>
        /// Convert current date time to local date time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public TimeSpan? ToLocalTime(TimeSpan? time)
        {
            return _webViewPageBase.ToLocalTime(time);
        }

        /// <summary>
        /// Convert local date time to UTC
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DateTime? ToUTCDateTime(DateTime? dateTime)
        {
            return _webViewPageBase.ToUTCTime(dateTime);
        }

        /// <summary>
        /// Get current culture
        /// </summary>
        /// <returns></returns>
        public string CurrentCulture()
        {
            return _webViewPageBase.CurrentCulture();
        }

        /// <summary>
        /// Convert date time to short date string
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string ToShortDateString(DateTime? date)
        {
            return _webViewPageBase.ToShortDateString(date);
        }

        /// <summary>
        /// Convert date time to date time string
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string ToDateTimeString(DateTime? date)
        {
            return _webViewPageBase.ToDateTimeString(date);
        }

        /// <summary>
        /// Convert date time to short time string
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string ToShortTimeString(DateTime? date)
        {
            return _webViewPageBase.ToShortTimeString(date);
        }

        #endregion

        #region Common Helpers

        public string ToUrlString(string text)
        {
            return _webViewPageBase.ToUrlString(text);
        }

        public MvcHtmlString Nl2Br(string text)
        {
            return _webViewPageBase.Nl2Br(text);
        }

        public string SafeSubstring(string input, int length)
        {
            return _webViewPageBase.SafeSubstring(input, length);
        }

        public bool HasPermissions(params Permission[] permission)
        {
            return _webViewPageBase.HasPermissions(permission);
        }

        public bool HasOneOfPermissions(params Permission[] permission)
        {
            return _webViewPageBase.HasOneOfPermissions(permission);
        }

        public IHtmlString YesNoString(bool? input)
        {
            return _webViewPageBase.YesNoString(input);
        }

        public string EzCMSVersion()
        {
            return _webViewPageBase.EzCMSVersion();
        }

        #endregion

        #region Style/Script

        public MvcHtmlString GetResourceStylePath(string name)
        {
            return _webViewPageBase.GetResourceStylePath(name);
        }

        public MvcHtmlString RenderResourceStyle(string name)
        {
            return _webViewPageBase.RenderResourceStyle(name);
        }

        public MvcHtmlString RenderResourceScript(string name)
        {
            return _webViewPageBase.RenderResourceScript(name);
        }
        #endregion

        /// <summary>
        /// Render template using RazorEngine
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public MvcHtmlString RenderTemplate(string templateName, TModel model)
        {
            var dynamicViewBag = ViewBag == null ? new DynamicViewBag(ViewBag) : new DynamicViewBag();
            return _webViewPageBase.RenderTemplate(templateName, model, dynamicViewBag);
        }

        /// <summary>
        /// Render template using RazorEngine
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="templatContent"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public MvcHtmlString RenderTemplate(string templateName, string templatContent, TModel model)
        {
            var dynamicViewBag = ViewBag == null ? new DynamicViewBag(ViewBag) : new DynamicViewBag();
            return _webViewPageBase.RenderTemplate(templateName, templatContent, model, dynamicViewBag);
        }

        /// <summary>
        /// Get current logged in user
        /// </summary>
        public UserSessionModel CurrentUser
        {
            get { return WorkContext.CurrentUser; }
            set { WorkContext.CurrentUser = value; }
        }

        public HttpContext HttpContext
        {
            get { return HttpContext.Current; }
        }
    }
}
