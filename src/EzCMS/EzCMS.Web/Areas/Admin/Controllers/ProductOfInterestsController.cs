using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.ProductOfInterests;
using EzCMS.Core.Services.ProductOfInterests;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class ProductOfInterestsController : BackendController
    {
        private readonly IProductOfInterestService _productOfInterestService;
        public ProductOfInterestsController(IProductOfInterestService productOfInterestService)
        {
            _productOfInterestService = productOfInterestService;
        }

        [AdministratorNavigation("Product_Of_Interests", "Contacts_Settings", "Product_Of_Interests", "fa-heart", 20, false)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_productOfInterestService.SearchProductOfInterests(si));
        }

        /// <summary>
        /// Export product of interests
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _productOfInterestService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "ProductOfInterests.xls");
        }

        #region Create

        [AdministratorNavigation("Product_Of_Interest_Create", "Product_Of_Interests", "Product_Of_Interest_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _productOfInterestService.GetProductOfInterestManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ProductOfInterestManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _productOfInterestService.SaveProductOfInterest(model);
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

        [AdministratorNavigation("Product_Of_Interest_Edit", "Product_Of_Interests", "Product_Of_Interest_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _productOfInterestService.GetProductOfInterestManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("ProductOfInterest_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ProductOfInterestManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _productOfInterestService.SaveProductOfInterest(model);
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
        public JsonResult DeleteProductOfInterest(int id)
        {
            var response = _productOfInterestService.DeleteProductOfInterest(id);
            return Json(response);
        }

        #endregion
    }
}
