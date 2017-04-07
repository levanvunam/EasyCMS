using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.BackgroundTasks;
using EzCMS.Core.Services.BackgroundTasks;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class BackgroundTasksController : BackendController
    {
        private readonly IBackgroundTaskService _backgroundTaskService;
        public BackgroundTasksController(IBackgroundTaskService backgroundTaskService)
        {
            _backgroundTaskService = backgroundTaskService;
        }

        #region Grid

        [AdministratorNavigation("Background_Tasks", "System_Setting_Holder", "Background_Tasks", "fa-tasks", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_backgroundTaskService.SearchBackgroundTasks(si));
        }

        /// <summary>
        /// Export background tasks
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _backgroundTaskService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "BackgroundTasks.xls");
        }

        #endregion

        #region Config

        public ActionResult Config(int id)
        {
            var model = _backgroundTaskService.GetBackgroundTaskManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("BackgroundTask_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        		[AdministratorNavigation("Background_Task_Config", "Background_Tasks", "Background_Task_Config", "fa-cog", 10, false)]
        public ActionResult Config(BackgroundTaskManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _backgroundTaskService.SaveBackgroundTask(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Config", new { id = model.Id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Logs

        public ActionResult Logs(int id)
        {
            var model = _backgroundTaskService.GetLogs(id, DateTime.UtcNow.ToString("d"));
            if (model == null)
            {
                SetErrorMessage(T("BackgroundTask_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult GetLogs(int id, string currentDateLog)
        {
            var model = _backgroundTaskService.GetLogs(id, currentDateLog);
            var content = RenderPartialViewToString("Partials/_GetLogs", model);
            var response = new ResponseModel
            {
                Success = true,
                Data = new
                {
                    model.LoadComplete,
                    content,
                    CurrentDateLog = model.NextDateLog
                }
            };
            return Json(response);
        }

        [HttpPost]
        public ActionResult GetLogsInfo(string backgroundTaskName, string dateLogs)
        {
            var logsDetails = _backgroundTaskService.GetLogsDetails(backgroundTaskName, dateLogs);
            return PartialView("Partials/_GetLogsDetails", logsDetails);
        }

        #endregion
    }
}
