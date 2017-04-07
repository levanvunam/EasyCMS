using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
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
using EzCMS.Core.Models.Events;
using EzCMS.Core.Services.EventSchedules;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.EventEventCategories;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Events
{
    public class EventService : ServiceHelper, IEventService
    {
        private readonly IEventEventCategoryRepository _eventEventCategoryRepository;
        private readonly IRepository<Event> _eventRepository;
        private readonly IEventScheduleService _eventScheduleService;

        public EventService(IRepository<Event> eventRepository, IEventScheduleService eventScheduleService,
            IEventEventCategoryRepository eventEventCategoryRepository)
        {
            _eventRepository = eventRepository;
            _eventScheduleService = eventScheduleService;
            _eventEventCategoryRepository = eventEventCategoryRepository;
        }

        #region Base

        public IQueryable<Event> GetAll()
        {
            return _eventRepository.GetAll();
        }

        public IQueryable<Event> Fetch(Expression<Func<Event, bool>> expression)
        {
            return _eventRepository.Fetch(expression);
        }

        public Event FetchFirst(Expression<Func<Event, bool>> expression)
        {
            return _eventRepository.FetchFirst(expression);
        }

        public Event GetById(object id)
        {
            return _eventRepository.GetById(id);
        }

        internal ResponseModel Insert(Event item)
        {
            return _eventRepository.Insert(item);
        }

        internal ResponseModel Update(Event item)
        {
            return _eventRepository.Update(item);
        }

        internal ResponseModel Delete(Event item)
        {
            return _eventRepository.Delete(item);
        }

        internal ResponseModel Delete(object id)
        {
            return _eventRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _eventRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid

        /// <summary>
        /// Search the events
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchEvents(JqSearchIn si, EventSearchModel model)
        {
            var data = SearchEvents(model);

            var events = Maps(data);

            return si.Search(events);
        }

        /// <summary>
        /// Export events
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, EventSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchEvents(model);

            var events = Maps(data);

            var exportData = si.Export(events, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search the events
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<Event> SearchEvents(EventSearchModel model)
        {
            return Fetch(@event => (string.IsNullOrEmpty(model.Keyword)
                                    || (!string.IsNullOrEmpty(@event.Title) && @event.Title.Contains(model.Keyword))
                                    ||
                                    (!string.IsNullOrEmpty(@event.EventDescription) &&
                                     @event.EventDescription.Contains(model.Keyword))
                                    ||
                                    (!string.IsNullOrEmpty(@event.EventSummary) &&
                                     @event.EventSummary.Contains(model.Keyword)))
                                   && (!model.EventCategoryId.HasValue
                                       ||
                                       @event.EventEventCategories.Any(
                                           eventEventCategory =>
                                               eventEventCategory.EventCategoryId == model.EventCategoryId)));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        private IQueryable<EventModel> Maps(IQueryable<Event> events)
        {
            var now = DateTime.UtcNow;

            return events.Select(e => new EventModel
            {
                Id = e.Id,
                Title = e.Title,
                EventSummary = e.EventSummary,
                EventDescription = e.EventDescription,
                MaxAttendees = e.MaxAttendees,
                RegistrationFullText = e.RegistrationFullText,
                RegistrationWaiver = e.RegistrationWaiver,
                UpcomingDate =
                    e.EventSchedules.OrderBy(m => m.TimeStart).FirstOrDefault(m => m.TimeStart >= now).TimeStart,
                RecordOrder = e.RecordOrder,
                Created = e.Created,
                CreatedBy = e.CreatedBy,
                LastUpdate = e.LastUpdate,
                LastUpdateBy = e.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get event manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EventManageModel GetEventManageModel(int? id = null)
        {
            var item = GetById(id);
            return item != null ? new EventManageModel(item) : new EventManageModel();
        }

        /// <summary>
        /// Get event create model
        /// </summary>
        /// <returns></returns>
        public EventCreateModel GetEventCreateModel()
        {
            return new EventCreateModel
            {
                Event = GetEventManageModel(),
                EventSchedule = _eventScheduleService.GetEventScheduleManageModel()
            };
        }

        /// <summary>
        /// Get all active events as a select list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetEvents(int? eventCategoryId = null)
        {
            return GetAll().Select(m => new SelectListItem
            {
                Text = m.Title,
                Value = SqlFunctions.StringConvert((double) m.Id).Trim(),
                Selected =
                    eventCategoryId.HasValue && m.EventEventCategories.Any(nc => nc.EventCategoryId == eventCategoryId)
            });
        }

        /// <summary>
        /// Save event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveEvent(EventManageModel model)
        {
            ResponseModel response;
            var item = GetById(model.Id);
            if (item != null)
            {
                item.Title = model.Title;
                item.EventSummary = model.EventSummary;
                item.EventDescription = model.EventDescription;
                item.MaxAttendees = model.MaxAttendees;
                item.RegistrationFullText = model.RegistrationFullText;
                item.RegistrationWaiver = model.RegistrationWaiver;

                response = Update(item);

                if (model.EventCategoryIds == null) model.EventCategoryIds = new List<int>();
                var currentCategorys = item.EventEventCategories != null
                    ? item.EventEventCategories.Select(t => t.EventCategoryId).ToList()
                    : new List<int>();

                var removedCategoryIds = currentCategorys.Where(id => !model.EventCategoryIds.Contains(id));
                _eventEventCategoryRepository.Delete(item.Id, removedCategoryIds);

                var addedCategoryIds = model.EventCategoryIds.Where(id => !currentCategorys.Contains(id));
                _eventEventCategoryRepository.Insert(item.Id, addedCategoryIds);

                response.SetMessage(response.Success
                    ? T("Event_Message_UpdateSuccessfully")
                    : T("Event_Message_UpdateFailure"));
            }
            else
            {
                Mapper.CreateMap<EventManageModel, Event>();
                item = Mapper.Map<EventManageModel, Event>(model);

                response = Insert(item);
                if (response.Success)
                {
                    if (model.EventCategoryIds != null)
                    {
                        foreach (var categoryId in model.EventCategoryIds)
                        {
                            var eventEventCategory = new EventEventCategory
                            {
                                EventCategoryId = categoryId,
                                EventId = item.Id
                            };
                            _eventEventCategoryRepository.Insert(eventEventCategory);
                        }
                    }
                }

                response.SetMessage(response.Success
                    ? T("Event_Message_CreateSuccessfully")
                    : T("Event_Message_CreateFailure"));
            }
            return response;
        }

        /// <summary>
        /// Save new event from event create model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveEvent(EventCreateModel model)
        {
            // Save Event
            var saveEventResponse = SaveEvent(model.Event);

            // If save Event success, save EventSchedule
            if (saveEventResponse.Success)
            {
                var eventId = (int) saveEventResponse.Data;
                model.EventSchedule.EventId = eventId;

                var saveEventScheduleResponse = _eventScheduleService.SaveEventSchedule(model.EventSchedule);
                return saveEventScheduleResponse.Success ? saveEventResponse : saveEventScheduleResponse;
            }
            return saveEventResponse;
        }

        /// <summary>
        /// Delete the event
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteEvent(int id)
        {
            // Stop deletion if schedule found
            var item = GetById(id);
            if (item != null)
            {
                if (item.EventSchedules.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Event_Message_DeleteFailureBasedOnRelatedEventSchedules")
                    };
                }

                if (item.EventEventCategories.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Event_Message_DeleteFailureBasedOnRelatedEventCategories")
                    };
                }

                // Delete the Event
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("Event_Message_DeleteSuccessfully")
                    : T("Event_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("Event_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get event detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EventDetailModel GetEventDetailModel(int id)
        {
            var item = GetById(id);
            return item != null ? new EventDetailModel(item) : null;
        }

        /// <summary>
        /// Update event data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateEventData(XEditableModel model)
        {
            var item = GetById(model.Pk);
            if (item != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (EventManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new EventManageModel(item);
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

                    item.SetProperty(model.Name, value);

                    var response = Update(item);
                    return response.SetMessage(response.Success
                        ? T("Event_Message_UpdateEventInfoSuccessfully")
                        : T("Event_Message_UpdateEventInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Event_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("Event_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}