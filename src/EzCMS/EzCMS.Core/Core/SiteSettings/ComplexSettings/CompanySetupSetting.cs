using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class CompanySetupSetting : ComplexSetting
    {
        #region Setup

        /// <summary>
        /// Get complex setting setup
        /// </summary>
        /// <returns></returns>
        public override ComplexSettingSetup GetSetup()
        {
            return new ComplexSettingSetup
            {
                Name = SettingNames.CompanySetupSetting,
                SettingType = "system",
                DefaultValue = new CompanySetupSetting()
            };
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_SiteTitle")]
        public string SiteTitle { get; set; }

        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_Keywords")]
        public string Keywords { get; set; }

        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_CompanyName")]
        public string CompanyName { get; set; }

        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_AddressLine1")]
        public string AddressLine1 { get; set; }

        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_AddressLine2")]
        public string AddressLine2 { get; set; }

        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_Suburb")]
        public string Suburb { get; set; }

        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_State")]
        public string State { get; set; }

        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_Postcode")]
        public string Postcode { get; set; }

        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_Country")]
        public string Country { get; set; }

        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_Phone1")]
        public string Phone1 { get; set; }

        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_Phone2")]
        public string Phone2 { get; set; }

        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_Phone3")]
        public string Phone3 { get; set; }

        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_Fax")]
        public string Fax { get; set; }

        [EmailAddress]
        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_Email1")]
        public string Email1 { get; set; }

        [EmailAddress]
        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_Email2")]
        public string Email2 { get; set; }

        [Url]
        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_Website")]
        public string Website { get; set; }

        [LocalizedDisplayName("SiteSetting_CompanySetup_Field_DealerLicense")]
        public string DealerLicense { get; set; }

        #endregion
    }
}
