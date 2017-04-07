using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.SocialMedia
{
    public class SocialMediaManageModel
    {
        #region Constructors

        public SocialMediaManageModel()
        {
        }

        public SocialMediaManageModel(Entity.Entities.Models.SocialMedia socialMedia)
            : this()
        {
            Id = socialMedia.Id;
            Name = socialMedia.Name;
            MaxCharacter = socialMedia.MaxCharacter;
            RecordOrder = socialMedia.RecordOrder;
        }

        #endregion

        #region Public Properties
        public int Id { get; set; }

        [Required]
        [LocalizedDisplayName("SocialMedia_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("SocialMedia_Field_MaxCharacter")]
        public int MaxCharacter { get; set; }

        [LocalizedDisplayName("SocialMedia_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion
    }
}
