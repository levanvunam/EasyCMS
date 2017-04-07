using Ez.Framework.Configurations;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.JqGrid;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Models.SQLTool;
using EzCMS.Core.Services.SQLTool;
using Newtonsoft.Json;
using System.Text;
using System.Web.Mvc;
using EzCMS.Core.Framework.Configuration;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class SQLToolController : BackendController
    {
        private readonly ISQLCommandService _sqlCommandService;

        public SQLToolController()
        {
            _sqlCommandService = HostContainer.GetInstance<ISQLCommandService>();
        }

        public ActionResult Index()
        {
            var executor = new SQLExecutor();
            var model = new SQLResult
            {
                ConnectionString = _sqlCommandService.GetConnectionString(),
                Histories = _sqlCommandService.GetHistories(EzCMSContants.DefaultHistoryStart, EzCMSContants.DefaultHistoryLength),
                ReadOnly = true,
                Tables = executor.GetTableNames()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(SQLRequest request, bool exportToFile = false)
        {
            var executor = new SQLExecutor();
            var result = executor.Execute(request);
            if (exportToFile)
            {
                return new FileContentResult(Encoding.UTF8.GetBytes(result.ToString()), "text/plain")
                {
                    FileDownloadName = "QueryResult.txt"
                };
            }
            return View(result);
        }

        public ActionResult Histories()
        {
            return View();
        }

        public string _AjaxHistory(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_sqlCommandService.SearchCommands(si));
        }

        public JsonResult GenerateSelectStatement(string tablename)
        {
            var executor = new SQLExecutor();
            return Json(executor.GenerateSelectCommand(tablename));
        }

        public JsonResult GetHistories()
        {
            return Json(_sqlCommandService.GetHistories(EzCMSContants.DefaultHistoryStart, EzCMSContants.DefaultHistoryLength));
        }

        #region Methods
        public JsonResult GenerateInsertStatement(string tablename)
        {
            var executor = new SQLExecutor();
            return Json(executor.GenerateInsertCommand(tablename));
        }

        public JsonResult GenerateUpdateStatement(string tablename)
        {
            var executor = new SQLExecutor();
            return Json(executor.GenerateUpdateCommand(tablename));
        }

        public JsonResult GenerateDeleteStatement(string tablename)
        {
            var executor = new SQLExecutor();
            return Json(executor.GenerateDeleteCommand(tablename));
        }

        public JsonResult GenerateCreateStatement(string tablename)
        {
            var executor = new SQLExecutor();
            return Json(executor.GenerateCreateCommand(tablename));
        }

        public JsonResult GenerateAlterStatement(string tablename)
        {
            var executor = new SQLExecutor();
            return Json(executor.GenerateAlterCommand(tablename));
        }

        #endregion
    }
}
