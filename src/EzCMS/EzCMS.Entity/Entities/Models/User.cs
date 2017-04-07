using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EzCMS.Entity.Entities.Models
{
    public class User : BaseModel
    {
        #region Constructors

        public User()
        {
        }

        public User(string email)
            : this()
        {
            Email = email;
        }

        #endregion

        #region Properties

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public double LastLoginHours
        {
            get
            {
                if (LastLogin.HasValue)
                    return (DateTime.UtcNow - LastLogin.Value).TotalHours;
                return 0;
            }
        }

        public int? Age
        {
            get
            {
                if (DateOfBirth.HasValue)
                {
                    return (int)(DateTime.UtcNow - DateOfBirth.Value).TotalDays / 365;
                }
                return null;
            }
        }

        #region Base Properties

        [StringLength(255)]
        public string Username { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(512)]
        public string Password { get; set; }

        [StringLength(512)]
        public string PasswordSalt { get; set; }

        [Required]
        [StringLength(512)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(512)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        public bool IsSystemAdministrator { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public UserEnums.Gender Gender { get; set; }

        public string About { get; set; }

        [StringLength(512)]
        public string AvatarFileName { get; set; }

        [StringLength(1024)]
        public string Address { get; set; }

        public UserEnums.UserStatus Status { get; set; }

        public DateTime? LastLogin { get; set; }

        [StringLength(512)]
        public string Facebook { get; set; }

        [StringLength(512)]
        public string Twitter { get; set; }

        [StringLength(512)]
        public string LinkedIn { get; set; }

        public bool IsRemoteAccount { get; set; }

        public string Settings { get; set; }

        public DateTime? ReleaseLockDate { get; set; }

        public DateTime LastPasswordChange { get; set; }

        public int PasswordFailsCount { get; set; }

        public DateTime? LastFailedLogin { get; set; }

        public string ResetPasswordCode { get; set; }

        public DateTime? ResetPasswordExpiryDate { get; set; }

        public bool ChangePasswordAfterLogin { get; set; }

        public DateTime? AccountExpiresDate { get; set; }

        public virtual ICollection<UserUserGroup> UserUserGroups { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }

        public virtual ICollection<UserLoginHistory> UserLoginHistories { get; set; }

        #endregion

        #endregion
    }
}