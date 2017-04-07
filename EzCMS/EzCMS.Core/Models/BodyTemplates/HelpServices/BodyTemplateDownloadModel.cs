using System.Collections.Generic;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.BodyTemplates.HelpServices
{
    public class BodyTemplateDownloadModel
    {
        public BodyTemplateDownloadModel()
        {
            Files = new List<string>();
        }

        public BodyTemplateDownloadModel(BodyTemplate bodyTemplate):this()
        {
            Id = bodyTemplate.Id;
            ImageUrl = bodyTemplate.ImageUrl.ToAbsoluteUrl();
            Name = bodyTemplate.Name;
            Description = bodyTemplate.Description;
            Content = bodyTemplate.Content;
        }

        #region Public Properties

        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public List<string> Files { get; set; }

        #endregion
    }
}
