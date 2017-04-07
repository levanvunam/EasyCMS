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
using EzCMS.Core.Models.EventCategories;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.EventEventCategories;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.EventCategories
{
    public class EventCategoryService : ServiceHelper, IEventCategoryService
    {
        private readonly IRepository<EventCategory> _eventCategoryRepository;
        private readonly IEventEventCategoryRepository _eventEventCategoryRepository;

        public EventCategoryService(IRepository<EventCategory> eventCategoryRepository,
            IEventEventCategoryRepository eventEventCategoryRepository)
        {
            _eventCategoryRepository = eventCategoryRepository;
            _eventEventCategoryRepository = eventEventCategoryRepository;
        }

        #region Validation

        /// <summary>
        /// Check if event category exists
        /// </summary>
        /// <param name="eventCategoryId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsCategoryExisted(int? eventCategoryId, string name)
        {
            return Fetch(u => u.Name.Equals(name) && u.Id != eventCategoryId).Any();
        }

        #endregion

        /// <summary>
        /// Get list of event category
        /// </summary>
        /// <param name="eventCategoryIds"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetEventCategories(List<int> eventCategoryIds = null)
        {
            if (eventCategoryIds == null) eventCategoryIds = new List<int>();
            return GetAll().Select(g => new SelectListItem
            {
                Text = g.Name,
                Value = SqlFunctions.StringConvert((double) g.Id).Trim(),
                Selected = eventCategoryIds.Any(id => id == g.Id)
            });
        }

        /// <summary>
        /// Delete event - event category mapping
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public ResponseModel DeleteEventEventCategoryMapping(int eventId, int categoryId)
        {
            // Delete the Event Category mapping
            var response = _eventEventCategoryRepository.Delete(categoryId, eventId);

            return response.SetMessage(response.Success
                ? T("EventEventCategory_Message_DeleteMappingSuccessfully")
                : T("EventEventCategory_Message_DeleteMappingFailure"));
        }

        #region Base

        public IQueryable<EventCategory> GetAll()
        {
            return _eventCategoryRepository.GetAll();
        }

        public IQueryable<EventCategory> Fetch(Expression<Func<EventCategory, bool>> expression)
        {
            return _eventCategoryRepository.Fetch(expression);
        }

        public EventCategory FetchFirst(Expression<Func<EventCategory, bool>> expression)
        {
            return _eventCategoryRepository.FetchFirst(expression);
        }

        public EventCategory GetById(object id)
        {
            return _eventCategoryRepository.GetById(id);
        }

        internal ResponseModel Insert(EventCategory eventCategory)
        {
            return _eventCategoryRepository.Insert(eventCategory);
        }

        internal ResponseModel Update(EventCategory eventCategory)
        {
            return _eventCategoryRepository.Update(eventCategory);
        }

        internal ResponseModel Delete(EventCategory eventCategory)
        {
            return _eventCategoryRepository.Delete(eventCategory);
        }

        internal ResponseModel Delete(object id)
        {
            return _eventCategoryRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _eventCategoryRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the event categories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchEventCategories(JqSearchIn si, int? eventId = null)
        {
            var data = SearchEventCategories(eventId);

            var eventCategories = Maps(data);

            return si.Search(eventCategories);
        }

        /// <summary>
        /// Export event categories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? eventId = null)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchEventCategories(eventId);

            var eventCategories = Maps(data);

            var exportData = si.Export(eventCategories, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search the event categories
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        private IQueryable<EventCategory> SearchEventCategories(int? eventId)
        {
            return
                Fetch(
                    eventCategory =>
                        !eventId.HasValue ||
                        eventCategory.EventEventCategories.Any(
                            eventEventCategory => eventEventCategory.EventId == eventId));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="eventCategories"></param>
        /// <returns></returns>
        private IQueryable<EventCategoryModel> Maps(IQueryable<EventCategory> eventCategories)
        {
            return eventCategories.Select(m => new EventCategoryModel
            {
                Id = m.Id,
                Name = m.Name,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get event category manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EventCategoryManageModel GetEventCategoryManageModel(int? id = null)
        {
            var eventCategory = GetById(id);
            if (eventCategory != null)
            {
                return new EventCategoryManageModel(eventCategory);
            }
            return new EventCategoryManageModel();
        }

        /// <summary>
        /// Save event category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveEventCategory(EventCategoryManageModel model)
        {
            ResponseModel response;
            var eventCategory = GetById(model.Id);
            if (eventCategory != null)
            {
                eventCategory.Name = model.Name;
                eventCategory.RecordOrder = model.RecordOrder;
                response = Update(eventCategory);

                return response.SetMessage(response.Success
                    ? T("EventCategory_Message_UpdateSuccessfully")
                    : T("EventCategory_Message_UpdateFailure"));
            }
            Mapper.CreateMap<EventCategoryManageModel, EventCategory>();
            eventCategory = Mapper.Map<EventCategoryManageModel, EventCategory>(model);

            response = Insert(eventCategory);

            return response.SetMessage(response.Success
                ? T("EventCategory_Message_CreateSuccessfully")
                : T("EventCategory_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete Event category by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteEventCategory(int id)
        {
            // Stop deletion if relation found
            var item = GetById(id);
            if (item != null)
            {
                if (item.EventEventCategories.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("EventCategory_Message_DeleteFailureBasedOnRelatedEvents")
                    };
                }

                // Delete the Event category
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("EventCategory_Message_DeleteSuccessfully")
                    : T("EventCategory_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("EventCategory_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get event category detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EventCategoryDetailModel GetEventCategoryDetailModel(int id)
        {
            var eventCategory = GetById(id);
            return eventCategory != null ? new EventCategoryDetailModel(eventCategory) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateEventCategoryData(XEditableModel model)
        {
            var eventCategory = GetById(model.Pk);
            if (eventCategory != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (EventCategoryManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new EventCategoryManageModel(eventCategory);
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

                    eventCategory.SetProperty(model.Name, value);
                    var response = Update(eventCategory);
                    return response.SetMessage(response.Success
                        ? T("EventCategory_Message_UpdateEventCategoryInfoSuccessfully")
                        : T("EventCategory_Message_UpdateEventCategoryInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("EventCategory_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("EventCategory_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}