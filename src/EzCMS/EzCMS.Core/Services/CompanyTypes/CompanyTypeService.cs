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
using EzCMS.Core.Models.CompanyTypes;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.AssociateCompanyTypes;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.CompanyTypes
{
    public class CompanyTypeService : ServiceHelper, ICompanyTypeService
    {
        private readonly IAssociateCompanyTypeRepository _associateCompanyTypeRepository;
        private readonly IRepository<CompanyType> _companyTypeRepository;

        public CompanyTypeService(IRepository<CompanyType> companyTypeRepository,
            IAssociateCompanyTypeRepository associateCompanyTypeRepository)
        {
            _companyTypeRepository = companyTypeRepository;
            _associateCompanyTypeRepository = associateCompanyTypeRepository;
        }

        /// <summary>
        /// Get list company types
        /// </summary>
        /// <param name="companyTypeIds"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetCompanyTypes(List<int> companyTypeIds = null)
        {
            if (companyTypeIds == null) companyTypeIds = new List<int>();
            return GetAll().Select(g => new SelectListItem
            {
                Text = g.Name,
                Value = SqlFunctions.StringConvert((double) g.Id).Trim(),
                Selected = companyTypeIds.Any(id => id == g.Id)
            });
        }

        /// <summary>
        /// Get company type name
        /// </summary>
        /// <param name="companyTypeId"></param>
        /// <returns></returns>
        public string GetCompanyTypeName(int companyTypeId)
        {
            var companyType = FetchFirst(ct => ct.Id == companyTypeId);
            return companyType != null ? companyType.Name : string.Empty;
        }

        /// <summary>
        /// Get company type names by company type ids
        /// </summary>
        /// <param name="companyTypeIds"></param>
        /// <returns></returns>
        public IQueryable<string> GetCompanyTypeNames(List<int> companyTypeIds = null)
        {
            if (companyTypeIds == null) companyTypeIds = new List<int>();

            return Fetch(companyType => companyTypeIds.Contains(companyType.Id)).Select(c => c.Name);
        }

        #region Validation

        /// <summary>
        /// Check if company type exists.
        /// </summary>
        /// <param name="id">the company id</param>
        /// <param name="name">the company name</param>
        /// <returns></returns>
        public bool IsNameExisted(int? id, string name)
        {
            return Fetch(u => u.Name.Equals(name) && u.Id != id).Any();
        }

        #endregion

        #region Base

        public IQueryable<CompanyType> GetAll()
        {
            return _companyTypeRepository.GetAll();
        }

        public IQueryable<CompanyType> Fetch(Expression<Func<CompanyType, bool>> expression)
        {
            return _companyTypeRepository.Fetch(expression);
        }

        public CompanyType FetchFirst(Expression<Func<CompanyType, bool>> expression)
        {
            return _companyTypeRepository.FetchFirst(expression);
        }

        public CompanyType GetById(object id)
        {
            return _companyTypeRepository.GetById(id);
        }

        internal ResponseModel Insert(CompanyType companyType)
        {
            return _companyTypeRepository.Insert(companyType);
        }

        internal ResponseModel Update(CompanyType companyType)
        {
            return _companyTypeRepository.Update(companyType);
        }

        internal ResponseModel Delete(CompanyType companyType)
        {
            return _companyTypeRepository.Delete(companyType);
        }

        internal ResponseModel Delete(object id)
        {
            return _companyTypeRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _companyTypeRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the company types
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchCompanyTypes(JqSearchIn si, int? associateId)
        {
            var data = SearchCompanyTypes(associateId);

            var companyTypes = Maps(data);

            return si.Search(companyTypes);
        }

        /// <summary>
        /// Export company types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? associateId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchCompanyTypes(associateId);

            var companyTypes = Maps(data);

            var exportData = si.Export(companyTypes, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search company types
        /// </summary>
        /// <param name="associateId"></param>
        /// <returns></returns>
        private IQueryable<CompanyType> SearchCompanyTypes(int? associateId)
        {
            return
                Fetch(
                    companyType =>
                        !associateId.HasValue ||
                        companyType.AssociateCompanyTypes.Any(act => act.AssociateId == associateId));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="companyTypes"></param>
        /// <returns></returns>
        private IQueryable<CompanyTypeModel> Maps(IQueryable<CompanyType> companyTypes)
        {
            return companyTypes.Select(m => new CompanyTypeModel
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
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
        /// Get CompanyType manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CompanyTypeManageModel GetCompanyTypeManageModel(int? id = null)
        {
            var companyType = GetById(id);
            if (companyType != null)
            {
                return new CompanyTypeManageModel(companyType);
            }
            return new CompanyTypeManageModel();
        }

        /// <summary>
        /// Save CompanyType
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveCompanyType(CompanyTypeManageModel model)
        {
            ResponseModel response;
            var companyType = GetById(model.Id);
            if (companyType != null)
            {
                companyType.Name = model.Name;
                companyType.Description = model.Description;
                companyType.RecordOrder = model.RecordOrder;
                response = Update(companyType);
                return response.SetMessage(response.Success
                    ? T("CompanyType_Message_UpdateSuccessfully")
                    : T("CompanyType_Message_UpdateFailure"));
            }
            Mapper.CreateMap<CompanyTypeManageModel, CompanyType>();
            companyType = Mapper.Map<CompanyTypeManageModel, CompanyType>(model);
            response = Insert(companyType);
            return response.SetMessage(response.Success
                ? T("CompanyType_Message_CreateSuccessfully")
                : T("CompanyType_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete company type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteCompanyType(int id)
        {
            // Stop deletion if relation found
            var item = GetById(id);
            if (item != null)
            {
                if (item.Companies.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("CompanyType_Message_DeleteFailureBasedOnRelatedCompanyData")
                    };
                }

                // Delete the Navigation
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("CompanyType_Message_DeleteSuccessfully")
                    : T("CompanyType_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("CompanyType_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get CompanyType detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CompanyTypeDetailModel GetCompanyTypeDetailModel(int id)
        {
            var companyType = GetById(id);
            return companyType != null ? new CompanyTypeDetailModel(companyType) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateCompanyTypeData(XEditableModel model)
        {
            var companyType = GetById(model.Pk);
            if (companyType != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (CompanyTypeManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new CompanyTypeManageModel(companyType);
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

                    companyType.SetProperty(model.Name, value);
                    var response = Update(companyType);
                    return response.SetMessage(response.Success
                        ? T("CompanyType_Message_UpdateCompanyInfoSuccessfully")
                        : T("CompanyType_Message_UpdateCompanyInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("CompanyType_Message_PropertyNotFound")
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("CompanyType_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete associate - company type mapping
        /// </summary>
        /// <param name="companyTypeId"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        public ResponseModel DeleteAssociateCompanyTypeMapping(int companyTypeId, int associateId)
        {
            var response = _associateCompanyTypeRepository.Delete(companyTypeId, associateId);

            return response.SetMessage(response.Success
                ? T("AssociateCompanyType_Message_DeleteMappingSuccessfully")
                : T("AssociateCompanyType_Message_DeleteMappingFailure"));
        }

        #endregion
    }
}