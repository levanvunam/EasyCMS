using Ez.Framework.Core.IoC.Attributes;
using EzCMS.Core.Models.Common;

namespace EzCMS.Core.Services.Common
{
    [Register(Lifetime.PerInstance)]
    public interface ICommonService
    {
        /// <summary>
        /// Get google analytic access token
        /// </summary>
        /// <returns></returns>
        GoogleAnalyticDashboardModel GetGoogleAnalyticAccessToken();
    }
}