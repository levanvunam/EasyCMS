using System.Collections.Generic;

namespace EzCMS.Core.Models.ClientNavigations.Widgets
{
    public class NavigationWidget
    {
        public int Id { get; set; }

        public int? PageId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public int? Order { get; set; }

        public int Level { get; set; }

        public List<int> ViewableGroupIds { get; set; } 

        public List<int> EditableGroupIds { get; set; } 

        public List<NavigationWidget> Children { get; set; }
    }
}
