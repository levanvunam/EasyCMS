using System;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Associates.Widgets
{
    public class AssociateWidget
    {
        public AssociateWidget()
        {

        }

        public AssociateWidget(Associate associate)
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
            Sex = associate.Gender;
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
        }

        #region Public Properties

        public int Id { get; set; }

        public string Title { get; set; }

        public string JobTitle { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        #region Address

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string Suburb { get; set; }

        public string State { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }

        #endregion

        #region Phone

        public string PhoneWork { get; set; }

        public string PhoneHome { get; set; }

        public string MobilePhone { get; set; }

        public string Fax { get; set; }

        #endregion

        public string Company { get; set; }

        public string Sex { get; set; }

        public string Photo { get; set; }

        public string University { get; set; }

        public string Qualification { get; set; }

        public string OtherQualification { get; set; }

        public string Achievements { get; set; }

        public string Memberships { get; set; }

        public string Appointments { get; set; }

        public string PersonalInterests { get; set; }

        public string ProfessionalInterests { get; set; }

        public string Positions { get; set; }

        public bool IsNew { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        #endregion
    }
}
