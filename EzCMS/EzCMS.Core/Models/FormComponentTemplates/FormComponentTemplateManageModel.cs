using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.FormComponentTemplates
{
    public class FormComponentTemplateManageModel
    {
        #region Constructors

        public FormComponentTemplateManageModel()
        {
        }

        public FormComponentTemplateManageModel(FormComponentTemplate formComponentTemplate)
            : this()
        {
            Id = formComponentTemplate.Id;
            Name = formComponentTemplate.Name;
            Content = formComponentTemplate.Content;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [StringLength(60)]
        [LocalizedDisplayName("FormComponentTemplate_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("FormComponentTemplate_Field_Content")]
        public string Content { get; set; }

        #endregion
    }
}
