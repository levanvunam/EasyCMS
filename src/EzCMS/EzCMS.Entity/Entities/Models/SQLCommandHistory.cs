using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class SQLCommandHistory : BaseModel
    {
        public string Query { get; set; }
    }
}