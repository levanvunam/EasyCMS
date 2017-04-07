using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Models.AnonymousContacts;
using EzCMS.Core.Services.AnonymousContacts;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class AnonymousContactsController : BackendController
    {
        private readonly IAnonymousContactService _pageReadService;
        public AnonymousContactsController(IAnonymousContactService pageReadService)
        {
            _pageReadService = pageReadService;
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, AnonymousContactSearchModel model)
        {
            return JsonConvert.SerializeObject(_pageReadService.SearchAnonymousContacts(si, model));
        }

        /// <summary>
        /// Export page reads
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, AnonymousContactSearchModel model)
        {
            var workbook = _pageReadService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "AnonymousContacts.xls");
        }
    }
}
