using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.ScriptLogs
{
    public class ScriptLogManageModel
    {
        #region Constructors

        public ScriptLogManageModel()
        {
            
        }

        public ScriptLogManageModel(Script script)
        {
            ScriptId = script.Id;
            Name = script.Name;
            Content = script.Content;
        }
        #endregion

        #region Public Properties
        public int Id { get; set; }

        public int ScriptId { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        #endregion
    }
}
