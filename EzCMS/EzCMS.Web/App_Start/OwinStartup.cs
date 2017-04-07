using Ez.Framework.Configurations;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.Users;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(EzCMS.Web.OwinInit))]
namespace EzCMS.Web
{
    public class OwinInit
    {
        public void Configuration(IAppBuilder app)
        {
            var oAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true,
                Provider = new EzCMSOAuthAuthorization()
            };

            app.UseOAuthBearerTokens(oAuthOptions);
        }
    }

    public class EzCMSOAuthAuthorization : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return base.ValidateClientAuthentication(context);
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userService = HostContainer.GetInstance<IUserService>();
            try
            {
                var user = userService.CheckLogin(context.UserName, context.Password);
                if (user != null)
                {
                    var id = new ClaimsIdentity("Embedded");

                    id.AddClaim(new Claim("sub", context.UserName));
                    id.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                    id.AddClaim(new Claim(ClaimTypes.Role, string.Join(FrameworkConstants.Colon, user.UserUserGroups.Select(i => i.UserGroup.Name))));
                    context.Validated(id);
                }
                else
                {
                    context.Rejected();
                }
            }
            catch (Exception)
            {
                context.Rejected();
            }

            return base.GrantResourceOwnerCredentials(context);
        }
    }
}