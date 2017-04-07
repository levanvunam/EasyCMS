using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Mvc.Controllers;
using EzCMS.Core.Framework.Attributes.ErrorHandling;
using EzCMS.Core.Services.Localizes;

namespace EzCMS.Core.Framework.Mvc.Controllers
{
    [EzCMSErrorHandling]
    public class BaseController : EzBaseController
    {
        public readonly IEzCMSLocalizedResourceService LocalizedResourceService;

        public BaseController()
        {
            LocalizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
        }
    }
}
