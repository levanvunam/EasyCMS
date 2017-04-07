using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Links;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.Links;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class LinksController : BackendController
    {
        private readonly ILinkService _linkService;
        public LinksController(ILinkService linkService)
        {
            _linkService = linkService;
        }

        [AdministratorNavigation("Link_Holder", "Module_Holder", "Link_Holder", "fa-link", 70, true, true)]
        [AdministratorNavigation("Links", "Link_Holder", "Links", "fa-link", 10)]
        public ActionResult Index()
        {
            var model = new LinkSearchModel();
            return View(model);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, LinkSearchModel model)
        {
            return JsonConvert.SerializeObject(_linkService.SearchLinks(si, model));
        }

        /// <summary>
        /// Export Links
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, LinkSearchModel model)
        {
            var workbook = _linkService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Links.xls");
        }

        #region Create

        [AdministratorNavigation("Link_Create", "Links", "Link_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _linkService.GetLinkManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(LinkManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _linkService.SaveLink(model);
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

        public ActionResult PopupCreate(int? linkTypeId)
        {
            SetupPopupAction();
            var model = _linkService.GetLinkManageModel(null, linkTypeId);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupCreate(LinkManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _linkService.SaveLink(model);
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

        [AdministratorNavigation("Link_Edit", "Links", "Link_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _linkService.GetLinkManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Link_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(LinkManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _linkService.SaveLink(model);
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
            var model = _linkService.GetLinkManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Link_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(LinkManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _linkService.SaveLink(model);
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

        [AdministratorNavigation("Link_Details", "Links", "Link_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _linkService.GetLinkDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Link_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateLinkData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_linkService.UpdateLinkData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteLink(int id)
        {
            return Json(_linkService.DeleteLink(id));
        }

        #endregion

        #endregion
    }
}
