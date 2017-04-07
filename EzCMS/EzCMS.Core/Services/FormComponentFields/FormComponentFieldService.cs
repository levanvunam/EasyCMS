using System;
using System.Linq;
using System.Linq.Expressions;
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
using EzCMS.Core.Models.FormComponentFields;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.FormComponentFields
{
    public class FormComponentFieldService : ServiceHelper, IFormComponentFieldService
    {
        private readonly IRepository<FormComponentField> _formComponentFieldRepository;

        public FormComponentFieldService(IRepository<FormComponentField> formComponentFieldRepository)
        {
            _formComponentFieldRepository = formComponentFieldRepository;
        }

        #region Base

        public IQueryable<FormComponentField> GetAll()
        {
            return _formComponentFieldRepository.GetAll();
        }

        public IQueryable<FormComponentField> Fetch(Expression<Func<FormComponentField, bool>> expression)
        {
            return _formComponentFieldRepository.Fetch(expression);
        }

        public FormComponentField FetchFirst(Expression<Func<FormComponentField, bool>> expression)
        {
            return _formComponentFieldRepository.FetchFirst(expression);
        }

        public FormComponentField GetById(object id)
        {
            return _formComponentFieldRepository.GetById(id);
        }

        internal ResponseModel Insert(FormComponentField formComponentField)
        {
            return _formComponentFieldRepository.Insert(formComponentField);
        }

        internal ResponseModel Update(FormComponentField formComponentField)
        {
            return _formComponentFieldRepository.Update(formComponentField);
        }

        internal ResponseModel Delete(FormComponentField formComponentField)
        {
            return _formComponentFieldRepository.Delete(formComponentField);
        }

        internal ResponseModel Delete(object id)
        {
            return _formComponentFieldRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _formComponentFieldRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the form component fields
        /// </summary>
        /// <param name="si"></param>
        /// <param name="formComponentId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchFormComponentFields(JqSearchIn si, int? formComponentId)
        {
            var data = SearchFormComponentFields(formComponentId);

            var formComponentFields = Maps(data);

            return si.Search(formComponentFields);
        }

        /// <summary>
        /// Export form component fields
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="formComponentId"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? formComponentId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchFormComponentFields(formComponentId);

            var formComponentFields = Maps(data);

            var exportData = si.Export(formComponentFields, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search the form component fields
        /// </summary>
        /// <param name="formComponentId"></param>
        /// <returns></returns>
        private IQueryable<FormComponentField> SearchFormComponentFields(int? formComponentId)
        {
            return
                Fetch(
                    formComponentField =>
                        !formComponentId.HasValue || formComponentField.FormComponentId == formComponentId);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="formComponentFields"></param>
        /// <returns></returns>
        private IQueryable<FormComponentFieldModel> Maps(IQueryable<FormComponentField> formComponentFields)
        {
            return formComponentFields.Select(formComponentField => new FormComponentFieldModel
            {
                Id = formComponentField.Id,
                Name = formComponentField.Name,
                FormComponentId = formComponentField.FormComponentId,
                FormComponentName = formComponentField.FormComponent.Name,
                Attributes = formComponentField.Attributes,
                RecordOrder = formComponentField.RecordOrder,
                Created = formComponentField.Created,
                CreatedBy = formComponentField.CreatedBy,
                LastUpdate = formComponentField.LastUpdate,
                LastUpdateBy = formComponentField.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get form component field manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FormComponentFieldManageModel GetFormComponentFieldManageModel(int? id = null)
        {
            var formComponentField = GetById(id);
            if (formComponentField != null)
            {
                return new FormComponentFieldManageModel(formComponentField);
            }
            return new FormComponentFieldManageModel();
        }

        /// <summary>
        /// Get form component field detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FormComponentFieldDetailModel GetFormComponentFieldDetailModel(int? id = null)
        {
            var formComponentField = GetById(id);
            return formComponentField != null ? new FormComponentFieldDetailModel(formComponentField) : null;
        }

        /// <summary>
        /// Save form component field
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveFormComponentField(FormComponentFieldManageModel model)
        {
            ResponseModel response;
            var formComponentField = GetById(model.Id);

            if (formComponentField != null)
            {
                formComponentField.FormComponentId = model.FormComponentId;
                formComponentField.Name = model.Name;
                formComponentField.Attributes = SerializeUtilities.Serialize(model.Attributes);
                formComponentField.RecordOrder = model.RecordOrder;
                response = Update(formComponentField);
                return response.SetMessage(response.Success
                    ? T("FormComponentField_Message_UpdateSuccessfully")
                    : T("FormComponentField_Message_UpdateFailure"));
            }

            formComponentField = new FormComponentField
            {
                FormComponentId = model.FormComponentId,
                Name = model.Name,
                Attributes = SerializeUtilities.Serialize(model.Attributes),
                RecordOrder = model.RecordOrder
            };
            response = Insert(formComponentField);
            return response.SetMessage(response.Success
                ? T("FormComponentField_Message_CreateSuccessfully")
                : T("FormComponentField_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete form component field
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteFormComponentField(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                var response = Delete(id);
                return response.SetMessage(response.Success
                    ? T("FormComponentField_Message_DeleteSuccessfully")
                    : T("FormComponentField_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("FormComponentField_Message_DeleteSuccessfully")
            };
        }

        /// <summary>
        /// Update form component field data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateFormComponentFieldData(XEditableModel model)
        {
            var formComponentField = GetById(model.Pk);
            if (formComponentField != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (FormComponentFieldManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new FormComponentFieldManageModel(formComponentField);
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

                    formComponentField.SetProperty(model.Name, value);

                    var response = Update(formComponentField);
                    return response.SetMessage(response.Success
                        ? T("FormComponentField_Message_UpdateFormComponentFieldInfoSuccessfully")
                        : T("FormComponentField_Message_UpdateFormComponentFieldInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("FormComponentField_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("FormComponentField_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}