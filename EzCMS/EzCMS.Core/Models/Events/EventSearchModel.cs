using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.EventCategories;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Events
{
    public class EventSearchModel
    {
        #region Constructors

        public EventSearchModel()
        {
            var eventCategoryService = HostContainer.GetInstance<IEventCategoryService>();
            EventCategories = eventCategoryService.GetEventCategories();
        }

        #endregion

        #region Public Properties

        public string Keyword { get; set; }

        public int? EventCategoryId { get; set; }

        public IEnumerable<SelectListItem> EventCategories { get; set; }

        #endregion
    }
}
