using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.Widgets;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EzCMS.Core.Models.WidgetTemplates
{
    public class WidgetTemplateSearchModel
    {
        public WidgetTemplateSearchModel()
        {
            var widgetsService = HostContainer.GetInstance<IWidgetService>();

            Widgets = widgetsService.GetAllWidgets().Select(c => new SelectListItem
            {
                Text = string.Format("{{{0}}}", c.Widget),
                Value = c.Widget
            });
        }

        #region Public Properties

        public string Keyword { get; set; }

        public string Widget { get; set; }

        public IEnumerable<SelectListItem> Widgets { get; set; }

        #endregion
    }
}
