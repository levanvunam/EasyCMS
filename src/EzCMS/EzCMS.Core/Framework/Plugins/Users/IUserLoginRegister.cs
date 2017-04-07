using Ez.Framework.Core.IoC.Attributes;

namespace EzCMS.Core.Framework.Plugins.Users
{
    [Register(Lifetime.SingleTon)]
    public interface IUserLoginRegister
    {
        /// <summary>
        /// Get client user redirection
        /// </summary>
        /// <returns></returns>
        string GetRedirectUrl();
    }
}