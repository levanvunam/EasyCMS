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
using EzCMS.Core.Models.LocationTypes;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.LocationLocationTypes;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.LocationTypes
{
    public class LocationTypeService : ServiceHelper, ILocationTypeService
    {
        private readonly ILocationLocationTypeRepository _locationLocationTypeRepository;
        private readonly IRepository<LocationType> _locationTypeRepository;

        public LocationTypeService(IRepository<LocationType> locationTypeRepository,
            ILocationLocationTypeRepository locationLocationTypeRepository)
        {
            _locationTypeRepository = locationTypeRepository;
            _locationLocationTypeRepository = locationLocationTypeRepository;
        }

        #region Validation

        /// <summary>
        /// Check if location type exists
        /// </summary>
        /// <param name="locationTypeId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsLocationTypeExisted(int? locationTypeId, string name)
        {
            return Fetch(u => u.Name.Equals(name) && u.Id != locationTypeId).Any();
        }

        #endregion

        /// <summary>
        /// Get list of location type
        /// </summary>
        /// <param name="locationTypeIds"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetLocationTypes(List<int> locationTypeIds = null)
        {
            if (locationTypeIds == null) locationTypeIds = new List<int>();
            return GetAll().Select(g => new SelectListItem
            {
                Text = g.Name,
                Value = SqlFunctions.StringConvert((double) g.Id).Trim(),
                Selected = locationTypeIds.Any(id => id == g.Id)
            });
        }

        /// <summary>
        /// Delete location - location type mapping
        /// </summary>
        /// <param name="locationTypeId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public ResponseModel DeleteLocationLocationTypeMapping(int locationTypeId, int locationId)
        {
            var response = _locationLocationTypeRepository.Delete(locationTypeId, locationId);

            return response.SetMessage(response.Success
                ? T("LocationLocationType_Message_DeleteMappingSuccessfully")
                : T("LocationLocationType_Message_DeleteMappingFailure"));
        }

        #region Base

        public IQueryable<LocationType> GetAll()
        {
            return _locationTypeRepository.GetAll();
        }

        public IQueryable<LocationType> Fetch(Expression<Func<LocationType, bool>> expression)
        {
            return _locationTypeRepository.Fetch(expression);
        }

        public LocationType FetchFirst(Expression<Func<LocationType, bool>> expression)
        {
            return _locationTypeRepository.FetchFirst(expression);
        }

        public LocationType GetById(object id)
        {
            return _locationTypeRepository.GetById(id);
        }

        internal ResponseModel Insert(LocationType locationType)
        {
            return _locationTypeRepository.Insert(locationType);
        }

        internal ResponseModel Update(LocationType locationType)
        {
            return _locationTypeRepository.Update(locationType);
        }

        internal ResponseModel Delete(LocationType locationType)
        {
            return _locationTypeRepository.Delete(locationType);
        }

        internal ResponseModel Delete(object id)
        {
            return _locationTypeRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _locationTypeRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search location types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchLocationTypes(JqSearchIn si, int? locationId)
        {
            var data = SearchLocationTypesByLocation(locationId);

            var locationTypes = Maps(data);

            return si.Search(locationTypes);
        }

        /// <summary>
        /// Export location types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? locationId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchLocationTypesByLocation(locationId);

            var locationTypes = Maps(data);

            var exportData = si.Export(locationTypes, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search location types
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        private IQueryable<LocationType> SearchLocationTypesByLocation(int? locationId)
        {
            return
                Fetch(
                    locationType =>
                        !locationId.HasValue ||
                        locationType.LocationLocationTypes.Any(act => act.LocationId == locationId));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="locationType"></param>
        /// <returns></returns>
        private IQueryable<LocationTypeModel> Maps(IQueryable<LocationType> locationType)
        {
            return locationType.Select(m => new LocationTypeModel
            {
                Id = m.Id,
                Name = m.Name,
                PinImage = m.PinImage,
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
        /// Get location type manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LocationTypeManageModel GetLocationTypeManageModel(int? id = null)
        {
            var locationType = GetById(id);
            if (locationType != null)
            {
                return new LocationTypeManageModel(locationType);
            }
            return new LocationTypeManageModel();
        }

        /// <summary>
        /// Save location type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveLocationType(LocationTypeManageModel model)
        {
            ResponseModel response;
            var locationType = GetById(model.Id);
            if (locationType != null)
            {
                locationType.Name = model.Name;
                locationType.PinImage = model.PinImage;
                locationType.RecordOrder = model.RecordOrder;
                response = Update(locationType);

                return response.SetMessage(response.Success
                    ? T("LocationType_Message_UpdateSuccessfully")
                    : T("LocationType_Message_UpdateFailure"));
            }
            Mapper.CreateMap<LocationTypeManageModel, LocationType>();
            locationType = Mapper.Map<LocationTypeManageModel, LocationType>(model);

            response = Insert(locationType);

            return response.SetMessage(response.Success
                ? T("LocationType_Message_CreateSuccessfully")
                : T("LocationType_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete location type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteLocationType(int id)
        {
            // Stop deletion if relation found
            var locationType = GetById(id);
            if (locationType != null)
            {
                if (locationType.LocationLocationTypes.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("LocationType_Message_DeleteFailureBasedOnRelatedLocations")
                    };
                }

                // Delete the location type
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("LocationType_Message_DeleteSuccessfully")
                    : T("LocationType_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("LocationType_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get location type detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LocationTypeDetailModel GetLocationTypeDetailModel(int id)
        {
            var locationType = GetById(id);
            return locationType != null ? new LocationTypeDetailModel(locationType) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateLocationTypeData(XEditableModel model)
        {
            var locationType = GetById(model.Pk);
            if (locationType != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (LocationTypeManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new LocationTypeManageModel(locationType);
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

                    locationType.SetProperty(model.Name, value);
                    var response = Update(locationType);
                    return response.SetMessage(response.Success
                        ? T("LocationType_Message_UpdateLocationTypeInfoSuccessfully")
                        : T("LocationType_Message_UpdateLocationTypeInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("LocationType_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("LocationType_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}