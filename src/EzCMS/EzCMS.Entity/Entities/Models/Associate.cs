using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Associate : BaseModel
    {
        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(255)]
        public string JobTitle { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(512)]
        public string Company { get; set; }

        [StringLength(20)]
        public string Gender { get; set; }

        [StringLength(255)]
        public string Photo { get; set; }

        [StringLength(255)]
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

        public virtual ICollection<AssociateAssociateType> AssociateAssociateTypes { get; set; }

        public virtual ICollection<AssociateCompanyType> AssociateCompanyTypes { get; set; }

        public virtual ICollection<AssociateLocation> AssociateLocations { get; set; }

        #region Address

        [StringLength(255)]
        public string AddressLine1 { get; set; }

        [StringLength(255)]
        public string AddressLine2 { get; set; }

        [StringLength(100)]
        public string Suburb { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(50)]
        public string Postcode { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        #endregion

        #region Phone

        [StringLength(50)]
        public string PhoneWork { get; set; }

        [StringLength(50)]
        public string PhoneHome { get; set; }

        [StringLength(50)]
        public string MobilePhone { get; set; }

        [StringLength(50)]
        public string Fax { get; set; }

        #endregion
    }
}