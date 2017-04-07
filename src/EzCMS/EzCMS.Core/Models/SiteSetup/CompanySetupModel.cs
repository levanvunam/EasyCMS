using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Ez.Framework.Core.Enums;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.SiteSetup
{
    public class CompanySetupModel
    {
        public CompanySetupModel()
        {
            States = EnumUtilities.GenerateSelectListItems<CommonEnums.AustraliaState>();
        }

        #region Public Properties

        public string SiteTitle { get; set; }

        public string Keywords { get; set; }

        public string CompanyName { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string Suburb { get; set; }

        public string State { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> States { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }

        public string Phone1 { get; set; }

        public string Phone2 { get; set; }

        public string Phone3 { get; set; }

        public string Fax { get; set; }

        [EmailAddress]
        public string Email1 { get; set; }

        [EmailAddress]
        public string Email2 { get; set; }

        [Url]
        public string Website { get; set; }

        public string DealerLicense { get; set; }

        #endregion
    }
}