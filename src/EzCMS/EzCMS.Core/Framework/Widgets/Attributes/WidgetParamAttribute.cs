using System;

namespace EzCMS.Core.Framework.Widgets.Attributes
{
    public class WidgetParamAttribute : Attribute
    {
        #region Public Properties

        public string Name { get; set; }

        public int Order { get; set; }

        public string Description { get; set; }

        #endregion
    }
}
