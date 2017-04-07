using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Utilities;
using System.Web.Mvc;
using System.Web.Routing;

namespace EzCMS.Web
{
    public class RouteConfig
    {
        public static string NameSpaces
        {
            get
            {
                return "EzCMS.Web.Controllers";
            }
        }
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRouteWithName(
                "Robots",
                "robots.txt",
                new
                {
                    controller = "Home",
                    action = "Robots"
                },
                new[] { NameSpaces });

            routes.MapRouteWithName(
                "SiteMap",
                "sitemap.xml",
                new
                {
                    controller = "Home",
                    action = "SiteMap"
                },
                new[] { NameSpaces });

            /*
             * Resource files
             */
            routes.MapRouteWithName(
                "CssResource",
                "Resources/{fileName}.css",
                new
                {
                    controller = "Resources",
                    action = "Style",
                    fileName = UrlParameter.Optional,
                },
                new[] { NameSpaces });

            routes.MapRouteWithName(
                "ScriptResource",
                "Resources/{fileName}.js",
                new
                {
                    controller = "Resources",
                    action = "Script",
                    fileName = UrlParameter.Optional,
                },
                new[] { NameSpaces });

            /*
             * Routing for some extend script file
             */
            routes.MapRouteWithName(
                "EditorStyleFile",
                "CkEditor/editor.css",
                new
                {
                    controller = "Pages",
                    action = "EditorStyleFile"
                },
                new[] { NameSpaces });

            routes.MapRouteWithName(
                "EditorScriptFile",
                "CkEditor/editor.js",
                new
                {
                    controller = "Pages",
                    action = "EditorScriptFile"
                },
                new[] { NameSpaces });

            //Default route
            routes.MapRouteWithName(
                "Default",
                "{controller}/{action}/{id}",
                new
                {
                    id = UrlParameter.Optional
                },
                new[] { NameSpaces }
                );

            //Empty route for home page
            routes.MapRouteWithName(
                "Empty",
                "",
                new
                {
                    controller = "Pages",
                    action = "Index",
                    url = string.Empty
                },
                new[] { NameSpaces });

            routes.MapRouteWithName(
                EzCMSContants.EzCMSPageFriendlyUrlRoute,
                "{*url}",
                new
                {
                    controller = "Pages",
                    action = "Index",
                    url = UrlParameter.Optional
                },
                new[] { NameSpaces });

        }
    }
}