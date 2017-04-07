using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Exceptions.Sites;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Pages;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.BodyTemplates;
using EzCMS.Core.Services.ClientNavigations;
using EzCMS.Core.Services.Pages;
using EzCMS.Core.Services.PageTemplates;
using EzCMS.Entity.Core.Enums;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize]
    public class PagesController : BackendController
    {
        private readonly IPageService _pageService;
        private readonly IPageTemplateService _pageTemplateService;
        private readonly IClientNavigationService _clientNavigationService;
        private readonly IBodyTemplateService _bodyTemplateService;

        public PagesController(IPageService pageService, IPageTemplateService pageTemplateService, IClientNavigationService clientNavigationService, IBodyTemplateService bodyTemplateService)
        {
            _pageService = pageService;
            _pageTemplateService = pageTemplateService;
            _clientNavigationService = clientNavigationService;
            _bodyTemplateService = bodyTemplateService;
        }

        #region Grid

        [AdministratorNavigation("Pages", "", "Pages", "fa-file-text", 30)]
        [AdministratorNavigation("All_Pages", "Pages", "All_Pages", "fa-file-text-o", 10)]
        [EzCMSAuthorize(IsAdministrator = true)]
        public ActionResult Index()
        {
            var model = new PageSearchModel();
            return View(model);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, PageSearchModel model)
        {
            return JsonConvert.SerializeObject(_pageService.SearchPages(si, model));
        }

        /// <summary>
        /// Export pages
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [EzCMSAuthorize(IsAdministrator = true)]
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, PageSearchModel model)
        {
            var workbook = _pageService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Pages.xls");
        }

        [HttpGet]
        public string _AjaxBindingForChildrenPages(JqSearchIn si, int parentId)
        {
            return JsonConvert.SerializeObject(_pageService.SearchChildrenPages(si, parentId));
        }

        /// <summary>
        /// Export pages
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [EzCMSAuthorize(IsAdministrator = true)]
        public ActionResult ExportsChildrenPages(JqSearchIn si, GridExportMode gridExportMode, int parentId)
        {
            var workbook = _pageService.ExportsChildrenPages(si, gridExportMode, parentId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Pages.xls");
        }

        #region Ajax Methods

        public JsonResult GetPageTemplates(int? id)
        {
            return Json(_pageTemplateService.GetPageTemplateSelectList(id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetParents(int? id)
        {
            return Json(_pageService.GetPossibleParents(id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatus(int? id)
        {
            return Json(_pageService.GetStatus(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeHomePage(int id)
        {
            return Json(_pageService.ChangeHomePage(id), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region Create

        [EzCMSAuthorize(IsAdministrator = true)]
        [AdministratorNavigation("Page_Create", "Pages", "Page_Create", "fa-plus", 10)]
        public ActionResult Create()
        {
            var model = _pageService.GetPageManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(PageManageModel model, SubmitType submitType = SubmitType.SaveAndContinueEdit)
        {
            if (ModelState.IsValid)
            {
                var response = _pageService.SavePageManageModel(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    //Notify contacts
                    if (model.Notify.NotifyContacts)
                    {
                        return RedirectToAction("InitializeNotification", "NotificationSetup", new
                        {
                            model.Notify.Id,
                            model.Notify.Parameters,
                            model.Notify.Module,
                        });
                    }

                    switch (submitType)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("Edit", new { id = (int)response.Data });
                    }
                }
            }

            return View(model);
        }
        #endregion

        #region Edit

        [EzCMSAuthorize(IsAdministrator = true)]
        [AdministratorNavigation("Page_Edit", "Pages", "Page_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int? id, int? logId)
        {
            PageManageModel model = null;
            if (id.HasValue)
            {
                model = _pageService.GetPageManageModel(id.Value);
            }
            else if (logId.HasValue)
            {
                model = _pageService.GetPageManageModelByLogId(logId.Value);
            }
            if (model == null)
            {
                SetErrorMessage(T("Page_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PageManageModel model, string returnUrl, SubmitType submitType = SubmitType.SaveAndContinueEdit, bool confirmedChangeUrl = false)
        {
            if (ModelState.IsValid)
            {
                var response = _pageService.SavePageManageModel(model, confirmedChangeUrl);
                SetResponseMessage(response);
                if (response.Success)
                {
                    //Notify contacts
                    if (model.Notify.NotifyContacts)
                    {
                        return RedirectToAction("InitializeNotification", "NotificationSetup", new
                        {
                            model.Notify.Id,
                            model.Notify.Parameters,
                            model.Notify.Module,
                        });
                    }

                    switch (submitType)
                    {
                        case SubmitType.Save:
                            if (!string.IsNullOrEmpty(returnUrl))
                                return Redirect(returnUrl);
                            return RedirectToAction("Index");
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("Edit", new { id = model.Id, returnUrl });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Popup Create

        public ActionResult PopupCreate(int? parentId, int? bodyTemplateId)
        {
            //Check if current user has permission to create any page or not
            if (!_pageService.CanCurrentUserCreateOrEditPage())
            {
                throw new EzCMSUnauthorizeException();
            }

            SetupPopupAction();
            var model = _pageService.GetPageManageModelWithParent(parentId, bodyTemplateId);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupCreate(PageManageModel model, SubmitType submitType = SubmitType.SaveAndContinueEdit)
        {
            //Check if current user has permission to create any page or not
            if (!_pageService.CanCurrentUserCreateOrEditPage(model.Id))
            {
                throw new EzCMSUnauthorizeException();
            }

            if (ModelState.IsValid)
            {
                var response = _pageService.SavePageManageModel(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    //Notify contacts
                    if (model.Notify.NotifyContacts)
                    {
                        return RedirectToAction("InitializeNotification", "NotificationSetup", new
                        {
                            model.Notify.Id,
                            model.Notify.Parameters,
                            model.Notify.Module,
                        });
                    }

                    var id = (int)response.Data;

                    switch (submitType)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox",
                                new CloseFancyBoxViewModel
                                {
                                    IsReload = true,
                                    ReturnUrl = "/" + model.FriendlyUrl
                                });
                        case SubmitType.SaveAndContinueEdit:
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
            //Check if current user has permission to edit any page or not
            if (!_pageService.CanCurrentUserCreateOrEditPage(id))
            {
                throw new EzCMSUnauthorizeException();
            }

            SetupPopupAction();
            var model = _pageService.GetPageManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Page_Message_ObjectNotFound"));
                return View("ShowMessageAndCloseFancyBox",
                    new CloseFancyBoxViewModel
                    {
                        IsReload = true
                    });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(PageManageModel model, SubmitType submitType = SubmitType.SaveAndContinueEdit, bool confirmedChangeUrl = false)
        {
            //Check if current user has permission to edit any page or not
            if (!_pageService.CanCurrentUserCreateOrEditPage(model.Id))
            {
                throw new EzCMSUnauthorizeException();
            }

            if (ModelState.IsValid)
            {
                var response = _pageService.SavePageManageModel(model, confirmedChangeUrl);
                SetResponseMessage(response);
                if (response.Success)
                {
                    //Notify contacts
                    if (model.Notify.NotifyContacts)
                    {
                        return RedirectToAction("InitializeNotification", "NotificationSetup", new
                        {
                            model.Notify.Id,
                            model.Notify.Parameters,
                            model.Notify.Module,
                        });
                    }

                    var page = _pageService.GetById(model.Id);

                    var returnUrl = page.FriendlyUrl.ToPageFriendlyUrl(page.IsHomePage);

                    switch (submitType)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox", new CloseFancyBoxViewModel
                            {
                                IsReload = true,
                                ReturnUrl = returnUrl
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

        public ActionResult PageInGoogleResult(int id)
        {
            //Check if current user has permission to edit any page or not
            if (!_pageService.CanCurrentUserCreateOrEditPage(id))
            {
                throw new EzCMSUnauthorizeException();
            }

            var model = _pageService.GetWebPageInformationModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Page_Message_ObjectNotFound"));
                return View("ShowMessageAndCloseFancyBox",
                    new CloseFancyBoxViewModel
                    {
                        IsReload = false
                    });
            }
            return View(model);
        }

        #region SEO Scoring

        [HttpPost]
        public JsonResult GetTitleChangedSEOScore(string title)
        {
            return Json(new ResponseModel
            {
                Success = true,
                Data = new
                {
                    title = _pageService.GetTitleChangedSEOScore(title)
                }
            });
        }

        [HttpPost]
        public JsonResult GetDescriptionChangedSEOScore(string description)
        {
            return Json(new ResponseModel
            {
                Success = true,
                Data = new
                {
                    description = _pageService.GetDescriptionChangedSEOScore(description)
                }
            });
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetContentChangedSEOScore(string content, string keywords)
        {
            PageEnums.SEOScore keywordWeightScore, keywordBoldedScore, headingTagScore, altTagScore;
            _pageService.GetContentChangedSEOScore(content, keywords, out keywordWeightScore, out keywordBoldedScore, out headingTagScore, out altTagScore);
            return Json(new ResponseModel
            {
                Success = true,
                Data = new
                {
                    keywordWeight = keywordWeightScore,
                    keywordBolded = keywordBoldedScore,
                    headingTag = headingTagScore,
                    altTag = altTagScore
                }
            });
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetKeywordsChangedSEOScore(string title, string description, string content, string keywords)
        {
            PageEnums.SEOScore keywordCountScore, keywordWeightScore, keywordBoldedScore, headingTagScore, altTagScore;
            _pageService.GetKeywordsChangedSEOScore(content, keywords, out keywordCountScore, out keywordWeightScore, out keywordBoldedScore, out headingTagScore, out altTagScore);
            return Json(new ResponseModel
            {
                Success = true,
                Data = new
                {
                    keywordCount = keywordCountScore,
                    keywordWeight = keywordWeightScore,
                    keywordBolded = keywordBoldedScore,
                    headingTag = headingTagScore,
                    altTag = altTagScore
                }
            });
        }

        #endregion

        #region Select Body Template

        public ActionResult SelectBodyTemplate(BodyTemplateEnums.Mode mode)
        {
            var model = _bodyTemplateService.GetBodyTemplateSelector(mode);
            return View("SelectBodyTemplate/SelectBodyTemplate", model);
        }

        public ActionResult PreviewOnlineBodyTemplate(int id)
        {
            var model = _bodyTemplateService.GetOnlineTemplate(id);

            if (model != null)
            {
                return View("_Preview", new ContentPreviewModel(model.Content));
            }

            SetErrorMessage(T("BodyTemplate_Message_OnlineTemplateNotFound"));
            return View("ShowMessageAndCloseFancyBox", new CloseFancyBoxViewModel
            {
                IsReload = false,
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult SearchOnlineBodyTemplates(string keyword, int? pageIndex, int? pageSize)
        {
            var model = _bodyTemplateService.SearchBodyTemplates(keyword, pageIndex, pageSize);
            return Json(new ResponseModel
            {
                Success = true,
                Data = RenderPartialViewToString("SelectBodyTemplate/_OnlineBodyTemplates", model)
            });
        }

        [HttpPost]
        public JsonResult ReloadInstalledTemplates()
        {
            var model = _bodyTemplateService.GetBodyTemplateSelector(BodyTemplateEnums.Mode.Create);
            return Json(new ResponseModel
            {
                Success = true,
                Data = RenderPartialViewToString("SelectBodyTemplate/_InstalledBodyTemplates", model)
            });
        }

        [HttpPost]
        public ActionResult DownloadOnlineBodyTemplate(int id, string name)
        {
            return Json(_bodyTemplateService.DownloadedOnlineTemplate(id, name));
        }

        [HttpPost]
        public JsonResult ChooseBodyTemplate(int id)
        {
            var model = _bodyTemplateService.GetChosenBodyTemplateModel(id);
            if (model == null)
            {
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = T("BodyTemplate_Message_ObjectNotFound")
                });
            }

            return Json(new ResponseModel
            {
                Success = true,
                Data = model
            });
        }

        #endregion

        #endregion

        #region Delete

        public ActionResult DeleteConfirm(int id)
        {
            var model = _pageService.GetPageDeleteModel(id);

            if (model == null)
            {
                SetErrorMessage(T("Page_Message_ObjectNotFound"));
                return View("ShowMessageAndCloseFancyBox", new CloseFancyBoxViewModel
                {
                    IsReload = false
                });
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult Delete(PageDeleteModel model)
        {
            var response = _pageService.DeletePage(model);
            return Json(response);
        }

        #endregion

        #region Details

        [EzCMSAuthorize(IsAdministrator = true)]
        [AdministratorNavigation("Page_Details", "Pages", "Page_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _pageService.GetPageDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Page_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [EzCMSAuthorize(IsAdministrator = true)]
        public JsonResult UpdatePageData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_pageService.UpdatePageData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #endregion

        #region Setup Page Order

        public ActionResult SetupPageOrder()
        {
            var model = _clientNavigationService.GenerateSiteMap();
            return View(model);
        }

        [HttpPost]
        public ActionResult SetupPageOrder(PageOrderSetupModel model)
        {
            return Json(_pageService.SavePageOrder(model));
        }

        #endregion

        #region Logs

        /// <summary>
        /// Manage page logs
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [EzCMSAuthorize(IsAdministrator = true)]
        public ActionResult Logs(int id)
        {
            var model = _pageService.GetLogs(id);
            if (model == null)
            {
                SetErrorMessage(T("Page_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult GetLogs(int id, int total, int index)
        {
            var model = _pageService.GetLogs(id, total, index);
            var content = RenderPartialViewToString("Partials/_GetLogs", model);
            var response = new ResponseModel
            {
                Success = true,
                Data = new
                {
                    model.LoadComplete,
                    content
                }
            };
            return Json(response);
        }

        #endregion

        #region Post Methods

        public JsonResult GetRelativePages(int? id, int? parentId)
        {
            return Json(_pageService.GetRelativePages(id, parentId));
        }

        public JsonResult GetBodyTemplates(int? id)
        {
            return Json(_bodyTemplateService.GetBodyTemplates(id));
        }

        [HttpPost]
        public JsonResult DeletePageBodyTemplateMapping(int pageId, int bodyTemplateId)
        {
            var response = _pageService.DeletePageBodyTemplateMapping(pageId, bodyTemplateId);
            return Json(response);
        }

        [HttpPost]
        public JsonResult DeletePageFileTemplateMapping(int pageId, int bodyTemplateId)
        {
            var response = _pageService.DeletePageFileTemplateMapping(pageId, bodyTemplateId);
            return Json(response);
        }

        [HttpPost]
        public JsonResult DeletePagePageTemplateMapping(int pageId, int bodyTemplateId)
        {
            var response = _pageService.DeletePagePageTemplateMapping(pageId, bodyTemplateId);
            return Json(response);
        }

        [HttpPost]
        public JsonResult GetEditConfirmInformation(int id, string newFriendlyUrl)
        {
            var model = _pageService.GetPageConfirmChangeUrlModel(id, newFriendlyUrl);

            if (model != null)
            {
                return Json(new ResponseModel
                {
                    Success = true,
                    Data = new
                    {
                        needConfirm = true,
                        content = RenderPartialViewToString("Partials/_ConfirmChangeUrl", model)
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new ResponseModel
            {
                Success = true,
                Data = new
                {
                    needConfirm = false
                }
            });
        }
        #endregion
    }
}