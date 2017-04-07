using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.WebPages;
using Ez.Framework.Core.Mvc.Context;

namespace Ez.Framework.Core.Mvc.Helpers
{
    /// <summary>
    /// Methods for helping to manage stylesheet in partials and templates.
    /// </summary>
    public static class StyleHelper
    {
        #region Adding Styles

        /// <summary>
        /// Add style block
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="style"></param>
        public static void AddStyleBlock(this HtmlHelper htmlHelper, string style)
        {
            AddToStyleContext(context => context.StyleBlocks.Add("<style>" + style + "</style>" + Environment.NewLine));
        }

        /// <summary>
        /// Add style block
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="style"></param>
        public static void AddStyleBlock(this HtmlHelper htmlHelper, IHtmlString style)
        {
            AddToStyleContext(context => context.StyleBlocks.Add(style + Environment.NewLine));
        }

        /// <summary>
        /// Add style block
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="styleTemplate"></param>
        public static void AddStyleBlock(this HtmlHelper htmlHelper, Func<dynamic, HelperResult> styleTemplate)
        {
            AddToStyleContext(context => context.StyleBlocks.Add(styleTemplate(null).ToString()));
        }

        /// <summary>
        /// Include a style to page
        /// </summary>
        /// <param name="htmlHelper">the <see cref="HtmlHelper"/></param>
        /// <param name="virtualPath">Path or resource name of script to include</param>
        public static void RequireStyle(this HtmlHelper htmlHelper, string virtualPath)
        {
            AddToStyleContext(context => context.Require(virtualPath));
        }

        /// <summary>
        /// Include a style to page
        /// </summary>
        /// <param name="htmlHelper">the <see cref="HtmlHelper"/></param>
        /// <param name="virtualPath">Path or resource name of script to include</param>
        public static void IncludeStyle(this HtmlHelper htmlHelper, string virtualPath)
        {
            AddToStyleContext(context => context.Include(virtualPath));
        }

        #region Private Methods

        /// <summary>
        /// Add to style context
        /// </summary>
        /// <param name="action"></param>
        private static void AddToStyleContext(Action<StyleContext> action)
        {
            var styleContext = HttpContext.Current.Items[StyleContext.StyleContextItem] as StyleContext;

            if (styleContext != null)
            {
                action(styleContext);
                styleContext.Dispose();
            }
        }

        #endregion

        #endregion

        #region Render

        /// <summary>
        /// Begin style context
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static StyleContext BeginStyleContext(this HtmlHelper htmlHelper)
        {
            var styleContext = new StyleContext();
            HttpContext.Current.Items[StyleContext.StyleContextItem] = styleContext;
            return styleContext;
        }

        /// <summary>
        /// Render styles
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static IHtmlString RenderStyles(this HtmlHelper htmlHelper)
        {
            Func<string[], IHtmlString> stylesPathResolver = paths => new HtmlString(Styles.Render(paths).ToHtmlString());

            return RenderStyles(htmlHelper, stylesPathResolver);
        }

        /// <summary>
        /// Render styles
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="stylesPathResolver"></param>
        /// <returns></returns>
        public static IHtmlString RenderStyles(this HtmlHelper htmlHelper, Func<string[], IHtmlString> stylesPathResolver)
        {
            var styleContexts = HttpContext.Current.Items[StyleContext.StyleContextItems] as Stack<StyleContext>;

            if (styleContexts != null)
            {
                var builder = new StringBuilder();
                var script = new List<string>();

                var requireFilesToBuild = new List<string>();
                var filesToBuild = new List<string>();
                foreach (var context in styleContexts.Reverse())
                {
                    requireFilesToBuild.AddRange(context.StyleSheetPaths[StyleContext.RequiredKey]
                                                     .Where(required => !requireFilesToBuild.Contains(required)));

                    filesToBuild.AddRange(context.StyleSheetPaths[StyleContext.IncludedKey]
                                              .Where(file => !filesToBuild.Contains(file)));


                    foreach (var block in context.StyleBlocks.Where(block => !script.Contains(block)))
                    {
                        script.Add(block);
                    }
                }
                requireFilesToBuild.AddRange(filesToBuild);
                builder.Append(stylesPathResolver(requireFilesToBuild.Distinct().ToArray()).ToString());

                foreach (var block in script)
                {
                    builder.Append(block);
                }

                return new HtmlString(builder.ToString());
            }

            return MvcHtmlString.Empty;
        }

        #endregion
    }
}