using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Locations;
using EzCMS.Core.Models.Locations.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Locations
{
    [Register(Lifetime.PerInstance)]
    public interface ILocationService : IBaseService<Location>
    {
        /// <summary>
        /// Get locations
        /// </summary>
        /// <param name="locationIds"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetLocations(List<int> locationIds = null);

        #region Grid Search

        /// <summary>
        /// Search locations
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchLocations(JqSearchIn si, LocationSearchModel model);

        /// <summary>
        /// Export locations
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, LocationSearchModel model);

        /// <summary>
        /// Get location search model
        /// </summary>
        /// <returns></returns>
        LocationSearchModel GetLocationSearchModel();

        #endregion

        #region Manage

        /// <summary>
        /// Get location manage model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="locationTypeId"></param>
        /// <returns></returns>
        LocationManageModel GetLocationManageModel(int? id = null, int? locationTypeId = null);

        /// <summary>
        /// Save location
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveLocation(LocationManageModel model);

        /// <summary>
        /// Delete location
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteLocation(int id);

        #endregion

        #region Details

        /// <summary>
        /// Get location detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LocationDetailModel GetLocationDetailModel(int id);

        /// <summary>
        /// Update location data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateLocationData(XEditableModel model);

        #endregion

        #region Widget

        /// <summary>
        /// Get location widget
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LocationWidget GetLocationWidget(int id);

        /// <summary>
        /// Get locations by selected types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        List<LocationWidget> GetLocationsByTypes(List<int> types);

        #endregion
    }
}