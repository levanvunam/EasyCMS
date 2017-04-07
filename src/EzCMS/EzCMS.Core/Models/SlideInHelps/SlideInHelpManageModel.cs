using Ez.Framework.Core.Attributes;
using EzCMS.Entity.Entities.Models;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.SlideInHelps
{
    public class SlideInHelpManageModel
    {
        public SlideInHelpManageModel()
        {

        }

        public SlideInHelpManageModel(int? languageId)
            : this()
        {
            if (languageId.HasValue)
                LanguageId = languageId.Value;
        }

        public SlideInHelpManageModel(SlideInHelp slideInHelp)
            : this()
        {
            Id = slideInHelp.Id;
            TextKey = slideInHelp.TextKey;
            Title = slideInHelp.HelpTitle;
            Content = slideInHelp.LocalHelpContent;

            LanguageId = slideInHelp.LanguageId;
        }

        #region Public Properties

        public int? Id { get; set; }

        public int LanguageId { get; set; }

        [LocalizedDisplayName("SlideInHelp_Field_TextKey")]
        public string TextKey { get; set; }

        [LocalizedDisplayName("SlideInHelp_Field_Title")]
        public string Title { get; set; }

        [Required]
        [LocalizedDisplayName("SlideInHelp_Field_Content")]
        public string Content { get; set; }

        [LocalizedDisplayName("SlideInHelp_Field_Active")]
        public bool Active { get; set; }

        #endregion
    }
}
