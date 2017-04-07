using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class FavouriteNavigation : BaseModel
    {
        public string NavigationId { get; set; }

        public int UserId { get; set; }
    }
}