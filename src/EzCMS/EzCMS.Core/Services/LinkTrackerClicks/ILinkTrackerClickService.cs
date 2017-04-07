using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.LinkTrackerClicks;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.LinkTrackerClicks
{
    [Register(Lifetime.PerInstance)]
    public interface ILinkTrackerClickService : IBaseService<LinkTrackerClick>
    {
        #region Details

        /// <summary>
        /// Get link tracker click detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LinkTrackerClickDetailModel GetLinkTrackerClickDetailModel(int id);

        #endregion

        #region Monthly Click Through

        /// <summary>
        /// Get all click years
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetYears();

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the link tracker clicks
        /// </summary>
        /// <returns></returns>
        JqGridSearchOut SearchLinkTrackerClicks(JqSearchIn si, int? linkTrackerId);

        /// <summary>
        /// Export link tracker clicks
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="linkTrackerId"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? linkTrackerId);

        #endregion

        #region Manage

        /// <summary>
        /// Add new link tracker click
        /// </summary>
        /// <param name="linkTrackerClick"></param>
        /// <returns></returns>
        ResponseModel AddLinkTrackerClick(LinkTrackerClick linkTrackerClick);

        /// <summary>
        /// Delete the link tracker click by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteLinkTrackerClick(int id);

        #endregion
    }
}