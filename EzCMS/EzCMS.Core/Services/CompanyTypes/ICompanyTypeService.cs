using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.CompanyTypes;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.CompanyTypes
{
    [Register(Lifetime.PerInstance)]
    public interface ICompanyTypeService : IBaseService<CompanyType>
    {
        /// <summary>
        /// Get company types
        /// </summary>
        /// <param name="companyTypeIds"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetCompanyTypes(List<int> companyTypeIds = null);

        /// <summary>
        /// Get company type name
        /// </summary>
        /// <param name="companyTypeId"></param>
        /// <returns></returns>
        string GetCompanyTypeName(int companyTypeId);

        /// <summary>
        /// Get company type names by ids
        /// </summary>
        /// <param name="companyTypeIds"></param>
        /// <returns></returns>
        IQueryable<string> GetCompanyTypeNames(List<int> companyTypeIds = null);

        #region Grid Search

        /// <summary>
        /// Search company types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchCompanyTypes(JqSearchIn si, int? associateId);

        /// <summary>
        /// Export company types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? associateId);

        #endregion

        #region Manage

        /// <summary>
        /// Get company type manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CompanyTypeManageModel GetCompanyTypeManageModel(int? id = null);

        /// <summary>
        /// Save company type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveCompanyType(CompanyTypeManageModel model);

        /// <summary>
        /// Delete company type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteCompanyType(int id);

        #endregion

        #region Details

        /// <summary>
        /// Get company type details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CompanyTypeDetailModel GetCompanyTypeDetailModel(int id);

        /// <summary>
        /// Update company type data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateCompanyTypeData(XEditableModel model);

        /// <summary>
        /// Delete associate - company type mapping
        /// </summary>
        /// <param name="companyTypeId"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        ResponseModel DeleteAssociateCompanyTypeMapping(int companyTypeId, int associateId);

        #endregion
    }
}