using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.NoticeTypes;
using EzCMS.Core.Services.NoticeTypes;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class NoticeTypesController : BackendController
    {
        private readonly INoticeTypeService _noticeTypeService;
        public NoticeTypesController(INoticeTypeService noticeTypeService)
        {
            _noticeTypeService = noticeTypeService;
        }

        [AdministratorNavigation("Notice_Types", "Notice_Holder", "Notice_Types", "fa-thumb-tack", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_noticeTypeService.SearchNoticeTypes(si));
        }

        /// <summary>
        /// Export NoticeTypes
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _noticeTypeService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "NoticeTypes.xls");
        }

        #region Create

        [AdministratorNavigation("Notice_Type_Create", "Notice_Types", "Notice_Type_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _noticeTypeService.GetNoticeTypeManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(NoticeTypeManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _noticeTypeService.SaveNoticeType(model);
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

        [AdministratorNavigation("Notice_Type_Edit", "Notice_Types", "Notice_Type_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _noticeTypeService.GetNoticeTypeManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("NoticeType_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(NoticeTypeManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _noticeTypeService.SaveNoticeType(model);
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

        [AdministratorNavigation("Notice_Type_Details", "Notice_Types", "Notice_Type_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _noticeTypeService.GetNoticeTypeDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("NoticeType_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        #region Methods

        [HttpPost]
        public JsonResult UpdateNoticeTypeData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_noticeTypeService.UpdateNoticeTypeData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        [HttpPost]
        public JsonResult DeleteNoticeType(int id)
        {
            return Json(_noticeTypeService.DeleteNoticeType(id));
        }

        #endregion

        #endregion
    }
}
