using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.ContactGroups;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Services.ContactGroups;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize]
    public class ContactGroupsController : BackendController
    {
        private readonly IContactGroupService _contactGroupService;

        public ContactGroupsController(IContactGroupService contactGroupService)
        {
            _contactGroupService = contactGroupService;
        }

        [AdministratorNavigation("Contact_Groups", "Contact_Holder", "Contacts", "fa-users", 10, true, true)]
        public ActionResult Index()
        {
            return View();
        }

        #region Grid

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_contactGroupService.SearchContactGroups(si));
        }

        /// <summary>
        /// Export contact groups
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _contactGroupService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "ContactGroups.xls");
        }

        #endregion

        #region Manage

        [HttpPost]
        public JsonResult ChangeActiveState(int id)
        {
            return Json(_contactGroupService.ChangeActiveState(id));
        }

        [HttpPost]
        public JsonResult RefreshContactGroup(int id)
        {
            return Json(_contactGroupService.RefreshContactGroup(id));
        }

        [HttpPost]
        public string RefreshConfirm(int id)
        {
            return TFormat("ContactGroup_Message_RefreshConfirm", _contactGroupService.CountContacts(id), _contactGroupService.CalculateContacts(id));
        }

        #endregion

        #region Create

        [AdministratorNavigation("Contact_Group_Create", "Contact_Groups", "Contact_Group_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _contactGroupService.GetContactGroupManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ContactGroupManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _contactGroupService.SaveContactGroup(model);
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

        [AdministratorNavigation("Contact_Group_Edit", "Contact_Groups", "Contact_Group_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _contactGroupService.GetContactGroupManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("ContactGroup_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ContactGroupManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _contactGroupService.SaveContactGroup(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = model.Id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Details

        [AdministratorNavigation("Contact_Group_Details", "Contact_Groups", "Contact_Group_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _contactGroupService.GetContactGroupDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("ContactGroup_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        #region Methods

        [HttpPost]
        public JsonResult UpdateContactGroupData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_contactGroupService.UpdateContactGroupData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        [HttpPost]
        public JsonResult DeleteContactGroup(int id)
        {
            return Json(_contactGroupService.DeleteContactGroup(id));
        }

        #endregion

        #endregion

        #region Add To Group

        [HttpGet]
        public ActionResult AddToGroup(ContactSearchModel model)
        {
            var addToGroupModel = new AddToGroupModel(model);
            return View(addToGroupModel);
        }

        [HttpPost]
        public JsonResult AddToGroup(AddToGroupModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_contactGroupService.AddToGroup(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #endregion
    }
}