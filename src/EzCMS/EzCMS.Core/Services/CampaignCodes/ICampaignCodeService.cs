using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.CampaignCodes;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.CampaignCodes
{
    [Register(Lifetime.PerInstance)]
    public interface ICampaignCodeService : IBaseService<CampaignCode>
    {
        /// <summary>
        /// Get campaign code select list
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetCampaignCodes();

        #region Grid Search

        /// <summary>
        /// Search campaign codes
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchCampaignCodes(JqSearchIn si);

        /// <summary>
        /// Export campaign codes
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get Campaign code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CampaignCodeManageModel GetCampaignCodeManageModel(int? id = null);

        /// <summary>
        /// Save Campaign code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveCampaignCode(CampaignCodeManageModel model);

        /// <summary>
        /// Delete Campaign code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteCampaignCode(int id);

        #endregion
    }
}