using EzCMS.Core.Framework.Configuration;
using EzCMS.Entity.Core.SiteInitialize;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EzCMS.Core.Framework.Mvc.ViewEngines
{
    public class EzCMSRazorEngine : RazorViewEngine
    {
        public EzCMSRazorEngine()
        {
            var siteConfig = SiteInitializer.GetConfiguration();
            var installedPlugins = siteConfig.Plugins.Select(m => m.Folder);

            AreaMasterLocationFormats = new[]
            {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.vbhtml"
            };

            AreaPartialViewLocationFormats = new[]
            {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.vbhtml"
            };

            var areaViewAndPartialViewLocationFormats = new List<string>
            {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.vbhtml"
            };

            var partialViewLocationFormats = new List<string>
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.vbhtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Shared/{0}.vbhtml"
            };

            var masterLocationFormats = new List<string>
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.vbhtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Shared/{0}.vbhtml"
            };

            foreach (var plugin in installedPlugins)
            {
                var plugInFolder = string.Format("~/{0}/{1}", EzCMSContants.PluginFolder, plugin);

                //Add master location
                masterLocationFormats.Add(
                    plugInFolder + "/Views/{1}/{0}.cshtml");
                masterLocationFormats.Add(
                    plugInFolder + "/Views/{1}/{0}.vbhtml");
                masterLocationFormats.Add(
                    plugInFolder + "/Views/Shared/{1}/{0}.cshtml");
                masterLocationFormats.Add(
                    plugInFolder + "/Views/Shared/{1}/{0}.vbhtml");

                //Add partial view location
                partialViewLocationFormats.Add(
                    plugInFolder + "/Views/{1}/{0}.cshtml");
                partialViewLocationFormats.Add(
                    plugInFolder + "/Views/{1}/{0}.vbhtml");
                partialViewLocationFormats.Add(
                    plugInFolder + "/Views/Shared/{0}.cshtml");
                partialViewLocationFormats.Add(
                    plugInFolder + "/Views/Shared/{0}.vbhtml");

                //Add area view location
                areaViewAndPartialViewLocationFormats.Add(
                    plugInFolder + "/Views/{1}/{0}.cshtml");
                areaViewAndPartialViewLocationFormats.Add(
                    plugInFolder + "/Views/{1}/{0}.vbhtml");
                areaViewAndPartialViewLocationFormats.Add(
                    plugInFolder + "/Areas/{2}/Views/{1}/{0}.cshtml");
                areaViewAndPartialViewLocationFormats.Add(
                    plugInFolder + "/Areas/{2}/Views/{1}/{0}.vbhtml");
                areaViewAndPartialViewLocationFormats.Add(
                    plugInFolder + "/Areas/{2}/Views/Shared/{0}.cshtml");
                areaViewAndPartialViewLocationFormats.Add(
                    plugInFolder + "/Areas/{2}/Views/Shared/{0}.vbhtml");
            }

            ViewLocationFormats = partialViewLocationFormats.ToArray();
            MasterLocationFormats = masterLocationFormats.ToArray();
            PartialViewLocationFormats = partialViewLocationFormats.ToArray();
            AreaPartialViewLocationFormats = areaViewAndPartialViewLocationFormats.ToArray();
            AreaViewLocationFormats = areaViewAndPartialViewLocationFormats.ToArray();
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return base.FindPartialView(controllerContext, partialViewName, useCache);
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return base.FindView(controllerContext, viewName, masterName, useCache);
        }
    }
}
