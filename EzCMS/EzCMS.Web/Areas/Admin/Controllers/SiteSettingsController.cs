using Ez.Framework.Core.Enums;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.SiteSettings;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Models.SiteSettings;
using EzCMS.Core.Services.SiteSettings;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class SiteSettingsController : BackendController
    {
        private readonly ISiteSettingService _siteSettingService;
        public SiteSettingsController(ISiteSettingService siteSettingService)
        {
            _siteSettingService = siteSettingService;
        }

        #region Listing

        [AdministratorNavigation("Setting_Holder", "", "Setting_Holder", "fa-cogs", 90, true, true)]
        [AdministratorNavigation("System_Setting_Holder", "", "System_Setting_Holder", "fa-adjust", 100, true, true)]
        [AdministratorNavigation("Module_Management", "System_Setting_Holder", "Module_Management", "fa-cogs", 30, true, true)]
        [AdministratorNavigation("Site_Settings", "Setting_Holder", "Site_Settings", "fa-cogs", 20)]
        public ActionResult Index()
        {
            var model = new SiteSettingSearchModel();
            return View(model);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, SiteSettingSearchModel model)
        {
            return JsonConvert.SerializeObject(_siteSettingService.SearchSiteSettings(si, model));
        }

        /// <summary>
        /// Export site settings
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, SiteSettingSearchModel model)
        {
            var workbook = _siteSettingService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "SiteSettings.xls");
        }

        #endregion

        #region Edit Setting
        [AdministratorNavigation("Site_Setting_Edit", "Site_Settings", "Site_Setting_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _siteSettingService.GetSettingManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("SiteSetting_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(SiteSettingManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var setting = _siteSettingService.GetSetting(model.SettingName);
                var updateSuccess = true;

                var settingParser = HostContainer.GetInstances<IComplexSetting>().FirstOrDefault(parser => parser.GetSetup().Name.Equals(setting.Name));
                if (settingParser != null)
                {
                    if (TryUpdateModel((dynamic)settingParser))
                    {
                        model.Value = SerializeUtilities.Serialize(settingParser);
                    }
                    else
                    {
                        updateSuccess = false;
                    }
                }

                if (updateSuccess)
                {
                    var response = _siteSettingService.SaveSettingManageModel(model);
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
                SetErrorMessage(T("SiteSetting_Message_UpdateFailure"));
                return RedirectToAction("Edit", new { id = model.Id });
            }
            return View(model);
        }

        #endregion

        #region Popup Edit

        public ActionResult PopupEdit(int id)
        {
            SetupPopupAction();
            var model = _siteSettingService.GetSettingManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("SiteSetting_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(SiteSettingManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _siteSettingService.SaveSettingManageModel(model);
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
    }
}
