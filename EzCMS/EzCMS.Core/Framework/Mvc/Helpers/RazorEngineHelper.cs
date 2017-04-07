using Ez.Framework.Core.Mvc.Helpers;
using EzCMS.Core.Framework.Configuration;
using RazorEngine.Templating;
using System;
using System.Linq;

namespace EzCMS.Core.Framework.Mvc.Helpers
{
    public static class RazorEngineHelper
    {
        #region Razor Engine

        /// <summary>
        /// Render template using Razor engine
        /// </summary>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <param name="viewBag"> </param>
        /// <param name="cacheName"></param>
        /// <param name="resolveType"></param>
        /// <returns></returns>
        public static string CompileAndRun(string template, dynamic model, DynamicViewBag viewBag = null, string cacheName = "", ResolveType resolveType = ResolveType.Global)
        {
            return EzRazorEngineHelper.CompileAndRun(template, model, viewBag, cacheName, resolveType);
        }

        /// <summary>
        /// Render template using Razor engine
        /// </summary>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <param name="cacheName"></param>
        /// <param name="resolveType"></param>
        /// <returns></returns>
        public static string CompileAndRun(string template, dynamic model, string cacheName = "", ResolveType resolveType = ResolveType.Global)
        {
            return EzRazorEngineHelper.CompileAndRun(template, model, null, cacheName, resolveType);
        }

        /// <summary>
        /// Get cache template name
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetTemplateCacheName(this string templateName, string content)
        {
            return EzRazorEngineHelper.GetTemplateCacheName(templateName, content);
        }

        #endregion

        /// <summary>
        /// Add template to cache
        /// </summary>
        /// <param name="template"></param>
        /// <param name="cacheName"></param>
        /// <param name="type"></param>
        /// <param name="resolveType"></param>
        public static ITemplateKey TryCompileAndAddTemplate(string template, string cacheName, Type type, ResolveType resolveType = ResolveType.Global)
        {
            return EzRazorEngineHelper.TryCompileAndAddTemplate(template, cacheName, type, resolveType);
        }

        /// <summary>
        /// Validate template
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ValidateTemplate(string name, string content, Type type)
        {
            return EzRazorEngineHelper.ValidateTemplate(name, content, type);
        }

        #region File Template Helpers

        /// <summary>
        /// Check if virtual path is Db template or not
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static bool IsDBTemplateMaster(this string virtualPath)
        {
            var templates = virtualPath.Split('/').Last().Split('.');
            if (!templates.First().Equals(EzCMSContants.DbTemplate, StringComparison.InvariantCultureIgnoreCase) || templates.Count() < 3)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check if virtual path is Db template or not
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static string GetDBTemplateMaster(this string virtualPath)
        {
            var templates = virtualPath.Split('/').Last().Split('.');
            if (!templates.First().Equals(EzCMSContants.DbTemplate, StringComparison.InvariantCultureIgnoreCase) || templates.Count() < 3)
            {
                return null;
            }
            return templates[1];
        }

        #endregion

        #region Page Content Helpers

        /// <summary>
        /// Parse @RenderBody() of Layout to @Raw(Model.Content) for rendering
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ParseRenderBodyToRenderPageContent(this string content)
        {
            content = content.Replace(EzCMSContants.RenderBody, EzCMSContants.RenderPageContent);
            return content;
        }

        /// <summary>
        /// Add parent layout to page template
        /// </summary>
        /// <param name="content"></param>
        /// <param name="masterPage"></param>
        /// <returns></returns>
        public static string InsertMasterPage(this string content, string masterPage)
        {
            return "@{ this.Layout = \"" + masterPage + "\";}" + content;
        }

        /// <summary>
        /// Remove widget tag
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveWidgetTag(this string content)
        {
            if (string.IsNullOrEmpty(content)) return content;

            return content.Replace("<widget>", string.Empty).Replace("<widget contenteditable=\"false\">", string.Empty).Replace("</widget>", string.Empty);
        }

        #endregion
    }
}
