using Ez.Framework.Core.Attributes;
using EzCMS.Core.Services.ContactGroups;
using EzCMS.Core.Services.Localizes;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.ContactGroups
{
    public class ContactGroupManageModel : IValidatableObject
    {
        #region Constructors

        public ContactGroupManageModel()
        {
        }

        public ContactGroupManageModel(ContactGroup contactGroup)
            : this()
        {
            Id = contactGroup.Id;
            Name = contactGroup.Name;
            Queries = contactGroup.Queries;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("ContactGroup_Field_Name")]
        public string Name { get; set; }

        [Required]
        public string Queries { get; set; }

        [Required]
        [LocalizedDisplayName("ContactGroup_Field_Active")]
        public bool Active { get; set; }

        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var contactGroupService = HostContainer.GetInstance<IContactGroupService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (contactGroupService.IsNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("ContactGroup_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}
