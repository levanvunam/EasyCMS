using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.BodyTemplates
{
    public class BodyTemplateSelectModel
    {
        public BodyTemplateSelectModel()
        {

        }

        public BodyTemplateSelectModel(BodyTemplate bodyTemplate)
            : this()
        {
            Id = bodyTemplate.Id;
            ImageUrl = bodyTemplate.ImageUrl;
            Name = bodyTemplate.Name;
            Description = bodyTemplate.Description;
        }

        #region Public Properties

        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        #endregion

    }
}
