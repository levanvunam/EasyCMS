using Ez.Framework.Core.JqGrid;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Services.Tools;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    public class ToolsController : BackendController
    {
        private readonly IToolService _toolService;

        public ToolsController(IToolService toolService)
        {
            _toolService = toolService;
        }

        #region Email Converter

        [EzCMSAuthorize(IsAdministrator = true)]

        [AdministratorNavigation("Tool_Holder", "System_Setting_Holder", "Tool_Holder", "fa-wrench", 80, true, true)]
        [AdministratorNavigation("Tools_Email_Converter", "Tool_Holder", "Tools_Email_Converter", "fa-envelope", 10)]
        public ActionResult EmailConverter()
        {
            return View("EmailConverter/EmailConverter");
        }

        [EzCMSAuthorize(IsAdministrator = true)]
        public ActionResult EmailConverterHelp()
        {
            return View("EmailConverter/Help");
        }

        [EzCMSAuthorize(IsAdministrator = true)]
        public ActionResult EmailConverterResult()
        {
            return View("EmailConverter/Result");
        }

        #endregion

        #region Session Manager

        [EzCMSAuthorize]
        [AdministratorNavigation("Tools_Session_Manager", "Tool_Holder", "Tools_Session_Manager", "fa-key", 30)]
        public ActionResult SessionManager()
        {
            return View("SessionManager/Index");
        }

        [HttpGet]
        [EzCMSAuthorize]
        public string _SessionAjaxBinding(JqSearchIn si, string key)
        {
            return JsonConvert.SerializeObject(_toolService.SearchSessions(si, key));
        }

        #endregion

        #region Cookie Manager

        [EzCMSAuthorize]
        [AdministratorNavigation("Tools_Cookie_Manager", "Tool_Holder", "Tools_Cookie_Manager", "fa-key", 40)]
        public ActionResult CookieManager()
        {
            return View("CookieManager/Index");
        }

        [HttpGet]
        [EzCMSAuthorize]
        public string _CookieAjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_toolService.SearchCookies(si));
        }

        #endregion
    }
}
