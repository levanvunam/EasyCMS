using EzCMS.Core.Framework.Utilities;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.BackgroundTasks
{
    public class BackgroundTasksAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "BackgroundTasks";
            }
        }

        public string NameSpaces
        {
            get
            {
                return "EzCMS.Web.Areas.BackgroundTasks.Controllers";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapAreaRouteWithName("BackgroundTasksDefault", "BackgroundTasks/{controller}/{action}/{id}",
                                         new
                                         {
                                             action = "Index",
                                             id = UrlParameter.Optional
                                         }, new[] { NameSpaces });
        }
    }
}