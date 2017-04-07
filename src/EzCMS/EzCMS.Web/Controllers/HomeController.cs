using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Models.Pages.Sitemap;
using EzCMS.Core.Services.ClientNavigations;
using EzCMS.Core.Services.SiteSettings;
using System.IO;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace EzCMS.Web.Controllers
{
    public class HomeController : ClientController
    {
        private readonly ISiteSettingService _siteSettingService;
        private readonly IClientNavigationService _clientNavigationService;

        public HomeController(ISiteSettingService siteSettingService, IClientNavigationService clientNavigationService)
        {
            _siteSettingService = siteSettingService;
            _clientNavigationService = clientNavigationService;
        }

        /// <summary>
        /// Return the content of RobotsSetting as a file.
        /// </summary>
        /// <returns></returns>
        public ActionResult Robots()
        {
            var enableLiveSiteMode = _siteSettingService.GetSetting<bool>(SettingNames.EnableLiveSiteMode);
            var robotsSetting = _siteSettingService.LoadSetting<RobotsSetting>();
            if (enableLiveSiteMode)
            {
                return Content(robotsSetting.LiveSiteModeContent, "text/plain");
            }

            return Content(robotsSetting.TestSiteModeContent, "text/plain");
        }

        /// <summary>
        /// Sitemap.xml
        /// </summary>
        /// <returns></returns>
        public ActionResult SiteMap()
        {
            var googleSiteMap = _clientNavigationService.GenerateGoogleSiteMap();
            var serializer = new XmlSerializer(typeof(GoogleSiteMap), "http://www.sitemaps.org/schemas/sitemap/0.9");
            var stream = new MemoryStream();
            serializer.Serialize(stream, googleSiteMap);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "text/xml");
        }
    }
}
