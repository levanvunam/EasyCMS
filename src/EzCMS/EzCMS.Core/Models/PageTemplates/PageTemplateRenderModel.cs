using System;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.PageTemplates
{
    public class PageTemplateRenderModel
    {
        public PageTemplateRenderModel()
        {

        }

        public PageTemplateRenderModel(PageTemplate template)
            : this()
        {
            Name = template.Name;
            Content = template.Content;
            LastUpdate = template.LastUpdate;
            Created = template.Created;
        }

        #region Public Properties

        public string Name { get; set; }

        public string Content { get; set; }

        public DateTime? LastUpdate { get; set; }

        public DateTime Created { get; set; }

        #endregion
    }
}
