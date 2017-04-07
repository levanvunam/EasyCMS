using System;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Scripts.Logs
{
    public class ScriptLogItem
    {
        #region Constructors
        public ScriptLogItem()
        {
        }

        public ScriptLogItem(ScriptLog model)
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