using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Associates;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.Associates;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class AssociatesController : BackendController
    {
        private readonly IAssociateService _associateService;
        public AssociatesController(IAssociateService associateService)
        {
            _associateService = associateService;
        }

        #region Grid

        [AdministratorNavigation("Associate_Holder", "Module_Holder", "Associate_Holder", "fa-user-md", 10, true, true)]
        [AdministratorNavigation("Associates", "Associate_Holder", "Associates", "fa-user-md", 10)]
        public ActionResult Index()
        {
            var model = _associateService.GetAssociateSearchModel();
            return View(model);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, AssociateSearchModel model)
        {
            return JsonConvert.SerializeObject(_associateService.SearchAssociates(si, model));
        }

        /// <summary>
        /// Export associates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, AssociateSearchModel model)
        {
            var workbook = _associateService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Associates.xls");
        }

        #endregion

        #region Create

        [AdministratorNavigation("Associate_Create", "Associates", "Associate_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _associateService.GetAssociateManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(AssociateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _associateService.SaveAssociate(model);
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

        #region Popup Create

        public ActionResult PopupCreate(int? associateTypeId)
        {
            SetupPopupAction();
            var model = _associateService.GetAssociateManageModel(null, associateTypeId);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupCreate(AssociateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _associateService.SaveAssociate(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var id = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox",
                                new CloseFancyBoxViewModel());
                        default:
                            return RedirectToAction("PopupEdit", new { id });
                    }
                }
            }
            SetupPopupAction();
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("Associate_Edit", "Associates", "Associate_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _associateService.GetAssociateManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Associate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(AssociateManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _associateService.SaveAssociate(model);
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
            var model = _associateService.GetAssociateManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Associate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(AssociateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _associateService.SaveAssociate(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox", new CloseFancyBoxViewModel());
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

        [AdministratorNavigation("Associate_Details", "Associates", "Associate_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _associateService.GetAssociateDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Associate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateAssociateData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_associateService.UpdateAssociateData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteAssociate(int id)
        {
            return Json(_associateService.DeleteAssociate(id));
        }

        #endregion

        #endregion

        [HttpPost]
        public JsonResult DeleteAssociateLocationMapping(int locationId, int associateId)
        {
            return Json(_associateService.DeleteAssociateLocationMapping(locationId, associateId));
        }
    }
}
