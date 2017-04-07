using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.Testimonials;
using EzCMS.Core.Models.Testimonials.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Testimonials
{
    public class TestimonialService : ServiceHelper, ITestimonialService
    {
        private readonly IRepository<Testimonial> _testimonialRepository;

        public TestimonialService(IRepository<Testimonial> testimonialRepository)
        {
            _testimonialRepository = testimonialRepository;
        }

        /// <summary>
        /// Get number of random testimonials
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<TestimonialWidget> GetRandom(int count)
        {
            var data = new List<TestimonialWidget>();
            var testimonials = GetAll().Select(t => new TestimonialWidget
            {
                Author = t.Author,
                Content = t.Content,
                AuthorImageUrl = t.AuthorImageUrl,
                AuthorDescription = t.AuthorDescription
            }).ToList();

            for (var i = 0; i < count; i++)
            {
                if (testimonials.Count == 0)
                    break;
                var index = new Random().Next(0, testimonials.Count);
                data.Add(testimonials[index]);
                testimonials.Remove(testimonials[index]);
            }
            return data;
        }

        /// <summary>
        /// Get testimonial detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TestimonialDetailModel GetTestimonialDetailModel(int id)
        {
            var item = GetById(id);
            return item != null ? new TestimonialDetailModel(item) : null;
        }

        #region Base

        public IQueryable<Testimonial> GetAll()
        {
            return _testimonialRepository.GetAll();
        }

        public IQueryable<Testimonial> Fetch(Expression<Func<Testimonial, bool>> expression)
        {
            return _testimonialRepository.Fetch(expression);
        }

        public Testimonial FetchFirst(Expression<Func<Testimonial, bool>> expression)
        {
            return _testimonialRepository.FetchFirst(expression);
        }

        public Testimonial GetById(object id)
        {
            return _testimonialRepository.GetById(id);
        }

        internal ResponseModel Insert(Testimonial testimonial)
        {
            return _testimonialRepository.Insert(testimonial);
        }

        internal ResponseModel Update(Testimonial testimonial)
        {
            return _testimonialRepository.Update(testimonial);
        }

        internal ResponseModel Delete(Testimonial testimonial)
        {
            return _testimonialRepository.Delete(testimonial);
        }

        internal ResponseModel Delete(object id)
        {
            return _testimonialRepository.Delete(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the testimonials
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchTestimonials(JqSearchIn si)
        {
            var data = GetAll();

            var testimonials = Maps(data);

            return si.Search(testimonials);
        }

        /// <summary>
        /// Export testimonials
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var testimonials = Maps(data);

            var exportData = si.Export(testimonials, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="testimonials"></param>
        /// <returns></returns>
        private IQueryable<TestimonialModel> Maps(IQueryable<Testimonial> testimonials)
        {
            return testimonials.Select(u => new TestimonialModel
            {
                Id = u.Id,
                AuthorImageUrl = u.AuthorImageUrl,
                Author = u.Author,
                Content = u.Content,
                AuthorDescription = u.AuthorDescription,
                RecordOrder = u.RecordOrder,
                Created = u.Created,
                CreatedBy = u.CreatedBy,
                LastUpdate = u.LastUpdate,
                LastUpdateBy = u.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get Testimonial manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TestimonialManageModel GetTestimonialManageModel(int? id = null)
        {
            var testimonial = GetById(id);
            if (testimonial != null)
            {
                return new TestimonialManageModel(testimonial);
            }
            return new TestimonialManageModel();
        }

        /// <summary>
        /// Save Testimonial
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveTestimonial(TestimonialManageModel model)
        {
            ResponseModel response;
            var testimonial = GetById(model.Id);
            if (testimonial != null)
            {
                testimonial.Author = model.Author;
                testimonial.Content = model.Content;
                testimonial.AuthorDescription = model.AuthorDescription;
                testimonial.AuthorImageUrl = model.AuthorImageUrl;

                response = Update(testimonial);
                return response.SetMessage(response.Success
                    ? T("Testimonial_Message_UpdateSuccessfully")
                    : T("Testimonial_Message_UpdateFailure"));
            }
            Mapper.CreateMap<TestimonialManageModel, Testimonial>();
            testimonial = Mapper.Map<TestimonialManageModel, Testimonial>(model);
            response = Insert(testimonial);
            return response.SetMessage(response.Success
                ? T("Testimonial_Message_CreateSuccessfully")
                : T("Testimonial_Message_CreateFailure"));
        }

        /// <summary>
        /// Update Testimonial data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateTestimonialData(XEditableModel model)
        {
            var testimonial = GetById(model.Pk);
            if (testimonial != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (TestimonialManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new TestimonialManageModel(testimonial);
                    manageModel.SetProperty(model.Name, value);

                    var validationResults = manageModel.ValidateModel();

                    if (validationResults.Any())
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = validationResults.BuildValidationMessages()
                        };
                    }

                    #endregion

                    testimonial.SetProperty(model.Name, value);

                    var response = Update(testimonial);
                    return response.SetMessage(response.Success
                        ? T("Testimonial_Message_UpdateTestimonialInfoSuccessfully")
                        : T("Testimonial_Message_UpdateTestimonialInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Testimonial_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("Testimonial_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete testimonial
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteTestimonial(int id)
        {
            var tag = GetById(id);
            if (tag != null)
            {
                var response = Delete(tag);
                return response.SetMessage(response.Success
                    ? T("Testimonial_Message_DeleteSuccessfully")
                    : T("Testimonial_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("Testimonial_Message_DeleteSuccessfully")
            };
        }

        #endregion
    }
}