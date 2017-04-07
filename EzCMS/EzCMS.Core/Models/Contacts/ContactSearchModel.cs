using Ez.Framework.Core.Attributes;
using EzCMS.Core.Services.CompanyTypes;
using EzCMS.Core.Services.ContactCommunications;
using EzCMS.Core.Services.ContactGroups;
using EzCMS.Core.Services.Contacts;
using EzCMS.Entity.Core.Enums;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Contacts
{
    public class ContactSearchModel
    {
        #region Constructors

        public ContactSearchModel()
        {
            var contactService = HostContainer.GetInstance<IContactService>();
            var contactCommunityService = HostContainer.GetInstance<IContactCommunicationService>();
            var contactGroupService = HostContainer.GetInstance<IContactGroupService>();

            var companyTypeService = HostContainer.GetInstance<ICompanyTypeService>();

            MarketingMaterials = EnumUtilities.GenerateSelectListItems<ContactEnums.DontSendMarketing>(GenerateEnumType.IntValueAndDescriptionText);
            InterestedInOwningList = EnumUtilities.GenerateSelectListItems<ContactEnums.InterestInOwning>(GenerateEnumType.IntValueAndDescriptionText);
            Newsletters = EnumUtilities.GenerateSelectListItems<ContactEnums.NewsletterSubscribed>(GenerateEnumType.IntValueAndDescriptionText);

            var contactSearchData = contactService.GetAll().Select(c => new
            {
                c.Country,
                c.Company,
                c.Sex
            }).ToList();

            // Get countries
            CountryList = contactSearchData.GroupBy(c => c.Country).Distinct()
                .Where(c => !string.IsNullOrWhiteSpace(c.Key))
                .Select(c => new SelectListItem
                {
                    Text = c.Key,
                    Value = c.Key
                });

            // Get companies
            Companies = new List<string>();
            CompanyList = contactSearchData.GroupBy(c => c.Company).Distinct()
                .Where(c => !string.IsNullOrWhiteSpace(c.Key))
                .Select(c => new SelectListItem
                {
                    Text = c.Key,
                    Value = c.Key
                });

            // Get company types
            CompanyTypeIds = new List<int>();
            CompanyTypes = companyTypeService.GetAll().ToList().Select(companyType => new SelectListItem
            {
                Text = companyType.Name,
                Value = companyType.Id + ""
            });

            // Get sales person
            SalesPerson = new List<string>();
            SalesPersonList = contactService.Fetch(c => !string.IsNullOrEmpty(c.SalesPerson))
                .OrderBy(c => c.SalesPerson)
                .Distinct()
                .Select(c => new SelectListItem
                {
                    Text = c.SalesPerson,
                    Value = c.SalesPerson
                });

            // Get sex list
            Sex = new List<string>();
            SexList = contactSearchData.GroupBy(c => c.Sex).Distinct()
                .Where(c => !string.IsNullOrWhiteSpace(c.Key))
                .Select(c => new SelectListItem
                {
                    Text = c.Key,
                    Value = c.Key
                });

            // Get contact groups
            ContactGroups = contactGroupService.GetAll()
                .Select(cg => new SelectListItem
                {
                    Text = cg.Name,
                    Value = SqlFunctions.StringConvert((double)cg.Id).Trim()
                });

            var communityData = contactCommunityService.GetAll().Select(c => new
            {
                c.Id,
                c.ReferredBy,
                c.SubscriberType,
                c.ProductOfInterest,
                c.CampaignCode,
                c.CurrentlyOwn,
                c.Certification,
                c.InterestedInOwning
            }).ToList();

            // Get referred by list
            ReferredBy = new List<string>();
            ReferredByList = communityData.GroupBy(c => c.ReferredBy).Distinct()
                .Where(c => !string.IsNullOrWhiteSpace(c.Key))
                .Select(c => new SelectListItem
                {
                    Text = c.Key,
                    Value = c.Key
                });

            // Get subscribe types
            SubscriberTypes = new List<string>();
            SubscriberTypeList = communityData.GroupBy(c => c.SubscriberType).Distinct()
                .Where(c => !string.IsNullOrWhiteSpace(c.Key))
                .Select(c => new SelectListItem
                {
                    Text = c.Key,
                    Value = c.Key
                });

            // Get product of interest
            ProductOfInterests = new List<string>();
            ProductOfInterestList = communityData.GroupBy(c => c.ProductOfInterest)
                .Where(c => !string.IsNullOrWhiteSpace(c.Key))
                .Distinct().Select(c => new SelectListItem
                {
                    Text = c.Key,
                    Value = c.Key
                });

            // Get campaign codes
            CampaignCodes = new List<string>();
            CampaignCodeList = communityData.GroupBy(c => c.CampaignCode).Distinct()
                .Where(c => !string.IsNullOrWhiteSpace(c.Key))
                .Select(c => new SelectListItem
                {
                    Text = c.Key,
                    Value = c.Key
                });

            // Get currently own list
            CurrentlyOwns = new List<string>();
            CurrentlyOwnList = communityData.GroupBy(c => c.CurrentlyOwn).Distinct()
                .Where(c => !string.IsNullOrWhiteSpace(c.Key))
                .Select(c => new SelectListItem
                {
                    Text = c.Key,
                    Value = c.Key
                });

            // Get certifications
            Certifications = new List<string>();
            CertificationList = communityData.GroupBy(c => c.Certification).Distinct()
                .Where(c => !string.IsNullOrWhiteSpace(c.Key))
                .Select(c => new SelectListItem
                {
                    Text = c.Key,
                    Value = c.Key
                });
        }

        #endregion

        #region Public Properties

        public int? NotificationId { get; set; }

        [LocalizedDisplayName("Contact_Search_ContactId")]
        public int? ContactId { get; set; }

        [LocalizedDisplayName("Contact_Search_Keyword")]
        public string Keyword { get; set; }

        [LocalizedDisplayName("Contact_Search_EmailNotContains")]
        public string EmailNotContains { get; set; }

        [LocalizedDisplayName("Contact_Search_MarketingMaterial")]
        public ContactEnums.DontSendMarketing MarketingMaterial { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> MarketingMaterials { get; set; }

        [LocalizedDisplayName("Contact_Search_Suburb")]
        public string Suburb { get; set; }

        [LocalizedDisplayName("Contact_Search_Country")]
        public string Country { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> CountryList { get; set; }

        [LocalizedDisplayName("Contact_Search_CompanyTypeIds")]
        public List<int> CompanyTypeIds { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> CompanyTypes { get; set; }

        [LocalizedDisplayName("Contact_Search_Companies")]
        public List<string> Companies { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> CompanyList { get; set; }

        [LocalizedDisplayName("Contact_Search_SalesPerson")]
        public List<string> SalesPerson { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> SalesPersonList { get; set; }

        [LocalizedDisplayName("Contact_Search_Gender")]
        public List<string> Sex { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> SexList { get; set; }

        [LocalizedDisplayName("Contact_Search_Newsletter")]
        public ContactEnums.NewsletterSubscribed Newsletter { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> Newsletters { get; set; }

        [LocalizedDisplayName("Contact_Search_State")]
        public string State { get; set; }

        [LocalizedDisplayName("Contact_Search_ReferredBy")]
        public List<string> ReferredBy { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> ReferredByList { get; set; }

        [LocalizedDisplayName("Contact_Search_SubscriberTypes")]
        public List<string> SubscriberTypes { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> SubscriberTypeList { get; set; }

        [LocalizedDisplayName("Contact_Search_ProductOfInterests")]
        public List<string> ProductOfInterests { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> ProductOfInterestList { get; set; }

        [LocalizedDisplayName("Contact_Search_CampaignCodes")]
        public List<string> CampaignCodes { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> CampaignCodeList { get; set; }

        [LocalizedDisplayName("Contact_Search_InterestedInOwning")]
        public ContactEnums.InterestInOwning InterestedInOwning { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> InterestedInOwningList { get; set; }

        [LocalizedDisplayName("Contact_Search_CurrentlyOwns")]
        public List<string> CurrentlyOwns { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> CurrentlyOwnList { get; set; }

        [LocalizedDisplayName("Contact_Search_Certifications")]
        public List<string> Certifications { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> CertificationList { get; set; }

        [LocalizedDisplayName("Contact_Search_ContactGroupId")]
        public int? ContactGroupId { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> ContactGroups { get; set; }

        #endregion
    }
}
