using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.News;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.News;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class NewsController : BackendController
    {
        private readonly INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        #region Listing

        [AdministratorNavigation("News_Holder", "", "News_Holder", "fa-newspaper-o", 60, true, true)]
        [AdministratorNavigation("News", "News_Holder", "News", "fa-newspaper-o", 10)]
        public ActionResult Index()
        {
            var model = new NewsSearchModel();
            return View(model);
        }

        #region Ajax Methods

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, NewsSearchModel model)
        {
            return JsonConvert.SerializeObject(_newsService.SearchNews(si, model));
        }

        #endregion

        /// <summary>
        /// Export News
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, NewsSearchModel model)
        {
            var workbook = _newsService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "News.xls");
        }
        #endregion

        #region Create

        [AdministratorNavigation("News_Create", "News", "News_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _newsService.GetNewsManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(NewsManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _newsService.SaveNews(model);
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

        [AdministratorNavigation("News_Edit", "News", "News_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _newsService.GetNewsManageModel(id);

            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("News_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(NewsManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _newsService.SaveNews(model);
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
            var model = _newsService.GetNewsManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("News_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(NewsManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _newsService.SaveNews(model);
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

        [AdministratorNavigation("News_Details", "News", "News_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _newsService.GetNewsDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("News_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateNewsData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_newsService.UpdateNewsData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteNews(int id)
        {
            return Json(_newsService.DeleteNews(id));
        }

        #endregion

        #endregion

        [HttpPost]
        public JsonResult DeleteNewsNewsCategoryMapping(int newsId, int newsCategoryId)
        {
            var response = _newsService.DeleteNewsNewsCategoryMapping(newsId, newsCategoryId);
            return Json(response);
        }
    }
}
