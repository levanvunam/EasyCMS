using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.RssFeeds;
using EzCMS.Core.Services.RssFeeds;
using EzCMS.Entity.Core.Enums;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class RssFeedsController : BackendController
    {
        private readonly IRssFeedService _rssFeedService;
        public RssFeedsController(IRssFeedService rssFeedService)
        {
            _rssFeedService = rssFeedService;
        }

        #region Listing

        public ActionResult Index()
        {
            return View();
        }

        #region Ajax Methods

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_rssFeedService.SearchRssFeed(si));
        }


        #endregion

        /// <summary>
        /// Export rss feeds
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _rssFeedService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "RSSFeeds.xls");
        }
        #endregion

        #region Create

		[AdministratorNavigation("RSS_Feed_Create", "RSS_Feeds", "RSS_Feed_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _rssFeedService.GetRssFeedManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(RssFeedManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _rssFeedService.SaveRssFeed(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var id = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id });
                    }
                }
            }
            return View(model);
        }
        #endregion

        #region Edit

		[AdministratorNavigation("RSS_Feed_Edit", "RSS_Feeds", "RSS_Feed_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _rssFeedService.GetRssFeedManageModel(id);

            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("RSSFeed_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(RssFeedManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _rssFeedService.SaveRssFeed(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            if (!string.IsNullOrEmpty(returnUrl))
                                return Redirect(returnUrl);
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = model.Id, returnUrl });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Details

		[AdministratorNavigation("RSS_Feed_Details", "RSS_Feeds", "RSS_Feed_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _rssFeedService.GetRssFeedDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("RSSFeed_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion

        #region Delete

        [HttpPost]
        public JsonResult DeleteRssFeed(int id)
        {
            return Json(_rssFeedService.DeleteRssFeed(id));
        }


        [HttpPost]
        public JsonResult UpdateRssFeedData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_rssFeedService.UpdateRssFeedData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #endregion

        [HttpPost]
        public JsonResult GetRssFeedsByType(RssFeedEnums.RssType? rssType)
        {
            var feeds = _rssFeedService.GetRssFeeds(rssType);
            return Json(feeds);
        }

        [HttpPost]
        public JsonResult GetRssTypes()
        {
            var rssTypes = EnumUtilities.GenerateSelectListItems<RssFeedEnums.RssType>(GenerateEnumType.IntValueAndDescriptionText);

            return Json(new ResponseModel
            {
                Success = true,
                Data = rssTypes
            });
        }
    }
}
