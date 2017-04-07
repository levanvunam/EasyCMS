using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Events;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Events
{
    [Register(Lifetime.PerInstance)]
    public interface IEventService : IBaseService<Event>
    {
        /// <summary>
        /// Get event details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EventDetailModel GetEventDetailModel(int id);

        /// <summary>
        /// Get events
        /// </summary>
        /// <param name="eventCategoryId"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetEvents(int? eventCategoryId = null);

        #region Grid

        /// <summary>
        /// Search events
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchEvents(JqSearchIn si, EventSearchModel model);

        /// <summary>
        /// Exports events
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, EventSearchModel model);

        #endregion

        #region Manage

        /// <summary>
        /// Get event manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EventManageModel GetEventManageModel(int? id = null);

        /// <summary>
        /// Get event create model
        /// </summary>
        /// <returns></returns>
        EventCreateModel GetEventCreateModel();

        /// <summary>
        /// Save event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveEvent(EventManageModel model);

        /// <summary>
        /// Save event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveEvent(EventCreateModel model);

        /// <summary>
        /// Delete event
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteEvent(int id);

        /// <summary>
        /// Update event data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateEventData(XEditableModel model);

        #endregion
    }
}