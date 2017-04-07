using System.Collections.Generic;

namespace EzCMS.Core.Models.Navigations
{
    public class BreadcrumbModel
    {
        public BreadcrumbModel()
        {
            Breadcrumbs = new List<Breadcrumb>();
            CurrentBreadcrumb = new Breadcrumb();
        }

        public List<Breadcrumb> Breadcrumbs { get; set; }

        public Breadcrumb CurrentBreadcrumb { get; set; }
    }
}
