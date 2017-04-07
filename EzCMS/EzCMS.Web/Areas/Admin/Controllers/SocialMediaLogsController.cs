using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Services.SocialMediaLogs;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class SocialMediaLogsController : BackendController
    {
        private readonly ISocialMediaLogService _socialMediaLogService;
        public SocialMediaLogsController(ISocialMediaLogService socialMediaLogService)
        {
            _socialMediaLogService = socialMediaLogService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int? socialMediaTokenId, int? socialMediaId)
        {
            return JsonConvert.SerializeObject(_socialMediaLogService.SearchSocialMediaLogs(si, socialMediaTokenId, socialMediaId));
        }


        /// <summary>
        /// Export Social Media Logs
        /// </summary>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, int? socialMediaTokenId, GridExportMode gridExportMode, int? socialMediaId)
        {
            var workbook = _socialMediaLogService.Exports(si, gridExportMode, socialMediaTokenId, socialMediaId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "SocialMediaLogs.xls");
        }
    }
}
