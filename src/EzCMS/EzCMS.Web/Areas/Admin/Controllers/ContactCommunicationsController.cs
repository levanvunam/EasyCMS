using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.ContactCommunications;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.ContactCommunications;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class ContactCommunicationsController : BackendController
    {
        private readonly IContactCommunicationService _contactCommunicationService;
        public ContactCommunicationsController(IContactCommunicationService contactCommunicationService)
        {
            _contactCommunicationService = contactCommunicationService;
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int contactId)
        {
            return JsonConvert.SerializeObject(_contactCommunicationService.SearchContactCommunications(si, contactId));
        }

        /// <summary>
        /// Export contact communications
        /// </summary>
        /// <returns></returns>
        public ActionResult Exports(int contactId)
        {
            var workbook = _contactCommunicationService.Exports(contactId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "ContactCommunications.xls");
        }

        [HttpPost]
        public JsonResult DeleteContactCommunication(int id)
        {
            return Json(_contactCommunicationService.DeleteContactCommunication(id));
        }

        #region Create
        public ActionResult Create(int contactId)
        {
            SetupPopupAction();
            var model = _contactCommunicationService.GetContactCommunicationManageModel(contactId);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ContactCommunicationManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _contactCommunicationService.SaveContactCommunication(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var id = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox",
                                new CloseFancyBoxViewModel());

                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("Edit", new { id = id, contactId = model.ContactId });
                    }
                }
            }
            SetupPopupAction();
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("Contact_Communication_Edit", "Contact_Communications", "Contact_Communication_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            SetupPopupAction();
            var model = _contactCommunicationService.GetContactCommunicationManageModel(null, id);
            if (model == null)
            {
                SetErrorMessage(T("ContactCommunication_Message_ObjectNotFound"));
                return View("CloseFancyBox",
                    new CloseFancyBoxViewModel
                    {
                        IsReload = true
                    });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ContactCommunicationManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _contactCommunicationService.SaveContactCommunication(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox",
                                new CloseFancyBoxViewModel());

                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("Edit", new { id = model.Id });
                    }
                }
            }
            SetupPopupAction();
            return View(model);
        }

        #endregion
    }
}
