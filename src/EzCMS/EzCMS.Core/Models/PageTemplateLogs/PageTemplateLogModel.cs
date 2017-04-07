using Ez.Framework.Models;

namespace EzCMS.Core.Models.PageTemplateLogs
{
    public class PageTemplateLogModel : BaseGridModel
    {
        public int PageTemplateId { get; set; }

        public string Name { get; set; }
        
        public int? ParentId { get; set; }
    }
}
