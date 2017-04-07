using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.CompanyTypes;
using EzCMS.Core.Services.ContactGroups;
using EzCMS.Entity.Core.Enums;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Contacts
{
    public class ContactSearchDetailsModel
    {
        private readonly ICompanyTypeService _companyTypeService;
        private readonly IContactGroupService _contactGroupService;

        #region Constructor

        public ContactSearchDetailsModel()
        {
            _contactGroupService = HostContainer.GetInstance<IContactGroupService>();
            _companyTypeService = HostContainer.GetInstance<ICompanyTypeService>();
            CompanyTypes = new List<string>();
        }

        public ContactSearchDetailsModel(ContactSearchModel searchModel)
            : this()
        {
            ContactGroupId = searchModel.ContactGroupId;
            ContactId = searchModel.ContactId;
            Keyword = searchModel.Keyword;
            Companies = searchModel.Companies;
            EmailNotContains = searchModel.EmailNotContains;
            MarketingMaterial = searchModel.MarketingMaterial;
            Suburb = searchModel.Suburb;
            Country = searchModel.Country;
            CompanyTypes = _companyTypeService.GetCompanyTypeNames(searchModel.CompanyTypeIds).ToList();
            SalesPerson = searchModel.SalesPerson;

            State = searchModel.State;
            ReferredBy = searchModel.ReferredBy;
            CampaignCodes = searchModel.CampaignCodes;
            ProductOfInterests = searchModel.ProductOfInterests;
            Certifications = searchModel.Certifications;
            SubscriberTypes = searchModel.SubscriberTypes;
            InterestedInOwning = searchModel.InterestedInOwning;

            var contactGroup = _contactGroupService.GetById(ContactGroupId);
            if (contactGroup != null)
                ContactGroupName = contactGroup.Name;
        }

        #endregion

        #region Public Properties

        public int? ContactId { get; set; }

        [LocalizedDisplayName("Contact_Search_Keyword")]
        public string Keyword { get; set; }

        [LocalizedDisplayName("Contact_Search_EmailNotContains")]
        public string EmailNotContains { get; set; }

        [LocalizedDisplayName("Contact_Search_MarketingMaterial")]
        public ContactEnums.DontSendMarketing MarketingMaterial { get; set; }

        [LocalizedDisplayName("Contact_Search_Suburb")]
        public string Suburb { get; set; }

        [LocalizedDisplayName("Contact_Search_Country")]
        public string Country { get; set; }

        public List<string> CompanyTypes { get; set; }

        [LocalizedDisplayName("Contact_Search_Companies")]
        public List<string> Companies { get; set; }

        [LocalizedDisplayName("Contact_Search_SalesPerson")]
        public List<string> SalesPerson { get; set; }

        [LocalizedDisplayName("Contact_Search_State")]
        public string State { get; set; }

        [LocalizedDisplayName("Contact_Search_ReferredBy")]
        public List<string> ReferredBy { get; set; }

        [LocalizedDisplayName("Contact_Search_SubscriberTypes")]
        public List<string> SubscriberTypes { get; set; }

        [LocalizedDisplayName("Contact_Search_ProductOfInterests")]
        public List<string> ProductOfInterests { get; set; }

        [LocalizedDisplayName("Contact_Search_CampaignCodes")]
        public List<string> CampaignCodes { get; set; }

        [LocalizedDisplayName("Contact_Search_InterestedInOwning")]
        public ContactEnums.InterestInOwning InterestedInOwning { get; set; }

        [LocalizedDisplayName("Contact_Search_Certifications")]
        public List<string> Certifications { get; set; }

        public int? ContactGroupId { get; set; }

        public string ContactGroupName { get; set; }

        #endregion
    }
}
