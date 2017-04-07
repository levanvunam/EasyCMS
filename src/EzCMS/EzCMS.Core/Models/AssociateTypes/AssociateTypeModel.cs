using Ez.Framework.Models;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.AssociateTypes
{
    public class AssociateTypeModel : BaseGridModel
    {
        public AssociateTypeModel()
        {

        }

        public AssociateTypeModel(AssociateType associateType)
            : base(associateType)
        {
            Name = associateType.Name;
        }

        #region Public Properties

        public string Name { get; set; }

        #endregion
    }
}
