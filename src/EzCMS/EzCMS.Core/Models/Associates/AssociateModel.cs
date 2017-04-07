using Ez.Framework.Models;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Associates
{
    public class AssociateModel : BaseGridModel
    {
        #region Constructors

        public AssociateModel()
        {
        }

        public AssociateModel(Associate associate)
            : base(associate)
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
        }

        #endregion

        #region Public Properties

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

        #endregion
    }
}
