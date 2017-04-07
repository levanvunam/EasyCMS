using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Models.Tags;
using EzCMS.Core.Services.Tags;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class TagsController : BackendController
    {
        private readonly ITagService _tagService;
        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        #region Grid

        [AdministratorNavigation("Tags", "Pages", "Tags", "fa-tags", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int? pageId)
        {
            return JsonConvert.SerializeObject(_tagService.SearchTags(si, pageId));
        }

        /// <summary>
        /// Export tags
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, int? pageId)
        {
            var workbook = _tagService.Exports(si, gridExportMode, pageId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Tags.xls");
        }

        #endregion

        #region Create

        [AdministratorNavigation("Tag_Create", "Tags", "Tag_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _tagService.GetTagManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(TagManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _tagService.SaveTag(model);
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

        [AdministratorNavigation("Tag_Edit", "Tags", "Tag_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _tagService.GetTagManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Tag_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(TagManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _tagService.SaveTag(model);
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
            var model = _tagService.GetTagManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Tag_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(TagManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _tagService.SaveTag(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox",
                                new CloseFancyBoxViewModel
                                {
                                    IsReload = true
                                });
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("PopupEdit", new { id = model.Id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        [HttpPost]
        public ActionResult GetTags(int? pageId)
        {
            return Json(new ResponseModel
            {
                Success = true,
                Data = _tagService.GetTags(pageId)
            });
        }

        #region Details

        [AdministratorNavigation("Tag_Details", "Tags", "Tag_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _tagService.GetTagDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Tag_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateTagData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_tagService.UpdateTagData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteTag(int id)
        {
            return Json(_tagService.DeleteTag(id));
        }

        #endregion

        #endregion

        [HttpPost]
        public JsonResult DeletePageTagMapping(int tagId, int pageId)
        {
            var response = _tagService.DeletePageTagMapping(tagId, pageId);
            return Json(response);
        }
    }
}
