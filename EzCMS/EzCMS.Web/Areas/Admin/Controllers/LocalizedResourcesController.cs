using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.LocalizedResources;
using EzCMS.Core.Services.Languages;
using EzCMS.Core.Services.Localizes;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class LocalizedResourcesController : BackendController
    {
        private readonly IEzCMSLocalizedResourceService _localizedResourceService;
        private readonly ILanguageService _languageService;

        public LocalizedResourcesController(IEzCMSLocalizedResourceService localizedResourceService, ILanguageService languageService)
        {
            _localizedResourceService = localizedResourceService;
            _languageService = languageService;
        }

        public ActionResult Index(LocalizedResourceSearchModel model)
        {
            var language = _languageService.GetById(model.LanguageId);
            if (language == null)
            {
                SetErrorMessage(T("Language_Message_ObjectNotFound"));
                return RedirectToAction("Index", "Languages");
            }

            ViewBag.Language = language.Name;

            return View(model);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, LocalizedResourceSearchModel model)
        {
            return JsonConvert.SerializeObject(_localizedResourceService.SearchLocalizedResources(si, model));
        }

        /// <summary>
        /// Export localized resources
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, LocalizedResourceSearchModel model)
        {
            var workbook = _localizedResourceService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "LocalizedResources.xls");
        }

        #region Create

        [AdministratorNavigation("Localized_Resource_Create", "Localized_Resources", "Localized_Resource_Create", "fa-plus", 10, false)]
        public ActionResult Create(int languageId)
        {
            var localizedResource = new LocalizedResourceManageModel(languageId);
            return View(localizedResource);
        }

        [HttpPost]
        public ActionResult Create(LocalizedResourceManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _localizedResourceService.SaveLocalizedResource(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var localizedResourceId = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index", new { languageId = model.LanguageId });
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("Edit", new { id = localizedResourceId });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("Localized_Resource_Edit", "LocalizedResources", "Localized_Resource_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _localizedResourceService.GetLocalizedResourceManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("LocalizedResource_Message_ObjectNotFound"));
                return RedirectToAction("Index", "Languages");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(LocalizedResourceManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _localizedResourceService.SaveLocalizedResource(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index", new { languageId = model.LanguageId });
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("Edit", new { id = model.Id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Delete

        [HttpPost]
        public JsonResult DeleteLocalizedResource(int id)
        {
            return Json(_localizedResourceService.DeleteLocalizedResource(id));
        }

        #endregion

        [HttpPost]
        public JsonResult UpdateLocalizedResource(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_localizedResourceService.UpdateLocalizedResource(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }
    }
}
