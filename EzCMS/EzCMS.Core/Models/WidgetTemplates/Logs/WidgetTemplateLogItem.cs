using EzCMS.Entity.Entities.Models;
using System;

namespace EzCMS.Core.Models.WidgetTemplates.Logs
{
    public class WidgetTemplateLogItem
    {
        #region Constructors
        public WidgetTemplateLogItem()
        {
        }

        public WidgetTemplateLogItem(WidgetTemplateLog model)
            : this()
        {
            Id = model.Id;
            Name = model.Name;
            SessionId = model.SessionId;
            ChangeLog = model.ChangeLog;
            Created = model.Created;
        }
        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string SessionId { get; set; }

        public string Name { get; set; }

        public string ChangeLog { get; set; }

        public DateTime Created { get; set; }

        #endregion
    }
}