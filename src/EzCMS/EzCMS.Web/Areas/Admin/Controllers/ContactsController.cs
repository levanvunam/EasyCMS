using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.Contacts;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class ContactsController : BackendController
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        #region Grid

        [AdministratorNavigation("Contact_Holder", "", "Contact_Holder", "fa-users", 50)]
        [AdministratorNavigation("Contacts_Settings", "Contact_Holder", "Contacts_Settings", "fa-users", 40, true, true)]
        [AdministratorNavigation("Contacts", "Contact_Holder", "Contacts", "fa-users", 10)]
        public ActionResult Index()
        {
            var model = new ContactSearchModel();
            return View(model);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, ContactSearchModel searchModel)
        {
            return JsonConvert.SerializeObject(_contactService.SearchContacts(si, searchModel));
        }

        [HttpGet]
        public string _AjaxBindingByUser(JqSearchIn si, int userId)
        {
            return JsonConvert.SerializeObject(_contactService.SearchContactsByUser(si, userId));
        }

        public ActionResult ExportsContactsByUser(JqSearchIn si, GridExportMode gridExportMode, int userId)
        {
            var workbook = _contactService.ExportsContactsByUser(si, gridExportMode, userId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Contacts.xls");
        }

        [HttpGet]
        public string _AjaxBindingByContactGroup(JqSearchIn si, int contactGroupId)
        {
            return JsonConvert.SerializeObject(_contactService.SearchContactsByContactGroup(si, contactGroupId));
        }

        public ActionResult ExportsContactsByContactGroup(JqSearchIn si, GridExportMode gridExportMode, int contactGroupId)
        {
            var workbook = _contactService.ExportsContactsByContactGroup(si, gridExportMode, contactGroupId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Contacts.xls");
        }

        public JsonResult SearchContacts(string term)
        {
            var data = _contactService.SearchContacts(term);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Export contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, ContactSearchModel searchModel)
        {
            var workbook = _contactService.Exports(si, gridExportMode, searchModel);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Contacts.xls");
        }

        [HttpPost]
        public JsonResult DeleteContact(int id)
        {
            return Json(_contactService.DeleteContact(id));
        }

        #endregion

        #region Create		
        [AdministratorNavigation("Contact_Create", "Contacts", "Contact_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _contactService.GetContactManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ContactManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {

                var response = _contactService.SaveContact(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var id = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("Contact_Edit", "Contacts", "Contact_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _contactService.GetContactManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Contact_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ContactManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _contactService.SaveContact(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);

                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = model.Id, returnUrl });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Popup Edit

        public ActionResult PopupEdit(int id)
        {
            SetupPopupAction();
            var model = _contactService.GetContactManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Contact_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(ContactManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _contactService.SaveContact(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox", new CloseFancyBoxViewModel
                            {
                                IsReload = false,
                                ReturnUrl = string.Empty
                            });
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("PopupEdit", new { id = model.Id });
                    }
                }
            }
            SetupPopupAction();
            return View(model);
        }

        #endregion

        #region Details

        [AdministratorNavigation("Contact_Details", "Contacts", "Contact_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _contactService.GetContactDetailsModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Contact_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        #region Methods

        [HttpPost]
        public ActionResult GetContactDetails(int id)
        {
            var model = _contactService.GetContactDetailsModel(id);

            return PartialView("Partials/_ContactDetails", model);
        }

        [HttpPost]
        public JsonResult UpdateContactData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_contactService.UpdateContactData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        [HttpPost]
        public ActionResult IsContactValidForCreateNewUser(int contactId)
        {
            return Json(_contactService.IsContactValidForCreateNewUser(contactId));
        }

        #endregion

        #endregion
    }
}
