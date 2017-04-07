using Ez.Framework.Models;

namespace EzCMS.Core.Models.Toolbars
{
    public class ToolbarModel : BaseGridModel
    {
        #region Public Properties

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        #endregion
    }
}
