using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Testimonials;
using EzCMS.Core.Services.Testimonials;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class TestimonialsController : BackendController
    {
        private readonly ITestimonialService _testimonialService;
        public TestimonialsController(ITestimonialService testimonialService)
        {
            _testimonialService = testimonialService;
        }

        [AdministratorNavigation("Testimonials", "Module_Holder", "Testimonials", "fa-comment-o", 170)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_testimonialService.SearchTestimonials(si));
        }

        /// <summary>
        /// Export testimonials
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _testimonialService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Testimonials.xls");
        }

        #region Create

        [AdministratorNavigation("Testimonial_Create", "Testimonials", "Testimonial_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _testimonialService.GetTestimonialManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(TestimonialManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _testimonialService.SaveTestimonial(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = (int)response.Data });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("Testimonial_Edit", "Testimonials", "Testimonial_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _testimonialService.GetTestimonialManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Testimonial_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(TestimonialManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _testimonialService.SaveTestimonial(model);
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

        [AdministratorNavigation("Testimonial_Details", "Testimonials", "Testimonial_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _testimonialService.GetTestimonialDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Testimonial_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateTestimonialData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_testimonialService.UpdateTestimonialData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteTestimonial(int id)
        {
            return Json(_testimonialService.DeleteTestimonial(id));
        }

        #endregion

        #endregion
    }
}
