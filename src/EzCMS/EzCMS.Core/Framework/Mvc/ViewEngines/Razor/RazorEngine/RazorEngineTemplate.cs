using Ez.Framework.Utilities;
using Ez.Framework.IoC;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.SiteSettings;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Services.Widgets;
using RazorEngine.Templating;
using System;
using System.Web;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Framework.Mvc.ViewEngines.Razor.RazorEngine
{
    public class RazorEngineTemplateBase<TModel> : TemplateBase<TModel>, IViewDataContainer
    {
        #region Html/Url Helpers

        private UrlHelper _urlhelper;

        private ViewDataDictionary _viewdata;

        private HtmlHelper<TModel> _helper;

        public HtmlHelper<TModel> Html
        {
            get
            {
                if (_helper == null)
                {
                    var controllerContext = WorkContext.CurrentControllerContext;
                    var controllerAction = WorkContext.CurrentControllerAction;
                    if (controllerContext != null && !string.IsNullOrEmpty(controllerAction))
                    {
                        _helper =
                            new HtmlHelper<TModel>(
                                new ViewContext(controllerContext, new WebFormView(controllerContext, controllerAction),
                                    ViewData, new TempDataDictionary(), CurrentWriter), this);
                        return _helper;

                    }

                    var writer = CurrentWriter;
                    var viewContext = new ViewContext
                    {
                        Writer = writer,
                        ViewData = ViewData
                    };

                    _helper = new HtmlHelper<TModel>(viewContext, this);
                }
                return _helper;
            }
        }

        public ViewDataDictionary ViewData
        {
            get
            {
                if (_viewdata == null)
                {

                    _viewdata = new ViewDataDictionary
                    {
                        TemplateInfo = new TemplateInfo { HtmlFieldPrefix = string.Empty }
                    };

                    if (Model != null)
                    {
                        _viewdata.Model = Model;
                    }

                }

                return _viewdata;
            }
            set
            {
                _viewdata = value;
            }
        }

        public UrlHelper Url
        {
            get { return _urlhelper ?? (_urlhelper = new UrlHelper(HttpContext.Current.Request.RequestContext)); }
        }

        #endregion

        private readonly IWidgetService _widgetService;
        private readonly WebViewPageBase _webViewPageBase;
        public RazorEngineTemplateBase()
        {
            _webViewPageBase = new WebViewPageBase();
            _widgetService = HostContainer.GetInstance<IWidgetService>();
        }

        #region Multi Languages Helpers
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

        #endregion

        #region Setting Helpers

        /// <summary>
        /// Get setting by key
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

        #region Image Helpers

        /// <summary>
        /// Generate image thumbnail url
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public string Thumbnail(string filePath, int width, int height)
        {
            return Url.Action("Thumbnail", "Media", new { area = "Admin", path = filePath, w = width, h = height });
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

        #region Widget Render

        /// <summary>
        /// Render widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public MvcHtmlString RenderWidget(string widget)
        {
            return new MvcHtmlString(_widgetService.Render(widget));
        }

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
        #endregion

        #region TimeZone & Culture Helpers

        /// <summary>
        /// Current timezone
        /// </summary>
        public TimeZoneInfo CurrentTimeZone
        {
            get
            {
                return _webViewPageBase.CurrentTimeZone;
            }
        }

        /// <summary>
        /// Convert to local date time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DateTime? ToLocalDateTime(DateTime? dateTime)
        {
            return _webViewPageBase.ToLocalDateTime(dateTime);
        }

        /// <summary>
        /// Convert to UTC date time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DateTime? ToUTCDateTime(DateTime? dateTime)
        {
            return _webViewPageBase.ToUTCTime(dateTime);
        }

        /// <summary>
        /// Current culture
        /// </summary>
        /// <returns></returns>
        public string CurrentCulture()
        {
            return _webViewPageBase.CurrentCulture();
        }

        /// <summary>
        /// To short date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string ToShortDateString(DateTime? date)
        {
            return _webViewPageBase.ToShortDateString(date);
        }

        /// <summary>
        /// To date time
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string ToDateTimeString(DateTime? date)
        {
            return _webViewPageBase.ToDateTimeString(date);
        }

        #endregion

        #region Common Helpers

        /// <summary>
        /// To url string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string ToUrlString(string text)
        {
            return _webViewPageBase.ToUrlString(text);
        }

        /// <summary>
        /// New line to br
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public MvcHtmlString Nl2Br(string text)
        {
            return _webViewPageBase.Nl2Br(text);
        }

        /// <summary>
        /// Safe substring
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string SafeSubstring(string input, int length)
        {
            return input.SafeSubstring(length);
        }

        /// <summary>
        /// Has permissions
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasPermissions(params Permission[] permission)
        {
            return _webViewPageBase.HasPermissions(permission);
        }

        /// <summary>
        /// Has one of permissions
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasOneOfPermissions(params Permission[] permission)
        {
            return _webViewPageBase.HasOneOfPermissions(permission);
        }

        /// <summary>
        /// Yes / No string from boolean
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IHtmlString YesNoString(bool? input)
        {
            return _webViewPageBase.YesNoString(input);
        }

        /// <summary>
        /// Get EzCMS version
        /// </summary>
        /// <returns></returns>
        public string EzCMSVersion()
        {
            return _webViewPageBase.EzCMSVersion();
        }

        #endregion

        #region Style/Script

        /// <summary>
        /// Render resource style
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MvcHtmlString RenderResourceStyle(string name)
        {
            return _webViewPageBase.RenderResourceStyle(name);
        }

        /// <summary>
        /// Render resource script
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MvcHtmlString RenderResourceScript(string name)
        {
            return _webViewPageBase.RenderResourceScript(name);
        }

        #endregion

        /// <summary>
        /// Check if context key has been init
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasNoContext(string key)
        {
            return HttpContext.Items[key] == null;
        }

        /// <summary>
        /// Check if context key has been init
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void SetContext(string key)
        {
            HttpContext.Items[key] = new object();
        }

        /// <summary>
        /// Current contact
        /// </summary>
        public ContactCookieModel CurrentContact
        {
            get
            {
                return WorkContext.CurrentContact;
            }
        }

        /// <summary>
        /// Get current logged in user
        /// </summary>
        public UserSessionModel CurrentUser
        {
            get { return WorkContext.CurrentUser; }
            set { WorkContext.CurrentUser = value; }
        }

        /// <summary>
        /// Current context
        /// </summary>
        public HttpContext HttpContext
        {
            get { return HttpContext.Current; }
        }
    }
}