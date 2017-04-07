using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Models.SlideInHelps;
using EzCMS.Core.Services.Languages;
using EzCMS.Core.Services.SlideInHelps;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class SlideInHelpsController : BackendController
    {
        private readonly ILanguageService _languageService;
        private readonly ISlideInHelpService _slideInHelpService;
        public SlideInHelpsController(ILanguageService languageService, ISlideInHelpService slideInHelpService)
        {
            _languageService = languageService;
            _slideInHelpService = slideInHelpService;
        }

        public ActionResult Index(int? languageId)
        {
            var model = _languageService.GetById(languageId);
            if (model == null)
            {
                SetErrorMessage(T("Language_Message_ObjectNotFound"));
                return RedirectToAction("Index", "Languages");
            }

            return View(model);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int languageId)
        {
            return JsonConvert.SerializeObject(_slideInHelpService.SearchSlideInHelps(si, languageId));
        }

        /// <summary>
        /// Export slide in helps
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, int languageId)
        {
            var workbook = _slideInHelpService.Exports(si, gridExportMode, languageId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "SlideInHelps.xls");
        }

        #region Edit

        [AdministratorNavigation("Slide_In_Help_Edit", "Slide_In_Helps", "Slide_In_Help_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id, int? languageId, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            var model = _slideInHelpService.GetSlideInHelpManageModel(languageId, id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("SlideInHelp_Message_ObjectNotFound"));
                return RedirectToAction("Index", new { languageId });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(SlideInHelpManageModel model, SubmitType submit, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var response = _slideInHelpService.SaveSlideInHelpManageModel(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            if (string.IsNullOrEmpty(returnUrl))
                                return RedirectToAction("Index", new { languageId = model.LanguageId });
                            return Redirect(returnUrl);
                        default:
                            return RedirectToAction("Edit", new { languageId = model.LanguageId, id = model.Id, returnUrl });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Popup Edit


        public ActionResult PopupEdit(int id)
        {
            ViewBag.IsPopup = true;
            var model = _slideInHelpService.GetSlideInHelpManageModel(null, id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("SlideInHelp_Message_ObjectNotFound"));
                return View("CloseFancyBox",
                    new CloseFancyBoxViewModel
                    {
                        IsReload = true
                    });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(SlideInHelpManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _slideInHelpService.SaveSlideInHelpManageModel(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox",
                                new CloseFancyBoxViewModel
                                {
                                    IsReload = true
                                });
                        default:
                            return RedirectToAction("PopupEdit", new { id = model.Id });
                    }
                }
            }
            ViewBag.IsPopup = true;
            return View(model);
        }

        #endregion

        [HttpPost]
        public JsonResult ChangeStatus(int id, bool status)
        {
            return Json(_slideInHelpService.ChangeSlideInHelpStatus(id, status));
        }

    }
}
