using Ez.Framework.IoC;
using EzCMS.Core.Core.SiteSettings;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.SiteSettings;

namespace EzCMS.Core.Framework.Attributes.Authorize
{
    /// <summary>
    /// Generic Basic Authentication filter that checks for basic authentication
    /// headers and challenges for authentication if no authentication is provided
    /// Sets the Thread Principle with a GenericAuthenticationPrincipal.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class WebApiAuthentication : AuthorizationFilterAttribute
    {
        private readonly string _webApiAuthorizeCode;
        public WebApiAuthentication()
        {
            var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
            _webApiAuthorizeCode = siteSettingService.GetSetting<string>(SettingNames.WebApiAuthorizeCodeConfiguration);
        }

        /// <summary>
        /// Override to Web API filter method to handle Basic Auth check
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!string.IsNullOrEmpty(_webApiAuthorizeCode))
            {
                var identity = ParseAuthorizationHeader(actionContext);
                if (identity == null)
                {
                    Challenge(actionContext);
                    return;
                }

                if (!OnAuthorizeUser(identity.AuthorizeCode, actionContext))
                {
                    Challenge(actionContext);
                    return;
                }

                var principal = new GenericPrincipal(identity, null);

                Thread.CurrentPrincipal = principal;

                // inside of ASP.NET this is required
                //if (HttpContext.Current != null)
                //    HttpContext.Current.User = principal;

                base.OnAuthorization(actionContext);
            }
        }

        /// <summary>
        /// The base implementation merely checks for username and password
        /// present and set the Thread principal.
        /// </summary>
        /// <param name="authorizeCode"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected virtual bool OnAuthorizeUser(string authorizeCode, HttpActionContext actionContext)
        {
            if (string.IsNullOrEmpty(authorizeCode))
                return false;

            if (_webApiAuthorizeCode.Equals(authorizeCode, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the Authorization header and creates user credentials
        /// </summary>
        /// <param name="actionContext"></param>
        protected virtual WebApiAuthenticationIdentity ParseAuthorizationHeader(HttpActionContext actionContext)
        {
            string authorizationHeader = null;
            var authorization = actionContext.Request.Headers.Authorization;
            if (authorization != null && authorization.Scheme == "Basic")
                authorizationHeader = authorization.Parameter;

            if (string.IsNullOrEmpty(authorizationHeader))
                return null;

            authorizationHeader = Encoding.Default.GetString(Convert.FromBase64String(authorizationHeader));

            var tokens = authorizationHeader;

            return new WebApiAuthenticationIdentity(authorizationHeader);
        }

        /// <summary>
        /// Send the Authentication Challenge request
        /// </summary>
        /// <param name="actionContext"></param>
        void Challenge(HttpActionContext actionContext)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));
        }
    }

    public class WebApiAuthenticationIdentity : GenericIdentity
    {
        public WebApiAuthenticationIdentity(string authorizeCode)
            : base("InteractivePartners", "Basic")
        {
            AuthorizeCode = authorizeCode;
        }

        /// <summary>
        /// Basic Authorization Password for custom authentication
        /// </summary>
        public string AuthorizeCode { get; set; }
    }
}