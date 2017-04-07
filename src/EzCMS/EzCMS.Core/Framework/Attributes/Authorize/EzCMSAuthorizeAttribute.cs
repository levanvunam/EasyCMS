using Ez.Framework.Core.IoC;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Exceptions.Sites;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Services.Users;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EzCMS.Core.Framework.Attributes.Authorize
{
    public class EzCMSAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Permission array
        /// </summary>
        public Permission[] Permissions { get; set; }

        /// <summary>
        /// Only administrator can access
        /// </summary>
        public bool IsAdministrator { get; set; }

        /// <summary>
        /// Only administrator can access
        /// </summary>
        public bool IsCompanyAdministrator { get; set; }

        /// <summary>
        /// On authorizing
        /// </summary>
        /// <param name="authorizationContext">the authorize context</param>
        public override void OnAuthorization(AuthorizationContext authorizationContext)
        {
            base.OnAuthorization(authorizationContext);

            if (authorizationContext.Result is HttpUnauthorizedResult)
            {
                throw new EzCMSUnauthorizeException(string.Empty);
            }
        }

        /// <summary>
        /// Authorize
        /// </summary>
        /// <param name="httpContext">the current context</param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            var isAuthorize = base.AuthorizeCore(httpContext);
            if (!isAuthorize)
            {
                return false;
            }

            var userService = HostContainer.GetInstance<IUserService>();

            if (WorkContext.CurrentUser == null)
            {
                var currentUser = userService.GetActiveUser(httpContext.User.Identity.Name);
                if (currentUser == null)
                {
                    FormsAuthentication.SignOut();
                    return false;
                }

                // Save this information for getting updates
                var lastTimeGettingUpdate = currentUser.LastLogin;

                currentUser.LastLogin = DateTime.UtcNow;
                userService.UpdateUser(currentUser);

                UserSessionModel.SetUserSession(currentUser, lastTimeGettingUpdate);
            }

            if (WorkContext.CurrentUser != null)
            {
                if (WorkContext.CurrentUser.IsSystemAdministrator)
                {
                    return true;
                }

                if (WorkContext.CurrentUser.HasPermissions(Permissions) && !IsAdministrator &&
                    (!IsCompanyAdministrator || WorkContext.CurrentUser.IsCompanyAdministrator))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
