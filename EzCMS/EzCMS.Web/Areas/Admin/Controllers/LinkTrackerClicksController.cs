using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Services.LinkTrackerClicks;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize]
    public class LinkTrackerClicksController : BackendController
    {
        private readonly ILinkTrackerClickService _linkTrackerClickService;
        public LinkTrackerClicksController(ILinkTrackerClickService linkTrackerClickService)
        {
            _linkTrackerClickService = linkTrackerClickService;
        }

        [AdministratorNavigation("Link_Tracker_Clicks", "LinkTracker_Holder", "Link_Tracker_Clicks", "fa-external-link-square", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int? linkTrackerId)
        {
            return JsonConvert.SerializeObject(_linkTrackerClickService.SearchLinkTrackerClicks(si, linkTrackerId));
        }

        /// <summary>
        /// Export link tracker clicks
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="linkTrackerId"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, int? linkTrackerId)
        {
            var workbook = _linkTrackerClickService.Exports(si, gridExportMode, linkTrackerId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "LinkTrackerClicks.xls");
        }

        #region Details

        [AdministratorNavigation("Link_Tracker_Click_Details", "Link_Tracker_Clicks", "Link_Tracker_Click_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _linkTrackerClickService.GetLinkTrackerClickDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("LinkTrackerClick_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteLinkTrackerClick(int id)
        {
            return Json(_linkTrackerClickService.DeleteLinkTrackerClick(id));
        }

        #endregion

        #endregion
    }
}
