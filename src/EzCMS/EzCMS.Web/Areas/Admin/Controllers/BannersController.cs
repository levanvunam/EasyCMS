using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Banners;
using EzCMS.Core.Services.Banners;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class BannersController : BackendController
    {
        private readonly IBannerService _bannerService;
        public BannersController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [AdministratorNavigation("Module_Holder", "", "Module_Holder", "fa-plug", 40, true, true)]
        [AdministratorNavigation("Banners", "Module_Holder", "Banners", "fa-picture-o", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_bannerService.SearchGrid(si));
        }

        /// <summary>
        /// Export banners
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _bannerService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Banners.xls");
        }

        #region Create

        [AdministratorNavigation("Banner_Create", "Banner", "Banner_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _bannerService.GetBannerManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(BannerManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _bannerService.SaveBanner(model);
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

        [AdministratorNavigation("Banner_Edit", "Banner", "Banner_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _bannerService.GetBannerManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Banner_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(BannerManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _bannerService.SaveBanner(model);
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
        public JsonResult DeleteBanner(int id)
        {
            var response = _bannerService.DeleteBanner(id);
            return Json(response);
        }
        #endregion
    }
}
