using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.CampaignCodes;
using EzCMS.Core.Services.CampaignCodes;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class CampaignCodesController : BackendController
    {
        private readonly ICampaignCodeService _campaignCodeService;
        public CampaignCodesController(ICampaignCodeService campaignCodeService)
        {
            _campaignCodeService = campaignCodeService;
        }

        [AdministratorNavigation("Campaign_Codes", "Contacts_Settings", "Campaign_Codes", "fa-qrcode", 10)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_campaignCodeService.SearchCampaignCodes(si));
        }

        /// <summary>
        /// Export campaign codes
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _campaignCodeService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "CampaignCodes.xls");
        }

        #region Create

        [AdministratorNavigation("Campaign_Code_Create", "Campaign_Codes", "Campaign_Code_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _campaignCodeService.GetCampaignCodeManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(CampaignCodeManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _campaignCodeService.SaveCampaignCode(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var id = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("Campaign_Code_Edit", "Campaign_Codes", "Campaign_Code_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _campaignCodeService.GetCampaignCodeManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("CampaignCode_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(CampaignCodeManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _campaignCodeService.SaveCampaignCode(model);
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

        #region Delete

        [HttpPost]
        public JsonResult DeleteCampaignCode(int id)
        {
            var response = _campaignCodeService.DeleteCampaignCode(id);
            return Json(response);
        }

        #endregion
    }
}
