using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.AssociateTypes;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.AssociateTypes
{
    [Register(Lifetime.PerInstance)]
    public interface IAssociateTypeService : IBaseService<AssociateType>
    {
        #region Validation

        /// <summary>
        /// Check if associate type exists
        /// </summary>
        /// <param name="associateTypeId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsAssociateTypeExisted(int? associateTypeId, string name);

        #endregion

        /// <summary>
        /// Delete associate - associate type mapping
        /// </summary>
        /// <param name="associateTypeId"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        ResponseModel DeleteAssociateAssociateTypeMapping(int associateTypeId, int associateId);

        #region Grid Search

        /// <summary>
        /// Search the associate types.
        /// </summary>
        /// <returns></returns>
        JqGridSearchOut SearchAssociateTypes(JqSearchIn si, int? associateId);

        /// <summary>
        /// Export associate types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? associateId);

        #endregion

        #region Manage

        /// <summary>
        /// Get list of associate type
        /// </summary>
        /// <param name="associateTypeIds"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetAssociateTypes(List<int> associateTypeIds = null);

        /// <summary>
        /// Get associate type manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AssociateTypeManageModel GetAssociateTypeManageModel(int? id = null);

        /// <summary>
        /// Save associate type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveAssociateType(AssociateTypeManageModel model);

        /// <summary>
        /// Delete associate type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteAssociateType(int id);

        #endregion

        #region Details

        /// <summary>
        /// Get associate type detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AssociateTypeDetailModel GetAssociateTypeDetailModel(int id);

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateAssociateTypeData(XEditableModel model);

        #endregion
    }
}