using EzCMS.Core.Framework.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EzCMS.Core.Framework.Utilities
{
    public static class RouteUtilities
    {
        #region Setup route name

        /// <summary>
        /// Map route and add RouteName token
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public static Route MapRouteWithName(this RouteCollection routes, string name, string url, object defaults, string[] namespaces)
        {
            Route route = routes.MapRoute(name, url, defaults, namespaces);

            return route.SetupRouteName(name);
        }

        /// <summary>
        /// Map route and add RouteName token
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="constraints"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public static Route MapRouteWithName(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces)
        {
            Route route = routes.MapRoute(name, url, defaults, constraints, namespaces);

            return route.SetupRouteName(name);
        }

        /// <summary>
        /// Map route and add RouteName token
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public static Route MapAreaRouteWithName(this AreaRegistrationContext context, string name, string url, object defaults, string[] namespaces)
        {
            Route route = context.MapRoute(name, url, defaults, namespaces);

            return route.SetupRouteName(name);
        }

        /// <summary>
        /// Map route and add RouteName token
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="constraints"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public static Route MapAreaRouteWithName(this AreaRegistrationContext context, string name, string url, object defaults, object constraints, string[] namespaces)
        {
            Route route = context.MapRoute(name, url, defaults, constraints, namespaces);

            return route.SetupRouteName(name);
        }

        /// <summary>
        /// Add RouteName token to route
        /// </summary>
        /// <param name="route"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Route SetupRouteName(this Route route, string name)
        {
            if (route.DataTokens == null)
            {
                route.DataTokens = new RouteValueDictionary();
            }

            route.DataTokens.Add("RouteName", name);

            return route;
        }

        #endregion

        #region Get route from url

        /// <summary>
        /// Get route from url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static RouteData GetRouteDataByUrl(string url)
        {
            return RouteTable.Routes.GetRouteData(new RewritedHttpContextBase(url));
        }

        #endregion

        /// <summary>
        /// Check if current friendly route is match with mvc route in the application
        /// </summary>
        /// <returns></returns>
        public static bool IsFriendlyUrlValid(this string friendlyUrl)
        {
            // Route for home
            if (string.IsNullOrEmpty(friendlyUrl))
            {
                return true;
            }

            var url = string.Format("~/{0}", friendlyUrl);
            var route = GetRouteDataByUrl(url);

            // if url is map into page routing then this url is valid
            if (route != null && route.DataTokens["RouteName"] != null && route.DataTokens["RouteName"].Equals(EzCMSContants.EzCMSPageFriendlyUrlRoute))
            {
                return true;
            };

            return false;
        }
    }

    internal class RewritedHttpContextBase : HttpContextBase
    {
        private readonly HttpRequestBase _mockHttpRequestBase;

        public RewritedHttpContextBase(string appRelativeUrl)
        {
            _mockHttpRequestBase = new MockHttpRequestBase(appRelativeUrl);
        }

        public override HttpRequestBase Request
        {
            get
            {
                return _mockHttpRequestBase;
            }
        }

        private class MockHttpRequestBase : HttpRequestBase
        {
            private readonly string _appRelativeUrl;

            public MockHttpRequestBase(string appRelativeUrl)
            {
                _appRelativeUrl = appRelativeUrl;
            }

            public override string AppRelativeCurrentExecutionFilePath
            {
                get { return _appRelativeUrl; }
            }

            public override string PathInfo
            {
                get { return ""; }
            }
        }
    }
}
