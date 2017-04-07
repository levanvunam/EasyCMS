using Ez.Framework.Configurations;
using Ez.Framework.Models;
using EzCMS.Entity.Core.Enums;
using System;
using System.Collections.Generic;

namespace EzCMS.Core.Models.Users
{
    public class UserModel : BaseGridModel
    {
        #region Public Properties

        public string Email { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserGroups
        {
            get
            {
                return string.Join(FrameworkConstants.SemicolonSeparator, UserGroupNames);
            }
        }

        public UserEnums.UserStatus Status { get; set; }

        public bool IsSystemAdministrator { get; set; }

        public bool IsRemoteAccount { get; set; }

        public int LoginTimes { get; set; }

        public int? LastLoginId { get; set; }

        public DateTime? LastLogin { get; set; }

        public string AvatarFileName { get; set; }

        public string AvatarPath { get; set; }

        public string Phone { get; set; }

        public double LastLoginHours { get; set; }

        public IEnumerable<string> UserGroupNames { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public UserEnums.Gender Gender { get; set; }

        public string About { get; set; }

        public DateTime? ReleaseLockDate { get; set; }

        public DateTime LastPasswordChange { get; set; }

        public int PasswordFailsCount { get; set; }

        public DateTime? LastFailedLogin { get; set; }

        public DateTime? AccountExpiresDate { get; set; }

        public string Address { get; set; }

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        public string LinkedIn { get; set; }

        public string Password { get; set; }

        #endregion
    }
}
