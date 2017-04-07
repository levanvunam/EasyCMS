using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Models.RotatingImages;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.RotatingImages;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class RotatingImagesController : BackendController
    {
        private readonly IRotatingImageService _rotatingImageService;
        public RotatingImagesController(IRotatingImageService rotatingImageService)
        {
            _rotatingImageService = rotatingImageService;
        }

        /// <summary>
        /// Delete the image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteImage(int id)
        {
            return Json(_rotatingImageService.Delete(id));
        }

        /// <summary>
        /// Update url of rotating image
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateUrl(int id, string url)
        {
            return Json(_rotatingImageService.UpdateRotatingImageUrl(id, url));
        }

        #region Popup Create

        public ActionResult PopupCreate(int? rotatingImageGroupId)
        {
            SetupPopupAction();
            var model = _rotatingImageService.GetRotatingImageManageModelForGroup(rotatingImageGroupId);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult PopupCreate(RotatingImageManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _rotatingImageService.SaveRotatingImage(model);
                switch (submit)
                {
                    case SubmitType.SaveAndContinueEdit:
                        SetResponseMessage(response);
                        response.Data = Url.Action("PopupEdit", new { id = response.Data });
                        break;
                    default:
                        response.Data = string.Empty;
                        break;
                }
                return Json(response);
            }
            SetupPopupAction();
            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #endregion

        #region Popup Edit


        public ActionResult PopupEdit(int id)
        {
            SetupPopupAction();
            var model = _rotatingImageService.GetRotatingImageManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("RotatingImage_Message_ObjectNotFound"));
                return View("CloseFancyBox",
                    new CloseFancyBoxViewModel
                    {
                        IsReload = true
                    });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult PopupEdit(RotatingImageManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _rotatingImageService.SaveRotatingImage(model);
                switch (submit)
                {
                    case SubmitType.SaveAndContinueEdit:
                        SetResponseMessage(response);
                        response.Data = Url.Action("PopupEdit", new { id = model.Id });
                        break;
                    default:
                        response.Data = string.Empty;
                        break;
                }
                return Json(response);
            }
            SetupPopupAction();
            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #endregion
    }
}
