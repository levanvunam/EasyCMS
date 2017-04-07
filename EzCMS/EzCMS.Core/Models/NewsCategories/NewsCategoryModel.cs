using Ez.Framework.Models;

namespace EzCMS.Core.Models.NewsCategories
{
    public class NewsCategoryModel : BaseGridModel
    {
        public string Name { get; set; }

        public string Abstract { get; set; }
        
        public int? ParentId { get; set; }

        public string ParentName { get; set; }
    }
}
