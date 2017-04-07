using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.BodyTemplates;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.BodyTemplates;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class BodyTemplatesController : BackendController
    {
        private readonly IBodyTemplateService _bodyTemplateService;
        public BodyTemplatesController(IBodyTemplateService bodyTemplateService)
        {
            _bodyTemplateService = bodyTemplateService;
        }

        [AdministratorNavigation("Body_Templates", "Content_Settings", "Body_Templates", "fa-file-text", 30)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Export body templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _bodyTemplateService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "BodyTemplates.xls");
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_bodyTemplateService.SearchBodyTemplates(si));
        }

        #region Create

        [AdministratorNavigation("Body_Template_Create", "Body_Templates", "Body_Template_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _bodyTemplateService.GetBodyTemplateManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(BodyTemplateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _bodyTemplateService.SaveBodyTemplate(model);
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

        [AdministratorNavigation("Body_Template_Edit", "Body_Templates", "Body_Template_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _bodyTemplateService.GetBodyTemplateManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("BodyTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(BodyTemplateManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _bodyTemplateService.SaveBodyTemplate(model);
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

        #region Details

        [AdministratorNavigation("Body_Template_Details", "Body_Templates", "Body_Template_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _bodyTemplateService.GetBodyTemplateDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("BodyTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Preview(int id)
        {
            var content = _bodyTemplateService.GetBodyTemplateContent(id);

            return View("_Preview", new ContentPreviewModel(content));
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteBodyTemplate(int id)
        {
            return Json(_bodyTemplateService.DeleteBodyTemplate(id));
        }

        [HttpPost]
        public JsonResult UpdateBodyTemplateData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_bodyTemplateService.UpdateBodyTemplateData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #endregion

        #endregion

        #region Save As New Template

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveAsNewBodyTemplate(BodyTemplateManageModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_bodyTemplateService.SaveBodyTemplate(model));
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
