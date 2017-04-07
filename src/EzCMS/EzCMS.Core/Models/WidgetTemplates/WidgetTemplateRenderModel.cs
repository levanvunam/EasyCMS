using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.WidgetTemplates
{
    public class WidgetTemplateRenderModel
    {
        public WidgetTemplateRenderModel(WidgetTemplate widgetTemplate)
        {
            Id = widgetTemplate.Id;
            Name = widgetTemplate.Name;
            Content = widgetTemplate.FullContent;
        }

        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public string CacheName
        {
            get { return Name.GetTemplateCacheName(Content); }
        }

        public string Content { get; set; }

        #endregion
    }
}
