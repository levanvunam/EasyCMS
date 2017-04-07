using System.Collections.Generic;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class PageTemplate : BaseHierachyModel<PageTemplate>
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public bool IsDefaultTemplate { get; set; }

        public string CacheName { get; set; }

        public bool IsValid { get; set; }

        public string CompileMessage { get; set; }

        public virtual ICollection<FileTemplate> FileTemplates { get; set; }

        public virtual ICollection<Page> Pages { get; set; }

        public virtual ICollection<PageTemplateLog> PageTemplateLogs { get; set; }

        public virtual ICollection<PageTemplate> ChildTemplates { get; set; }
    }
}