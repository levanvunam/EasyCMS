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
using EzCMS.Core.Models.FormDefaultComponents;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.FormDefaultComponents
{
    public class FormDefaultComponentService : ServiceHelper, IFormDefaultComponentService
    {
        private readonly IRepository<FormDefaultComponent> _formDefaultComponentRepository;

        public FormDefaultComponentService(IRepository<FormDefaultComponent> formDefaultComponentRepository)
        {
            _formDefaultComponentRepository = formDefaultComponentRepository;
        }

        /// <summary>
        /// Get form default component select list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetFormDefaultComponents(int? id = null)
        {
            return GetAll().Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = SqlFunctions.StringConvert((double) t.Id).Trim(),
                Selected = id.HasValue && id == t.Id
            });
        }

        #region Base

        public IQueryable<FormDefaultComponent> GetAll()
        {
            return _formDefaultComponentRepository.GetAll();
        }

        public IQueryable<FormDefaultComponent> Fetch(Expression<Func<FormDefaultComponent, bool>> expression)
        {
            return _formDefaultComponentRepository.Fetch(expression);
        }

        public FormDefaultComponent FetchFirst(Expression<Func<FormDefaultComponent, bool>> expression)
        {
            return _formDefaultComponentRepository.FetchFirst(expression);
        }

        public FormDefaultComponent GetById(object id)
        {
            return _formDefaultComponentRepository.GetById(id);
        }

        internal ResponseModel Insert(FormDefaultComponent formDefaultComponent)
        {
            return _formDefaultComponentRepository.Insert(formDefaultComponent);
        }

        internal ResponseModel Update(FormDefaultComponent formDefaultComponent)
        {
            return _formDefaultComponentRepository.Update(formDefaultComponent);
        }

        internal ResponseModel Delete(FormDefaultComponent formDefaultComponent)
        {
            return _formDefaultComponentRepository.Delete(formDefaultComponent);
        }

        internal ResponseModel Delete(object id)
        {
            return _formDefaultComponentRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _formDefaultComponentRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the form default components
        /// </summary>
        /// <param name="si"></param>
        /// <param name="formComponentTemplateId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchFormDefaultComponents(JqSearchIn si, int? formComponentTemplateId)
        {
            var data = SearchFormDefaultComponents(formComponentTemplateId);

            var formDefaultComponents = Maps(data);

            return si.Search(formDefaultComponents);
        }

        /// <summary>
        /// Export form default components
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="formComponentTemplateId"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? formComponentTemplateId)
        {
            var data = gridExportMode == GridExportMode.All
                ? GetAll()
                : SearchFormDefaultComponents(formComponentTemplateId);

            var formDefaultComponents = Maps(data);

            var exportData = si.Export(formDefaultComponents, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search the form default components
        /// </summary>
        /// <param name="formComponentTemplateId"></param>
        /// <returns></returns>
        private IQueryable<FormDefaultComponent> SearchFormDefaultComponents(int? formComponentTemplateId)
        {
            return
                Fetch(
                    formDefaultComponent =>
                        !formComponentTemplateId.HasValue ||
                        formDefaultComponent.FormComponentTemplateId == formComponentTemplateId);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="formDefaultComponents"></param>
        /// <returns></returns>
        private IQueryable<FormDefaultComponentModel> Maps(IQueryable<FormDefaultComponent> formDefaultComponents)
        {
            return formDefaultComponents.Select(formDefaultComponent => new FormDefaultComponentModel
            {
                Id = formDefaultComponent.Id,
                Name = formDefaultComponent.Name,
                FormComponentTemplateId = formDefaultComponent.FormComponentTemplateId,
                FormComponentTemplateName = formDefaultComponent.FormComponentTemplate.Name,
                RecordOrder = formDefaultComponent.RecordOrder,
                Created = formDefaultComponent.Created,
                CreatedBy = formDefaultComponent.CreatedBy,
                LastUpdate = formDefaultComponent.LastUpdate,
                LastUpdateBy = formDefaultComponent.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get form default component manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FormDefaultComponentManageModel GetFormDefaultComponentManageModel(int? id = null)
        {
            var formDefaultComponent = GetById(id);
            if (formDefaultComponent != null)
            {
                return new FormDefaultComponentManageModel(formDefaultComponent);
            }
            return new FormDefaultComponentManageModel();
        }

        /// <summary>
        /// Save form default component
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveFormDefaultComponent(FormDefaultComponentManageModel model)
        {
            ResponseModel response;
            var formDefaultComponent = GetById(model.Id);
            if (formDefaultComponent != null)
            {
                formDefaultComponent.Name = model.Name;
                formDefaultComponent.RecordOrder = model.RecordOrder;
                response = Update(formDefaultComponent);
                return response.SetMessage(response.Success
                    ? T("FormDefaultComponent_Message_UpdateSuccessfully")
                    : T("FormDefaultComponent_Message_UpdateFailure"));
            }
            Mapper.CreateMap<FormDefaultComponentManageModel, FormDefaultComponent>();
            formDefaultComponent = Mapper.Map<FormDefaultComponentManageModel, FormDefaultComponent>(model);
            response = Insert(formDefaultComponent);
            return response.SetMessage(response.Success
                ? T("FormDefaultComponent_Message_CreateSuccessfully")
                : T("FormDefaultComponent_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete form default component
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteFormDefaultComponent(int id)
        {
            // Stop deletion if form default component field found
            var item = GetById(id);

            if (item != null)
            {
                if (item.FormDefaultComponentFields.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message =
                            T("FormDefaultComponent_Message_DeleteFailureBasedOnRelatedFormDefaultComponentFields")
                    };
                }

                // Delete form default component
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("FormDefaultComponent_Message_DeleteSuccessfully")
                    : T("FormDefaultComponent_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("FormDefaultComponent_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get form default component detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FormDefaultComponentDetailModel GetFormDefaultComponentDetailModel(int? id = null)
        {
            var formDefaultComponent = GetById(id);
            return formDefaultComponent != null ? new FormDefaultComponentDetailModel(formDefaultComponent) : null;
        }

        /// <summary>
        /// Update form default component data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateFormDefaultComponentData(XEditableModel model)
        {
            var formDefaultComponent = GetById(model.Pk);
            if (formDefaultComponent != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (FormDefaultComponentManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new FormDefaultComponentManageModel(formDefaultComponent);
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

                    formDefaultComponent.SetProperty(model.Name, value);

                    var response = Update(formDefaultComponent);
                    return response.SetMessage(response.Success
                        ? T("FormDefaultComponent_Message_UpdateFormDefaultComponentInfoSuccessfully")
                        : T("FormDefaultComponent_Message_UpdateFormDefaultComponentInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("FormDefaultComponent_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("FormDefaultComponent_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}