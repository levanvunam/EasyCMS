using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Countries;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Countries
{
    [Register(Lifetime.PerInstance)]
    public interface ICountryService : IBaseService<Country>
    {
        /// <summary>
        /// Get country select list
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetCountries(string keyword = "");

        #region Grid Search

        /// <summary>
        /// Search countries
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchCountries(JqSearchIn si);

        /// <summary>
        /// Exports countries
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get country manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CountryManageModel GetCountryManageModel(int? id = null);

        /// <summary>
        /// Save country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveCountry(CountryManageModel model);

        #endregion
    }
}