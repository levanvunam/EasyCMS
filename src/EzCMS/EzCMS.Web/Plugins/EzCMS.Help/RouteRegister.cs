using EzCMS.Core.Framework.Plugins.Route;
using System.Web.Mvc;

namespace EzCMS.Help
{
    public class RouteRegister : RoutingRegister
    {
        public override string AreaName
        {
            get
            {
                return "EzCMS.Help";
            }
        }

        public string NameSpaces
        {
            get
            {
                return "EzCMS.Help.Controllers";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
        }
    }
}
