using System;
using System.Linq;
using System.Net.Http;
using System.Web.Routing;
using Ez.Framework.Core.IoC;
using Ez.Framework.Utilities.Web;
using EzCMS.Core.Framework.Exceptions.Users;
using EzCMS.Core.Models.RemoteAuthentications;
using EzCMS.Core.Models.Users.Remotes;
using EzCMS.Core.Services.RemoteAuthentications;

namespace EzCMS.Core.Services.Users.RemoteUsers
{
    public class RemoteUserService : IRemoteUserService
    {
        private readonly IRemoteAuthenticationService _remoteAuthenticationService;

        public RemoteUserService()
        {
            _remoteAuthenticationService =
                HostContainer.GetInstance<IRemoteAuthenticationService>();
        }

        /// <summary>
        /// Get remote user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public RemoteUser GetRemoteUser(string username)
        {
            var activeRemoteServices = _remoteAuthenticationService.GetActiveRemoteServices();
            if (activeRemoteServices.Any())
            {
                foreach (var remoteService in activeRemoteServices)
                {
                    var remoteUser = SendRequest<RemoteUser>(remoteService, "api/AuthenticationService/GetUser",
                        HttpMethod.Get, new RouteValueDictionary(new
                        {
                            Username = username,
                            time = DateTime.UtcNow.ToString("yyMMddhhmmss")
                        }));

                    //Check if current user is valid or not
                    if (remoteUser != null && !string.IsNullOrEmpty(remoteUser.Email))
                    {
                        return remoteUser;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Validate user name and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidateUsernamePassword(string username, string password)
        {
            var activeRemoteServices = _remoteAuthenticationService.GetActiveRemoteServices();
            if (activeRemoteServices.Any())
            {
                foreach (var remoteService in activeRemoteServices)
                {
                    var loginResult = SendRequest<UserLoginResult>(remoteService, "api/AuthenticationService/Login",
                        HttpMethod.Post, new RouteValueDictionary(new
                        {
                            Username = username,
                            Password = password
                        }));

                    if (loginResult.IsValid) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ResetPassword(string username, string password)
        {
            var activeRemoteServices = _remoteAuthenticationService.GetActiveRemoteServices();
            if (activeRemoteServices.Any())
            {
                foreach (var remoteService in activeRemoteServices)
                {
                    var resetPasswordResult = SendRequest<bool>(remoteService, "api/AuthenticationService/ResetPassword",
                        HttpMethod.Post, new RouteValueDictionary(new
                        {
                            Username = username,
                            NewPassword = password
                        }));

                    if (resetPasswordResult) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var activeRemoteServices = _remoteAuthenticationService.GetActiveRemoteServices();
            if (activeRemoteServices.Any())
            {
                foreach (var remoteService in activeRemoteServices)
                {
                    var changePasswordResult = SendRequest<bool>(remoteService,
                        "api/AuthenticationService/ChangePassword",
                        HttpMethod.Post, new RouteValueDictionary(new
                        {
                            Username = username,
                            OldPassword = oldPassword,
                            NewPassword = newPassword
                        }));

                    if (changePasswordResult) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Send request to remote service and getting result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="apiAction"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private T SendRequest<T>(RemoteAuthenticationModel configuration, string apiAction, HttpMethod method,
            RouteValueDictionary parameters)
        {
            try
            {
                return WebUtilities.SendApiRequest<T>(configuration.ServiceUrl, configuration.AuthorizeCode, apiAction,
                    method, parameters);
            }
            catch (Exception)
            {
                throw new ServiceNotAvailableException();
            }
        }
    }
}