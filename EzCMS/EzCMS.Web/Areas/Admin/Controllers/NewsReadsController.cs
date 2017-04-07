using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Models.NewsReads;
using EzCMS.Core.Services.NewsReads;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class NewsReadsController : BackendController
    {
        private readonly INewsReadService _newsReadService;
        public NewsReadsController(INewsReadService newsReadService)
        {
            _newsReadService = newsReadService;
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, NewsReadSearchModel model)
        {
            return JsonConvert.SerializeObject(_newsReadService.SearchNewsReads(si, model));
        }

        /// <summary>
        /// Export news reads
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, NewsReadSearchModel model)
        {
            var workbook = _newsReadService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "NewsReads.xls");
        }
    }
}
