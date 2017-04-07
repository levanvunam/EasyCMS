using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Services;
using EzCMS.Core.Services.Services;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class ServicesController : BackendController
    {
        private readonly IServiceService _serviceService;
        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [AdministratorNavigation("Services", "Module_Holder", "Services", "fa-signal", 130)]
        public ActionResult Index()
        {
            return View();
        }

        #region Ajax Methods

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_serviceService.SearchService(si));
        }

        public JsonResult GetStatus()
        {
            return Json(_serviceService.GetStatus(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        /// <summary>
        /// Export services
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _serviceService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Services.xls");
        }

        #region Create

		[AdministratorNavigation("Service_Create", "Services", "Service_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _serviceService.GetServiceManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ServiceManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _serviceService.SaveServiceManageModel(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var id = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id });
                    }
                }
            }
            model.StatusList = _serviceService.GetStatus();
            return View(model);
        }

        #endregion

        #region Edit

		[AdministratorNavigation("Service_Edit", "Services", "Service_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _serviceService.GetServiceManageModel(id);
            return View(model);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ServiceManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _serviceService.SaveServiceManageModel(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            if (!string.IsNullOrEmpty(returnUrl))
                                return Redirect(returnUrl);
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = model.Id, returnUrl });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Details

		[AdministratorNavigation("Service_Details", "Services", "Service_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _serviceService.GetServiceDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Service_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateServiceData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_serviceService.UpdateServiceData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteService(int id)
        {
            return Json(_serviceService.DeleteService(id));
        }

        #endregion

        #endregion
    }
}
