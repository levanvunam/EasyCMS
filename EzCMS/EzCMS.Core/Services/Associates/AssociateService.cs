using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using EzCMS.Core.Models.Associates;
using EzCMS.Core.Models.Associates.Widgets;
using EzCMS.Core.Services.Companies;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.AssociateAssociateTypes;
using EzCMS.Entity.Repositories.AssociateCompanyTypes;
using EzCMS.Entity.Repositories.AssociateLocations;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Associates
{
    public class AssociateService : ServiceHelper, IAssociateService
    {
        private readonly IAssociateAssociateTypeRepository _associateAssociateTypeRepository;
        private readonly IAssociateCompanyTypeRepository _associateCompanyTypeRepository;
        private readonly IAssociateLocationRepository _associateLocationRepository;
        private readonly IRepository<Associate> _associateRepository;
        private readonly IRepository<AssociateType> _associateTypeRepository;
        private readonly ICompanyService _companyService;

        public AssociateService(IRepository<Associate> associateRepository,
            ICompanyService companyService,
            IAssociateAssociateTypeRepository associateAssociateTypeRepository,
            IAssociateLocationRepository associateLocationRepository,
            IAssociateCompanyTypeRepository associateCompanyTypeRepository,
            IRepository<AssociateType> associateTypeRepository)
        {
            _associateRepository = associateRepository;
            _companyService = companyService;
            _associateAssociateTypeRepository = associateAssociateTypeRepository;
            _associateLocationRepository = associateLocationRepository;
            _associateCompanyTypeRepository = associateCompanyTypeRepository;
            _associateTypeRepository = associateTypeRepository;
        }

        #region Widgets

        /// <summary>
        /// Load associate listing
        /// </summary>
        /// <param name="associateTypeId"></param>
        /// <param name="locationId"></param>
        /// <param name="companyTypeId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public AssociateListingWidget GetAssociateListingWidget(int? associateTypeId, int? locationId,
            int? companyTypeId,
            AssociateEnums.AssociateStatus status)
        {
            var now = DateTime.UtcNow;

            var data =
                Fetch(
                    associate =>
                        (!associateTypeId.HasValue ||
                         associate.AssociateAssociateTypes.Any(aat => aat.AssociateTypeId == associateTypeId))
                        &&
                        (!locationId.HasValue || associate.AssociateLocations.Any(aat => aat.LocationId == locationId))
                        &&
                        (!companyTypeId.HasValue ||
                         associate.AssociateCompanyTypes.Any(aat => aat.CompanyTypeId == companyTypeId))
                        && (!associate.DateStart.HasValue || associate.DateStart <= now)
                        && (!associate.DateEnd.HasValue || associate.DateEnd >= now));

            if (status == AssociateEnums.AssociateStatus.New)
            {
                data = data.Where(associate => associate.IsNew);
            }
            else if (status == AssociateEnums.AssociateStatus.NotNew)
            {
                data = data.Where(associate => !associate.IsNew);
            }

            var associates = data.ToList();
            var associateIds = associates.Select(associate => associate.Id).ToList();

            var associateTypes =
                _associateTypeRepository.Fetch(
                    associateType =>
                        associateType.AssociateAssociateTypes.Any(aat => associateIds.Contains(aat.AssociateId))
                        && (!associateTypeId.HasValue || associateType.Id == associateTypeId))
                    .OrderBy(associateType => associateType.RecordOrder).ToList();

            return new AssociateListingWidget
            {
                AssociateTypes = associateTypes.Select(associateType => new AssociateTypeWidget
                {
                    Id = associateType.Id,
                    Name = associateType.Name,
                    Associates =
                        associates.Where(
                            associate =>
                                associate.AssociateAssociateTypes.Any(aat => aat.AssociateTypeId == associateType.Id))
                            .ToList()
                            .Select(associate => new AssociateWidget(associate))
                            .ToList()
                }).ToList(),
                Associates = associates.Select(associate => new AssociateWidget(associate)).ToList()
            };
        }

        #endregion

        #region Base

        public IQueryable<Associate> GetAll()
        {
            return _associateRepository.GetAll();
        }

        public IQueryable<Associate> Fetch(Expression<Func<Associate, bool>> expression)
        {
            return _associateRepository.Fetch(expression);
        }

        public Associate FetchFirst(Expression<Func<Associate, bool>> expression)
        {
            return _associateRepository.FetchFirst(expression);
        }

        public Associate GetById(object id)
        {
            return _associateRepository.GetById(id);
        }

        internal ResponseModel Insert(Associate associate)
        {
            return _associateRepository.Insert(associate);
        }

        internal ResponseModel Update(Associate associate)
        {
            return _associateRepository.Update(associate);
        }

        internal ResponseModel Delete(Associate associate)
        {
            return _associateRepository.Delete(associate);
        }

        internal ResponseModel Delete(object id)
        {
            return _associateRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _associateRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the Associates.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchAssociates(JqSearchIn si, AssociateSearchModel model)
        {
            var data = SearchAssociates(model);

            var associates = Maps(data);

            return si.Search(associates);
        }

        /// <summary>
        /// Export Associates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, AssociateSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchAssociates(model);

            var associates = Maps(data);

            var exportData = si.Export(associates, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Get associate search model
        /// </summary>
        /// <returns></returns>
        public AssociateSearchModel GetAssociateSearchModel()
        {
            return new AssociateSearchModel();
        }

        #region Private

        /// <summary>
        /// Search associates
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<Associate> SearchAssociates(AssociateSearchModel model)
        {
            if (model.AssociateTypeIds == null) model.AssociateTypeIds = new List<int>();
            if (model.CompanyTypeIds == null) model.CompanyTypeIds = new List<int>();
            if (model.LocationIds == null) model.LocationIds = new List<int>();

            return Fetch(associate =>
                (string.IsNullOrEmpty(model.Keyword)
                 || (!string.IsNullOrEmpty(associate.FirstName) && associate.FirstName.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.LastName) && associate.LastName.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.Email) && associate.Email.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.AddressLine1) && associate.AddressLine1.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.AddressLine2) && associate.AddressLine2.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.State) && associate.State.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.Suburb) && associate.Suburb.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.Postcode) && associate.Postcode.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.Country) && associate.Country.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.PhoneWork) && associate.PhoneWork.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.PhoneHome) && associate.PhoneHome.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.PhoneHome) && associate.PhoneHome.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.MobilePhone) && associate.MobilePhone.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.Fax) && associate.Fax.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.Gender) && associate.Gender.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.Company) && associate.Company.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.Photo) && associate.Photo.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.University) && associate.University.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.Qualification) && associate.Qualification.Contains(model.Keyword))
                 ||
                 (!string.IsNullOrEmpty(associate.OtherQualification) &&
                  associate.OtherQualification.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.Achievements) && associate.Achievements.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.Memberships) && associate.Memberships.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.Appointments) && associate.Appointments.Contains(model.Keyword))
                 ||
                 (!string.IsNullOrEmpty(associate.PersonalInterests) &&
                  associate.PersonalInterests.Contains(model.Keyword))
                 ||
                 (!string.IsNullOrEmpty(associate.ProfessionalInterests) &&
                  associate.ProfessionalInterests.Contains(model.Keyword))
                 || (!string.IsNullOrEmpty(associate.Positions) && associate.Positions.Contains(model.Keyword)))
                &&
                (!model.AssociateTypeIds.Any() ||
                 associate.AssociateAssociateTypes.Any(t => model.AssociateTypeIds.Contains(t.AssociateTypeId)))
                &&
                (!model.LocationIds.Any() ||
                 associate.AssociateLocations.Any(t => model.LocationIds.Contains(t.LocationId)))
                &&
                (!model.CompanyTypeIds.Any() ||
                 associate.AssociateCompanyTypes.Any(t => model.CompanyTypeIds.Contains(t.CompanyTypeId))));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="associates"></param>
        /// <returns></returns>
        private IQueryable<AssociateModel> Maps(IQueryable<Associate> associates)
        {
            return associates.Select(associate => new AssociateModel
            {
                Id = associate.Id,
                FirstName = associate.FirstName,
                LastName = associate.LastName,
                Email = associate.Email,
                Title = associate.Title,
                JobTitle = associate.JobTitle,
                AddressLine1 = associate.AddressLine1,
                AddressLine2 = associate.AddressLine2,
                Suburb = associate.Suburb,
                State = associate.State,
                Postcode = associate.Postcode,
                Country = associate.Country,
                PhoneWork = associate.PhoneWork,
                PhoneHome = associate.PhoneHome,
                MobilePhone = associate.MobilePhone,
                Fax = associate.Fax,
                RecordOrder = associate.RecordOrder,
                Created = associate.Created,
                CreatedBy = associate.CreatedBy,
                LastUpdate = associate.LastUpdate,
                LastUpdateBy = associate.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get Associate manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <param name="associateTypeId"></param>
        /// <returns></returns>
        public AssociateManageModel GetAssociateManageModel(int? id = null, int? associateTypeId = null)
        {
            var associate = GetById(id);
            if (associate != null)
            {
                return new AssociateManageModel(associate);
            }

            return new AssociateManageModel(associateTypeId);
        }

        /// <summary>
        /// Save Associate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveAssociate(AssociateManageModel model)
        {
            ResponseModel response;

            var associate = GetById(model.Id);
            if (associate != null)
            {
                associate.Title = model.Title;
                associate.JobTitle = model.JobTitle;
                associate.FirstName = model.FirstName;
                associate.LastName = model.LastName;
                associate.Email = model.Email;
                associate.AddressLine1 = model.AddressLine1;
                associate.AddressLine2 = model.AddressLine2;
                associate.Suburb = model.Suburb;
                associate.State = model.State;
                associate.Postcode = model.Postcode;
                associate.Country = model.Country;
                associate.PhoneWork = model.PhoneWork;
                associate.PhoneHome = model.PhoneHome;
                associate.MobilePhone = model.MobilePhone;
                associate.Fax = model.Fax;
                associate.Gender = model.Gender;
                associate.Photo = model.Photo;
                associate.University = model.University;
                associate.Qualification = model.Qualification;
                associate.OtherQualification = model.OtherQualification;
                associate.Achievements = model.Achievements;
                associate.Memberships = model.Memberships;
                associate.Appointments = model.Appointments;
                associate.PersonalInterests = model.PersonalInterests;
                associate.ProfessionalInterests = model.ProfessionalInterests;
                associate.Positions = model.Positions;
                associate.IsNew = model.IsNew;
                associate.DateStart = model.DateStart;
                associate.DateEnd = model.DateEnd;

                associate.Company = _companyService.SaveCompany(model.Company);

                #region Associate Types

                var currentAssociateTypes = associate.AssociateAssociateTypes.Select(nc => nc.AssociateTypeId).ToList();

                if (model.AssociateTypeIds == null) model.AssociateTypeIds = new List<int>();

                // Remove reference to deleted types
                var removedAssociateTypeIds = currentAssociateTypes.Where(id => !model.AssociateTypeIds.Contains(id));
                _associateAssociateTypeRepository.DeleteByAssociateTypeId(associate.Id, removedAssociateTypeIds);

                // Add new reference to types
                var addedAssociateTypeIds = model.AssociateTypeIds.Where(id => !currentAssociateTypes.Contains(id));
                _associateAssociateTypeRepository.InsertByAssociateId(associate.Id, addedAssociateTypeIds);

                #endregion

                #region Company Types

                var currentCompanyTypes = associate.AssociateCompanyTypes.Select(nc => nc.CompanyTypeId).ToList();

                if (model.CompanyTypeIds == null) model.CompanyTypeIds = new List<int>();

                // Remove reference to deleted types
                var removedCompanyTypeIds = currentCompanyTypes.Where(id => !model.CompanyTypeIds.Contains(id));
                _associateCompanyTypeRepository.DeleteByAssociateId(associate.Id, removedCompanyTypeIds);

                // Add new reference to types
                var addedCompanyTypeIds = model.CompanyTypeIds.Where(id => !currentCompanyTypes.Contains(id));
                _associateCompanyTypeRepository.InsertByAssociateId(associate.Id, addedCompanyTypeIds);

                #endregion

                #region Locations

                var currentLocations = associate.AssociateLocations.Select(nc => nc.LocationId).ToList();

                if (model.LocationIds == null) model.LocationIds = new List<int>();

                // Remove reference to deleted locations
                var removedLocationIds = currentLocations.Where(id => !model.LocationIds.Contains(id));
                _associateLocationRepository.DeleteByAssociateId(associate.Id, removedLocationIds);

                // Add new reference to locations
                var addedLocationIds = model.LocationIds.Where(id => !currentLocations.Contains(id));
                _associateLocationRepository.InsertByAssociateId(associate.Id, addedLocationIds);

                #endregion

                response = Update(associate);
                return response.SetMessage(response.Success
                    ? T("Associate_Message_UpdateSuccessfully")
                    : T("Associate_Message_UpdateFailure"));
            }
            Mapper.CreateMap<AssociateManageModel, Associate>();
            associate = Mapper.Map<AssociateManageModel, Associate>(model);

            associate.Company = _companyService.SaveCompany(model.Company);

            response = Insert(associate);

            #region Associate Types

            if (model.AssociateTypeIds == null) model.AssociateTypeIds = new List<int>();

            _associateAssociateTypeRepository.InsertByAssociateId(associate.Id, model.AssociateTypeIds);

            #endregion

            #region Company Types

            if (model.CompanyTypeIds == null) model.CompanyTypeIds = new List<int>();

            _associateCompanyTypeRepository.InsertByAssociateId(associate.Id, model.CompanyTypeIds);

            #endregion

            #region Locations

            if (model.LocationIds == null) model.LocationIds = new List<int>();

            _associateLocationRepository.InsertByAssociateId(associate.Id, model.LocationIds);

            #endregion

            return response.SetMessage(response.Success
                ? T("Associate_Message_CreateSuccessfully")
                : T("Associate_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete Associate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteAssociate(int id)
        {
            var associate = GetById(id);
            if (associate != null)
            {
                if (associate.AssociateAssociateTypes.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Associate_Message_DeleteFailureBasedOnRelatedAssociateTypes")
                    };
                }

                if (associate.AssociateCompanyTypes.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Associate_Message_DeleteFailureBasedOnRelatedCompanyTypes")
                    };
                }

                if (associate.AssociateLocations.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Associate_Message_DeleteFailureBasedOnRelatedLocations")
                    };
                }

                var response = Delete(associate);
                return
                    response.SetMessage(response.Success
                        ? T("Associate_Message_DeleteSuccessfully")
                        : T("Associate_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("Associate_Message_DeleteSuccessfully")
            };
        }

        /// <summary>
        /// Delete mapping between associate and location
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        public ResponseModel DeleteAssociateLocationMapping(int locationId, int associateId)
        {
            var response = _associateLocationRepository.Delete(locationId, associateId);

            return response.SetMessage(response.Success
                ? T("AssociateLocation_Message_DeleteMappingSuccessfully")
                : T("AssociateLocation_Message_DeleteMappingFailure"));
        }

        #endregion

        #region Details

        /// <summary>
        /// Get Associate detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AssociateDetailModel GetAssociateDetailModel(int id)
        {
            var item = GetById(id);
            return item != null ? new AssociateDetailModel(item) : null;
        }

        /// <summary>
        /// Update Associate data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateAssociateData(XEditableModel model)
        {
            var associate = GetById(model.Pk);
            if (associate != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (AssociateManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    // Generate property value
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new AssociateManageModel(associate);
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

                    associate.SetProperty(model.Name, value);

                    var response = Update(associate);
                    return response.SetMessage(response.Success
                        ? T("Associate_Message_UpdateAssociateInfoSuccessfully")
                        : T("Associate_Message_UpdateAssociateInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Associate_Message_PropertyNotFound")
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("Associate_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}