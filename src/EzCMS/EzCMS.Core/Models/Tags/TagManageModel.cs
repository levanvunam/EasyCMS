using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Tags
{
    public class TagManageModel
    {

        #region Constructors

        public TagManageModel()
        {

        }

        public TagManageModel(Tag tag)
            : this()
        {
            Id = tag.Id;
            Name = tag.Name;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [StringLength(512)]
        [LocalizedDisplayName("Tag_Field_Name")]
        public string Name { get; set; }

        #endregion
    }
}
