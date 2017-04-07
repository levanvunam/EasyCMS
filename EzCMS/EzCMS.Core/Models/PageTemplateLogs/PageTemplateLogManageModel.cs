using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.PageTemplateLogs
{
    public class PageTemplateLogManageModel
    {
        #region Constructors

        public PageTemplateLogManageModel()
        {
            
        }

        public PageTemplateLogManageModel(PageTemplate pageTemplate)
        {
            PageTemplateId = pageTemplate.Id;
            Name = pageTemplate.Name;
            Content = pageTemplate.Content;
            ParentId = pageTemplate.ParentId;
        }
        #endregion

        #region Public Properties
        public int Id { get; set; }

        public int PageTemplateId { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public int? ParentId { get; set; }

        #endregion
    }
}
