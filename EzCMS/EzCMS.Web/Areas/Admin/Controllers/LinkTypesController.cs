using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.LinkTypes;
using EzCMS.Core.Services.LinkTypes;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class LinkTypesController : BackendController
    {
        private readonly ILinkTypeService _linkTypeService;
        public LinkTypesController(ILinkTypeService linkTypeService)
        {
            _linkTypeService = linkTypeService;
        }

        [AdministratorNavigation("Link_Types", "Link_Holder", "Link_Types", "fa-list-ol", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_linkTypeService.SearchLinkTypes(si));
        }

        /// <summary>
        /// Export LinkTypes
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _linkTypeService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "LinkTypes.xls");
        }

        #region Create		

        [AdministratorNavigation("Link_Type_Create", "Link_Types", "Link_Type_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _linkTypeService.GetLinkTypeManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(LinkTypeManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _linkTypeService.SaveLinkType(model);
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

        [AdministratorNavigation("Link_Type_Edit", "Link_Types", "Link_Type_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _linkTypeService.GetLinkTypeManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("LinkType_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(LinkTypeManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _linkTypeService.SaveLinkType(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            if (!string.IsNullOrEmpty(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }

                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = model.Id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Details

        [AdministratorNavigation("Link_Type_Details", "Link_Types", "Link_Type_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _linkTypeService.GetLinkTypeDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("LinkType_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateLinkTypeData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_linkTypeService.UpdateLinkTypeData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteLinkType(int id)
        {
            return Json(_linkTypeService.DeleteLinkType(id));
        }

        #endregion

        #endregion
    }
}
