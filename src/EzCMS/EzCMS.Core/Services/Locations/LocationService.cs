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
using EzCMS.Core.Models.Locations;
using EzCMS.Core.Models.Locations.Widgets;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.LocationLocationTypes;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Locations
{
    public class LocationService : ServiceHelper, ILocationService
    {
        private readonly ILocationLocationTypeRepository _locationLocationTypeRepository;
        private readonly IRepository<Location> _locationRepository;

        public LocationService(IRepository<Location> locationRepository,
            ILocationLocationTypeRepository locationLocationTypeRepository)
        {
            _locationRepository = locationRepository;
            _locationLocationTypeRepository = locationLocationTypeRepository;
        }

        /// <summary>
        /// Get locations
        /// </summary>
        /// <param name="locationIds"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetLocations(List<int> locationIds = null)
        {
            if (locationIds == null) locationIds = new List<int>();
            return GetAll().Select(g => new SelectListItem
            {
                Text = g.Name,
                Value = SqlFunctions.StringConvert((double) g.Id).Trim(),
                Selected = locationIds.Any(id => id == g.Id)
            });
        }

        #region Base

        public IQueryable<Location> GetAll()
        {
            return _locationRepository.GetAll();
        }

        public IQueryable<Location> Fetch(Expression<Func<Location, bool>> expression)
        {
            return _locationRepository.Fetch(expression);
        }

        public Location FetchFirst(Expression<Func<Location, bool>> expression)
        {
            return _locationRepository.FetchFirst(expression);
        }

        public Location GetById(object id)
        {
            return _locationRepository.GetById(id);
        }

        internal ResponseModel Insert(Location location)
        {
            return _locationRepository.Insert(location);
        }

        internal ResponseModel Update(Location location)
        {
            return _locationRepository.Update(location);
        }

        internal ResponseModel Delete(Location location)
        {
            return _locationRepository.Delete(location);
        }

        internal ResponseModel Delete(object id)
        {
            return _locationRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _locationRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the Locations.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchLocations(JqSearchIn si, LocationSearchModel model)
        {
            var data = SearchLocations(model);

            var locations = Maps(data);

            return si.Search(locations);
        }

        /// <summary>
        /// Export Locations
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, LocationSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchLocations(model);

            var locations = Maps(data);

            var exportData = si.Export(locations, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Get location search model
        /// </summary>
        /// <returns></returns>
        public LocationSearchModel GetLocationSearchModel()
        {
            return new LocationSearchModel();
        }

        #region Private Methods

        /// <summary>
        /// Search locations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<Location> SearchLocations(LocationSearchModel model)
        {
            if (model.LocationTypeIds == null) model.LocationTypeIds = new List<int>();

            return Fetch(location =>
                (string.IsNullOrEmpty(model.Keyword)
                 || (!string.IsNullOrEmpty(location.Name) && location.Name.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(location.AddressLine1) && location.AddressLine1.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(location.AddressLine2) && location.AddressLine2.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(location.State) && location.State.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(location.Suburb) && location.Suburb.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(location.Postcode) && location.Postcode.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(location.Country) && location.Country.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(location.ContactName) && location.ContactName.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(location.ContactTitle) && location.ContactTitle.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(location.Email) && location.Email.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(location.Phone) && location.Phone.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(location.Fax) && location.Fax.Contains(model.Keyword))
                 ||
                 (!string.IsNullOrEmpty(location.TrainingAffiliation) &&
                  location.TrainingAffiliation.Contains(model.Keyword)))
                &&
                (!model.LocationTypeIds.Any() ||
                 location.LocationLocationTypes.Any(
                     locationLocationType => model.LocationTypeIds.Contains(locationLocationType.LocationTypeId)))
                &&
                (!model.AssociateId.HasValue ||
                 location.AssociateLocations.Any(associateLocation => associateLocation.AssociateId == model.AssociateId)));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="locations"></param>
        /// <returns></returns>
        private IQueryable<LocationModel> Maps(IQueryable<Location> locations)
        {
            return locations.Select(location => new LocationModel
            {
                Id = location.Id,
                Name = location.Name,
                ContactName = location.ContactName,
                ContactTitle = location.ContactTitle,
                AddressLine1 = location.AddressLine1,
                AddressLine2 = location.AddressLine2,
                Suburb = location.Suburb,
                State = location.State,
                Postcode = location.Postcode,
                Country = location.Country,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Phone = location.Phone,
                Fax = location.Fax,
                Email = location.Email,
                TrainingAffiliation = location.TrainingAffiliation,
                PinImage = location.PinImage,
                OpeningHoursWeekdays = location.OpeningHoursWeekdays,
                OpeningHoursSaturday = location.OpeningHoursSaturday,
                OpeningHoursSunday = location.OpeningHoursSunday,
                TimeZone = location.TimeZone,
                Types = location.LocationLocationTypes.Select(l => l.LocationType.Name),
                RecordOrder = location.RecordOrder,
                Created = location.Created,
                CreatedBy = location.CreatedBy,
                LastUpdate = location.LastUpdate,
                LastUpdateBy = location.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get Location manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <param name="locationTypeId"></param>
        /// <returns></returns>
        public LocationManageModel GetLocationManageModel(int? id = null, int? locationTypeId = null)
        {
            var location = GetById(id);
            if (location != null)
            {
                return new LocationManageModel(location);
            }

            return new LocationManageModel(locationTypeId);
        }

        /// <summary>
        /// Save Location
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveLocation(LocationManageModel model)
        {
            ResponseModel response;
            var location = GetById(model.Id);

            if (location != null)
            {
                location.Name = model.Name;
                location.ContactName = model.ContactName;
                location.ContactTitle = model.ContactTitle;
                location.AddressLine1 = model.AddressLine1;
                location.AddressLine2 = model.AddressLine2;
                location.Suburb = model.Suburb;
                location.State = model.State;
                location.Postcode = model.Postcode;
                location.Country = model.Country;
                location.Latitude = model.Latitude;
                location.Longitude = model.Longitude;
                location.Phone = model.Phone;
                location.Fax = model.Fax;
                location.Email = model.Email;
                location.TrainingAffiliation = model.TrainingAffiliation;
                location.PinImage = model.PinImage;
                location.OpeningHoursWeekdays = model.OpeningHoursWeekdays;
                location.OpeningHoursSaturday = model.OpeningHoursSaturday;
                location.OpeningHoursSunday = model.OpeningHoursSunday;
                location.TimeZone = model.TimeZone;

                #region Location Types

                var currentTypes = location.LocationLocationTypes.Select(nc => nc.LocationTypeId).ToList();

                if (model.LocationTypeIds == null) model.LocationTypeIds = new List<int>();

                // Remove reference to deleted types
                var removedTypeIds = currentTypes.Where(id => !model.LocationTypeIds.Contains(id));
                _locationLocationTypeRepository.DeleteByLocationTypeId(location.Id, removedTypeIds);

                // Add new reference to types
                var addedTypeIds = model.LocationTypeIds.Where(id => !currentTypes.Contains(id));
                _locationLocationTypeRepository.Insert(location.Id, addedTypeIds);

                #endregion

                response = Update(location);
                return response.SetMessage(response.Success
                    ? T("Location_Message_UpdateSuccessfully")
                    : T("Location_Message_UpdateFailure"));
            }
            Mapper.CreateMap<LocationManageModel, Location>();
            location = Mapper.Map<LocationManageModel, Location>(model);
            response = Insert(location);

            #region Location Types

            if (model.LocationTypeIds == null) model.LocationTypeIds = new List<int>();

            _locationLocationTypeRepository.Insert(location.Id, model.LocationTypeIds);

            #endregion

            return response.SetMessage(response.Success
                ? T("Location_Message_CreateSuccessfully")
                : T("Location_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete Location
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteLocation(int id)
        {
            var location = GetById(id);
            if (location != null)
            {
                if (location.LocationLocationTypes.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Location_Message_DeleteFailureBasedOnRelatedLocationTypes")
                    };
                }

                if (location.AssociateLocations.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Location_Message_DeleteFailureBasedOnRelatedAssociates")
                    };
                }

                var response = Delete(location);
                return response.SetMessage(response.Success
                    ? T("Location_Message_DeleteSuccessfully")
                    : T("Location_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("Location_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get Location detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LocationDetailModel GetLocationDetailModel(int id)
        {
            var item = GetById(id);
            return item != null ? new LocationDetailModel(item) : null;
        }

        /// <summary>
        /// Update Location data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateLocationData(XEditableModel model)
        {
            var location = GetById(model.Pk);
            if (location != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (LocationManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new LocationManageModel(location);
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

                    location.SetProperty(model.Name, value);

                    var response = Update(location);
                    return response.SetMessage(response.Success
                        ? T("Location_Message_UpdateLocationInfoSuccessfully")
                        : T("Location_Message_UpdateLocationInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Location_Message_PropertyNotFound")
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("Location_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Widget

        /// <summary>
        /// Get location widget
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LocationWidget GetLocationWidget(int id)
        {
            var location = GetById(id);

            if (location == null)
                return null;

            return new LocationWidget(location);
        }

        /// <summary>
        /// Get locations by selected types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public List<LocationWidget> GetLocationsByTypes(List<int> types)
        {
            /* Get list of locations have selected types
             * If no specific type, get all locations
             */
            var locations = _locationRepository.GetAll();
            if (types != null && types.Any())
            {
                locations = locations.Where(location => location.LocationLocationTypes.Any(
                    locationLocationType => types.Contains(locationLocationType.LocationTypeId)));
            }

            if (locations == null)
                return null;

            var locationWidgets = new List<LocationWidget>();
            foreach (var location in locations)
            {
                locationWidgets.Add(new LocationWidget(location));
            }

            return locationWidgets;
        }

        #endregion
    }
}