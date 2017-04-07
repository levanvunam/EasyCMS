using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;

namespace EzCMS.Core.Services.Tools
{
    [Register(Lifetime.PerInstance)]
    public interface IToolService
    {
        #region Session Manager

        /// <summary>
        /// Search the sessions.
        /// </summary>
        /// <returns></returns>
        JqGridSearchOut SearchSessions(JqSearchIn si, string key);

        #endregion

        #region Cookie Manager

        /// <summary>
        /// Search the cookies.
        /// </summary>
        /// <returns></returns>
        JqGridSearchOut SearchCookies(JqSearchIn si);

        #endregion
    }
}