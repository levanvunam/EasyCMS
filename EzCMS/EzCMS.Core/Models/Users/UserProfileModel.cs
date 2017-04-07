using EzCMS.Core.Framework.Configuration;
using EzCMS.Entity.Entities.Models;
using System.IO;
using System.Web;

namespace EzCMS.Core.Models.Users
{
    public class UserProfileModel : User
    {
        public UserProfileModel()
        {

        }

        public UserProfileModel(User user)
        {

            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Email = user.Email;
            IsSystemAdministrator = user.IsSystemAdministrator;
            Status = user.Status;
            Phone = user.Phone;
            Gender = user.Gender;
            About = user.About;
            Address = user.Address;
            Facebook = user.Facebook;
            Twitter = user.Twitter;
            LinkedIn = user.LinkedIn;
            AvatarFileName = user.AvatarFileName;
            DateOfBirth = user.DateOfBirth;
            IsRemoteAccount = user.IsRemoteAccount;
            ChangePasswordAfterLogin = user.ChangePasswordAfterLogin;

            RecordOrder = user.RecordOrder;
            RecordDeleted = user.RecordDeleted;

            Created = user.Created;
            CreatedBy = user.CreatedBy;
            LastUpdate = user.LastUpdate;
            LastUpdateBy = user.LastUpdateBy;
        }

        public string AvatarPath
        {
            get
            {
                if (string.IsNullOrEmpty(AvatarFileName) ||
                    !File.Exists(
                        HttpContext.Current.Server.MapPath(EzCMSContants.AvatarFolder + AvatarFileName)))
                {
                    return EzCMSContants.NoAvatar;
                }

                return EzCMSContants.AvatarFolder + AvatarFileName;
            }
        }
    }
}
