using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.LocationTypes;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.LocationTypes
{
    [Register(Lifetime.PerInstance)]
    public interface ILocationTypeService : IBaseService<LocationType>
    {
        #region Validation

        /// <summary>
        /// Check if location type exists
        /// </summary>
        /// <param name="locationTypeId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsLocationTypeExisted(int? locationTypeId, string name);

        #endregion

        /// <summary>
        /// Delete location - location type mapping
        /// </summary>
        /// <param name="locationTypeId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        ResponseModel DeleteLocationLocationTypeMapping(int locationTypeId, int locationId);

        #region Grid Search

        /// <summary>
        /// Search location types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchLocationTypes(JqSearchIn si, int? locationId);

        /// <summary>
        /// Export location types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? locationId);

        #endregion

        #region Manage

        /// <summary>
        /// Get list of location type
        /// </summary>
        /// <param name="locationTypeIds"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetLocationTypes(List<int> locationTypeIds = null);

        /// <summary>
        /// Get location type manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LocationTypeManageModel GetLocationTypeManageModel(int? id = null);

        /// <summary>
        /// Save location type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveLocationType(LocationTypeManageModel model);

        /// <summary>
        /// Delete Event category by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteLocationType(int id);

        #endregion

        #region Details

        /// <summary>
        /// Get location type detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LocationTypeDetailModel GetLocationTypeDetailModel(int id);

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateLocationTypeData(XEditableModel model);

        #endregion
    }
}