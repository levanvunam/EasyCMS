using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Services.Widgets;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Models.Widgets.WidgetBuilders
{
    public class SelectWidgetModel
    {
        public SelectWidgetModel()
        {
            var widgetService = HostContainer.GetInstance<IWidgetService>();

            Types = EnumUtilities.GetAllItems<WidgetType>();
            Widgets = widgetService.GetAllWidgets();
        }

        #region Public Properties

        public string CallBack { get; set; }

        public bool IsCkEditor { get; set; }

        public IEnumerable<WidgetType> Types { get; set; }

        public IEnumerable<WidgetSetupModel> Widgets { get; set; }

        #endregion
    }
}
