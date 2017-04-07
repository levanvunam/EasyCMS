using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.ClientNavigations;
using EzCMS.Core.Services.ClientNavigations;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class ClientNavigationsController : BackendController
    {
        private readonly IClientNavigationService _clientNavigationService;
        public ClientNavigationsController(IClientNavigationService clientNavigationService)
        {
            _clientNavigationService = clientNavigationService;
        }

        #region Grid

        [AdministratorNavigation("Client_Navigations", "Page_Content", "Client_Navigations", "fa-list", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_clientNavigationService.SearchClientNavigations(si));
        }

        /// <summary>
        /// Export client menus
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _clientNavigationService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "ClientNavigations.xls");
        }
        #region Ajax Methods
        public JsonResult GetParents(int? id)
        {
            return Json(_clientNavigationService.GetPossibleParents(id), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region Create

        [AdministratorNavigation("Client_Navigation_Create", "Client_Navigations", "Client_Navigation_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _clientNavigationService.GetClientNavigationManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ClientNavigationManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _clientNavigationService.SaveClientNavigationManageModel(model);
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
            model.Parents = _clientNavigationService.GetPossibleParents();
            return View(model);
        }
        #endregion

        #region Edit

        [AdministratorNavigation("Client_Navigation_Edit", "Client_Navigations", "Client_Navigation_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _clientNavigationService.GetClientNavigationManageModel(id);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ClientNavigationManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _clientNavigationService.SaveClientNavigationManageModel(model);
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
            model.Parents = _clientNavigationService.GetPossibleParents();
            return View(model);
        }

        #endregion

        [HttpPost]
        public JsonResult DeleteNavigation(int id)
        {
            return Json(_clientNavigationService.DeleteNavigation(id));
        }

        public JsonResult GetRelativeNavigations(int? id, int? parentId)
        {
            return Json(_clientNavigationService.GetRelativeNavigations(id, parentId));
        }
    }
}
