using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.Styles;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Widgets.WidgetBuilders
{
    public class WidgetPreviewModel
    {
        public WidgetPreviewModel()
        {
            var styleService = HostContainer.GetInstance<IStyleService>();

            IncludedStyles = styleService.GetIncludedStyles();
        }

        public WidgetPreviewModel(string widget)
            : this()
        {
            Widget = widget;
        }

        #region Public Properties

        public string Widget { get; set; }

        public List<string> IncludedStyles { get; set; }

        #endregion
    }
}
