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
using EzCMS.Core.Models.FormDefaultComponentFields;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.FormDefaultComponentFields
{
    public class FormDefaultComponentFieldService : ServiceHelper, IFormDefaultComponentFieldService
    {
        private readonly IRepository<FormDefaultComponentField> _formDefaultComponentFieldRepository;

        public FormDefaultComponentFieldService(
            IRepository<FormDefaultComponentField> formDefaultComponentFieldRepository)
        {
            _formDefaultComponentFieldRepository = formDefaultComponentFieldRepository;
        }

        #region Base

        public IQueryable<FormDefaultComponentField> GetAll()
        {
            return _formDefaultComponentFieldRepository.GetAll();
        }

        public IQueryable<FormDefaultComponentField> Fetch(Expression<Func<FormDefaultComponentField, bool>> expression)
        {
            return _formDefaultComponentFieldRepository.Fetch(expression);
        }

        public FormDefaultComponentField FetchFirst(Expression<Func<FormDefaultComponentField, bool>> expression)
        {
            return _formDefaultComponentFieldRepository.FetchFirst(expression);
        }

        public FormDefaultComponentField GetById(object id)
        {
            return _formDefaultComponentFieldRepository.GetById(id);
        }

        internal ResponseModel Insert(FormDefaultComponentField formDefaultComponentField)
        {
            return _formDefaultComponentFieldRepository.Insert(formDefaultComponentField);
        }

        internal ResponseModel Update(FormDefaultComponentField formDefaultComponentField)
        {
            return _formDefaultComponentFieldRepository.Update(formDefaultComponentField);
        }

        internal ResponseModel Delete(FormDefaultComponentField formDefaultComponentField)
        {
            return _formDefaultComponentFieldRepository.Delete(formDefaultComponentField);
        }

        internal ResponseModel Delete(object id)
        {
            return _formDefaultComponentFieldRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _formDefaultComponentFieldRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the form default component fields
        /// </summary>
        /// <param name="si"></param>
        /// <param name="formDefaultComponentId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchFormDefaultComponentFields(JqSearchIn si, int? formDefaultComponentId)
        {
            var data = SearchFormDefaultComponentFields(formDefaultComponentId);

            var formDefaultComponentFields = Maps(data);

            return si.Search(formDefaultComponentFields);
        }

        /// <summary>
        /// Export form default component fields
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="formDefaultComponentId"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? formDefaultComponentId)
        {
            var data = gridExportMode == GridExportMode.All
                ? GetAll()
                : SearchFormDefaultComponentFields(formDefaultComponentId);

            var formDefaultComponentFields = Maps(data);

            var exportData = si.Export(formDefaultComponentFields, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search the form default component fields
        /// </summary>
        /// <param name="formDefaultComponentId"></param>
        /// <returns></returns>
        private IQueryable<FormDefaultComponentField> SearchFormDefaultComponentFields(int? formDefaultComponentId)
        {
            return
                Fetch(
                    formDefaultComponentField =>
                        !formDefaultComponentId.HasValue ||
                        formDefaultComponentField.FormDefaultComponentId == formDefaultComponentId);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="formDefaultComponentFields"></param>
        /// <returns></returns>
        private IQueryable<FormDefaultComponentFieldModel> Maps(
            IQueryable<FormDefaultComponentField> formDefaultComponentFields)
        {
            return formDefaultComponentFields.Select(formDefaultComponentField => new FormDefaultComponentFieldModel
            {
                Id = formDefaultComponentField.Id,
                Name = formDefaultComponentField.Name,
                FormDefaultComponentId = formDefaultComponentField.FormDefaultComponentId,
                FormDefaultComponentName = formDefaultComponentField.FormDefaultComponent.Name,
                Attributes = formDefaultComponentField.Attributes,
                RecordOrder = formDefaultComponentField.RecordOrder,
                Created = formDefaultComponentField.Created,
                CreatedBy = formDefaultComponentField.CreatedBy,
                LastUpdate = formDefaultComponentField.LastUpdate,
                LastUpdateBy = formDefaultComponentField.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get form default component field manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FormDefaultComponentFieldManageModel GetFormDefaultComponentFieldManageModel(int? id = null)
        {
            var formDefaultComponentField = GetById(id);
            if (formDefaultComponentField != null)
            {
                return new FormDefaultComponentFieldManageModel(formDefaultComponentField);
            }
            return new FormDefaultComponentFieldManageModel();
        }

        /// <summary>
        /// Save form default component field
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveFormDefaultComponentField(FormDefaultComponentFieldManageModel model)
        {
            ResponseModel response;
            var formDefaultComponentField = GetById(model.Id);

            if (formDefaultComponentField != null)
            {
                formDefaultComponentField.FormDefaultComponentId = model.FormDefaultComponentId;
                formDefaultComponentField.Name = model.Name;
                formDefaultComponentField.Attributes = SerializeUtilities.Serialize(model.Attributes);
                formDefaultComponentField.RecordOrder = model.RecordOrder;
                response = Update(formDefaultComponentField);
                return response.SetMessage(response.Success
                    ? T("FormDefaultComponentField_Message_UpdateSuccessfully")
                    : T("FormDefaultComponentField_Message_UpdateFailure"));
            }

            formDefaultComponentField = new FormDefaultComponentField
            {
                FormDefaultComponentId = model.FormDefaultComponentId,
                Name = model.Name,
                Attributes = SerializeUtilities.Serialize(model.Attributes),
                RecordOrder = model.RecordOrder
            };
            response = Insert(formDefaultComponentField);
            return response.SetMessage(response.Success
                ? T("FormDefaultComponentField_Message_CreateSuccessfully")
                : T("FormDefaultComponentField_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete form default component field
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteFormDefaultComponentField(int id)
        {
            var item = GetById(id);

            if (item != null)
            {
                var response = Delete(id);
                return response.SetMessage(response.Success
                    ? T("FormDefaultComponentField_Message_DeleteSuccessfully")
                    : T("FormDefaultComponentField_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("FormDefaultComponentField_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get form default component field detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FormDefaultComponentFieldDetailModel GetFormDefaultComponentFieldDetailModel(int? id = null)
        {
            var formDefaultComponentField = GetById(id);
            return formDefaultComponentField != null
                ? new FormDefaultComponentFieldDetailModel(formDefaultComponentField)
                : null;
        }

        /// <summary>
        /// Update form default component field data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateFormDefaultComponentFieldData(XEditableModel model)
        {
            var formDefaultComponentField = GetById(model.Pk);
            if (formDefaultComponentField != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (FormDefaultComponentFieldManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new FormDefaultComponentFieldManageModel(formDefaultComponentField);
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

                    formDefaultComponentField.SetProperty(model.Name, value);

                    var response = Update(formDefaultComponentField);
                    return response.SetMessage(response.Success
                        ? T("FormDefaultComponentField_Message_UpdateFormDefaultComponentFieldInfoSuccessfully")
                        : T("FormDefaultComponentField_Message_UpdateFormDefaultComponentFieldInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("FormDefaultComponentField_Message_PropertyNotFound")
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("FormDefaultComponentField_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}