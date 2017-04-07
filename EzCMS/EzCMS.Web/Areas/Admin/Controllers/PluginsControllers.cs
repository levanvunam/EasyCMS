using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Plugins;
using EzCMS.Core.Services.Plugins;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class PluginsController : BackendController
    {
        private readonly IPluginService _pluginService;
        public PluginsController(IPluginService pluginService)
        {
            _pluginService = pluginService;
        }

        [AdministratorNavigation("Plugins", "Plugin_Management", "Plugins", "fa-plug", 30)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_pluginService.SearchPlugins(si));
        }

        /// <summary>
        /// Export Plugins
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _pluginService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Plugins.xls");
        }

        #region Config

        [AdministratorNavigation("Plugin_Config", "Plugins", "Plugin_Config", "fa-cog", 10, false)]
        public ActionResult Config(string id)
        {
            var model = _pluginService.GetPluginManageModel(id);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Config(PluginInformationModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _pluginService.SavePluginManageModel(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Config", new { name = model.Name });
                    }
                }
            }
            return View(model);
        }

        #endregion
    }
}
