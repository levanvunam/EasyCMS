using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.SocialMedia
{
    public class SocialMediaDetailModel
    {
        #region Constructors

        public SocialMediaDetailModel()
        {
        }

        public SocialMediaDetailModel(Entity.Entities.Models.SocialMedia socialMedia)
            : this()
        {
            Id = socialMedia.Id;

            SocialMedia = new SocialMediaManageModel(socialMedia);

            Created = socialMedia.Created;
            CreatedBy = socialMedia.CreatedBy;
            LastUpdate = socialMedia.LastUpdate;
            LastUpdateBy = socialMedia.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public SocialMediaManageModel SocialMedia { get; set; }

        [LocalizedDisplayName("SocialMedia_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("SocialMedia_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("SocialMedia_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("SocialMedia_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        #endregion
    }
}
