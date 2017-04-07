using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.EventCategories;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.EventCategories
{
    [Register(Lifetime.PerInstance)]
    public interface IEventCategoryService : IBaseService<EventCategory>
    {
        #region Validation

        /// <summary>
        /// Check if event category exists
        /// </summary>
        /// <param name="eventCategoryId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsCategoryExisted(int? eventCategoryId, string name);

        #endregion

        /// <summary>
        /// Get event category detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EventCategoryDetailModel GetEventCategoryDetailModel(int id);

        /// <summary>
        /// Delete event - event category mapping
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        ResponseModel DeleteEventEventCategoryMapping(int eventId, int categoryId);

        #region Grid Search

        /// <summary>
        /// Search the event categories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchEventCategories(JqSearchIn si, int? eventId = null);

        /// <summary>
        /// Export event categories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? eventId = null);

        #endregion

        #region Manage

        /// <summary>
        /// Get list of event category
        /// </summary>
        /// <param name="eventCategoryIds"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetEventCategories(List<int> eventCategoryIds = null);

        /// <summary>
        /// Get event category manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EventCategoryManageModel GetEventCategoryManageModel(int? id = null);

        /// <summary>
        /// Save event category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveEventCategory(EventCategoryManageModel model);

        /// <summary>
        /// Delete Event category by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteEventCategory(int id);

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateEventCategoryData(XEditableModel model);

        #endregion
    }
}