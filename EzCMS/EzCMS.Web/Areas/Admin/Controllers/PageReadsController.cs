using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Models.PageReads;
using EzCMS.Core.Services.PageReads;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class PageReadsController : BackendController
    {
        private readonly IPageReadService _pageReadService;
        public PageReadsController(IPageReadService pageReadService)
        {
            _pageReadService = pageReadService;
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, PageReadSearchModel model)
        {
            return JsonConvert.SerializeObject(_pageReadService.SearchPageReads(si, model));
        }

        /// <summary>
        /// Export page reads
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, PageReadSearchModel model)
        {
            var workbook = _pageReadService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "PageReads.xls");
        }
    }
}
