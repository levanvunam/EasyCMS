using Ez.Framework.Core.JqGrid;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Services.PageLogs;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class PageLogsController : BackendController
    {
        private readonly IPageLogService _pageLogService;
        public PageLogsController(IPageLogService pageLogService)
        {
            _pageLogService = pageLogService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_pageLogService.SearchPageLogs(si));
        }
    }
}
