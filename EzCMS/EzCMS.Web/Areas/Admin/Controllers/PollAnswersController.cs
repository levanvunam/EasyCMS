using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.PollAnswers;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.PollAnswers;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class PollAnswersController : BackendController
    {
        private readonly IPollAnswerService _pollAnswerService;

        public PollAnswersController(IPollAnswerService pollAnswerService)
        {
            _pollAnswerService = pollAnswerService;
        }

        #region Listing

        [AdministratorNavigation("Poll_Answers", "Poll_Holder", "Poll_Answers", "fa-reply", 20)]
        public ActionResult Index()
        {
            var model = new PollAnswerSearchModel();
            return View(model);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, PollAnswerSearchModel model)
        {
            return JsonConvert.SerializeObject(_pollAnswerService.SearchPollAnswers(si, model));
        }

        /// <summary>
        /// Export poll answers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, PollAnswerSearchModel model)
        {
            var workbook = _pollAnswerService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "PollAnswers.xls");
        }

        #endregion

        #region Create

		[AdministratorNavigation("Poll_Answer_Create", "Poll_Answers", "Poll_Answer_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _pollAnswerService.GetPollAnswerManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(PollAnswerManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _pollAnswerService.SavePollAnswer(model);
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

		[AdministratorNavigation("Poll_Answer_Edit", "Poll_Answers", "Poll_Answer_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _pollAnswerService.GetPollAnswerManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("PollAnswer_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PollAnswerManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _pollAnswerService.SavePollAnswer(model);
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
                            return RedirectToAction("Edit", new { id = model.Id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Details

		[AdministratorNavigation("Poll_Answer_Details", "Poll_Answers", "Poll_Answer_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _pollAnswerService.GetPollAnswerDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("PollAnswer_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        #region Methods

        [HttpPost]
        public JsonResult UpdatePollAnswerData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_pollAnswerService.UpdatePollAnswerData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        [HttpPost]
        public JsonResult DeletePollAnswer(int id)
        {
            return Json(_pollAnswerService.DeletePollAnswer(id));
        }

        #endregion

        #endregion

        #region Popup Create

        public ActionResult PopupCreate(int? pollId)
        {
            SetupPopupAction();
            var model = _pollAnswerService.GetPollAnswerManageModel(null, pollId);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupCreate(PollAnswerManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _pollAnswerService.SavePollAnswer(model);
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
            var model = _pollAnswerService.GetPollAnswerManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("PollAnswer_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(PollAnswerManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _pollAnswerService.SavePollAnswer(model);
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

        #region Get Poll Answers

        public JsonResult GetPollAnswers(int pollId)
        {
            var pollAnswers = _pollAnswerService.GetPollAnswers(pollId);
            return Json(pollAnswers, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
