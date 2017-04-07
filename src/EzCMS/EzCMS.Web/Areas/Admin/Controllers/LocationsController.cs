using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Locations;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.Locations;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class LocationsController : BackendController
    {
        private readonly ILocationService _locationService;
        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        #region Grid

        [AdministratorNavigation("Location_Holder", "Module_Holder", "Location_Holder", "fa-map-marker", 80, true, true)]
        [AdministratorNavigation("Locations", "Location_Holder", "Locations", "fa-map-marker", 10)]
        public ActionResult Index()
        {
            var model = _locationService.GetLocationSearchModel();
            return View(model);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, LocationSearchModel model)
        {
            return JsonConvert.SerializeObject(_locationService.SearchLocations(si, model));
        }

        /// <summary>
        /// Export Locations
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, LocationSearchModel model)
        {
            var workbook = _locationService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Locations.xls");
        }

        #endregion

        #region Create

        [AdministratorNavigation("Location_Create", "Locations", "Location_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _locationService.GetLocationManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(LocationManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _locationService.SaveLocation(model);
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
            return View(model);
        }

        #endregion

        #region Popup Create

        public ActionResult PopupCreate(int? locationTypeId)
        {
            SetupPopupAction();
            var model = _locationService.GetLocationManageModel(null, locationTypeId);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupCreate(LocationManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _locationService.SaveLocation(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var id = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox",
                                new CloseFancyBoxViewModel());
                        default:
                            return RedirectToAction("PopupEdit", new { id });
                    }
                }
            }
            SetupPopupAction();
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("Location_Edit", "Locations", "Location_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _locationService.GetLocationManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Location_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(LocationManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _locationService.SaveLocation(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);

                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = model.Id, returnUrl });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Popup Edit

        public ActionResult PopupEdit(int id)
        {
            SetupPopupAction();
            var model = _locationService.GetLocationManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Location_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(LocationManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _locationService.SaveLocation(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox", new CloseFancyBoxViewModel());
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("PopupEdit", new { id = model.Id });
                    }
                }
            }
            SetupPopupAction();
            return View(model);
        }

        #endregion

        #region Details

        [AdministratorNavigation("Location_Details", "Locations", "Location_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _locationService.GetLocationDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Location_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateLocationData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_locationService.UpdateLocationData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteLocation(int id)
        {
            return Json(_locationService.DeleteLocation(id));
        }

        #endregion

        #endregion
    }
}
