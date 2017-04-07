using Ez.Framework.Models;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.ContactGroups
{
    public class ContactGroupModel : BaseGridModel
    {
        #region Constructors

        public ContactGroupModel()
        {
        }

        public ContactGroupModel(ContactGroup contactGroup)
            : this()
        {
            Id = contactGroup.Id;
            Name = contactGroup.Name;
            Queries = contactGroup.Queries;
            ContactCount = contactGroup.ContactGroupContacts.Count;

            RecordOrder = contactGroup.RecordOrder;
            Created = contactGroup.Created;
            CreatedBy = contactGroup.CreatedBy;
            LastUpdate = contactGroup.LastUpdate;
            LastUpdateBy = contactGroup.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public string Name { get; set; }

        public string Queries { get; set; }

        public int ContactCount { get; set; }

        #endregion
    }
}
