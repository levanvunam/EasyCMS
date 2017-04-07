using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.RotatingImageGroups
{
    public class RotatingImageGroupManageModel
    {
        #region Constructors

        public RotatingImageGroupManageModel()
        {
            
        }

        public RotatingImageGroupManageModel(RotatingImageGroup group): this()
        {
            Id = group.Id;
            Name = group.Name;
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("RotatingImageGroup_Field_Id")]
        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("RotatingImageGroup_Field_Name")]
        public string Name { get; set; }

        #endregion
    }
}
