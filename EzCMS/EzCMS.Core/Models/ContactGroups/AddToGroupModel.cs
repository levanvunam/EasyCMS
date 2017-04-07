using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Services.ContactGroups;
using EzCMS.Core.Services.Localizes;
using EzCMS.Entity.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.ContactGroups
{
    public class AddToGroupModel : IValidatableObject
    {
        private readonly IContactGroupService _contactGroupService;

        #region Constructors

        public AddToGroupModel()
        {
            _contactGroupService = HostContainer.GetInstance<IContactGroupService>();

            // Get ids
            Ids = _contactGroupService.GetActiveContactGroups();

            AddToGroupType = ContactGroupEnums.AddToGroupType.New;
            AddToGroupTypes = EnumUtilities.GetAllItems<ContactGroupEnums.AddToGroupType>();
        }

        public AddToGroupModel(ContactSearchModel contactSearchModel)
            : this()
        {
            ContactSearchModel = SerializeUtilities.Serialize(contactSearchModel);
        }

        #endregion

        #region Public Properties

        [RequiredIf("Name", null)]
        [LocalizedDisplayName("ContactGroup_Field_ContactGroup")]
        public int? Id { get; set; }

        public IEnumerable<SelectListItem> Ids { get; set; }

        [RequiredIf("Id", null)]
        [LocalizedDisplayName("ContactGroup_Field_Name")]
        public string Name { get; set; }

        [Required]
        [LocalizedDisplayName("ContactGroup_Field_AddToGroupType")]
        public ContactGroupEnums.AddToGroupType AddToGroupType { get; set; }

        public IEnumerable<ContactGroupEnums.AddToGroupType> AddToGroupTypes { get; set; }

        [Required]
        public string ContactSearchModel { get; set; }

        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (_contactGroupService.IsNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("ContactGroup_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}
