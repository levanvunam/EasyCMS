using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Core.Enums;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Services.AssociateTypes;
using EzCMS.Core.Services.CompanyTypes;
using EzCMS.Core.Services.Countries;
using EzCMS.Core.Services.Locations;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Associates
{
    public class AssociateManageModel
    {
        #region Constructors

        private readonly IAssociateTypeService _associateTypeService;
        private readonly ICompanyTypeService _companyTypeService;
        private readonly ILocationService _locationService;
        public AssociateManageModel()
        {
            _associateTypeService = HostContainer.GetInstance<IAssociateTypeService>();
            _companyTypeService = HostContainer.GetInstance<ICompanyTypeService>();
            _locationService = HostContainer.GetInstance<ILocationService>();
            var countryService = HostContainer.GetInstance<ICountryService>();

            AssociateTypeIds = new List<int>();
            AssociateTypes = _associateTypeService.GetAssociateTypes();

            LocationIds = new List<int>();
            Locations = _locationService.GetLocations();

            CompanyTypeIds = new List<int>();
            CompanyTypes = _companyTypeService.GetCompanyTypes();

            Countries = countryService.GetCountries();
            States = EnumUtilities.GenerateSelectListItems<CommonEnums.AustraliaState>(GenerateEnumType.DescriptionValueAndDescriptionText);
            Genders = EnumUtilities.GenerateSelectListItems<UserEnums.Gender>(GenerateEnumType.DescriptionValueAndDescriptionText);
        }

        public AssociateManageModel(int? associateTypeId)
            : this()
        {
            if (associateTypeId.HasValue)
            {
                AssociateTypeIds = new List<int>
                {
                    associateTypeId.Value
                };

                AssociateTypes = _associateTypeService.GetAssociateTypes(AssociateTypeIds);
            }
        }

        public AssociateManageModel(Associate associate)
            : this()
        {
            Id = associate.Id;

            Title = associate.Title;
            JobTitle = associate.JobTitle;
            FirstName = associate.FirstName;
            LastName = associate.LastName;
            Email = associate.Email;
            AddressLine1 = associate.AddressLine1;
            AddressLine2 = associate.AddressLine2;
            Suburb = associate.Suburb;
            State = associate.State;
            Postcode = associate.Postcode;
            Country = associate.Country;
            PhoneWork = associate.PhoneWork;
            PhoneHome = associate.PhoneHome;
            MobilePhone = associate.MobilePhone;
            Fax = associate.Fax;
            Company = associate.Company;
            Gender = associate.Gender;
            Photo = associate.Photo;
            University = associate.University;
            Qualification = associate.Qualification;
            OtherQualification = associate.OtherQualification;
            Achievements = associate.Achievements;
            Memberships = associate.Memberships;
            Appointments = associate.Appointments;
            PersonalInterests = associate.PersonalInterests;
            ProfessionalInterests = associate.ProfessionalInterests;
            Positions = associate.Positions;
            IsNew = associate.IsNew;
            DateStart = associate.DateStart;
            DateEnd = associate.DateEnd;

            AssociateTypeIds = associate.AssociateAssociateTypes.Select(a => a.AssociateTypeId).ToList();
            AssociateTypes = _associateTypeService.GetAssociateTypes(AssociateTypeIds);

            LocationIds = associate.AssociateLocations.Select(a => a.LocationId).ToList();
            Locations = _locationService.GetLocations(LocationIds);

            CompanyTypeIds = associate.AssociateCompanyTypes.Select(a => a.CompanyTypeId).ToList();
            CompanyTypes = _companyTypeService.GetCompanyTypes(CompanyTypeIds);
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Associate_Field_Title")]
        public string Title { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Associate_Field_JobTitle")]
        public string JobTitle { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Associate_Field_FirstName")]
        public string FirstName { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Associate_Field_LastName")]
        public string LastName { get; set; }

        [EmailValidation]
        [StringLength(255)]
        [LocalizedDisplayName("Associate_Field_Email")]
        public string Email { get; set; }

        #region Address

        [StringLength(255)]
        [LocalizedDisplayName("Associate_Field_AddressLine1")]
        public string AddressLine1 { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Associate_Field_AddressLine2")]
        public string AddressLine2 { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Associate_Field_Suburb")]
        public string Suburb { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Associate_Field_State")]
        public string State { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Associate_Field_Postcode")]
        public string Postcode { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Associate_Field_Country")]
        public string Country { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        #endregion

        #region Phone

        [StringLength(50)]
        [LocalizedDisplayName("Associate_Field_PhoneWork")]
        public string PhoneWork { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Associate_Field_PhoneHome")]
        public string PhoneHome { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Associate_Field_MobilePhone")]
        public string MobilePhone { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Associate_Field_Fax")]
        public string Fax { get; set; }

        #endregion

        [StringLength(512)]
        [LocalizedDisplayName("Associate_Field_Company")]
        public string Company { get; set; }

        public IEnumerable<SelectListItem> Companies { get; set; }

        [StringLength(20)]
        [LocalizedDisplayName("Associate_Field_Gender")]
        public string Gender { get; set; }

        public IEnumerable<SelectListItem> Genders { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Associate_Field_Photo")]
        public string Photo { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Associate_Field_University")]
        public string University { get; set; }

        [LocalizedDisplayName("Associate_Field_Qualification")]
        public string Qualification { get; set; }

        [LocalizedDisplayName("Associate_Field_OtherQualification")]
        public string OtherQualification { get; set; }

        [LocalizedDisplayName("Associate_Field_Achievements")]
        public string Achievements { get; set; }

        [LocalizedDisplayName("Associate_Field_Memberships")]
        public string Memberships { get; set; }

        [LocalizedDisplayName("Associate_Field_Appointments")]
        public string Appointments { get; set; }

        [LocalizedDisplayName("Associate_Field_PersonalInterests")]
        public string PersonalInterests { get; set; }

        [LocalizedDisplayName("Associate_Field_ProfessionalInterests")]
        public string ProfessionalInterests { get; set; }

        [LocalizedDisplayName("Associate_Field_Positions")]
        public string Positions { get; set; }

        [LocalizedDisplayName("Associate_Field_IsNew")]
        public bool IsNew { get; set; }

        [LocalizedDisplayName("Associate_Field_DateStart")]
        public DateTime? DateStart { get; set; }

        [DateGreaterThan("DateStart")]
        [LocalizedDisplayName("Associate_Field_DateEnd")]
        public DateTime? DateEnd { get; set; }

        [LocalizedDisplayName("Associate_Field_AssociateTypeIds")]
        public List<int> AssociateTypeIds { get; set; }

        public IEnumerable<SelectListItem> AssociateTypes { get; set; }

        [LocalizedDisplayName("Associate_Field_LocationIds")]
        public List<int> LocationIds { get; set; }

        public IEnumerable<SelectListItem> Locations { get; set; }

        [LocalizedDisplayName("Associate_Field_CompanyTypeIds")]
        public List<int> CompanyTypeIds { get; set; }

        public IEnumerable<SelectListItem> CompanyTypes { get; set; }

        #endregion
    }
}
