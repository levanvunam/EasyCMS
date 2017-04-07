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
using EzCMS.Core.Models.AssociateTypes;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.AssociateAssociateTypes;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.AssociateTypes
{
    public class AssociateTypeService : ServiceHelper, IAssociateTypeService
    {
        private readonly IAssociateAssociateTypeRepository _associateAssociateTypeRepository;
        private readonly IRepository<AssociateType> _associateTypeRepository;

        public AssociateTypeService(IRepository<AssociateType> associateTypeRepository,
            IAssociateAssociateTypeRepository associateAssociateTypeRepository)
        {
            _associateTypeRepository = associateTypeRepository;
            _associateAssociateTypeRepository = associateAssociateTypeRepository;
        }

        #region Validation

        /// <summary>
        /// Check if associate type exists
        /// </summary>
        /// <param name="associateTypeId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsAssociateTypeExisted(int? associateTypeId, string name)
        {
            return Fetch(u => u.Name.Equals(name) && u.Id != associateTypeId).Any();
        }

        #endregion

        /// <summary>
        /// Delete associate - associate type mapping
        /// </summary>
        /// <param name="associateTypeId"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        public ResponseModel DeleteAssociateAssociateTypeMapping(int associateTypeId, int associateId)
        {
            var response = _associateAssociateTypeRepository.Delete(associateTypeId, associateId);

            return response.SetMessage(response.Success
                ? T("AssociateAssociateType_Message_DeleteMappingSuccessfully")
                : T("AssociateAssociateType_Message_DeleteMappingFailure"));
        }

        /// <summary>
        /// Get list of associate type
        /// </summary>
        /// <param name="associateTypeIds"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetAssociateTypes(List<int> associateTypeIds = null)
        {
            if (associateTypeIds == null) associateTypeIds = new List<int>();
            return GetAll().Select(associateType => new SelectListItem
            {
                Text = associateType.Name,
                Value = SqlFunctions.StringConvert((double) associateType.Id).Trim(),
                Selected = associateTypeIds.Any(id => id == associateType.Id)
            });
        }

        #region Base

        public IQueryable<AssociateType> GetAll()
        {
            return _associateTypeRepository.GetAll();
        }

        public IQueryable<AssociateType> Fetch(Expression<Func<AssociateType, bool>> expression)
        {
            return _associateTypeRepository.Fetch(expression);
        }

        public AssociateType FetchFirst(Expression<Func<AssociateType, bool>> expression)
        {
            return _associateTypeRepository.FetchFirst(expression);
        }

        public AssociateType GetById(object id)
        {
            return _associateTypeRepository.GetById(id);
        }

        internal ResponseModel Insert(AssociateType associateType)
        {
            return _associateTypeRepository.Insert(associateType);
        }

        internal ResponseModel Update(AssociateType associateType)
        {
            return _associateTypeRepository.Update(associateType);
        }

        internal ResponseModel Delete(AssociateType associateType)
        {
            return _associateTypeRepository.Delete(associateType);
        }

        internal ResponseModel Delete(object id)
        {
            return _associateTypeRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _associateTypeRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the associate types.
        /// </summary>
        /// <param name="si"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchAssociateTypes(JqSearchIn si, int? associateId)
        {
            var data = SearchAssociateTypes(associateId);

            var associateType = Maps(data);

            return si.Search(associateType);
        }

        /// <summary>
        /// Export associate types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? associateId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchAssociateTypes(associateId);

            var associateTypes = Maps(data);

            var exportData = si.Export(associateTypes, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search associate types
        /// </summary>
        /// <param name="associateId"></param>
        /// <returns></returns>
        private IQueryable<AssociateType> SearchAssociateTypes(int? associateId)
        {
            return
                Fetch(
                    associateType =>
                        !associateId.HasValue ||
                        associateType.AssociateAssociateTypes.Any(act => act.AssociateId == associateId));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="associateType"></param>
        /// <returns></returns>
        private IQueryable<AssociateTypeModel> Maps(IQueryable<AssociateType> associateType)
        {
            return associateType.Select(m => new AssociateTypeModel
            {
                Id = m.Id,
                Name = m.Name,
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
        /// Get associate type manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AssociateTypeManageModel GetAssociateTypeManageModel(int? id = null)
        {
            var associateType = GetById(id);
            if (associateType != null)
            {
                return new AssociateTypeManageModel(associateType);
            }

            return new AssociateTypeManageModel();
        }

        /// <summary>
        /// Save associate type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveAssociateType(AssociateTypeManageModel model)
        {
            ResponseModel response;
            var associateType = GetById(model.Id);
            if (associateType != null)
            {
                associateType.Name = model.Name;
                associateType.RecordOrder = model.RecordOrder;
                response = Update(associateType);

                return response.SetMessage(response.Success
                    ? T("AssociateType_Message_UpdateSuccessfully")
                    : T("AssociateType_Message_UpdateFailure"));
            }
            Mapper.CreateMap<AssociateTypeManageModel, AssociateType>();
            associateType = Mapper.Map<AssociateTypeManageModel, AssociateType>(model);

            response = Insert(associateType);

            return response.SetMessage(response.Success
                ? T("AssociateType_Message_CreateSuccessfully")
                : T("AssociateType_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete associate type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteAssociateType(int id)
        {
            var associateType = GetById(id);
            if (associateType != null)
            {
                // Stop deletion if relation found
                if (associateType.AssociateAssociateTypes.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("AssociateType_Message_DeleteFailureBasedOnRelatedAssociateData")
                    };
                }

                // Delete the associate type
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("AssociateType_Message_DeleteSuccessfully")
                    : T("AssociateType_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("AssociateType_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get associate type detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AssociateTypeDetailModel GetAssociateTypeDetailModel(int id)
        {
            var associateType = GetById(id);

            return associateType != null ? new AssociateTypeDetailModel(associateType) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateAssociateTypeData(XEditableModel model)
        {
            var associateType = GetById(model.Pk);
            if (associateType != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (AssociateTypeManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    //Generate property value
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new AssociateTypeManageModel(associateType);
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

                    associateType.SetProperty(model.Name, value);
                    var response = Update(associateType);

                    return response.SetMessage(response.Success
                        ? T("AssociateType_Message_UpdateAssociateTypeInfoSuccessfully")
                        : T("AssociateType_Message_UpdateAssociateTypeInfoFailure"));
                }

                return new ResponseModel
                {
                    Success = false,
                    Message = T("AssociateType_Message_PropertyNotFound")
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("AssociateType_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}