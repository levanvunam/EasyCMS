using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Polls;
using EzCMS.Core.Services.Polls;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class PollsController : BackendController
    {
        private readonly IPollService _pollService;

        public PollsController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [AdministratorNavigation("Poll_Holder", "Module_Holder", "Poll_Holder", "fa-pie-chart", 110, true, true)]
        [AdministratorNavigation("Polls", "Poll_Holder", "Polls", "fa-bar-chart-o", 10)]
        public ActionResult Index()
        {
            return View();
        }

        #region Grid

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_pollService.SearchPolls(si));
        }

        /// <summary>
        /// Export polls
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _pollService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Polls.xls");
        }

        #endregion

        #region Create

		[AdministratorNavigation("Poll_Create", "Polls", "Poll_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _pollService.GetPollManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(PollManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _pollService.SavePoll(model);
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

		[AdministratorNavigation("Poll_Edit", "Polls", "Poll_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _pollService.GetPollManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Poll_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PollManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _pollService.SavePoll(model);
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

		[AdministratorNavigation("Poll_Details", "Polls", "Poll_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _pollService.GetPollDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Poll_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        #region Methods

        [HttpPost]
        public JsonResult UpdatePollData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_pollService.UpdatePollData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        [HttpPost]
        public JsonResult DeletePoll(int id)
        {
            return Json(_pollService.DeletePoll(id));
        }

        #endregion

        #endregion
    }
}
