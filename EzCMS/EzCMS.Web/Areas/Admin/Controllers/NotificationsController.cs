using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Services.Notifications;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class NotificationsController : BackendController
    {
        private readonly INotificationService _notificationService;
        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        [AdministratorNavigation("Notification_Holder", "Module_Holder", "Notification_Holder", "fa-bullhorn", 100, true, true)]
        [AdministratorNavigation("Notifications", "Notification_Holder", "Notifications", "fa-bullhorn", 10)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_notificationService.SearchNotifications(si));
        }

        /// <summary>
        /// Export notifications
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _notificationService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Notifications.xls");
        }

        #region Details

        [AdministratorNavigation("Notification_Details", "Notifications", "Notification_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _notificationService.GetNotificationDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Notification_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public string _AjaxBindingForNotifiedContacts(JqSearchIn si, int notificationId)
        {
            return JsonConvert.SerializeObject(_notificationService.SearchNotifiedContacts(si, notificationId));
        }

        /// <summary>
        /// Export notified contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public ActionResult ExportsNotifiedContacts(JqSearchIn si, GridExportMode gridExportMode, int notificationId)
        {
            var workbook = _notificationService.ExportsNotifiedContacts(si, gridExportMode, notificationId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "NotifiedContacts.xls");
        }

        #endregion
    }
}
