using Ez.Framework.Models;

namespace EzCMS.Core.Models.Services.Widgets
{
    public class ServiceWidget : BaseGridModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string ImageUrl { get; set; }
    }
}
