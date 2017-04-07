using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Web;
using EzCMS.Core.Models.SiteSetup;
using EzCMS.Core.Services.SiteSetup;
using EzCMS.Entity.Core.SiteInitialize;
using System;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.SiteSetup.Controllers
{
    public class SetupController : Controller
    {
        private readonly ISiteSetupService _siteSetupService;

        public SetupController(ISiteSetupService siteSetupService)
        {
            _siteSetupService = siteSetupService;
        }

        #region Setup

        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                var configuration = SiteInitializer.GetConfiguration();

                if (configuration != null && !configuration.IsSetupFinish)
                {
                    SiteInitializer.SiteConfigError = true;
                    return View("Error", new ResponseModel
                    {
                        Success = false,
                        Message =
                            "Site configuration is invalid. Please remove the site.config in /App_Data folder to continue."
                    });
                }

                if (SiteInitializer.SiteConfigError)
                {
                    WebUtilities.RestartAppDomain();
                }
            }
            catch (Exception)
            {
            }

            return View();
        }

        #region Database Setup
        public ActionResult DatabaseSetup()
        {
            var model = _siteSetupService.GetDatabaseSetupModel();
            return View("Steps/DatabaseSetup", model);
        }

        [HttpPost]
        public JsonResult DatabaseSetup(DatabaseSetupModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _siteSetupService.SaveDatabaseSetupModel(model);
                return Json(response);
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }
        #endregion

        #region Email Setup
        public ActionResult EmailSetup()
        {
            var model = _siteSetupService.GetEmailSetupModel();
            return View("Steps/EmailSetup", model);
        }

        [HttpPost]
        public JsonResult EmailSetup(EmailSetupModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _siteSetupService.SaveEmailSetupModel(model);

                return Json(response);
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #endregion

        #region User Setup
        public ActionResult UserSetup()
        {
            var model = _siteSetupService.GetUserSetupModel();
            return View("Steps/UserSetup", model);
        }

        [HttpPost]
        public JsonResult UserSetup(UserSetupModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _siteSetupService.SaveUserSetupModel(model);

                return Json(response);
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #endregion

        #region Company Setup
        public ActionResult CompanySetup()
        {
            var model = _siteSetupService.GetCompanySetupModel();
            return View("Steps/CompanySetup", model);
        }

        [HttpPost]
        public JsonResult CompanySetup(CompanySetupModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _siteSetupService.SaveCompanySetupModel(model);

                return Json(response);
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #endregion

        #region Starter Kit
        public ActionResult StarterKit()
        {
            var model = _siteSetupService.GetStarterKitsModel();
            return View("Steps/StarterKit", model);
        }

        [HttpPost]
        public JsonResult StarterKit(string kit)
        {
            var response = _siteSetupService.SaveStarterKitsModel(kit);

            return Json(response);
        }

        #endregion

        #endregion

        [HttpPost]
        public JsonResult Restart()
        {
            var response = _siteSetupService.FinishSetup();

            return Json(response);
        }
    }
}