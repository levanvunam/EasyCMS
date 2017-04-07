using Ez.Framework.Models;

namespace EzCMS.Core.Models.FileTemplates
{
    public class FileTemplateModel : BaseGridModel
    {
        public string Name { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Area { get; set; }

        public string Parameters { get; set; }

        public int? PageTemplateId { get; set; }

        public string PageTemplateName { get; set; }
    }
}
