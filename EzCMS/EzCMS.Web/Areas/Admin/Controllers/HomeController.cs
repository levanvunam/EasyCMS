using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Html;
using Ez.Framework.Utilities.Time;
using Ez.Framework.Utilities.Web;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Exceptions.Sites;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Models.Shared.Maps;
using EzCMS.Core.Services.Common;
using EzCMS.Core.Services.Navigations;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Core.Services.Toolbars;
using EzCMS.Entity.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    public class HomeController : BackendController
    {
        private readonly INavigationService _navigationService;
        private readonly ISiteSettingService _siteSettingService;
        private readonly ICommonService _commonService;
        private readonly IToolbarService _toolbarService;

        public HomeController(INavigationService navigationService, ISiteSettingService siteSettingService, ICommonService commonService, IToolbarService toolbarService)
        {
            _navigationService = navigationService;
            _siteSettingService = siteSettingService;
            _commonService = commonService;
            _toolbarService = toolbarService;
        }

        [AdministratorNavigation("Dashboard", "", "Dashboard", "fa-dashboard", 20)]
        [EzCMSAuthorize(IsAdministrator = true)]
        public ActionResult Index()
        {
            var googleAnalyticModel = _commonService.GetGoogleAnalyticAccessToken();
            return View("Index", googleAnalyticModel);
        }

        public ActionResult CloseFancyBox(CloseFancyBoxViewModel model)
        {
            return View("CloseFancyBox", model);
        }

        #region Navigation & Breadcrumbs

        [ChildActionOnly]
        public ActionResult Navigation()
        {
            var controller = ControllerContext.ParentActionViewContext.RouteData.Values["controller"].ToString();
            var action = ControllerContext.ParentActionViewContext.RouteData.Values["action"].ToString();

            // Get current navigation
            var navigation = _navigationService.GetAll().FirstOrDefault(n => n.Controller.Equals(controller)
                                                                                && n.Action.Equals(action));
            ViewBag.Hierarchy = navigation != null ? navigation.Hierarchy : string.Empty;

            // Get admin navigations
            var model = _navigationService.GetNavigations();
            return PartialView("Partials/_Navigation", model);
        }

        [ChildActionOnly]
        public PartialViewResult GetBreadCrumb()
        {
            var controller = ControllerContext.ParentActionViewContext.RouteData.Values["controller"].ToString();
            var action = ControllerContext.ParentActionViewContext.RouteData.Values["action"].ToString();
            var model = _navigationService.GetBreadcrumbs(controller, action);
            return PartialView("Partials/_Breadcrumb", model);
        }

        #endregion

        #region 404, 500 & Unauthorize Pages

        #endregion

        #region Analytics

        public ActionResult GoogleAnalyticApiSetup()
        {
            var model = _siteSettingService.LoadSetting<GoogleAnalyticApiSetting>();

            return View("Analytics/Partials/_AnalyticApiSetup", model);
        }

        [HttpPost]
        public JsonResult GoogleAnalyticApiSetup(GoogleAnalyticApiSetting model)
        {
            if (ModelState.IsValid)
            {
                var setting = _siteSettingService.GetSetting(model.GetSetup().Name);

                var manageModel = _siteSettingService.GetSettingManageModel(setting.Id);
                manageModel.Value = SerializeUtilities.Serialize(model);

                var response = _siteSettingService.SaveSettingManageModel(manageModel);

                return Json(response);
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        public ActionResult ConfigGoogleAnalyticApi()
        {
            var apiSetting = _siteSettingService.GetById(SettingNames.GoogleAnalyticApi);

            if (apiSetting != null)
            {
                return RedirectToAction("Index", "SiteSettings", new { area = "Admin", id = apiSetting.Id });
            }

            throw new EzCMSNotFoundException(System.Web.HttpContext.Current.Request.RawUrl);
        }

        #endregion

        #region CkEditor

        [HttpPost]
        public JsonResult GetToolbars(List<string> toolbars)
        {
            return Json(new ResponseModel
            {
                Success = true,
                Data = RenderPartialViewToString("Partials/_EditorToolbar", toolbars).RemoveNewLine()
            });
        }

        public string BasicToolbar()
        {
            var currentUserToolbar = _toolbarService.GetCurrentUserToolbar();
            return RenderPartialViewToString("Partials/_EditorToolbar", currentUserToolbar.BasicTools).RemoveNewLine();
        }

        public string PageToolbar()
        {
            var currentUserToolbar = _toolbarService.GetCurrentUserToolbar();
            return RenderPartialViewToString("Partials/_EditorToolbar", currentUserToolbar.PageTools).RemoveNewLine();
        }

        #endregion

        /// <summary>
        /// Keep alive when user use Admin module
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public void HeartBeat()
        {
        }

        [HttpPost]
        public JsonResult Restart()
        {
            ResponseModel response;
            try
            {
                WebUtilities.RestartAppDomain();
                response = new ResponseModel
                {
                    Success = true
                };
            }
            catch (Exception exception)
            {
                response = new ResponseModel
                {
                    Success = false,
                    Message = exception.Message
                };
            }

            return Json(response);
        }

        #region Ajax Methods

        [HttpPost]
        public JsonResult GetTimeZones()
        {
            var timeZones = TimeZoneUtilities.GetTimeZones();

            return Json(new ResponseModel
            {
                Success = true,
                Data = timeZones
            });
        }

        [HttpPost]
        public JsonResult GetAustraliaStates()
        {
            var states = EnumUtilities.GenerateSelectListItems<CommonEnums.AustraliaState>(GenerateEnumType.DescriptionValueAndDescriptionText);

            return Json(new ResponseModel
            {
                Success = true,
                Data = states
            });
        }

        [HttpPost]
        public JsonResult GetGenders()
        {
            var genders = EnumUtilities.GenerateSelectListItems<UserEnums.Gender>(GenerateEnumType.DescriptionValueAndDescriptionText);

            return Json(new ResponseModel
            {
                Success = true,
                Data = genders
            });
        }

        [HttpPost]
        public JsonResult GetUrlTargets()
        {
            var urlTargets = EnumUtilities.GenerateSelectListItems<CommonEnums.UrlTarget>(GenerateEnumType.DescriptionValueAndDescriptionText);

            return Json(new ResponseModel
            {
                Success = true,
                Data = urlTargets
            });
        }

        #endregion

        #region Maps

        public ActionResult ShowMap(ShowMapModel model)
        {
            return PartialView("Maps/_ShowMap", model);
        }

        #endregion
    }
}
