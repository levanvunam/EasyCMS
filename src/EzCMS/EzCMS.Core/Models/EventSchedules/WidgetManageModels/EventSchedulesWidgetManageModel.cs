using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.EventCategories;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.EventSchedules.WidgetManageModels
{
    public class EventSchedulesWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public EventSchedulesWidgetManageModel()
        {
        }

        public EventSchedulesWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var eventCategoryService = HostContainer.GetInstance<IEventCategoryService>();
            Categories = eventCategoryService.GetEventCategories();

            ParseParams(parameters);
        }

        #region Private Methods

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * CategoryId
             * * Total
             * * DateFormat
             * * TimeFormat
             * * Template 
             */

            //CategoryId
            if (parameters.Length > 1)
            {
                CategoryId = parameters[1].ToInt(0);
            }

            //Total
            if (parameters.Length > 2)
            {
                Total = parameters[2].ToInt(0);
            }

            //DateFormat
            if (parameters.Length > 3)
            {
                DateFormat = parameters[3];
            }

            //TimeFormat
            if (parameters.Length > 4)
            {
                TimeFormat = parameters[4];
            }

            //Template 
            if (parameters.Length > 5)
            {
                Template = parameters[5];
            }
        }

        #endregion

        #endregion

        #region Public Properties

        [LocalizedDisplayName("Widget_EventSchedules_Field_CategoryId")]
        public int? CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        [LocalizedDisplayName("Widget_EventSchedules_Field_Total")]
        public int Total { get; set; }

        [LocalizedDisplayName("Widget_EventSchedules_Field_DateFormat")]
        public string DateFormat { get; set; }

        [LocalizedDisplayName("Widget_EventSchedules_Field_TimeFormat")]
        public string TimeFormat { get; set; }

        public IEnumerable<SelectListItem> EventScheduless { get; set; }

        #endregion
    }
}
