using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Countries;
using EzCMS.Core.Services.Countries;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class CountriesController : BackendController
    {
        private readonly ICountryService _countryService;
        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [AdministratorNavigation("Countries", "International_Settings", "Countries", "fa-globe", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_countryService.SearchCountries(si));
        }

        /// <summary>
        /// Export countries
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _countryService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Countries.xls");
        }

        #region Create

        [AdministratorNavigation("Country_Create", "Countries", "Country_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _countryService.GetCountryManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(CountryManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _countryService.SaveCountry(model);
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

        [AdministratorNavigation("Country_Edit", "Countries", "Country_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _countryService.GetCountryManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Country_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(CountryManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _countryService.SaveCountry(model);
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

        public JsonResult GetCountries(string keyword)
        {
            return Json(_countryService.GetCountries(keyword), JsonRequestBehavior.AllowGet);
        }
    }
}
