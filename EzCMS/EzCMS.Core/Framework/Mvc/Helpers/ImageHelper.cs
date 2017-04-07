using EzCMS.Core.Framework.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EzCMS.Core.Framework.Mvc.Helpers
{
    public static class ImageHelper
    {
        public static IHtmlString Image(this HtmlHelper helper,
            string url,
            string altText = "",
            object htmlAttributes = null)
        {
            if (string.IsNullOrEmpty(url)) url = EzCMSContants.NoImage;

            url = url.Replace("~", string.Empty);

            var builder = new TagBuilder("img");

            builder.Attributes.Add("src", url);

            if (!string.IsNullOrEmpty(altText))
            {
                builder.Attributes.Add("alt", altText);
            }
            builder.Attributes.Add("title", altText);

            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return new HtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }
    }
}
