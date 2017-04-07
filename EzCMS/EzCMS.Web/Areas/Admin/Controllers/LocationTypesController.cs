using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.LocationTypes;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.LocationTypes;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class LocationTypesController : BackendController
    {
        private readonly ILocationTypeService _locationTypeService;
        public LocationTypesController(ILocationTypeService locationTypeService)
        {
            _locationTypeService = locationTypeService;
        }

        #region Grid

        [AdministratorNavigation("Location_Types", "Location_Holder", "Location_Types", "fa-list-ol", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int? locationId)
        {
            return JsonConvert.SerializeObject(_locationTypeService.SearchLocationTypes(si, locationId));
        }

        /// <summary>
        /// Export Location Types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, int? locationId)
        {
            var workbook = _locationTypeService.Exports(si, gridExportMode, locationId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "LocationTypes.xls");
        }

        #endregion

        #region Create

        [AdministratorNavigation("Location_Type_Create", "Location_Types", "Location_Type_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _locationTypeService.GetLocationTypeManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(LocationTypeManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _locationTypeService.SaveLocationType(model);
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

        #region Edit

        [AdministratorNavigation("Location_Type_Edit", "Location_Types", "Location_Type_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _locationTypeService.GetLocationTypeManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("LocationType_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(LocationTypeManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _locationTypeService.SaveLocationType(model);
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

        #region Popup Edit

        public ActionResult PopupEdit(int id)
        {
            SetupPopupAction();
            var model = _locationTypeService.GetLocationTypeManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("LocationType_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(LocationTypeManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _locationTypeService.SaveLocationType(model);
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

        #region Details

        [AdministratorNavigation("Location_Type_Details", "Location_Types", "Location_Type_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _locationTypeService.GetLocationTypeDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("LocationType_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateLocationTypeData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_locationTypeService.UpdateLocationTypeData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteLocationType(int id)
        {
            return Json(_locationTypeService.DeleteLocationType(id));
        }

        #endregion

        #endregion

        [HttpPost]
        public JsonResult DeleteLocationLocationTypeMapping(int locationTypeId, int locationId)
        {
            return Json(_locationTypeService.DeleteLocationLocationTypeMapping(locationTypeId, locationId));
        }
    }
}
