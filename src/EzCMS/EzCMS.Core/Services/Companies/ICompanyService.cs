using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Companies;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Companies
{
    [Register(Lifetime.PerInstance)]
    public interface ICompanyService : IBaseService<Company>
    {
        #region Validation

        /// <summary>
        /// Check if company name is existed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsNameExisted(int? id, string name);

        #endregion

        /// <summary>
        /// Search companies
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> SearchCompaniesByKeyword(string keyword = "");

        /// <summary>
        /// Get companies
        /// </summary>
        /// <param name="companyIds"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetCompanies(List<int> companyIds = null);

        /// <summary>
        /// Get company by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Company GetByName(string name);

        #region Grid Search

        /// <summary>
        /// Search companies
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchCompanies(JqSearchIn si, CompanySearchModel model);

        /// <summary>
        /// Export companies
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, CompanySearchModel model);

        #endregion

        #region Manage

        /// <summary>
        /// Get company manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CompanyManageModel GetCompanyManageModel(int? id = null);

        /// <summary>
        /// Save company
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveCompany(CompanyManageModel model);

        /// <summary>
        /// Save company
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string SaveCompany(string name);

        /// <summary>
        /// Delete company
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteCompany(int id);

        /// <summary>
        /// Insert company
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        ResponseModel Insert(Company company);

        #endregion

        #region Details

        /// <summary>
        /// Get company details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CompanyDetailModel GetCompanyDetailModel(int id);

        /// <summary>
        /// Update company data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateCompanyData(XEditableModel model);

        #endregion
    }
}