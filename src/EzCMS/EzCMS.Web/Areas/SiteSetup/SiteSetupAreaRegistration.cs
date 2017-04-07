using EzCMS.Core.Framework.Utilities;
using EzCMS.Entity.Core.SiteInitialize;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.SiteSetup
{
    public class SiteSetupAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "SiteSetup"; }
        }

        public static string NameSpaces
        {
            get
            {
                return "EzCMS.Web.Areas.SiteSetup.Controllers";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            if (!SiteInitializer.IsSetupFinish())
            {
                context.MapAreaRouteWithName("Setup Step",
                    "Setup/{action}",
                    new
                    {
                        controller = "Setup"
                    },
                    new[] { NameSpaces });

                context.MapAreaRouteWithName("Setup",
                    "{*url}",
                    new
                    {
                        controller = "Setup",
                        action = "Index"
                    },
                    new[] { NameSpaces });
            }
        }
    }
}
