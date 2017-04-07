using RazorEngine.Templating;

namespace EzCMS.Core.Framework.Mvc.Models
{
    public class TemplateCache
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public ResolveType Type { get; set; }
    }
}