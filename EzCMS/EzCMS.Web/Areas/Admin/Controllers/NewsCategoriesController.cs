using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.NewsCategories;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.NewsCategories;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class NewsCategoriesController : BackendController
    {
        private readonly INewsCategoryService _newsCategoryService;
        public NewsCategoriesController(INewsCategoryService newsCategoryService)
        {
            _newsCategoryService = newsCategoryService;
        }

        [AdministratorNavigation("News_Categories", "News_Holder", "News_Categories", "fa-list-ol", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_newsCategoryService.SearchNewsCategories(si));
        }

        /// <summary>
        /// Export news categories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _newsCategoryService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "NewsCategories.xls");
        }

        [HttpGet]
        public string _AjaxBindingByNews(JqSearchIn si, int newsId)
        {
            return JsonConvert.SerializeObject(_newsCategoryService.SearchNewsCategoriesOfNews(si, newsId));
        }

        /// <summary>
        /// Export news categories of news
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public ActionResult ExportsNewsCategoriesByNews(JqSearchIn si, GridExportMode gridExportMode, int newsId)
        {
            var workbook = _newsCategoryService.ExportsNewsCategoriesOfNews(si, gridExportMode, newsId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "News_Categories.xls");
        }

        [HttpGet]
        public string _AjaxBindingChildren(JqSearchIn si, int newsCategoryId)
        {
            return JsonConvert.SerializeObject(_newsCategoryService.SearchChildrenNewsCategories(si, newsCategoryId));
        }

        /// <summary>
        /// Export news categories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="newsCategoryId"></param>
        /// <returns></returns>
        public ActionResult ExportsChildrenNewsCategories(JqSearchIn si, GridExportMode gridExportMode, int newsCategoryId)
        {
            var workbook = _newsCategoryService.ExportsChildrenNewsCategories(si, gridExportMode, newsCategoryId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "NewsCategories.xls");
        }

        #region Create

        [AdministratorNavigation("News_Category_Create", "News_Categories", "News_Category_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _newsCategoryService.GetNewsCategoryManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(NewsCategoryManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _newsCategoryService.SaveNewsCategoryManageModel(model);
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

        [AdministratorNavigation("News_Category_Edit", "News_Categories", "News_Category_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _newsCategoryService.GetNewsCategoryManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("NewsCategory_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(NewsCategoryManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _newsCategoryService.SaveNewsCategoryManageModel(model);
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
            var model = _newsCategoryService.GetNewsCategoryManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("NewsCategory_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(NewsCategoryManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _newsCategoryService.SaveNewsCategoryManageModel(model);
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

        [AdministratorNavigation("News_Category_Details", "News_Categories", "News_Category_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _newsCategoryService.GetNewsCategoryDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("NewsCategory_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateNewsCategoryData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_newsCategoryService.UpdateNewsCategoryData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteNewsCategory(int id)
        {
            return Json(_newsCategoryService.DeleteNewsCategory(id));
        }

        #endregion

        #endregion
    }
}
