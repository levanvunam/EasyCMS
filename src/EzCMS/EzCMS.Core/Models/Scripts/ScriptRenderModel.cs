using System;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Scripts
{
    public class ScriptRenderModel
    {
        public ScriptRenderModel()
        {

        }

        public ScriptRenderModel(Script script)
            : this()
        {
            Id = script.Id;
            Name = script.Name;
            Content = script.Content;
            ContentData = StringUtilities.GetBytes(Content);
            LastUpdate = script.LastUpdate ?? script.Created;
        }

        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public byte[] ContentData { get; set; }

        public DateTime LastUpdate { get; set; }

        #endregion
    }
}
