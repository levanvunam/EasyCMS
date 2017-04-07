using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.WebPages;
using Ez.Framework.Core.Mvc.Helpers;

namespace Ez.Framework.Core.Resources
{
    public class ResourceStyleManager : SiteResourceManager
    {
        public ResourceStyleManager(WebViewPage pageView, HtmlHelper htmlHelper)
            : base(pageView, htmlHelper)
        {
            HtmlHelper.BeginStyleContext();
        }

        /// <summary>
        /// Add required style files to page
        /// </summary>
        /// <param name="requiredStyles">Required style files</param>
        /// <returns></returns>
        public ResourceStyleManager Required(params string[] requiredStyles)
        {
            if (requiredStyles != null)
            {
                foreach (var requiredStyle in requiredStyles)
                {
                    HtmlHelper.RequireStyle(requiredStyle);
                }
            }

            return this;
        }

        /// <summary>
        /// Include style files to page
        /// </summary>
        /// <param name="includeStyles">Style files to include</param>
        /// <returns></returns>
        public ResourceStyleManager Include(params string[] includeStyles)
        {
            if (includeStyles != null)
            {
                foreach (var includeScript in includeStyles)
                {
                    HtmlHelper.IncludeStyle(includeScript);
                }
            }

            return this;
        }

        /// <summary>
        /// Include a block of style to render on page
        /// </summary>
        /// <param name="styleTemplate">Style to include</param>
        /// <returns></returns>
        public ResourceStyleManager Block(Func<dynamic, HelperResult> styleTemplate)
        {
            HtmlHelper.AddStyleBlock(styleTemplate);
            return this;
        }

        /// <summary>
        /// Include a block of style to render on page
        /// </summary>
        /// <param name="styleTemplate">Style to include</param>
        /// <returns></returns>
        public ResourceStyleManager Block(string styleTemplate)
        {
            HtmlHelper.AddStyleBlock(styleTemplate);
            return this;
        }

        /// <summary>
        /// Add bundle styles
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        public ResourceStyleManager Bundle(params string[] bundles)
        {
            if (bundles != null)
            {
                foreach (var bundle in bundles)
                {
                    HtmlHelper.AddStyleBlock(Styles.Render(bundle));
                }
            }

            return this;
        }
    }
}