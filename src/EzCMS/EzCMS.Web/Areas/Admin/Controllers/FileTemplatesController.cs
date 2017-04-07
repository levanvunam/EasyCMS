using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.FileTemplates;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.FileTemplates;
using EzCMS.Core.Services.PageTemplates;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class FileTemplatesController : BackendController
    {
        private readonly IFileTemplateService _fileTemplateService;
        private readonly IPageTemplateService _pageTemplateService;
        public FileTemplatesController(IFileTemplateService fileTemplateService, IPageTemplateService pageTemplateService)
        {
            _fileTemplateService = fileTemplateService;
            _pageTemplateService = pageTemplateService;
        }

        #region Grid

        [AdministratorNavigation("File_Templates", "Content_Settings", "File_Templates", "fa-file-text-o", 50)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Export file templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _fileTemplateService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "FileTemplates.xls");
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_fileTemplateService.SearchFileTemplates(si));
        }

        /// <summary>
        /// get page template by file template id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetPageTemplates(int? id)
        {
            return Json(_pageTemplateService.GetPageTemplateSelectListForFileTemplate(id), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Create

        [AdministratorNavigation("File_Template_Create", "File_Templates", "File_Template_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var template = _fileTemplateService.GetTemplateManageModel();
            return View(template);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FileTemplateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _fileTemplateService.SaveFileTemplate(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = (int)response.Data });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("File_Template_Edit", "File_Templates", "File_Template_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _fileTemplateService.GetTemplateManageModel(id);
            if (!model.Id.HasValue)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FileTemplateManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _fileTemplateService.SaveFileTemplate(model);
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
            var model = _fileTemplateService.GetTemplateManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("FileTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(FileTemplateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _fileTemplateService.SaveFileTemplate(model);
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

        [AdministratorNavigation("File_Template_Details", "File_Templates", "File_Template_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _fileTemplateService.GetFileTemplateDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("FileTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        /// <summary>
        /// Update attributes of file template when edit on detail page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateFileTemplateData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_fileTemplateService.UpdateFileTemplateData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        /// <summary>
        /// Delete file template by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteFileTemplate(int id)
        {
            return Json(_fileTemplateService.DeleteFileTemplate(id));
        }

        #endregion

        #endregion
    }
}
