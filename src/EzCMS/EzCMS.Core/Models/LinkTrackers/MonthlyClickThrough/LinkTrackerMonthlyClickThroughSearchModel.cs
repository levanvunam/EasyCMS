using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.LinkTrackerClicks;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.LinkTrackers.MonthlyClickThrough
{
    public class LinkTrackerMonthlyClickThroughSearchModel
    {
        #region Constructors

        public LinkTrackerMonthlyClickThroughSearchModel()
        {
            var linkTrackerClickService = HostContainer.GetInstance<ILinkTrackerClickService>();
            Years = linkTrackerClickService.GetYears();

            var selected = Years.FirstOrDefault(i => i.Selected);
            if (selected != null)
            {
                Year = selected.Value.ToInt();
            }
        }

        #endregion

        #region Public Properties

        public string Keyword { get; set; }

        public int? Year { get; set; }

        public IEnumerable<SelectListItem> Years { get; set; }

        #endregion
    }
}
