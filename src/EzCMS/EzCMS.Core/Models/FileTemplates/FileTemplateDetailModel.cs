using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.FileTemplates
{
    public class FileTemplateDetailModel
    {
        public FileTemplateDetailModel()
        {
        }

        public FileTemplateDetailModel(FileTemplate fileTemplate)
            : this()
        {
            Id = fileTemplate.Id;

            FileTemplate = new FileTemplateManageModel(fileTemplate);

            Created = fileTemplate.Created;
            CreatedBy = fileTemplate.CreatedBy;
            LastUpdate = fileTemplate.LastUpdate;
            LastUpdateBy = fileTemplate.LastUpdateBy;

        }

        #region Public Properties
        public int? Id { get; set; }

        [LocalizedDisplayName("FileTemplate_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("FileTemplate_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("FileTemplate_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("FileTemplate_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public FileTemplateManageModel FileTemplate { get; set; }

        #endregion
    }
}
