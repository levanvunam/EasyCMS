using Ez.Framework.Configurations;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Html;
using Ez.Framework.Utilities.Time;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Framework.SiteSettings;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.Scripts;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Core.Services.Styles;
using EzCMS.Core.Services.Users;
using EzCMS.Core.Services.WidgetTemplates;
using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EzCMS.Core.Framework.Mvc.ViewEngines.Razor
{
    public class WebViewPageBase
    {
        private readonly ControllerBase _currentController;
        private readonly IEzCMSLocalizedResourceService _localizedResourceService;
        private readonly ISiteSettingService _siteSettingService;
        private readonly IStyleService _styleService;
        private readonly IScriptService _scriptService;
        private readonly IUserService _userService;
        private readonly IWidgetTemplateService _widgetTemplateService;
        public WebViewPageBase()
        {
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            _userService = HostContainer.GetInstance<IUserService>();
            _localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            _siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
            _styleService = HostContainer.GetInstance<IStyleService>();
            _scriptService = HostContainer.GetInstance<IScriptService>();

            _currentController = (ControllerBase)HttpContext.Current.Items[FrameworkConstants.EzCurrentController];
        }

        #region Multi Languages Helpers

        /// <summary>
        /// Get translated value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string T(string key)
        {
            return _localizedResourceService.T(key);
        }

        /// <summary>
        /// Get translated value by key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string T(string key, string defaultValue)
        {
            return _localizedResourceService.T(key, defaultValue);
        }

        /// <summary>
        /// Get translated value by key and format result
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string TFormat(string key, params object[] parameters)
        {
            return _localizedResourceService.TFormat(key, parameters);
        }

        /// <summary>
        /// Render a tag with text in multi language
        /// </summary>
        /// <param name="htmlTagName">Tag name to render</param>
        /// <param name="textKey">Key of the text</param>
        /// <returns>Rendered Tag</returns>
        public IHtmlString MText(HtmlTag htmlTagName, string textKey)
        {
            return MText(htmlTagName, textKey, null, null);
        }


        /// <summary>
        /// Render a tag with text in multi language
        /// </summary>
        /// <param name="htmlTagName">Tag name to render</param>
        /// <param name="textKey">Key of the text</param>
        /// <param name="defaultValue">Default value of the text</param>
        /// <returns>Rendered Tag</returns>
        public IHtmlString MText(HtmlTag htmlTagName,
                                 string textKey,
                                 string defaultValue)
        {
            return MText(htmlTagName, textKey, defaultValue, null);
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
        public IHtmlString MText(HtmlTag htmlTagName, string textKey, string defaultValue, object htmlAttributes, object[] parameters = null)
        {
            var text = _localizedResourceService.GetLocalizedResource(textKey, defaultValue, parameters);

            var tag = new TagBuilder(htmlTagName.ToString().ToLower());
            tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            if (HasPermissions(Permission.ManageLabelOverride))
            {
                tag.Attributes.Append("language-edit", "true");
                tag.Attributes.Append("data-name", textKey);
                tag.Attributes.Append("data-validation", "required");
            }
            tag.Attributes.Append("class", "label-override");

            tag.SetInnerText(text);

            return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
        }
        #endregion

        #region Setting Helpers

        /// <summary>
        /// Load setting by key and parsing to type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T SValue<T>(string key)
        {
            return _siteSettingService.GetSetting<T>(key);
        }

        /// <summary>
        /// Load complex setting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SValue<T>() where T : ComplexSetting
        {
            return _siteSettingService.LoadSetting<T>();
        }

        #endregion

        #region Message Helpers

        #region Private Properties

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public ResponseModel EzCMSMessageResponse
        {
            get
            {
                if (_currentController == null) return null;
                return (ResponseModel)_currentController.TempData[FrameworkConstants.EzMessageResponse];
            }
            set
            {
                if (_currentController == null) return;
                _currentController.TempData[FrameworkConstants.EzMessageResponse] = value;
            }
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public String ErrorMessage
        {
            get
            {
                if (_currentController == null) return null;
                return (string)_currentController.TempData[FrameworkConstants.EzErrorMessage];
            }
            set
            {
                if (_currentController == null) return;
                _currentController.TempData[FrameworkConstants.EzErrorMessage] = value;
            }
        }

        /// <summary>
        /// Gets or sets the warning message.
        /// </summary>
        public String WarningMessage
        {
            get
            {
                if (_currentController == null) return null;
                return (string)_currentController.TempData[FrameworkConstants.EzWarningMessage];
            }
            set
            {
                if (_currentController == null) return;
                _currentController.TempData[FrameworkConstants.EzWarningMessage] = value;
            }
        }

        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        public String SuccessMessage
        {
            get
            {
                if (_currentController == null) return null;
                return (string)_currentController.TempData[FrameworkConstants.EzSuccessMessage];
            }
            set
            {
                if (_currentController == null) return;
                _currentController.TempData[FrameworkConstants.EzSuccessMessage] = value;
            }
        }

        #endregion

        /// <summary>
        /// Show status message after page refresh
        /// </summary>
        /// <returns></returns>
        public MvcHtmlString ShowStatusMessage()
        {
            const string template = @"<div class='alert alert-{0} fade in'>
<button class='close' data-dismiss='alert' type='button'>
<i class='fa fa-times'></i>
</button>
{1}
</div>";

            const string detailErrorTemplate = @"<div class='alert alert-{0} fade in'>
    <div class='col-sm-9'>
        {1}
    </div>
    <button class='close mg2t pull-right' data-dismiss='alert' type='button'>
        <i class='fa fa-times'></i>
    </button>
    <div class='col-sm-2 pull-right'>
        <a class='toggle-error-btn' href='javascript:void(0)'>
            <i class='fa fa-chevron-down'></i>More Details
        </a>
    </div>
    <div class='col-xs-12 error-detail-box'>
        <div class='detail-message' style='display: none;'>
            {2}
        </div>
    </div><div class='clearfix'></div>
</div>";

            string html = string.Empty;
            if (EzCMSMessageResponse != null)
            {
                string errorType;
                if (EzCMSMessageResponse.Success)
                {
                    errorType = EzCMSMessageResponse.ResponseStatus == ResponseStatusEnums.Warning ? "warning" : "info";
                    html = string.Format(template, errorType, EzCMSMessageResponse.Message);
                }
                else
                {
                    errorType = "danger";

                    if (!string.IsNullOrEmpty(EzCMSMessageResponse.DetailMessage))
                    {
                        html = string.Format(detailErrorTemplate, errorType, EzCMSMessageResponse.Message, EzCMSMessageResponse.DetailMessage);
                    }
                    else
                    {
                        html = string.Format(template, "danger", ErrorMessage);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(SuccessMessage))
                {
                    html = string.Format(template, "info", SuccessMessage);
                }
                else if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    html = string.Format(template, "danger", ErrorMessage);
                }
                else if (!string.IsNullOrEmpty(WarningMessage))
                {
                    html = string.Format(template, "warning", WarningMessage);
                }
            }


            return new MvcHtmlString(html);
        }

        #endregion

        #region Common Helpers

        /// <summary>
        /// Convert string to url
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string ToUrlString(string text)
        {
            return text.ToUrlString();
        }

        /// <summary>
        /// Convert environment new line to <br/>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public MvcHtmlString Nl2Br(string text)
        {
            return new MvcHtmlString(text.Nl2Br());
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
        /// Check if current user has permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public bool HasPermissions(params Permission[] permissions)
        {
            if (WorkContext.CurrentUser == null)
                return false;
            return _userService.HasPermissions(WorkContext.CurrentUser.Id, permissions);
        }

        /// <summary>
        /// Check if current user has one of permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public bool HasOneOfPermissions(params Permission[] permissions)
        {
            if (WorkContext.CurrentUser == null)
                return false;
            return _userService.HasOneOfPermissions(WorkContext.CurrentUser.Id, permissions);
        }

        /// <summary>
        /// Convert bool to Yes / No
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IHtmlString YesNoString(bool? input)
        {
            if (input.HasValue && input.Value)
            {
                return new HtmlString("Yes");
            }
            return new HtmlString("No");
        }

        /// <summary>
        /// Get EzCMS version
        /// </summary>
        /// <returns></returns>
        public string EzCMSVersion()
        {
            return WorkContext.EzCMSVersion;
        }

        #endregion

        #region TimeZone & Culture Helpers

        /// <summary>
        /// Current user timezone
        /// </summary>
        public TimeZoneInfo CurrentTimeZone
        {
            get
            {
                return WorkContext.CurrentTimezone;
            }
        }

        /// <summary>
        /// Get label for string
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public string LabelForTime(string label)
        {
            return WorkContext.TimezoneOffset == 0 ? string.Format("{0} (UTC)", label) : string.Format("{0} (Local)", label);
        }

        /// <summary>
        /// Convert current date time to local date time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DateTime? ToLocalDateTime(DateTime? dateTime)
        {
            return dateTime.ToUserDateTime(CurrentTimeZone);
        }

        /// <summary>
        /// Convert current time to local time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public TimeSpan? ToLocalTime(TimeSpan? time)
        {
            return time.ToUserTime(CurrentTimeZone);
        }

        /// <summary>
        /// Convert local date time to UTC
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DateTime? ToUTCTime(DateTime? dateTime)
        {
            return dateTime.ToUTCTime(CurrentTimeZone);
        }

        /// <summary>
        /// Get current culture
        /// </summary>
        /// <returns></returns>
        public string CurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture.Name;
        }

        /// <summary>
        /// Convert date time to short date string
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string ToShortDateString(DateTime? date)
        {
            return date.ToDateFormat();
        }

        /// <summary>
        /// Convert date time to date time string
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string ToDateTimeString(DateTime? date)
        {
            return date.ToDateTimeFormat();
        }

        /// <summary>
        /// Convert date time to short time string
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string ToShortTimeString(DateTime? date)
        {
            return date.ToTimeFormat();
        }

        /// <summary>
        /// Convert date time to short time string
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string ToShortTimeString(TimeSpan? time)
        {
            return time.ToTimeFormat();
        }

        #endregion

        #region Style/Script

        public MvcHtmlString GetResourceStylePath(string name)
        {
            return new MvcHtmlString(_styleService.GetStyleUrl(name));
        }

        public MvcHtmlString RenderResourceStyle(string name)
        {
            if (_styleService.GetAll().Any(s => s.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
            {
                var tag = new TagBuilder("link");
                tag.Attributes.Add("rel", "stylesheet");
                tag.Attributes.Add("type", "text/css");
                tag.Attributes.Add("href", string.Format(EzCMSContants.CssResourceUrl, name));
                return new MvcHtmlString(tag.ToString(TagRenderMode.SelfClosing));
            }
            return new MvcHtmlString(string.Empty);
        }

        public MvcHtmlString RenderResourceScript(string name)
        {
            if (_scriptService.GetAll().Any(s => s.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
            {
                var tag = new TagBuilder("script");
                tag.Attributes.Add("src", string.Format(EzCMSContants.ScriptResourceUrl, name));
                return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
            }
            return new MvcHtmlString(string.Empty);
        }
        #endregion

        /// <summary>
        /// Render template using RazorEngine
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="model"></param>
        /// <param name="viewBag"></param>
        /// <returns></returns>
        public MvcHtmlString RenderTemplate(string templateName, dynamic model, dynamic viewBag)
        {
            var template = _widgetTemplateService.GetTemplateByName(templateName);
            if (template != null)
            {
                return new MvcHtmlString(EzRazorEngineHelper.CompileAndRun(template.Content, model, viewBag, template.CacheName));
            }
            return new MvcHtmlString(string.Empty);
        }

        /// <summary>
        /// Render template using RazorEngine
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="templateContent"></param>
        /// <param name="model"></param>
        /// <param name="viewBag"></param>
        /// <returns></returns>
        public MvcHtmlString RenderTemplate(string templateName, string templateContent, dynamic model, dynamic viewBag)
        {
            return new MvcHtmlString(EzRazorEngineHelper.CompileAndRun(templateContent, model, viewBag, templateName.GetTemplateCacheName(templateContent)));
        }

        public HttpContext HttpContext
        {
            get { return HttpContext.Current; }
        }
    }
}
