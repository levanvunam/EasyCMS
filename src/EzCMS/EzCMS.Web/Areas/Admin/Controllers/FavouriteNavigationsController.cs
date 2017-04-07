using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Services.FavouriteNavigations;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class FavouriteNavigationsController : BackendController
    {
        private readonly IFavouriteNavigationService _favouriteNavigationService;
        public FavouriteNavigationsController(IFavouriteNavigationService favouriteNavigationService)
        {
            _favouriteNavigationService = favouriteNavigationService;
        }


        [AdministratorNavigation("Favourite_Navigations", "", "Favourite_Navigations", "fa-star", 10)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_favouriteNavigationService.SearchFavouriteNavigations(si));
        }

        /// <summary>
        /// Export FavouriteNavigations
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _favouriteNavigationService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "FavouriteNavigations.xls");
        }

        [HttpPost]
        public JsonResult AddToFavourites(string id)
        {
            return Json(_favouriteNavigationService.AddToFavourites(id));
        }

        [HttpPost]
        public JsonResult RemoveFromFavourites(string id)
        {
            return Json(_favouriteNavigationService.RemoveFromFavourites(id));
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            return Json(_favouriteNavigationService.Delete(id));
        }

        [HttpPost]
        public JsonResult MoveUp(int id)
        {
            return Json(_favouriteNavigationService.MoveUp(id));
        }

        [HttpPost]
        public JsonResult MoveDown(int id)
        {
            return Json(_favouriteNavigationService.MoveDown(id));
        }
    }
}
