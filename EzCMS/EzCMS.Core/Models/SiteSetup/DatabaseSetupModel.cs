using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Utilities;
using EzCMS.Entity.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EzCMS.Core.Models.SiteSetup
{
    public class DatabaseSetupModel
    {
        public DatabaseSetupModel()
        {
            DatabaseTypes = EnumUtilities.GenerateSelectListItems<DatabaseType>(GenerateEnumType.IntValueAndDescriptionText);
        }

        #region Public Properties

        public IEnumerable<SelectListItem> DatabaseTypes { get; set; }

        [Required]
        [DisplayName("Server")]
        public string Server { get; set; }

        [Required]
        [DisplayName("Database Name")]
        public string DatabaseName { get; set; }

        [RequiredIf("IntegratedSecurity", false)]
        [DisplayName("Username")]
        public string UserName { get; set; }

        [RequiredIf("IntegratedSecurity", false)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Integrated Security")]
        public bool IntegratedSecurity { get; set; }
        #endregion
    }
}