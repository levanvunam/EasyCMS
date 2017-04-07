using System.Collections.Generic;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Toolbars
{
    public class ToolbarRenderModel
    {
        public ToolbarRenderModel()
        {
            BasicTools = new List<string>();
            PageTools = new List<string>();
        }

        public ToolbarRenderModel(Toolbar toolbar):this()
        {
            BasicTools = SerializeUtilities.Deserialize<List<string>>(toolbar.BasicToolbar);
            PageTools = SerializeUtilities.Deserialize<List<string>>(toolbar.PageToolbar);
        }

        #region Public Properties

        public List<string> BasicTools { get; set; }

        public List<string> PageTools { get; set; }

        #endregion
    }
}
