using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.PageTemplates;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.PageTemplates;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class PageTemplatesController : BackendController
    {
        private readonly IPageTemplateService _pageTemplateService;
        public PageTemplatesController(IPageTemplateService pageTemplateService)
        {
            _pageTemplateService = pageTemplateService;
        }

        #region Grid

        [AdministratorNavigation("Content_Settings", "Setting_Holder", "Content_Settings", "fa-file-text", 10, true, true)]
        [AdministratorNavigation("Page_Templates", "Content_Settings", "Page_Templates", "fa-file-text-o", 30)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int? pageTemplateId)
        {
            return JsonConvert.SerializeObject(_pageTemplateService.SearchPageTemplates(si, pageTemplateId));
        }

        /// <summary>
        /// Export page templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="pageTemplateId"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, int? pageTemplateId)
        {
            var workbook = _pageTemplateService.Exports(si, gridExportMode, pageTemplateId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "PageTemplates.xls");
        }

        #endregion

        #region Create

        [AdministratorNavigation("Page_Template_Create", "Page_Templates", "Page_Template_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var template = _pageTemplateService.GetTemplateManageModel();
            return View(template);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(PageTemplateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _pageTemplateService.SavePageTemplate(model);
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
            model.Parents = _pageTemplateService.GetPossibleParents();
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("Page_Template_Edit", "Page_Templates", "Page_Template_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int? id, int? logId)
        {
            PageTemplateManageModel model = null;
            if (id.HasValue)
            {
                model = _pageTemplateService.GetTemplateManageModel(id.Value);
            }
            else if (logId.HasValue)
            {
                model = _pageTemplateService.GetTemplateManageModelByLogId(logId.Value);
            }
            if (model == null)
            {
                SetErrorMessage(T("PageTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PageTemplateManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _pageTemplateService.SavePageTemplate(model);
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
            var model = _pageTemplateService.GetTemplateManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("PageTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(PageTemplateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _pageTemplateService.SavePageTemplate(model);
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

        [AdministratorNavigation("Page_Template_Details", "Page_Templates", "Page_Template_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _pageTemplateService.GetPageTemplateDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("PageTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdatePageTemplateData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_pageTemplateService.UpdatePageTemplateData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeletePageTemplate(int id)
        {
            return Json(_pageTemplateService.DeletePageTemplate(id));
        }

        #endregion

        #endregion

        public ActionResult Logs(int id)
        {
            var model = _pageTemplateService.GetLogs(id);
            if (model == null)
            {
                SetErrorMessage(T("PageTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult GetLogs(int id, int total, int index)
        {
            var model = _pageTemplateService.GetLogs(id, total, index);
            var content = RenderPartialViewToString("Partials/_GetLogs", model);
            var response = new ResponseModel
            {
                Success = true,
                Data = new
                {
                    model.LoadComplete,
                    content
                }
            };
            return Json(response);
        }
    }
}
