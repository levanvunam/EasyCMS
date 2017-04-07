using System.Collections.Generic;

namespace EzCMS.Core.Models.Users.Remotes
{
    public class RemoteUser
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<string> UserGroups { get; set; }

        public bool IsSystemAdministrator { get; set; }

        public dynamic Session { get; set; }
    }
}