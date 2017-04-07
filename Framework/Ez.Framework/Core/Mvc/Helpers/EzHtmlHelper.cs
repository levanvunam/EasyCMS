using Ez.Framework.Configurations;
using Ez.Framework.Core.Enums;
using Ez.Framework.Utilities;
using Recaptcha.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Ez.Framework.Core.Mvc.Helpers
{
    /// <summary>
    /// Ez html helper
    /// </summary>
    public static class EzHtmlHelper
    {
        #region Checkbox

        /// <summary>
        /// Generate ace checkbox
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="showLabel"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString AceCheckBoxFor<TModel>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression, bool showLabel = false, object htmlAttributes = null)
        {
            var attributes = htmlAttributes != null
                ? new RouteValueDictionary(htmlAttributes)
                : new RouteValueDictionary();

            //Fix under score
            attributes.FixUnderScoreAttribute();

            if (attributes["class"] != null && !attributes["class"].ToString().Split(' ').Contains("ace"))
            {
                attributes["class"] = attributes["class"] + " ace";
            }
            else
            {
                attributes.Add("class", "ace");
            }

            var html = new StringBuilder();
            html.Append(helper.CheckBoxFor(expression, attributes));

            var tag = new TagBuilder("label");
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            tag.Attributes.Add("class", "lbl");
            tag.Attributes.Add("for", helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

            if (showLabel)
            {
                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
                if (metadata != null)
                {
                    string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
                    if (!string.IsNullOrEmpty(labelText))
                    {
                        tag.SetInnerText(string.Format(" {0} ", labelText));
                    }
                }
            }

            html.Append(tag.ToString(TagRenderMode.Normal));

            return MvcHtmlString.Create(html.ToString());
        }

        /// <summary>
        /// Generate ace checkbox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="label"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString AceCheckBox(this HtmlHelper helper, string name, bool value = false, string label = "", object htmlAttributes = null)
        {
            var attributes = htmlAttributes != null
                ? new RouteValueDictionary(htmlAttributes)
                : new RouteValueDictionary();

            //Fix under score
            attributes.FixUnderScoreAttribute();

            if (attributes["class"] != null && !attributes["class"].ToString().Split(' ').Contains("ace"))
            {
                attributes["class"] = attributes["class"] + " ace";
            }
            else
            {
                attributes.Add("class", "ace");
            }

            var html = new StringBuilder();
            html.Append(helper.CheckBox(name, value, attributes));

            var tag = new TagBuilder("label");
            tag.SetInnerText(string.Format(" {0}", label));
            tag.Attributes.Add("class", "lbl");
            tag.Attributes.Add("for", name);

            html.Append(tag.ToString(TagRenderMode.Normal));

            return MvcHtmlString.Create(html.ToString());
        }

        #endregion

        #region Recaptcha

        /// <summary>
        /// Generate Recaptcha
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        /// <param name="notSetupMessage"></param>
        /// <returns></returns>
        public static IHtmlString GenerateCaptcha<TModel>(this HtmlHelper<TModel> helper, string publicKey, string privateKey, string notSetupMessage)
        {
            if (!string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(privateKey))
            {
                return Recaptcha.Web.Mvc.RecaptchaMvcExtensions.Recaptcha(helper, publicKey, RecaptchaTheme.Clean);
            }

            return new HtmlString(notSetupMessage);
        }

        #endregion

        #region Url

        /// <summary>
        /// Generate empty route url
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static string EmtpyRouteUrl(this UrlHelper helper, string action, string controller, object routeValues = null)
        {
            var currentRouteValues = helper.RequestContext.RouteData.Values;
            var routeValueKeys = currentRouteValues.Keys.ToList();

            // Temporarily remove routevalues
            var oldRouteValues = new Dictionary<string, object>();
            foreach (var key in routeValueKeys)
            {
                oldRouteValues[key] = currentRouteValues[key];
                currentRouteValues.Remove(key);
            }

            // Generate URL
            var url = helper.Action(action, controller, routeValues);

            // Reinsert routevalues
            foreach (var keyValuePair in oldRouteValues)
            {
                currentRouteValues.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return url;
        }

        #endregion

        #region Media browser

        /// <summary>
        /// Get editable text for
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="mode"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="rootFolder"></param>
        /// <param name="uploadFolder"></param>
        /// <returns></returns>
        public static MvcHtmlString MediaBrowserFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, MediaEnums.MediaBrowserSelectMode mode = MediaEnums.MediaBrowserSelectMode.All,
            object htmlAttributes = null, string rootFolder = FrameworkConstants.MediaPath,
            string uploadFolder = "")
        {
            var attributes = htmlAttributes != null
                ? new RouteValueDictionary(htmlAttributes)
                : new RouteValueDictionary();


            attributes.Append("data-mode", mode.ToString());
            attributes.Append("data-root", rootFolder);
            attributes.Append("data-upload", string.IsNullOrEmpty(uploadFolder) ? rootFolder : uploadFolder);
            var textbox = helper.TextBoxFor(expression, attributes);

            var simpleUploadTag = new TagBuilder("div");
            simpleUploadTag.Attributes.Append("class", "simple-upload");
            simpleUploadTag.Attributes.Append("data-rel", "tooltip");

            var tag = new TagBuilder("a");
            tag.Attributes.Append("class", "btn btn-info btn-xs browsefile");
            tag.Attributes.Append("href", "javascript:void(0)");
            tag.Attributes.Append("data-rel", "tooltip");

            switch (mode)
            {
                case MediaEnums.MediaBrowserSelectMode.Image:
                    simpleUploadTag = null;

                    tag.Attributes.Append("title", "Select Image");
                    tag.InnerHtml = "<i class='fa fa-picture-o bigger-110 icon-only'></i>";
                    break;
                case MediaEnums.MediaBrowserSelectMode.File:
                    simpleUploadTag = null;

                    tag.Attributes.Append("title", "Select File");
                    tag.InnerHtml = "<i class='fa fa-file-o bigger-110 icon-only'></i>";
                    break;
                case MediaEnums.MediaBrowserSelectMode.Folder:
                    simpleUploadTag = null;

                    tag.Attributes.Append("title", "Select Folder");
                    tag.InnerHtml = "<i class='fa fa-folder-o bigger-110 icon-only'></i>";
                    break;
                case MediaEnums.MediaBrowserSelectMode.All:
                    simpleUploadTag = null;

                    tag.Attributes.Append("title", "Select File Or Folder");
                    tag.InnerHtml = "<i class='fa fa-file-o bigger-110 icon-only'></i>";
                    break;
                case MediaEnums.MediaBrowserSelectMode.SimpleImageUpload:
                    simpleUploadTag.Attributes.Append("title", "Upload Image");

                    tag = null;
                    break;
                case MediaEnums.MediaBrowserSelectMode.ComplexImageUpload:
                    simpleUploadTag.Attributes.Append("title", "Upload Image");

                    tag.Attributes.Append("title", "Select Image");
                    tag.InnerHtml = "<i class='fa fa-picture-o bigger-110 icon-only'></i>";
                    break;
                case MediaEnums.MediaBrowserSelectMode.SimpleFileUpload:
                    simpleUploadTag.Attributes.Append("title", "Upload File");

                    tag = null;
                    break;
                case MediaEnums.MediaBrowserSelectMode.ComplexFileUpload:
                    simpleUploadTag.Attributes.Append("title", "Upload File");

                    tag.Attributes.Append("title", "Select File");
                    tag.InnerHtml = "<i class='fa fa-file-o bigger-110 icon-only'></i>";
                    break;
            }

            var tagHtml = tag == null ? string.Empty : tag.ToString(TagRenderMode.Normal);
            var simpleUploadHtml = simpleUploadTag == null
                ? string.Empty
                : simpleUploadTag.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(textbox + tagHtml + simpleUploadHtml);
        }

        #endregion

        #region Back Link

        /// <summary>
        /// Generate return url hidden field
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static MvcHtmlString ReturnHiddenField(this HtmlHelper helper)
        {
            var html = new StringBuilder();
            var returnUrl = HttpContext.Current.Request["ReturnUrl"];
            html.Append(helper.Hidden("returnUrl", returnUrl));

            return new MvcHtmlString(html.ToString());
        }

        /// <summary>
        /// Create back link
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="url"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static MvcHtmlString BackLink(this HtmlHelper helper, string url, string text = "Cancel")
        {
            var tag = new TagBuilder("a");
            tag.Attributes.Append("id", "btnBackToList");
            tag.Attributes.Append("class", "btn btn-link");

            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.Params["ReturnUrl"]))
                {
                    url = HttpContext.Current.Request.Params["ReturnUrl"];
                }
            }
            catch (Exception)
            {
                // Error when post back
            }

            tag.Attributes.Append("href", url);
            tag.SetInnerText(text);

            return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
        }

        #endregion

        #region Validation

        /// <summary>
        /// Html validation message
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IHtmlString HtmlValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression
        )
        {
            return helper.Raw(HttpUtility.HtmlDecode(helper.ValidationMessageFor(expression).ToHtmlString()));
        }

        /// <summary>
        /// Html validation message
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IHtmlString HtmlValidationMessage(this HtmlHelper helper, string name)
        {
            return helper.Raw(HttpUtility.HtmlDecode(helper.ValidationMessage(name).ToHtmlString()));
        }

        #endregion

        /// <summary>
        /// Empty partial which will allows to generate null model partial
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="partial"></param>
        /// <returns></returns>
        public static MvcHtmlString EmptyPartial(this HtmlHelper helper, string partial)
        {
            return helper.Partial(partial, null, new ViewDataDictionary());
        }
    }
}
