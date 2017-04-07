using Ez.Framework.Models;

namespace EzCMS.Core.Models.PageTemplates
{
    public class PageTemplateModel : BaseGridModel
    {
        #region Public Properties

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public string ParentName { get; set; }

        public bool IsDefaultTemplate { get; set; }

        public bool IsValid { get; set; }

        public string CompileMessage { get; set; }

        #endregion
    }
}
