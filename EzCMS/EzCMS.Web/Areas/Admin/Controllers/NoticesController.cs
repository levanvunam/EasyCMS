using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Notices;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.Notices;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class NoticesController : BackendController
    {
        private readonly INoticeService _noticeService;
        public NoticesController(INoticeService noticeService)
        {
            _noticeService = noticeService;
        }

        [AdministratorNavigation("Notice_Holder", "Module_Holder", "Notice_Holder", "fa-thumb-tack", 80, true, true)]
        [AdministratorNavigation("Notices", "Notice_Holder", "Notices", "fa-thumb-tack", 10)]
        public ActionResult Index()
        {
            var model = new NoticeSearchModel();
            return View(model);
        }

        #region Grid

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, NoticeSearchModel model)
        {
            return JsonConvert.SerializeObject(_noticeService.SearchNotices(si, model));
        }

        /// <summary>
        /// Export notices
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, NoticeSearchModel model)
        {
            var workbook = _noticeService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Notices.xls");
        }

        #endregion

        #region Create

        [AdministratorNavigation("Notice_Create", "Notices", "Notice_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _noticeService.GetNoticeManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(NoticeManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _noticeService.SaveNotice(model);
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

        [AdministratorNavigation("Notice_Edit", "Notices", "Notice_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _noticeService.GetNoticeManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Notice_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(NoticeManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _noticeService.SaveNotice(model);
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

        #region Popup Create

        public ActionResult PopupCreate(int? noticeTypeId)
        {
            SetupPopupAction();
            var model = _noticeService.GetNoticeManageModel(null, noticeTypeId);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupCreate(NoticeManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _noticeService.SaveNotice(model);
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

        #region Popup Edit

        public ActionResult PopupEdit(int id)
        {
            SetupPopupAction();
            var model = _noticeService.GetNoticeManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Notice_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(NoticeManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _noticeService.SaveNotice(model);
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

        [AdministratorNavigation("Notice_Details", "Notices", "Notice_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _noticeService.GetNoticeDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Notice_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateNoticeData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_noticeService.UpdateNoticeData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteNotice(int id)
        {
            return Json(_noticeService.DeleteNotice(id));
        }

        #endregion

        #endregion
    }
}
