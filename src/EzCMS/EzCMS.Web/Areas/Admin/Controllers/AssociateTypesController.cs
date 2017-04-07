using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.AssociateTypes;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.AssociateTypes;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class AssociateTypesController : BackendController
    {
        private readonly IAssociateTypeService _associateTypeService;
        public AssociateTypesController(IAssociateTypeService associateTypeService)
        {
            _associateTypeService = associateTypeService;
        }

        #region Grid

        [AdministratorNavigation("AssociatesTypes", "Associate_Holder", "AssociatesTypes", "fa-list-ol", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int? associateId)
        {
            return JsonConvert.SerializeObject(_associateTypeService.SearchAssociateTypes(si, associateId));
        }

        /// <summary>
        /// Export associate types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, int? associateId)
        {
            var workbook = _associateTypeService.Exports(si, gridExportMode, associateId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "AssociateTypes.xls");
        }

        #endregion

        #region Create

        [AdministratorNavigation("AssociateType_Create", "AssociateTypes", "AssociateType_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _associateTypeService.GetAssociateTypeManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(AssociateTypeManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _associateTypeService.SaveAssociateType(model);
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

        [AdministratorNavigation("AssociateType_Edit", "AssociateTypes", "AssociateType_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _associateTypeService.GetAssociateTypeManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("AssociateType_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(AssociateTypeManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _associateTypeService.SaveAssociateType(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            if (!string.IsNullOrEmpty(returnUrl))
                                return Redirect(returnUrl);
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
            var model = _associateTypeService.GetAssociateTypeManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("AssociateType_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(AssociateTypeManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _associateTypeService.SaveAssociateType(model);
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

        [AdministratorNavigation("AssociateType_Details", "AssociateTypes", "AssociateType_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _associateTypeService.GetAssociateTypeDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("AssociateType_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateAssociateTypeData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_associateTypeService.UpdateAssociateTypeData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteAssociateType(int id)
        {
            return Json(_associateTypeService.DeleteAssociateType(id));
        }

        #endregion

        #endregion

        [HttpPost]
        public JsonResult DeleteAssociateAssociateTypeMapping(int associateTypeId, int associateId)
        {
            return Json(_associateTypeService.DeleteAssociateAssociateTypeMapping(associateTypeId, associateId));
        }
    }
}
