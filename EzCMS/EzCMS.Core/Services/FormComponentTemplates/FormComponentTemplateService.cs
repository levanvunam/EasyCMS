using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
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
using EzCMS.Core.Models.FormComponentTemplates;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.FormComponentTemplates
{
    public class FormComponentTemplateService : ServiceHelper, IFormComponentTemplateService
    {
        private readonly IRepository<FormComponentTemplate> _formComponentTemplateRepository;

        public FormComponentTemplateService(IRepository<FormComponentTemplate> formComponentTemplateRepository)
        {
            _formComponentTemplateRepository = formComponentTemplateRepository;
        }

        /// <summary>
        /// Get form component template select list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetFormComponentTemplates(int? id = null)
        {
            return GetAll().Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = SqlFunctions.StringConvert((double) t.Id).Trim(),
                Selected = id.HasValue && id == t.Id
            });
        }

        #region Base

        public IQueryable<FormComponentTemplate> GetAll()
        {
            return _formComponentTemplateRepository.GetAll();
        }

        public IQueryable<FormComponentTemplate> Fetch(Expression<Func<FormComponentTemplate, bool>> expression)
        {
            return _formComponentTemplateRepository.Fetch(expression);
        }

        public FormComponentTemplate FetchFirst(Expression<Func<FormComponentTemplate, bool>> expression)
        {
            return _formComponentTemplateRepository.FetchFirst(expression);
        }

        public FormComponentTemplate GetById(object id)
        {
            return _formComponentTemplateRepository.GetById(id);
        }

        internal ResponseModel Insert(FormComponentTemplate formComponentTemplate)
        {
            return _formComponentTemplateRepository.Insert(formComponentTemplate);
        }

        internal ResponseModel Update(FormComponentTemplate formComponentTemplate)
        {
            return _formComponentTemplateRepository.Update(formComponentTemplate);
        }

        internal ResponseModel Delete(FormComponentTemplate formComponentTemplate)
        {
            return _formComponentTemplateRepository.Delete(formComponentTemplate);
        }

        internal ResponseModel Delete(object id)
        {
            return _formComponentTemplateRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _formComponentTemplateRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the form component templates
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchFormComponentTemplates(JqSearchIn si)
        {
            var data = GetAll();

            var formComponentTemplates = Maps(data);

            return si.Search(formComponentTemplates);
        }

        /// <summary>
        /// Export form component templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var formComponentTemplates = Maps(data);

            var exportData = si.Export(formComponentTemplates, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="formComponentTemplates"></param>
        /// <returns></returns>
        private IQueryable<FormComponentTemplateModel> Maps(IQueryable<FormComponentTemplate> formComponentTemplates)
        {
            return formComponentTemplates.Select(m => new FormComponentTemplateModel
            {
                Id = m.Id,
                Name = m.Name,
                Content = m.Content,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get form component template manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FormComponentTemplateManageModel GetFormComponentTemplateManageModel(int? id = null)
        {
            var formComponentTemplate = GetById(id);
            if (formComponentTemplate != null)
            {
                return new FormComponentTemplateManageModel(formComponentTemplate);
            }
            return new FormComponentTemplateManageModel();
        }

        /// <summary>
        /// Get form component template detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FormComponentTemplateDetailModel GetFormComponentTemplateDetailModel(int? id = null)
        {
            var formComponentTemplate = GetById(id);
            return formComponentTemplate != null ? new FormComponentTemplateDetailModel(formComponentTemplate) : null;
        }

        /// <summary>
        /// Save form component template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveFormComponentTemplate(FormComponentTemplateManageModel model)
        {
            ResponseModel response;
            var formComponentTemplate = GetById(model.Id);
            if (formComponentTemplate != null)
            {
                formComponentTemplate.Name = model.Name;
                formComponentTemplate.Content = model.Content;
                response = Update(formComponentTemplate);
                return response.SetMessage(response.Success
                    ? T("FormComponentTemplate_Message_UpdateSuccessfully")
                    : T("FormComponentTemplate_Message_UpdateFailure"));
            }
            Mapper.CreateMap<FormComponentTemplateManageModel, FormComponentTemplate>();
            formComponentTemplate = Mapper.Map<FormComponentTemplateManageModel, FormComponentTemplate>(model);
            response = Insert(formComponentTemplate);
            return response.SetMessage(response.Success
                ? T("FormComponentTemplate_Message_CreateSuccessfully")
                : T("FormComponentTemplate_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete form component template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteFormComponentTemplate(int id)
        {
            var item = GetById(id);

            if (item != null)
            {
                // Stop deletion if form component or form default component found
                if (item.FormComponents.Any() || item.FormDefaultComponents.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("FormComponentTemplate_Message_DeleteFailureBasedOnRelatedFormComponents")
                    };
                }

                // Delete form component template
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("FormComponentTemplate_Message_DeleteSuccessfully")
                    : T("FormComponentTemplate_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("FormComponentTemplate_Message_DeleteSuccessfully")
            };
        }

        /// <summary>
        /// Update form component template data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateFormComponentTemplateData(XEditableModel model)
        {
            var formComponentTemplate = GetById(model.Pk);
            if (formComponentTemplate != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (FormComponentTemplateManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new FormComponentTemplateManageModel(formComponentTemplate);
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

                    formComponentTemplate.SetProperty(model.Name, value);

                    var response = Update(formComponentTemplate);
                    return response.SetMessage(response.Success
                        ? T("FormComponentTemplate_Message_UpdateFormComponentTemplateInfoSuccessfully")
                        : T("FormComponentTemplate_Message_UpdateFormComponentTemplateInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("FormComponentTemplate_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("FormComponentTemplate_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}