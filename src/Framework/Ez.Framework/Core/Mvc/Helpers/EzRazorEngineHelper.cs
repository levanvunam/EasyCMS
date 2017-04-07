using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Linq;

namespace Ez.Framework.Core.Mvc.Helpers
{
    public static class EzRazorEngineHelper
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
            Type type = model.GetType();

            // Add template to cache if not existed
            var key = TryCompileAndAddTemplate(template, cacheName, type, resolveType);

            // Parse the template
            return RazorEngineServiceExtensions.Run(Engine.Razor, key, type, model, viewBag);
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
            return CompileAndRun(template, model, null, cacheName, resolveType);
        }

        /// <summary>
        /// Get cache template name
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetTemplateCacheName(string templateName, string content)
        {
            return String.Format("{0}-{1}", templateName, content == null ? 0 : content.GetHashCode());
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
            // Get template key
            ITemplateKey key = Engine.Razor.GetKey(cacheName, resolveType);

            // Check if template is cached or not
            if (!Engine.Razor.IsTemplateCached(key, type))
            {
                Engine.Razor.AddTemplate(key, template);

                // Compile the template
                Engine.Razor.Compile(key, type);
            }

            return key;
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
            var razorValidMessage = string.Empty;
            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    var cacheName = GetTemplateCacheName(name, content);
                    TryCompileAndAddTemplate(content, cacheName, type);
                }
                catch (TemplateParsingException exception)
                {
                    razorValidMessage = exception.Message;
                }
                catch (TemplateCompilationException exception)
                {
                    razorValidMessage = string.Join("\n", exception.CompilerErrors.Select(e => e.ErrorText));
                }
                catch (Exception exception)
                {
                    razorValidMessage = exception.Message;
                }
            }
            return razorValidMessage;
        }
    }
}
