using System.Collections.Generic;

namespace EzCMS.Core.Models.ClientNavigations
{
    public class ClientBreadCrumbModel
    {
        public ClientBreadCrumbModel()
        {
            BreadCrumbs = new List<ClientBreadCrumbItem>();
            CurrentBreadCrumbItem = new ClientBreadCrumbItem();
        }

        public List<ClientBreadCrumbItem> BreadCrumbs { get; set; }

        public ClientBreadCrumbItem CurrentBreadCrumbItem { get; set; }
    }

    public class ClientBreadCrumbItem
    {
        public string Url { get; set; }
        public string Name { get;set; }
    }
}
