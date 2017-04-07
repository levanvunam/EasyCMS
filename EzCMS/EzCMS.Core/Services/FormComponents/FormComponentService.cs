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
using EzCMS.Core.Models.FormComponents;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.FormComponents
{
    public class FormComponentService : ServiceHelper, IFormComponentService
    {
        private readonly IRepository<FormComponent> _formComponentRepository;

        public FormComponentService(IRepository<FormComponent> formComponentRepository)
        {
            _formComponentRepository = formComponentRepository;
        }

        /// <summary>
        /// Get form component select list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetFormComponents(int? id = null)
        {
            return GetAll().Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = SqlFunctions.StringConvert((double) t.Id).Trim(),
                Selected = id.HasValue && id == t.Id
            });
        }

        #region Base

        public IQueryable<FormComponent> GetAll()
        {
            return _formComponentRepository.GetAll();
        }

        public IQueryable<FormComponent> Fetch(Expression<Func<FormComponent, bool>> expression)
        {
            return _formComponentRepository.Fetch(expression);
        }

        public FormComponent FetchFirst(Expression<Func<FormComponent, bool>> expression)
        {
            return _formComponentRepository.FetchFirst(expression);
        }

        public FormComponent GetById(object id)
        {
            return _formComponentRepository.GetById(id);
        }

        internal ResponseModel Insert(FormComponent formComponent)
        {
            return _formComponentRepository.Insert(formComponent);
        }

        internal ResponseModel Update(FormComponent formComponent)
        {
            return _formComponentRepository.Update(formComponent);
        }

        internal ResponseModel Delete(FormComponent formComponent)
        {
            return _formComponentRepository.Delete(formComponent);
        }

        internal ResponseModel Delete(object id)
        {
            return _formComponentRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _formComponentRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the form components
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchFormComponents(JqSearchIn si, FormComponentSearchModel model)
        {
            var data = SearchFormComponents(model);

            var formComponents = Maps(data);

            return si.Search(formComponents);
        }

        /// <summary>
        /// Export form components
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, FormComponentSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchFormComponents(model);

            var formComponents = Maps(data);

            var exportData = si.Export(formComponents, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private methods

        /// <summary>
        /// Search form components
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<FormComponent> SearchFormComponents(FormComponentSearchModel model)
        {
            var formComponents = GetAll();

            if (model != null)
            {
                if (model.FormTabId.HasValue)
                {
                    formComponents = formComponents.Where(m => m.FormTabId == model.FormTabId);
                }

                if (model.FormComponentTemplateId.HasValue)
                {
                    formComponents =
                        formComponents.Where(m => m.FormComponentTemplateId == model.FormComponentTemplateId);
                }
            }

            return formComponents;
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="formComponents"></param>
        /// <returns></returns>
        private IQueryable<FormComponentModel> Maps(IQueryable<FormComponent> formComponents)
        {
            return formComponents.Select(m => new FormComponentModel
            {
                Id = m.Id,
                Name = m.Name,
                FormComponentTemplateId = m.FormComponentTemplateId,
                FormComponentTemplateName = m.FormComponentTemplate.Name,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get form component manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FormComponentManageModel GetFormComponentManageModel(int? id = null)
        {
            var formComponent = GetById(id);
            if (formComponent != null)
            {
                return new FormComponentManageModel(formComponent);
            }
            return new FormComponentManageModel();
        }

        /// <summary>
        /// Save form component
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveFormComponent(FormComponentManageModel model)
        {
            ResponseModel response;
            var formComponent = GetById(model.Id);
            if (formComponent != null)
            {
                formComponent.Name = model.Name;
                formComponent.RecordOrder = model.RecordOrder;
                response = Update(formComponent);
                return response.SetMessage(response.Success
                    ? T("FormComponent_Message_UpdateSuccessfully")
                    : T("FormComponent_Message_UpdateFailure"));
            }
            Mapper.CreateMap<FormComponentManageModel, FormComponent>();
            formComponent = Mapper.Map<FormComponentManageModel, FormComponent>(model);
            response = Insert(formComponent);
            return response.SetMessage(response.Success
                ? T("FormComponent_Message_CreateSuccessfully")
                : T("FormComponent_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete form component
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteFormComponent(int id)
        {
            // Stop deletion if form component field found
            var item = GetById(id);
            if (item != null)
            {
                if (item.FormComponentFields.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("FormComponent_Message_DeleteFailureBasedOnRelatedFormComponentFields")
                    };
                }

                // Delete form component
                var response = Delete(id);

                return
                    response.SetMessage(response.Success
                        ? T("FormComponent_Message_DeleteSuccessfully")
                        : T("FormComponent_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("FormComponent_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get form component detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FormComponentDetailModel GetFormComponentDetailModel(int? id = null)
        {
            var formComponent = GetById(id);
            return formComponent != null ? new FormComponentDetailModel(formComponent) : null;
        }

        /// <summary>
        /// Update form component data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateFormComponentData(XEditableModel model)
        {
            var formComponent = GetById(model.Pk);
            if (formComponent != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (FormComponentManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new FormComponentManageModel(formComponent);
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

                    formComponent.SetProperty(model.Name, value);

                    var response = Update(formComponent);
                    return response.SetMessage(response.Success
                        ? T("FormComponent_Message_UpdateFormComponentInfoSuccessfully")
                        : T("FormComponent_Message_UpdateFormComponentInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("FormComponent_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("FormComponent_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}