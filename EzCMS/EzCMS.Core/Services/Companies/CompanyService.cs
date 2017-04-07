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
using EzCMS.Core.Models.Companies;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Companies
{
    public class CompanyService : ServiceHelper, ICompanyService
    {
        private readonly IRepository<Company> _companyRepository;

        public CompanyService(IRepository<Company> companyRepository)
        {
            _companyRepository = companyRepository;
        }

        #region Validation

        /// <summary>
        /// Check if company exists.
        /// </summary>
        /// <param name="id">the company id</param>
        /// <param name="name">the company name</param>
        /// <returns></returns>
        public bool IsNameExisted(int? id, string name)
        {
            return Fetch(u => u.Name.Equals(name) && u.Id != id).Any();
        }

        #endregion

        /// <summary>
        /// Search company by keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> SearchCompaniesByKeyword(string keyword = "")
        {
            return
                Fetch(c => string.IsNullOrEmpty(keyword) || c.Name.ToLower().Contains(keyword.ToLower()))
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Name,
                        Selected = !string.IsNullOrEmpty(keyword) && keyword.ToLower().Equals(c.Name.ToLower())
                    });
        }

        /// <summary>
        /// Get company list
        /// </summary>
        /// <param name="companyIds"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetCompanies(List<int> companyIds = null)
        {
            if (companyIds == null) companyIds = new List<int>();
            return GetAll().Select(g => new SelectListItem
            {
                Text = g.Name,
                Value = SqlFunctions.StringConvert((double) g.Id).Trim(),
                Selected = companyIds.Any(id => id == g.Id)
            });
        }

        #region Base

        public IQueryable<Company> GetAll()
        {
            return _companyRepository.GetAll();
        }

        public IQueryable<Company> Fetch(Expression<Func<Company, bool>> expression)
        {
            return _companyRepository.Fetch(expression);
        }

        public Company FetchFirst(Expression<Func<Company, bool>> expression)
        {
            return _companyRepository.FetchFirst(expression);
        }

        public Company GetById(object id)
        {
            return _companyRepository.GetById(id);
        }

        public Company GetByName(string name)
        {
            return _companyRepository.GetAll().FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
        }

        public ResponseModel Insert(Company company)
        {
            return _companyRepository.Insert(company);
        }

        internal ResponseModel Update(Company company)
        {
            return _companyRepository.Update(company);
        }

        internal ResponseModel Delete(Company company)
        {
            return _companyRepository.Delete(company);
        }

        internal ResponseModel Delete(object id)
        {
            return _companyRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _companyRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the companies
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchCompanies(JqSearchIn si, CompanySearchModel model)
        {
            var data = SearchCompanies(model);

            var companies = Maps(data);

            return si.Search(companies);
        }

        /// <summary>
        /// Export companies
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, CompanySearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchCompanies(model);

            var companies = Maps(data);

            var exportData = si.Export(companies, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search companies
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<Company> SearchCompanies(CompanySearchModel model)
        {
            return Fetch(company => (string.IsNullOrEmpty(model.Keyword)
                                     || (!string.IsNullOrEmpty(company.Name) && company.Name.Contains(model.Keyword))
                                     ||
                                     (!company.CompanyTypeId.HasValue && !string.IsNullOrEmpty(company.CompanyType.Name)
                                      && company.CompanyType.Name.Contains(model.Keyword)))
                                    && (!model.CompanyTypeId.HasValue || company.CompanyTypeId == model.CompanyTypeId));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="companies"></param>
        /// <returns></returns>
        private IQueryable<CompanyModel> Maps(IQueryable<Company> companies)
        {
            return companies.Select(m => new CompanyModel
            {
                Id = m.Id,
                Name = m.Name,
                CompanyTypeId = m.CompanyTypeId,
                CompanyTypeName = m.CompanyTypeId.HasValue ? m.CompanyType.Name : string.Empty,
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
        /// Get Company manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CompanyManageModel GetCompanyManageModel(int? id = null)
        {
            var company = GetById(id);
            if (company != null)
            {
                return new CompanyManageModel(company);
            }
            return new CompanyManageModel();
        }

        /// <summary>
        /// Save Company
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveCompany(CompanyManageModel model)
        {
            ResponseModel response;
            var company = GetById(model.Id);
            if (company != null)
            {
                company.Name = model.Name;
                company.CompanyTypeId = model.CompanyTypeId;
                company.RecordOrder = model.RecordOrder;
                response = Update(company);
                return response.SetMessage(response.Success
                    ? T("Company_Message_UpdateSuccessfully")
                    : T("Company_Message_UpdateFailure"));
            }
            Mapper.CreateMap<CompanyManageModel, Company>();
            company = Mapper.Map<CompanyManageModel, Company>(model);
            response = Insert(company);
            return response.SetMessage(response.Success
                ? T("Company_Message_CreateSuccessfully")
                : T("Company_Message_CreateFailure"));
        }

        /// <summary>
        /// Save company by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string SaveCompany(string name)
        {
            // Insert company if not available
            if (!string.IsNullOrWhiteSpace(name))
            {
                var company = GetByName(name);

                if (company == null)
                {
                    var companyManageModel = new CompanyManageModel
                    {
                        Name = name,
                        RecordOrder =
                            GetAll().Any() ? GetAll().Max(c => c.RecordOrder) + 10 : 10
                    };

                    var response = SaveCompany(companyManageModel);
                    if (response.Success)
                    {
                        return name;
                    }
                }
                else
                {
                    // Make sure company name is exactly the same with database (lower, upper characters)
                    name = company.Name;
                }
            }

            return name;
        }

        /// <summary>
        /// Delete company by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteCompany(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                // Delete the compnay
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("Company_Message_DeleteSuccessfully")
                    : T("Company_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("Company_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get Company detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CompanyDetailModel GetCompanyDetailModel(int id)
        {
            var company = GetById(id);
            return company != null ? new CompanyDetailModel(company) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateCompanyData(XEditableModel model)
        {
            var company = GetById(model.Pk);
            if (company != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (CompanyManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new CompanyManageModel(company);
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

                    company.SetProperty(model.Name, value);

                    var response = Update(company);
                    return response.SetMessage(response.Success
                        ? T("Company_Message_UpdateCompanyInfoSuccessfully")
                        : T("Company_Message_UpdateCompanyInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Company_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("Company_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}