using Ez.Framework.IoC;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Services.Users;
using System.IO;
using System.Web;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Models.Users
{
    public class SimpleUserModel
    {
        public SimpleUserModel()
        {
            AvatarPath = EzCMSContants.NoAvatar;
        }

        public SimpleUserModel(string username)
            : this()
        {
            var userService = HostContainer.GetInstance<IUserService>();
            var user = userService.GetByUsername(username);

            if (user != null)
            {
                FullName = user.FullName;
                Username = string.IsNullOrEmpty(user.Username) ? user.Email : user.Username;
                if (string.IsNullOrEmpty(user.AvatarFileName) ||
                    !File.Exists(
                        HttpContext.Current.Server.MapPath(EzCMSContants.AvatarFolder + user.AvatarFileName)))
                {
                    AvatarPath = EzCMSContants.NoAvatar;
                }
                else
                {
                    AvatarPath = EzCMSContants.AvatarFolder + user.AvatarFileName;
                }
            }
            else
            {
                FullName = username;
                Username = username;
            }
        }

        #region Public Properties

        public string FullName { get; set; }

        public string AvatarPath { get; set; }

        public string Username { get; set; }

        #endregion

    }
}
