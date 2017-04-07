using Ez.Framework.Configurations;
using Ez.Framework.Core.Mvc.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.WebPages;

namespace Ez.Framework.Core.Mvc.Helpers
{
    /// <summary>
    /// Methods for helping to manage scripts in partials and templates.
    /// </summary>
    public static class ScriptHelper
    {
        #region Adding Scripts

        /// <summary>
        /// Add script block
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="script"></param>
        public static void AddScriptBlock(this HtmlHelper htmlHelper, string script)
        {
            AddToScriptContext(context =>
                    context.ScriptBlocks.Add("<script type='text/javascript'>" + script + "</script>" +
                                             Environment.NewLine));
        }

        /// <summary>
        /// Add script block
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="script"></param>
        public static void AddScriptBlock(this HtmlHelper htmlHelper, IHtmlString script)
        {
            AddToScriptContext(context => context.ScriptBlocks.Add(script + Environment.NewLine));
        }

        /// <summary>
        /// Add script block
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="scriptTemplate"></param>
        public static void AddScriptBlock(this HtmlHelper htmlHelper, Func<dynamic, HelperResult> scriptTemplate)
        {
            AddToScriptContext(context =>
                               context.ScriptBlocks.Add(scriptTemplate(null).ToString() + Environment.NewLine));
        }

        /// <summary>
        /// Insert a required script on top of the scripts block
        /// </summary>
        /// <param name="htmlHelper">the <see cref="HtmlHelper"/></param>
        /// <param name="virtualPath">Virtual path or name of resource script to include</param>
        public static void RequiredScript(this HtmlHelper htmlHelper, string virtualPath)
        {
            AddToScriptContext(context => context.Require(virtualPath));
        }

        /// <summary>
        /// Include a script or resource to page
        /// </summary>
        /// <param name="htmlHelper">the <see cref="HtmlHelper"/></param>
        /// <param name="virtualPath">Path or resource name of script to include</param>
        public static void IncludeScript(this HtmlHelper htmlHelper, string virtualPath)
        {
            AddToScriptContext(context => context.Include(virtualPath));
        }

        #region Private Methods

        /// <summary>
        /// Add to script context
        /// </summary>
        /// <param name="action"></param>
        private static void AddToScriptContext(Action<ScriptContext> action)
        {
            var scriptContext = HttpContext.Current.Items[ScriptContext.ScriptContextItem] as ScriptContext;

            if (scriptContext != null)
            {
                action(scriptContext);
                scriptContext.Dispose();
            }
        }

        #endregion

        #endregion

        #region Render

        /// <summary>
        /// Begin script context
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static ScriptContext BeginScriptContext(this HtmlHelper htmlHelper)
        {
            var scriptContext = new ScriptContext();
            HttpContext.Current.Items[ScriptContext.ScriptContextItem] = scriptContext;

            return scriptContext;
        }

        /// <summary>
        /// Render scripts
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
        {
            Func<string[], IHtmlString> scriptPathResolver =
                paths => new HtmlString(Scripts.Render(paths).ToHtmlString());

            return RenderScripts(htmlHelper, scriptPathResolver);
        }

        /// <summary>
        /// Render scripts
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="scriptPathResolver"></param>
        /// <returns></returns>
        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper, Func<string[], IHtmlString> scriptPathResolver)
        {
            var scriptContexts = HttpContext.Current.Items[ScriptContext.ScriptContextItems] as Stack<ScriptContext>;

            if (scriptContexts != null)
            {
                var builder = new StringBuilder();
                var script = new List<string>();

                var requireFilesToBuild = new List<string>();
                var filesToBuild = new List<string>();
                foreach (var context in scriptContexts.Reverse())
                {
                    requireFilesToBuild.AddRange(context.ScriptPaths[ScriptContext.RequiredKey]
                                                     .Where(required => !requireFilesToBuild.Contains(required)));

                    filesToBuild.AddRange(context.ScriptPaths[ScriptContext.IncludedKey]
                                              .Where(file => !filesToBuild.Contains(file)));

                    foreach (var block in context.ScriptBlocks
                        .Where(block => !script.Contains(block)))
                    {
                        script.Add(block);
                    }
                }

                requireFilesToBuild.AddRange(filesToBuild);

                builder.Append(scriptPathResolver(requireFilesToBuild.Distinct().ToArray()).ToString());

                foreach (var block in script)
                {
                    builder.Append(block);
                }

                return new HtmlString(builder.ToString());
            }

            return MvcHtmlString.Empty;
        }

        #endregion

        /// <summary>
        /// Google Maps script url
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="culture"></param>
        /// <param name="libraries"></param>
        /// <returns></returns>
        public static string GoogleMapFile(this HtmlHelper helper, string culture, params string[] libraries)
        {
            var libraryString = libraries != null ? string.Join(",", libraries) : string.Empty;
            return string.Format("http://maps.google.com/maps/api/js?sensor=false&language={0}&libraries={1}&key={2}", culture, libraryString, FrameworkConstants.GoogleApikey);
        }

        /// <summary>
        /// Include Google Maps script
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="libraries"></param>
        /// <returns></returns>
        public static MvcHtmlString GoogleMapScript(this HtmlHelper helper, string culture, params string[] libraries)
        {
            var tag = new TagBuilder("script");
            var libraryString = (libraries != null && libraries.Any()) ? string.Join(",", libraries) : "places";
            tag.Attributes["src"] = string.Format("http://maps.google.com/maps/api/js?sensor=false&language={0}&libraries={1}&key={2}", culture, libraryString, FrameworkConstants.GoogleApikey);

            return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
        }
    }
}
