using System;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.PageTemplates.Logs
{
    public class PageTemplateLogItem
    {
        #region Constructors
        public PageTemplateLogItem()
        {
        }


        public PageTemplateLogItem(PageTemplateLog model): this()
        {
            Id = model.Id;
            Name = model.Name;
            ChangeLog = model.ChangeLog;
            Created = model.Created;
            ParentId = model.ParentId;
        }
        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public string ChangeLog { get; set; }

        public int? ParentId { get; set; }

        public DateTime Created { get; set; }

        #endregion
    }
}
