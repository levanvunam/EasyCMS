using System.Collections.Generic;

namespace EzCMS.Core.Models.ClientNavigations
{
    public class ClientNavigationRenderingModel
    {
        public string Url { get; set; }
        public string Name { get; set; }

        public List<ClientNavigationRenderingModel> ChildNavigations { get; set; } 
    }
}
