using System;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Styles
{
    public class StyleRenderModel
    {
        public StyleRenderModel()
        {

        }

        public StyleRenderModel(Style style)
            : this()
        {
            Id = style.Id;
            Name = style.Name;
            Content = style.Content;
            ContentData = StringUtilities.GetBytes(Content);
            LastUpdate = style.LastUpdate ?? style.Created;
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
