using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class RemoteAuthentication : BaseModel
    {
        public string Name { get; set; }

        public string ServiceUrl { get; set; }

        public string AuthorizeCode { get; set; }

        public bool Active { get; set; }
    }
}