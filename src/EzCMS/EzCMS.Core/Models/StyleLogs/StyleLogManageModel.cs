using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.StyleLogs
{
    public class StyleLogManageModel
    {
        #region Constructors

        public StyleLogManageModel()
        {

        }

        public StyleLogManageModel(Style style)
            : this()
        {
            StyleId = style.Id;
            Name = style.Name;
            CdnUrl = style.CdnUrl;
            Content = style.Content;
            IncludeIntoEditor = style.IncludeIntoEditor;
        }
        #endregion

        #region Public Properties
        public int Id { get; set; }

        public int StyleId { get; set; }

        public string Name { get; set; }

        public string CdnUrl { get; set; }

        public string Content { get; set; }

        public bool IncludeIntoEditor { get; set; }

        #endregion
    }
}
