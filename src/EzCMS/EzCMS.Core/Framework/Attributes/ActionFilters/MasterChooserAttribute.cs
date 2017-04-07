using Ez.Framework.IoC;
using EzCMS.Core.Services.FileTemplates;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Framework.Attributes.ActionFilters
{
    public class MasterChooserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            var result = filterContext.Result as ViewResult;
            if (result != null)
            {
                int fileTemplateId;
                if (int.TryParse(filterContext.HttpContext.Request["FileTemplateId"], out fileTemplateId))
                {
                    var fileTemplateService = HostContainer.GetInstance<IFileTemplateService>();
                    var master = fileTemplateService.GetFileTemplateMaster(fileTemplateId);
                    if (!string.IsNullOrEmpty(master))
                    {
                        result.MasterName = master;
                    }
                }
            }
        }
    }
}
