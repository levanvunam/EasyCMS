using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.SocialMedia;
using EzCMS.Core.Services.SocialMedia;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class SocialMediaController : BackendController
    {
        private readonly ISocialMediaService _socialMediaService;
        public SocialMediaController(ISocialMediaService socialMediaService)
        {
            _socialMediaService = socialMediaService;
        }

        [AdministratorNavigation("Social_Media_Holder", "SystemSetting_Holder", "Social_Media_Holder", "fa-globe", 10, true, true)]
        [AdministratorNavigation("Social_Media", "Social_Media_Holder", "Social_Media", "fa-globe", 10)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_socialMediaService.SearchSocialMedia(si));
        }

        /// <summary>
        /// Export Social Media
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _socialMediaService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "SocialMedia.xls");
        }

        #region Edit

        [AdministratorNavigation("Social_Media_Edit", "Social_Media", "Social_Media_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _socialMediaService.GetSocialMediaManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Social_Media_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(SocialMediaManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _socialMediaService.SaveSocialMedia(model);
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

        [AdministratorNavigation("Social_Media_Details", "Social_Media", "Social_Media_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _socialMediaService.GetSocialMediaDetailModel(id);

            if (model == null)
            {
                SetErrorMessage(T("Social_Media_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
