using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.RemoteAuthentications;
using EzCMS.Core.Services.RemoteAuthentications;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class RemoteAuthenticationsController : BackendController
    {
        private readonly IRemoteAuthenticationService _remoteAuthenticationService;
        public RemoteAuthenticationsController(IRemoteAuthenticationService remoteAuthenticationService)
        {
            _remoteAuthenticationService = remoteAuthenticationService;
        }

        [AdministratorNavigation("Remote_Authenticate_Configurations", "SystemSetting_Holder", "Remote_Authenticate_Configurations", "fa-wifi", 50)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_remoteAuthenticationService.SearchRemoteAuthentications(si));
        }

        /// <summary>
        /// Export remote authenticate configurations
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _remoteAuthenticationService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "RemoteAuthentications.xls");
        }
        #region Create
		
		[AdministratorNavigation("Remote_Authenticate_Configuration_Create", "Remote_Authenticate_Configurations", "Remote_Authenticate_Configuration_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _remoteAuthenticationService.GetRemoteAuthenticationManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(RemoteAuthenticationManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _remoteAuthenticationService.SaveRemoteAuthentication(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var id = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("Remote_Authenticate_Configuration_Edit", "Remote_Authenticate_Configurations", "Remote_Authenticate_Configuration_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _remoteAuthenticationService.GetRemoteAuthenticationManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("RemoteAuthentication_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(RemoteAuthenticationManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _remoteAuthenticationService.SaveRemoteAuthentication(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = model.Id });
                    }
                }
            }
            return View(model);
        }

        #endregion
    }
}
