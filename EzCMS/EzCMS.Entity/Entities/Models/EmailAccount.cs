using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class EmailAccount : BaseModel
    {
        [StringLength(512)]
        public string Email { get; set; }

        [StringLength(512)]
        public string DisplayName { get; set; }

        [StringLength(512)]
        public string Host { get; set; }

        public int Port { get; set; }

        [StringLength(512)]
        public string Username { get; set; }

        [StringLength(512)]
        public string Password { get; set; }

        public bool EnableSsl { get; set; }

        public bool UseDefaultCredentials { get; set; }

        public int TimeOut { get; set; }

        public bool IsDefault { get; set; }

        public virtual List<EmailLog> EmailLogs { get; set; }
    }
}