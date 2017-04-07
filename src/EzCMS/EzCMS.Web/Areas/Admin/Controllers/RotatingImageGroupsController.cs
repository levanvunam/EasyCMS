using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.RotatingImageGroups;
using EzCMS.Core.Services.RotatingImageGroups;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class RotatingImageGroupsController : BackendController
    {
        private readonly IRotatingImageGroupService _rotatingImageGroupService;

        public RotatingImageGroupsController(IRotatingImageGroupService rotatingImageGroupService)
        {
            _rotatingImageGroupService = rotatingImageGroupService;
        }

        #region Grid


        [AdministratorNavigation("Rotating_Image_Groups", "Module_Holder", "Rotating_Image_Groups", "fa-image", 140)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_rotatingImageGroupService.SearchRotatingImageGroups(si));
        }

        /// <summary>
        /// Export rotating image groups
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _rotatingImageGroupService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "RotatingImageGroups.xls");
        }

        #endregion

        public JsonResult GetGroups()
        {
            return Json(_rotatingImageGroupService.GetRotatingImageGroups(), JsonRequestBehavior.AllowGet);
        }

        #region Create


        [AdministratorNavigation("Rotating_Image_Group_Create", "Rotating_Image_Groups", "Rotating_Image_Group_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _rotatingImageGroupService.GetRotatingImageGroupManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(RotatingImageGroupManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _rotatingImageGroupService.SaveRotatingImageGroup(model);
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

		[AdministratorNavigation("Rotating_Image_Group_Edit", "Rotating_Image_Groups", "Rotating_Image_Group_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _rotatingImageGroupService.GetRotatingImageGroupManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("RotatingImageGroup_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(RotatingImageGroupManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _rotatingImageGroupService.SaveRotatingImageGroup(model);
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

        #region Edit Settings

        [AdministratorNavigation("Rotating_Image_Group_Edit_Settings", "Rotating_Image_Groups", "Rotating_Image_Group_Edit_Settings", "fa-cogs", 20, false)]
        public ActionResult EditSettings(int id)
        {
            var model = _rotatingImageGroupService.GetGroupManageSettingModel(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditSettings(GroupManageSettingModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _rotatingImageGroupService.SaveGroupSettings(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("EditSettings", new { id = model.Id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Rotating Image Gallery

        [AdministratorNavigation("Rotating_Image_Groups_Gallery", "Rotating_Image_Groups", "Rotating_Image_Groups_Gallery", "fa-image", 30, false)]
        public ActionResult Gallery(int id)
        {
            var model = _rotatingImageGroupService.GetRotatingImageWidget(id);
            if (model == null)
            {
                SetErrorMessage(T("RotatingImageGroup_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult SortRotatingImages(GroupImageSortingModel model)
        {
            return Json(_rotatingImageGroupService.SortImages(model));
        }

        #endregion

        [HttpPost]
        public JsonResult DeleteRotatingImageGroup(int id)
        {
            return Json(_rotatingImageGroupService.DeleteRotatingImageGroup(id));
        }
    }
}
