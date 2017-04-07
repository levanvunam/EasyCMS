using System;
using System.Linq;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Users
{
    public class UserDetailModel
    {
        #region Constructors

        public UserDetailModel()
        {
        }

        public UserDetailModel(User user)
            : this()
        {
            Id = user.Id;
            UserGroups = user.UserUserGroups.Any()
                ? string.Join(",", user.UserUserGroups.Select(g => g.UserGroup.Name))
                : string.Empty;
            LastLogin = user.LastLogin;
            Join = user.Created;
            User = new UserManageModel(user);
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        public DateTime? LastLogin { get; set; }

        [LocalizedDisplayName("User_Field_UserGroups")]
        public string UserGroups { get; set; }

        [LocalizedDisplayName("User_Field_LastLoginHours")]
        public double LastLoginHours
        {
            get
            {
                if (LastLogin.HasValue)
                    return (DateTime.UtcNow - LastLogin.Value).TotalHours;
                return 0;
            }
        }

        [LocalizedDisplayName("User_Field_Join")]
        public DateTime Join { get; set; }

        public UserManageModel User { get; set; }

        #endregion
    }
}
