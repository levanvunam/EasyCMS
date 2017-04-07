using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.UserLoginHistories;
using EzCMS.Core.Services.UserLoginHistories;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class UserLoginHistoriesController : BackendController
    {
        private readonly IUserLoginHistoryService _userLoginHistoryService;
        public UserLoginHistoriesController(IUserLoginHistoryService userLoginHistoryService)
        {
            _userLoginHistoryService = userLoginHistoryService;
        }

        [AdministratorNavigation("User_Login_Histories", "User_Management", "User_Login_Histories", "fa-search-plus", 30, false)]
        public ActionResult Index(int? userId)
        {
            var model = new UserLoginHistorySearchModel(userId);
            return View(model);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, UserLoginHistorySearchModel model)
        {
            return JsonConvert.SerializeObject(_userLoginHistoryService.SearchUserLoginHistories(si, model));
        }

        /// <summary>
        /// Export user login histories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, UserLoginHistorySearchModel model)
        {
            var workbook = _userLoginHistoryService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "UserLoginHistories.xls");
        }

        #region Details

        [AdministratorNavigation("User_Login_History_Details", "User_Login_Histories", "User_Login_History_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _userLoginHistoryService.GetUserLoginHistoryDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("UserLoginHistory_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        #endregion
    }
}