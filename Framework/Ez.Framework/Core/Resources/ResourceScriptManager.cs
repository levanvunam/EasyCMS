using Ez.Framework.Core.Mvc.Helpers;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.WebPages;

namespace Ez.Framework.Core.Resources
{
    public class ResourceScriptManager : SiteResourceManager
    {
        public ResourceScriptManager(WebViewPage pageView, HtmlHelper htmlHelper)
            : base(pageView, htmlHelper)
        {
            HtmlHelper.BeginScriptContext();
        }

        /// <summary>
        /// Add required script files to page
        /// </summary>
        /// <param name="requiredScripts">Required script files</param>
        /// <returns></returns>
        public ResourceScriptManager Required(params string[] requiredScripts)
        {
            if (requiredScripts != null)
            {
                foreach (var requiredScript in requiredScripts)
                {
                    HtmlHelper.RequiredScript(requiredScript);
                }
            }

            return this;
        }

        /// <summary>
        /// Include script files to page
        /// </summary>
        /// <param name="includeScripts">Script files to include</param>
        /// <returns></returns>
        public ResourceScriptManager Include(params string[] includeScripts)
        {
            if (includeScripts != null)
            {
                foreach (var includeScript in includeScripts)
                {
                    HtmlHelper.IncludeScript(includeScript);
                }
            }

            return this;
        }

        /// <summary>
        /// Include a block of script to page
        /// </summary>
        /// <param name="blockScript">Script to include</param>
        /// <returns></returns>
        public ResourceScriptManager Block(string blockScript)
        {
            HtmlHelper.AddScriptBlock(blockScript);

            return this;
        }

        /// <summary>
        /// Include a block of script to page using Html helper
        /// </summary>
        /// <param name="scriptTemplate">Script to include</param>
        /// <returns></returns>
        public ResourceScriptManager Block(Func<dynamic, HelperResult> scriptTemplate)
        {
            HtmlHelper.AddScriptBlock(scriptTemplate);

            return this;
        }

        /// <summary>
        /// Add bundle scripts
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        public ResourceScriptManager Bundle(params string[] bundles)
        {
            if (bundles != null)
            {
                foreach (var bundle in bundles)
                {
                    HtmlHelper.AddScriptBlock(Scripts.Render(bundle));
                }
            }

            return this;
        }
    }
}