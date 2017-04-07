using System.Collections.Generic;

namespace EzCMS.Core.Models.Pages.Widgets.Member
{
    public class BreadcrumbsWidget
    {
        #region Public Properties

        public List<MemberItem> Breadcrumbs { get; set; }

        public MemberItem CurrentBreadcrumb { get; set; }

        #endregion
    }
}
