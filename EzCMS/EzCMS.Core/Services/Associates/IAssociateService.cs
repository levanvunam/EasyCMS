using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Associates;
using EzCMS.Core.Models.Associates.Widgets;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Associates
{
    [Register(Lifetime.PerInstance)]
    public interface IAssociateService : IBaseService<Associate>
    {
        #region Widgets

        /// <summary>
        /// Load associate listing
        /// </summary>
        /// <param name="associateTypeId"></param>
        /// <param name="locationId"></param>
        /// <param name="companyTypeId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        AssociateListingWidget GetAssociateListingWidget(int? associateTypeId, int? locationId,
            int? companyTypeId, AssociateEnums.AssociateStatus status);

        #endregion

        #region Grid Search

        /// <summary>
        /// Search associates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchAssociates(JqSearchIn si, AssociateSearchModel model);

        /// <summary>
        /// Export associates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, AssociateSearchModel model);

        /// <summary>
        /// Get associate search model
        /// </summary>
        /// <returns></returns>
        AssociateSearchModel GetAssociateSearchModel();

        #endregion

        #region Manage

        /// <summary>
        /// Get associate manage model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="associateTypeId"></param>
        /// <returns></returns>
        AssociateManageModel GetAssociateManageModel(int? id = null, int? associateTypeId = null);

        /// <summary>
        /// Save associate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveAssociate(AssociateManageModel model);

        /// <summary>
        /// Delete associate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteAssociate(int id);

        /// <summary>
        /// Delete mapping between associate and location
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        ResponseModel DeleteAssociateLocationMapping(int locationId, int associateId);

        #endregion

        #region Details

        /// <summary>
        /// Get associate detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AssociateDetailModel GetAssociateDetailModel(int id);

        /// <summary>
        /// Update associate data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateAssociateData(XEditableModel model);

        #endregion
    }
}