using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.Widgets;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Widgets.WidgetBuilders
{
    public class WidgetDropdownModel
    {
        public WidgetDropdownModel()
        {
            var widgetService = HostContainer.GetInstance<IWidgetService>();

            Widgets = widgetService.GetFavouriteWidgets();
        }

        public WidgetDropdownModel(string callback)
            : this()
        {
            CallBack = callback;
        }

        #region Public Properties

        public string CallBack { get; set; }

        public IEnumerable<WidgetSetupModel> Widgets { get; set; }

        #endregion
    }
}
