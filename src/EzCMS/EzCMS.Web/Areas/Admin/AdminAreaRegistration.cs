using EzCMS.Core.Framework.Utilities;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public string NameSpaces
        {
            get
            {
                return "EzCMS.Web.Areas.Admin.Controllers";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapAreaRouteWithName("ThumbnailHandler",
                                         "Thumbnail/{id}",
                                         new { controller = "Media", action = "Thumbnail", id = UrlParameter.Optional },
                                         new[] { NameSpaces }
                );
            context.MapAreaRouteWithName("AdminDefault",
                                         "Admin/{controller}/{action}/{id}",
                                         new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                                         new[] { NameSpaces }
                );
        }
    }
}
