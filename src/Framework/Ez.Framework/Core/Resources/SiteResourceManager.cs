using System;
using System.Web.Mvc;

namespace Ez.Framework.Core.Resources
{
    public abstract class SiteResourceManager : IDisposable
    {
        protected WebViewPage WebViewPage;
        protected HtmlHelper HtmlHelper;

        /// <summary>
        /// Create a new instance of Resouce Manager class
        /// </summary>
        /// <param name="webViewPage">The web page that currently loading and will use resource manager</param>
        /// <param name="htmlHelper"></param>
        protected SiteResourceManager(WebViewPage webViewPage, HtmlHelper htmlHelper)
        {
            WebViewPage = webViewPage;
            HtmlHelper = htmlHelper;
        }

        public void Dispose()
        {

        }
    }
}
