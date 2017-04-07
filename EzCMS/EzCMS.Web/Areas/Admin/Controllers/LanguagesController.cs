using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Languages;
using EzCMS.Core.Services.Languages;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class LanguagesController : BackendController
    {
        private readonly ILanguageService _languageService;
        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [AdministratorNavigation("International_Settings", "Module_Holder", "International_Settings", "fa-cog", 70)]
        [AdministratorNavigation("Languages", "International_Settings", "Languages", "fa-globe", 10)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_languageService.SearchLanguages(si));
        }

        /// <summary>
        /// Export languages
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _languageService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Languages.xls");
        }

        #region Create

        [AdministratorNavigation("Language_Create", "Languages", "Language_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _languageService.GetLanguageManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(LanguageManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _languageService.SaveLanguage(model);
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

        [AdministratorNavigation("Language_Edit", "Languages", "Language_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _languageService.GetLanguageManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Language_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(LanguageManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _languageService.SaveLanguage(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = model.Id });
                    }
                }
            }
            return View(model);
        }

        #endregion

    }
}
