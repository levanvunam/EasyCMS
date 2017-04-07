using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Entity.Entities.Models
{
    public class EmailTemplate : BaseModel
    {
        public string Name { get; set; }

        public string Subject { get; set; }

        public string From { get; set; }

        public string FromName { get; set; }

        public string CC { get; set; }

        public string BCC { get; set; }

        public string Body { get; set; }

        public string DataType { get; set; }

        public EmailEnums.EmailTemplateType Type { get; set; }
    }
}