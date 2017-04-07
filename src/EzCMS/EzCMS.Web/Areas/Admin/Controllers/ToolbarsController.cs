using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Models.Toolbars;
using EzCMS.Core.Services.Toolbars;
using EzCMS.Core.Services.UserGroups;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class ToolbarsController : BackendController
    {
        private readonly IToolbarService _toolbarService;
        private readonly IUserGroupService _userGroupService;

        public ToolbarsController(IToolbarService toolbarService, IUserGroupService userGroupService)
        {
            _toolbarService = toolbarService;
            _userGroupService = userGroupService;
        }

        #region Grid

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_toolbarService.SearchToolbars(si));
        }

        /// <summary>
        /// Export toolbars
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _toolbarService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Toolbars.xls");
        }

        #endregion

        #region Create

        [AdministratorNavigation("Toolbar_Create", "Toolbars", "Toolbar_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _toolbarService.GetToolbarManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ToolbarManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _toolbarService.SaveToolbar(model);
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

        [AdministratorNavigation("Toolbar_Edit", "Toolbars", "Toolbar_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _toolbarService.GetToolbarManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Toolbar_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ToolbarManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _toolbarService.SaveToolbar(model);
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
            var model = _toolbarService.GetToolbarManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Toolbar_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(ToolbarManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _toolbarService.SaveToolbar(model);
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

        [AdministratorNavigation("Toolbar_Details", "Toolbars", "Toolbar_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _toolbarService.GetToolbarDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Toolbar_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateToolbarData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_toolbarService.UpdateToolbarData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteToolbar(int id)
        {
            return Json(_toolbarService.DeleteToolbar(id));
        }

        [HttpPost]
        public JsonResult RemoveToolbarFromUserGroup(int userGroupId)
        {
            return Json(_userGroupService.RemoveToolbarFromUserGroup(userGroupId));
        }

        #endregion

        #endregion
    }
}
