using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.EventSchedules;
using EzCMS.Core.Models.EventSchedules.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.EventSchedules
{
    public class EventScheduleService : ServiceHelper, IEventScheduleService
    {
        private readonly IRepository<EventSchedule> _eventScheduleRepository;

        public EventScheduleService(IRepository<EventSchedule> eventScheduleRepository)
        {
            _eventScheduleRepository = eventScheduleRepository;
        }

        #region Base

        public IQueryable<EventSchedule> GetAll()
        {
            return _eventScheduleRepository.GetAll();
        }

        public IQueryable<EventSchedule> Fetch(Expression<Func<EventSchedule, bool>> expression)
        {
            return _eventScheduleRepository.Fetch(expression);
        }

        public EventSchedule FetchFirst(Expression<Func<EventSchedule, bool>> expression)
        {
            return _eventScheduleRepository.FetchFirst(expression);
        }

        public EventSchedule GetById(object id)
        {
            return _eventScheduleRepository.GetById(id);
        }

        internal ResponseModel Insert(EventSchedule eventSchedule)
        {
            return _eventScheduleRepository.Insert(eventSchedule);
        }

        internal ResponseModel Update(EventSchedule eventSchedule)
        {
            return _eventScheduleRepository.Update(eventSchedule);
        }

        internal ResponseModel Delete(EventSchedule eventSchedule)
        {
            return _eventScheduleRepository.Delete(eventSchedule);
        }

        internal ResponseModel Delete(object id)
        {
            return _eventScheduleRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _eventScheduleRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the event schedules
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchEventSchedules(JqSearchIn si, int? eventId)
        {
            var data = SearchEventSchedules(eventId);

            var eventSchedules = Maps(data);

            return si.Search(eventSchedules);
        }

        /// <summary>
        /// Export EventSchedules
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? eventId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchEventSchedules(eventId);

            var eventSchedules = Maps(data);

            var exportData = si.Export(eventSchedules, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search the event schedules
        /// </summary>
        /// <returns></returns>
        private IQueryable<EventSchedule> SearchEventSchedules(int? eventId)
        {
            return Fetch(eventSchedule => !eventId.HasValue || eventSchedule.EventId == eventId);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="eventSchedules"></param>
        /// <returns></returns>
        private IQueryable<EventScheduleModel> Maps(IQueryable<EventSchedule> eventSchedules)
        {
            return eventSchedules.Select(s => new EventScheduleModel
            {
                Id = s.Id,
                EventId = s.EventId,
                EventTitle = s.Event.Title,
                Location = s.Location,
                TimeStart = s.TimeStart,
                TimeEnd = s.TimeEnd,
                MaxAttendees = s.MaxAttendees,
                RecordOrder = s.RecordOrder,
                Created = s.Created,
                CreatedBy = s.CreatedBy,
                LastUpdate = s.LastUpdate,
                LastUpdateBy = s.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get event schedule manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EventScheduleManageModel GetEventScheduleManageModel(int? id = null)
        {
            var eventSchedule = GetById(id);
            return eventSchedule != null ? new EventScheduleManageModel(eventSchedule) : new EventScheduleManageModel();
        }

        /// <summary>
        /// Save event schedule
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveEventSchedule(EventScheduleManageModel model)
        {
            ResponseModel response;
            var eventSchedule = GetById(model.Id);
            if (eventSchedule != null)
            {
                eventSchedule.EventId = model.EventId;
                eventSchedule.Location = model.Location;
                eventSchedule.TimeStart = model.TimeStart ?? DateTime.UtcNow;
                eventSchedule.TimeEnd = model.TimeEnd;
                eventSchedule.MaxAttendees = model.MaxAttendees;
                response = Update(eventSchedule);
                response.SetMessage(response.Success
                    ? T("EventSchedule_Message_UpdateSuccessfully")
                    : T("EventSchedule_Message_UpdateFailure"));
            }
            else
            {
                Mapper.CreateMap<EventScheduleManageModel, EventSchedule>();
                eventSchedule = Mapper.Map<EventScheduleManageModel, EventSchedule>(model);

                response = Insert(eventSchedule);
                response.SetMessage(response.Success
                    ? T("EventSchedule_Message_CreateSuccessfully")
                    : T("EventSchedule_Message_CreateFailure"));
            }
            return response;
        }

        /// <summary>
        /// Delete the event schedule by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteEventSchedule(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("EventSchedule_Message_DeleteSuccessfully")
                    : T("EventSchedule_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("EventSchedule_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Widgets

        /// <summary>
        /// Get EventScheduleWidget
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="total"></param>
        /// <param name="dateFormat"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        public EventSchedulesWidget GetEventSchedulesWidget(int? categoryId, int total, string dateFormat,
            string timeFormat)
        {
            var now = DateTime.UtcNow;
            var data = Fetch(m => m.TimeStart >= now);

            if (categoryId.HasValue && categoryId > 0)
            {
                data = data.Where(m => m.Event.EventEventCategories.Any(x => x.EventCategoryId == categoryId));
            }

            data = total > 0 ? data.OrderBy(m => m.TimeStart).Take(total) : data.OrderBy(m => m.TimeStart);

            dateFormat = string.IsNullOrEmpty(dateFormat) ? "d MMMM" : dateFormat;
            timeFormat = string.IsNullOrEmpty(timeFormat) ? "htt" : timeFormat;

            return new EventSchedulesWidget
            {
                EventSchedules =
                    data.ToList().Select(m => new EventScheduleWidget(m, dateFormat, timeFormat)).ToList()
            };
        }

        /// <summary>
        /// Get string represent the time frame of the event from start time to end time
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="dateFormat"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        public string GetEventScheduleTimeFrame(DateTime timeStart, DateTime? timeEnd, string dateFormat,
            string timeFormat)
        {
            var timeFrame = new StringBuilder();

            timeFrame.Append(timeStart.ToString(dateFormat));
            if (!timeEnd.HasValue)
            {
                timeFrame.Append(string.Format(" | {0}", T("EventSchedule_Text_AllDayEventText")));
            }
            else if (timeStart.Date == timeEnd.Value.Date)
            {
                timeFrame.Append(string.Format(" | {0} - {1}", timeStart.ToString(timeFormat),
                    timeEnd.Value.ToString(timeFormat)));
            }
            else
            {
                timeFrame.Append(string.Format(" - {0}", timeEnd.Value.ToString(dateFormat)));
            }
            return timeFrame.ToString();
        }

        #endregion

        #region Details

        /// <summary>
        /// Get event schedule detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EventScheduleDetailModel GetEventScheduleDetailModel(int? id = null)
        {
            var eventSchedule = GetById(id);
            return eventSchedule != null ? new EventScheduleDetailModel(eventSchedule) : null;
        }

        /// <summary>
        /// Update event schedule data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateEventScheduleData(XEditableModel model)
        {
            var eventSchedule = GetById(model.Pk);
            if (eventSchedule != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (EventScheduleManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new EventScheduleManageModel(eventSchedule);
                    manageModel.SetProperty(model.Name, value);

                    var validationResults = manageModel.ValidateModel();

                    if (validationResults.Any())
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = validationResults.BuildValidationMessages()
                        };
                    }

                    #endregion

                    eventSchedule.SetProperty(model.Name, value);

                    var response = Update(eventSchedule);
                    return response.SetMessage(response.Success
                        ? T("EventSchedule_Message_UpdateEventScheduleInfoSuccessfully")
                        : T("EventSchedule_Message_UpdateEventScheduleInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("EventSchedule_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("EventSchedule_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}