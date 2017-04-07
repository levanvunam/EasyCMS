using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Testimonials;
using EzCMS.Core.Models.Testimonials.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Testimonials
{
    [Register(Lifetime.PerInstance)]
    public interface ITestimonialService : IBaseService<Testimonial>
    {
        List<TestimonialWidget> GetRandom(int count);

        TestimonialDetailModel GetTestimonialDetailModel(int id);

        ResponseModel UpdateTestimonialData(XEditableModel model);

        #region Grid Search

        /// <summary>
        /// Search the testimonials
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchTestimonials(JqSearchIn si);

        /// <summary>
        /// Export the testimonials
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        TestimonialManageModel GetTestimonialManageModel(int? id = null);

        ResponseModel SaveTestimonial(TestimonialManageModel model);

        ResponseModel DeleteTestimonial(int id);

        #endregion
    }
}