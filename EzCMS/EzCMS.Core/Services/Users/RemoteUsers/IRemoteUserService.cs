using Ez.Framework.Core.IoC.Attributes;
using EzCMS.Core.Models.Users.Remotes;

namespace EzCMS.Core.Services.Users.RemoteUsers
{
    [Register(Lifetime.PerInstance)]
    public interface IRemoteUserService
    {
        RemoteUser GetRemoteUser(string username);

        bool ValidateUsernamePassword(string username, string password);

        bool ResetPassword(string username, string password);

        bool ChangePassword(string username, string oldPassword, string newPassword);
    }
}