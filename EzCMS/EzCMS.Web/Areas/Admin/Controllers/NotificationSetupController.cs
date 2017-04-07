using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Models.Notifications.Setup;
using EzCMS.Core.Models.Notifications.Setup.NotificationSetup;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.Contacts;
using EzCMS.Core.Services.Notifications;
using EzCMS.Core.Services.NotificationTemplates;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize]
    public class NotificationSetupController : BackendController
    {
        private readonly INotificationService _notificationService;
        private readonly IContactService _contactService;
        private readonly INotificationTemplateService _notificationTemplateService;

        public NotificationSetupController(INotificationService notificationService, IContactService contactService, INotificationTemplateService notificationTemplateService)
        {
            _notificationService = notificationService;
            _contactService = contactService;
            _notificationTemplateService = notificationTemplateService;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Initialize Notification

        public ActionResult InitializeNotification(NotificationInitializeModel model)
        {
            if (ModelState.IsValid)
            {
                if (!model.NotificationConfiguration.DisableNotifyContacts)
                {
                    var response = _notificationService.SaveNotificationInitialize(model);
                    if (response.Success)
                    {
                        var id = (int)response.Data;
                        return RedirectToAction("ContactGroup", new { id });
                    }
                }
            }

            SetErrorMessage(ModelState.BuildValidationMessages());
            return View("ShowMessageAndCloseFancyBox", new CloseFancyBoxViewModel
            {
                IsReload = false
            });
        }

        #endregion

        #region Contact Group

        public ActionResult ContactGroup(int id)
        {
            var model = _notificationService.GetContactGroupSetupModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Notification_Message_ObjectNotFound"));
                return View("ShowMessageAndCloseFancyBox", new CloseFancyBoxViewModel
                {
                    IsReload = false
                });
            }

            return View("Steps/ContactGroup", model);
        }

        /// <summary>
        /// Add contact search model to a specific notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ContactGroup(ContactGroupSetupModel model)
        {
            if (ModelState.IsValid)
            {
                string message;
                var statisticModel = _notificationService.SaveContactGroup(model, out message);

                if (statisticModel != null)
                {
                    statisticModel.ContactNotificationSearchPartial = RenderPartialViewToString("Partials/_SearchQueries",
                        statisticModel.ContactSearchDetailsModel);

                    return Json(new ResponseModel
                    {
                        Success = true,
                        Message = message,
                        Data = statisticModel
                    });
                }

                return Json(new ResponseModel
                {
                    Success = false,
                    Message = message
                });
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        /// <summary>
        /// Render search queries from notification id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RenderSearchQueries(int id)
        {
            var contactsNumber = _notificationService.CountContacts(id);
            var data = new StringBuilder();

            var contactNotificationSearchDetailsModels = _notificationService.GetContactSearchDetailsModels(id);
            foreach (var contactNotificationSearchDetailsModel in contactNotificationSearchDetailsModels)
            {
                data.Append(RenderPartialViewToString("Partials/_SearchQueries", contactNotificationSearchDetailsModel));
            }

            if (contactsNumber > 0)
            {
                return Json(new ResponseModel
                {
                    Success = true,
                    Message = contactsNumber.ToString(CultureInfo.InvariantCulture),
                    Data = data.ToString()
                });
            }

            return Json(new ResponseModel
            {
                Success = false
            });
        }

        /// <summary>
        /// Change contact group of a notification to an existing contact group
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="contactGroupId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ChangeContactGroup(int notificationId, int contactGroupId)
        {
            return Json(_notificationService.ChangeContactGroup(notificationId, contactGroupId));
        }

        /// <summary>
        /// Check if there are any contact group in a notification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult IsNotificationSetupFilter(int id)
        {
            return Json(_notificationService.IsNotificationSetupFilter(id));
        }

        /// <summary>
        /// Ajax binding for search grid
        /// </summary>
        /// <param name="si"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, ContactSearchModel searchModel)
        {
            return JsonConvert.SerializeObject(_contactService.SearchContacts(si, searchModel));
        }

        /// <summary>
        /// Count contacts in query
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public JsonResult CountContacts(ContactSearchModel searchModel)
        {
            var contacts = _notificationService.CountContacts(searchModel);
            return Json(contacts, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Notification Template

        public ActionResult NotificationTemplate(int id)
        {
            var model = _notificationService.GetNotificationTemplateSetupModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Notification_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View("Steps/NotificationTemplate", model);
        }

        /// <summary>
        /// Add notification template manage model to a specific notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult NotificationTemplate(NotificationTemplateSetupModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_notificationService.SaveNotificationTemplate(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        /// <summary>
        /// Get notification template manage model from id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetNotificationTemplate(int id)
        {
            var notificationTemplateManageModel = _notificationTemplateService.GetNotificationTemplateManageModel(id);
            return Json(notificationTemplateManageModel);
        }

        #endregion

        #region Saving Configuration

        public ActionResult SavingNotificationConfiguration(int id)
        {
            var model = _notificationService.GetNotificationConfigurationSetupModel(id);

            if (model == null)
            {
                SetErrorMessage(T("Notification_Message_ObjectNotFound"));
                return View("ShowMessageAndCloseFancyBox", new CloseFancyBoxViewModel
                {
                    IsReload = false
                });
            }

            return View("Steps/SavingNotificationConfiguration", model);
        }

        /// <summary>
        /// Save notification template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SavingNotificationConfiguration(NotificationConfigurationSetupModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_notificationService.SaveNotificationConfiguration(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Notification Confirmation

        [HttpPost]
        public JsonResult ConfirmNotification(int id, string contactGroupName, string notificationTemplateName, DateTime? sendTime)
        {
            var model = _notificationService.GetNotificationConfirmationModel(id);

            model.ContactGroupName = contactGroupName;
            model.NotificationTemplateName = notificationTemplateName;
            model.SendTime = sendTime;

            return Json(new ResponseModel
            {
                Success = true,
                Data = RenderPartialViewToString("Partials/ConfirmNotification", model)
            });

        }

        #endregion

        #endregion
    }
}