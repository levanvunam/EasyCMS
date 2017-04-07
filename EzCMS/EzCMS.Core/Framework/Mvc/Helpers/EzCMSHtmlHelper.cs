using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Reflection;
using Ez.Framework.Utilities.Time;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Core.Services.SlideInHelps;
using EzCMS.Core.Services.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Framework.Mvc.Helpers
{
    public static class EzCMSHtmlHelper
    {
        #region Label

        /// <summary>
        /// Generate localize label for property
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString LocalizeLabelFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            var attributes = htmlAttributes != null
                ? new RouteValueDictionary(htmlAttributes)
                : new RouteValueDictionary();
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            var tag = new TagBuilder("label");
            if (metadata != null)
            {
                string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
                string displayName = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
                if (String.IsNullOrEmpty(displayName))
                {
                    return MvcHtmlString.Empty;
                }

                tag.MergeAttributes(attributes);
                tag.Attributes.Add("for", helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

                if (!string.IsNullOrEmpty(metadata.PropertyName))
                {
                    MemberInfo property = metadata.ContainerType.GetProperty(metadata.PropertyName);

                    var attribute =
                        property.GetCustomAttributes(typeof(LocalizedDisplayNameAttribute), true)
                            .Cast<LocalizedDisplayNameAttribute>()
                            .FirstOrDefault();
                    if (attribute != null)
                    {
                        var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
                        displayName = localizedResourceService.T(attribute.Key, attribute.DisplayName);

                        if (WorkContext.CurrentUser != null &&
                            WorkContext.CurrentUser.HasPermissions(Permission.ManageLabelOverride))
                        {
                            tag.Attributes.Replace("language-edit", "true");
                            tag.Attributes.Replace("data-name", attribute.Key);
                            tag.Attributes.Replace("data-validation", "required");
                        }
                    }
                    tag.Attributes.Append("class", "label-override");
                }

                var span = new TagBuilder("span");
                span.SetInnerText(displayName);

                tag.InnerHtml = span.ToString(TagRenderMode.Normal);
            }

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Generate text for property
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string TextFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            if (metadata != null)
            {
                string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
                string displayName = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
                if (String.IsNullOrEmpty(displayName))
                {
                    return string.Empty;
                }

                if (!string.IsNullOrEmpty(metadata.PropertyName))
                {
                    MemberInfo property = metadata.ContainerType.GetProperty(metadata.PropertyName);

                    var attribute =
                        property.GetCustomAttributes(typeof(LocalizedDisplayNameAttribute), true)
                            .Cast<LocalizedDisplayNameAttribute>()
                            .FirstOrDefault();
                    if (attribute != null)
                    {
                        var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
                        displayName = localizedResourceService.T(attribute.Key, attribute.DisplayName);
                    }
                }

                var span = new TagBuilder("span");
                span.SetInnerText(displayName);

                return displayName;

            }

            return string.Empty;
        }

        #endregion

        #region Help text

        /// <summary>
        /// Generate help text for property
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static MvcHtmlString HelpTextFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes,
            SlideInHelpPosition position = SlideInHelpPosition.Top, string text = "?")
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            var tag = new TagBuilder("span");
            if (metadata != null)
            {
                string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
                string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
                if (String.IsNullOrEmpty(labelText))
                {
                    return MvcHtmlString.Empty;
                }

                if (!string.IsNullOrEmpty(metadata.PropertyName))
                {
                    MemberInfo property = metadata.ContainerType.GetProperty(metadata.PropertyName);

                    //Get localize attribute
                    var attribute =
                        property.GetCustomAttributes(typeof(LocalizedDisplayNameAttribute), true)
                            .Cast<LocalizedDisplayNameAttribute>()
                            .FirstOrDefault();
                    if (attribute != null)
                    {
                        return HelpText(helper, attribute.Key, htmlAttributes, position, text);
                    }
                }
            }

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }


        /// <summary>
        /// Generate help text with key
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="textKey"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static MvcHtmlString HelpText(this HtmlHelper helper, string textKey,
            object htmlAttributes, SlideInHelpPosition position = SlideInHelpPosition.Top, string text = "?")
        {
            var userService = HostContainer.GetInstance<IUserService>();
            var slideInHelpService = HostContainer.GetInstance<ISlideInHelpService>();
            var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();

            var tag = new TagBuilder("span");

            var slideInHelp = slideInHelpService.GetSlideInHelp(textKey);

            /* 
             * There are some conditions to disable a slide in help
             *      Cannot find slide in help
             *      User has no permission to see no active slide in help
             *      If site is not disalbed slide in help service (it means that site is client site not the master site for setting up slide in hel), then we will disable the help
             */
            if (slideInHelp == null
                || (string.IsNullOrEmpty(slideInHelp.HelpTitle)
                    || string.IsNullOrEmpty(slideInHelp.MasterHelpContent)
                    && !siteSettingService.LoadSetting<EzCMSHelpConfigurationSetting>().DisabledSlideInHelpService)
                || (!userService.HasPermissions(WorkContext.CurrentUser.Id, Permission.ManageSlideInHelp) && !slideInHelp.Active))
            {
                return MvcHtmlString.Create("");
            }

            // Generate required attributes
            var htmlAttributeDictionary = new RouteValueDictionary(htmlAttributes)
                        {
                            {"data-id", slideInHelp.Id},
                            {"data-placement", position.GetEnumName().ToLower()},
                            {"data-trigger", "hover"},
                            {"data-rel", "popover"},
                            {"data-original-title", slideInHelp.HelpTitle }
                        };

            // Place local content at top of help content then the master content
            var helpContent = slideInHelp.MasterHelpContent;
            if (!string.IsNullOrEmpty(slideInHelp.LocalHelpContent))
            {
                helpContent = string.Format("{0} <br/> {1}", slideInHelp.LocalHelpContent, slideInHelp.MasterHelpContent);
            }

            if (WorkContext.CurrentUser != null &&
                userService.HasPermissions(WorkContext.CurrentUser.Id,
                    Permission.ManageSlideInHelp))
            {
                const string contentTemplate =
                    @"{0} <hr class='slide-in-help'/><div class='slide-in-help-edit-box'>
<a class='edit-slide-in-help' href='javascript:void(0)' data-id='{1}'><i class='fa fa-pencil'></i></a>
<a class='change-status-slide-in-help' href='javascript:void(0)' data-id='{1}' data-status='{2}'><i class='fa {3}'></i></a>
</div>";
                string className;
                string content;
                if (!slideInHelp.Active)
                {
                    className = "slide-in-help popover-hover popover-disabled";
                    content = string.Format(contentTemplate, helpContent, slideInHelp.Id, slideInHelp.Active, "fa-check");
                }
                else
                {
                    className = "slide-in-help popover-hover";
                    content = string.Format(contentTemplate, helpContent, slideInHelp.Id, slideInHelp.Active, "fa-power-off");
                }

                htmlAttributeDictionary.Append("data-content", content);

                htmlAttributeDictionary.Append("class", className);
            }
            else
            {
                htmlAttributeDictionary.Append("data-content", helpContent);
            }

            tag.SetInnerText(text);
            tag.MergeAttributes(htmlAttributeDictionary);

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        #endregion

        #region Editable

        /// <summary>
        /// Get editable text for
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MvcHtmlString EditableFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, CommonEnums.EditableType type = CommonEnums.EditableType.Text, object htmlAttributes = null)
        {
            var attributes = htmlAttributes != null
                ? new RouteValueDictionary(htmlAttributes)
                : new RouteValueDictionary();
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            var tag = new TagBuilder("span");
            if (metadata != null)
            {
                var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
                var displayName = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();

                #region Attributes

                tag.Attributes.Append("data-name", metadata.PropertyName);

                MemberInfo property = null;
                if (!string.IsNullOrEmpty(metadata.PropertyName))
                {
                    property = metadata.ContainerType.GetProperty(metadata.PropertyName);
                }

                // Add title
                if (property != null)
                {
                    // Check localized display name attribute
                    var attribute =
                        property.GetCustomAttributes(typeof(LocalizedDisplayNameAttribute), true)
                            .Cast<LocalizedDisplayNameAttribute>()
                            .FirstOrDefault();
                    if (attribute != null)
                    {
                        var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
                        displayName = localizedResourceService.T(attribute.Key);
                    }
                }
                tag.Attributes.Append("title", string.Format("Click to edit {0}", displayName));
                tag.Attributes.Append("data-rel", "tooltip");

                // Add class
                switch (type)
                {
                    case CommonEnums.EditableType.Text:
                        var className = "editable-text";

                        if (property != null)
                        {
                            // Check if property is email
                            var emailAddress = property.GetAttribute<EmailAddressAttribute>();
                            if (emailAddress != null)
                            {
                                className = "editable-property";
                                tag.Attributes.Append("data-type", "email");
                            }
                            else
                            {
                                var emailValidation = property.GetAttribute<EmailValidationAttribute>();
                                if (emailValidation != null)
                                {
                                    className = "editable-property";
                                    tag.Attributes.Append("data-type", "email");
                                }
                            }

                            // Check if property is bool
                            if (property.DeclaringType == typeof(bool) || property.DeclaringType == typeof(bool?))
                            {
                                className = "editable-boolean";
                            }
                        }
                        tag.Attributes.Append("class", className);
                        break;
                    case CommonEnums.EditableType.Date:
                        tag.Attributes.Append("class", "editable-date");
                        tag.Attributes.Replace("title", string.Format("Select {0}", displayName));
                        break;
                    case CommonEnums.EditableType.DateTime:
                        tag.Attributes.Append("class", "editable-datetime");
                        tag.Attributes.Replace("title", string.Format("Select {0}", displayName));
                        break;
                    case CommonEnums.EditableType.Boolean:
                        tag.Attributes.Append("class", "editable-boolean");
                        break;
                    default:
                        tag.Attributes.Append("class", "editable-property");
                        tag.Attributes.Append("data-type", type.GetEnumName().ToLower());
                        break;
                }

                #region Validation

                if (property != null)
                {
                    var required = property.GetAttribute<RequiredAttribute>();
                    if (required != null)
                    {
                        tag.Attributes.Append("data-validation", "required");
                    }

                    var emailAddress = property.GetAttribute<EmailAddressAttribute>();
                    if (emailAddress != null)
                    {
                        tag.Attributes.Append("data-validation", "email");
                    }
                    else
                    {
                        var emailValidation = property.GetAttribute<EmailValidationAttribute>();
                        if (emailValidation != null)
                        {
                            tag.Attributes.Append("data-validation", "email");
                        }
                    }
                }

                #endregion

                // Merge all attributes
                tag.MergeAttributes(attributes);

                #endregion

                var value = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;

                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    if (type == CommonEnums.EditableType.Date)
                    {
                        value = ((DateTime)value).ToUserDateTime(WorkContext.CurrentTimezone);
                        tag.SetInnerText(DateTimeUtilities.ToDateFormat((DateTime)value));
                    }
                    else if (type == CommonEnums.EditableType.DateTime)
                    {
                        value = ((DateTime)value).ToUserDateTime(WorkContext.CurrentTimezone);
                        tag.SetInnerText(DateTimeUtilities.ToDateTimeFormat((DateTime)value));
                    }
                    else if (type == CommonEnums.EditableType.Boolean)
                    {
                        tag.SetInnerText(((bool)value) ? "Yes" : "No");
                    }
                    else
                    {
                        tag.SetInnerText(value.ToString());
                    }
                }
                else
                {
                    if (type == CommonEnums.EditableType.Boolean)
                    {
                        tag.SetInnerText("No");
                    }
                }
            }

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Get editable text for
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="ajax"></param>
        /// <param name="multiple"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        public static MvcHtmlString EditableSelectFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, string dataSource, bool ajax = false, bool multiple = false, object htmlAttributes = null)
        {
            var attributes = htmlAttributes != null
                ? new RouteValueDictionary(htmlAttributes)
                : new RouteValueDictionary();
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            var tag = new TagBuilder("span");
            if (metadata != null)
            {
                var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
                var displayName = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();

                var value = ModelMetadata.FromLambdaExpression(
                    expression, helper.ViewData
                    ).Model;

                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    tag.SetInnerText(value.ToString());
                    tag.Attributes.Append("data-value", value.ToString());
                }

                #region Attributes

                MemberInfo property = null;
                if (!string.IsNullOrEmpty(metadata.PropertyName))
                {
                    property = metadata.ContainerType.GetProperty(metadata.PropertyName);
                }

                // Add title
                if (property != null)
                {
                    // Check localized display name attribute
                    var attribute =
                        property.GetCustomAttributes(typeof(LocalizedDisplayNameAttribute), true)
                            .Cast<LocalizedDisplayNameAttribute>()
                            .FirstOrDefault();
                    if (attribute != null)
                    {
                        var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
                        displayName = localizedResourceService.T(attribute.Key);
                    }
                }
                tag.Attributes.Append("title", string.Format("Select {0}", displayName));
                tag.Attributes.Append("data-name", metadata.PropertyName);
                tag.Attributes.Append("data-rel", "tooltip");
                tag.Attributes.Append("data-editable-ajax", ajax.ToString().ToLower());
                tag.Attributes.Append("data-multiple", multiple ? "true" : "false");
                tag.Attributes.Append("data-editable-source", dataSource);

                // Add class
                tag.Attributes.Append("class", "editable-select");

                #region Validation

                if (property != null)
                {
                    var required = property.GetAttribute<RequiredAttribute>();
                    if (required != null)
                    {
                        tag.Attributes.Append("data-validation", "required");
                    }
                }

                #endregion

                // Merge all attributes
                tag.MergeAttributes(attributes);

                #endregion
            }

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        #endregion

        #region Recaptcha

        /// <summary>
        /// Generate Recaptcha
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static IHtmlString GenerateCaptcha<TModel>(this HtmlHelper<TModel> helper)
        {
            var settingService = HostContainer.GetInstance<ISiteSettingService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            var captchaSetting = settingService.LoadSetting<CaptchaSetting>();
            return helper.GenerateCaptcha(captchaSetting.PrivateKey, captchaSetting.PublicKey, localizedResourceService.T("Captcha_Message_CaptchaNotSetup"));
        }

        #endregion

        #region Timezone

        /// <summary>
        /// Get editable text for
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString DisplayTimeFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, CommonEnums.DateTimeFormat type = CommonEnums.DateTimeFormat.DateTime,
            object htmlAttributes = null)
        {
            var attributes = htmlAttributes != null
                ? new RouteValueDictionary(htmlAttributes)
                : new RouteValueDictionary();

            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            var tag = new TagBuilder("span");
            if (metadata != null)
            {
                var value = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;

                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    var date = (DateTime)value;
                    tag.Attributes.Append("data-rel", "tooltip");

                    switch (type)
                    {
                        case CommonEnums.DateTimeFormat.Date:
                            tag.Attributes.Append("title", string.Format("UTC: {0}", DateTimeUtilities.ToDateFormat(date)));
                            tag.SetInnerText(DateTimeUtilities.ToDateFormat(date.ToUserDateTime(WorkContext.CurrentTimezone)));
                            break;
                        case CommonEnums.DateTimeFormat.Time:
                            tag.Attributes.Append("title", string.Format("UTC: {0}", DateTimeUtilities.ToTimeFormat(date)));
                            tag.SetInnerText(DateTimeUtilities.ToTimeFormat(date.ToUserDateTime(WorkContext.CurrentTimezone)));
                            break;
                        case CommonEnums.DateTimeFormat.DateTime:
                            tag.Attributes.Append("title", string.Format("UTC: {0}", DateTimeUtilities.ToDateTimeFormat(date)));
                            tag.SetInnerText(DateTimeUtilities.ToDateTimeFormat(date.ToUserDateTime(WorkContext.CurrentTimezone)));
                            break;
                    }
                }

                // Merge all attributes
                tag.MergeAttributes(attributes);
            }

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Generate localize label for property
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString LocalizeLabelTimeFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            var attributes = htmlAttributes != null
                ? new RouteValueDictionary(htmlAttributes)
                : new RouteValueDictionary();
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            var tagBuilder = new TagBuilder("span");

            var tag = new TagBuilder("label");
            if (metadata != null)
            {
                string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
                string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
                if (String.IsNullOrEmpty(labelText))
                {
                    return MvcHtmlString.Empty;
                }

                tagBuilder.MergeAttributes(attributes);

                tag.Attributes.Add("for", helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

                if (!string.IsNullOrEmpty(metadata.PropertyName))
                {
                    MemberInfo property = metadata.ContainerType.GetProperty(metadata.PropertyName);

                    var attribute =
                        property.GetCustomAttributes(typeof(LocalizedDisplayNameAttribute), true)
                            .Cast<LocalizedDisplayNameAttribute>()
                            .FirstOrDefault();
                    if (attribute != null)
                    {
                        var userService = HostContainer.GetInstance<IUserService>();
                        if (WorkContext.CurrentUser != null &&
                            userService.HasPermissions(WorkContext.CurrentUser.Id,
                                Permission.ManageLabelOverride))
                        {
                            tag.Attributes.Replace("language-edit", "true");
                            tag.Attributes.Replace("data-name", attribute.Key);
                            tag.Attributes.Replace("data-validation", "required");
                        }
                    }
                    tag.Attributes.Append("class", "label-override");
                }

                var span = new TagBuilder("span");
                span.SetInnerText(labelText);

                tag.InnerHtml = span.ToString(TagRenderMode.Normal);

            }

            //Add specific class for time tag
            tagBuilder.Attributes.Append("class", "time-tag");

            var timeTag = new TagBuilder("label");
            timeTag.SetInnerText(string.Format("({0})", WorkContext.TimezoneOffset == 0 ? "UTC" : "Local"));
            tagBuilder.InnerHtml = string.Format("{0} {1}", tag.ToString(TagRenderMode.Normal), timeTag.ToString(TagRenderMode.Normal));

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        #endregion
    }
}
