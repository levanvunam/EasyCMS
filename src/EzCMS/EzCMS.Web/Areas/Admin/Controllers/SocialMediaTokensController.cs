using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.SocialMediaTokens;
using EzCMS.Core.Services.SocialMediaTokens;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class SocialMediaTokensController : BackendController
    {
        private readonly ISocialMediaTokenService _socialMediaTokenService;
        public SocialMediaTokensController(ISocialMediaTokenService socialMediaTokenService)
        {
            _socialMediaTokenService = socialMediaTokenService;
        }

        [AdministratorNavigation("Social_Media_Tokens", "Social_Media_Holder", "Social_Media_Tokens", "fa-qrcode", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_socialMediaTokenService.SearchSocialMediaTokens(si));
        }

        /// <summary>
        /// Export Social Media Tokens
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _socialMediaTokenService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "SocialMediaTokens.xls");
        }

        #region Create

        [AdministratorNavigation("Social_Media_Token_Create", "Social_Media_Tokens", "Social_Media_Token_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _socialMediaTokenService.GetSocialMediaTokenManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(SocialMediaTokenManageModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _socialMediaTokenService.SaveSocialMediaToken(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var authorizeUrl = (string)response.Data;
                    if (string.IsNullOrEmpty(authorizeUrl))
                    {
                        return RedirectToAction("Index");
                    }
                    return Redirect(authorizeUrl);
                }
            }
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("Social_Media_Token_Edit", "Social_Media_Tokens", "Social_Media_Token_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _socialMediaTokenService.GetSocialMediaTokenManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("SocialMediaToken_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(SocialMediaTokenManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _socialMediaTokenService.SaveSocialMediaToken(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var authorizeUrl = (string)response.Data;
                    if (string.IsNullOrEmpty(authorizeUrl))
                    {
                        return RedirectToAction("Index");
                    }
                    return Redirect(authorizeUrl);
                }
            }
            return View(model);
        }

        #endregion

        #region Setup

        public ActionResult CallBack(int id)
        {
            var response = _socialMediaTokenService.SaveSocialMediaTokenInformation(id);

            if (!response.Success)
            {
                SetResponseMessage(response);
                return RedirectToAction("Edit", new { id });
            }

            return RedirectToAction("Index");
        }

        public ActionResult LinkedInCallBack()
        {
            if (HttpContext.Session != null)
            {
                var id = StateManager.GetSession<int>(EzCMSContants.LinkedInCallbackId);
                var response = _socialMediaTokenService.SaveSocialMediaTokenInformation(id);

                if (!response.Success)
                {
                    SetResponseMessage(response);
                    return RedirectToAction("Edit", new { id });
                }
            }
            return RedirectToAction("Index");
        }

        #endregion

        [HttpPost]
        public JsonResult DeleteSocialMediaToken(int id)
        {
            return Json(_socialMediaTokenService.DeleteSocialMediaToken(id));
        }

        [HttpPost]
        public JsonResult GetSocialMediaTokens(int socialMediaId)
        {
            return Json(_socialMediaTokenService.GetActiveTokens(socialMediaId));
        }

        [HttpPost]
        public JsonResult SetDefaultToken(int id)
        {
            return Json(_socialMediaTokenService.SetDefaultToken(id));
        }
        [AdministratorNavigation("Social_Media_Token_Details", "Social_Media_Tokens", "Social_Media_Token_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _socialMediaTokenService.GetSocialMediaTokenDetailModel(id);

            if (model == null)
            {
                SetErrorMessage(T("SocialMediaToken_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
