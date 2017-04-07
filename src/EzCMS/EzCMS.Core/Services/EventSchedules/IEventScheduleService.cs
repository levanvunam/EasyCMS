using System;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.EventSchedules;
using EzCMS.Core.Models.EventSchedules.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.EventSchedules
{
    [Register(Lifetime.PerInstance)]
    public interface IEventScheduleService : IBaseService<EventSchedule>
    {
        #region Grid Search

        /// <summary>
        /// Search the event schedules
        /// </summary>
        /// <param name="si"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchEventSchedules(JqSearchIn si, int? eventId);

        /// <summary>
        /// Export the event schedules
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? eventId);

        #endregion

        #region Manage

        /// <summary>
        /// Get event schedule manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EventScheduleManageModel GetEventScheduleManageModel(int? id = null);

        /// <summary>
        /// Save event schedule
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveEventSchedule(EventScheduleManageModel model);

        /// <summary>
        /// Delete event schedule
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteEventSchedule(int id);

        #endregion

        #region Widgets

        /// <summary>
        /// Get event schedules widget
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="total"></param>
        /// <param name="dateFormat"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        EventSchedulesWidget GetEventSchedulesWidget(int? categoryId, int total, string dateFormat,
            string timeFormat);

        /// <summary>
        /// Get event schedule time frame
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="dateFormat"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        string GetEventScheduleTimeFrame(DateTime timeStart, DateTime? timeEnd, string dateFormat, string timeFormat);

        #endregion

        #region Details

        /// <summary>
        /// Get event schedule details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EventScheduleDetailModel GetEventScheduleDetailModel(int? id = null);

        /// <summary>
        /// Update event schedule data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateEventScheduleData(XEditableModel model);

        #endregion
    }
}