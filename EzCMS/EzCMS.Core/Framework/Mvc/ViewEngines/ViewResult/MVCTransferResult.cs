using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EzCMS.Core.Framework.Mvc.ViewEngines.ViewResult
{
    public class MVCTransferResult : RedirectResult
    {
        public MVCTransferResult(string url)
            : base(url)
        {
        }

        public MVCTransferResult(object routeValues, string parameters)
            : base(GetRouteUrl(routeValues, parameters))
        {
        }

        private static string GetRouteUrl(object routeValues, string parameters)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var url = urlHelper.RouteUrl(routeValues);
            return string.Format("{0}{1}", url, string.IsNullOrEmpty(parameters) ? string.Empty : string.Format("?{0}", parameters));
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var httpContext = HttpContext.Current;

            // ASP.NET MVC 3.0
            if (context.Controller.TempData != null &&
                context.Controller.TempData.Any())
            {
                throw new ApplicationException("TempData won't work with Server.TransferRequest!");
            }

            httpContext.Server.TransferRequest(Url, false); // change to false to pass query string parameters if you have already processed them
        }
    }
}
