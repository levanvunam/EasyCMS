using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.Navigations;
using EzCMS.Core.Models.Navigations;
using System.Collections.Generic;

namespace EzCMS.Core.Services.Navigations
{
    [Register(Lifetime.PerInstance)]
    public interface INavigationService
    {
        /// <summary>
        /// Get all navigations
        /// </summary>
        /// <returns></returns>
        IEnumerable<Navigation> GetAll();

        /// <summary>
        /// Get navigation by id
        /// </summary>
        /// <param name="id">the navigation id</param>
        /// <returns></returns>
        Navigation GetById(string id);

        #region Navigation Permissions

        /// <summary>
        /// Initialize the navigation permissions when needed
        /// </summary>
        void InitializePermissions();

        #endregion

        #region Render Navigations

        /// <summary>
        /// Get navigations tree by parent navigation id
        /// </summary>
        /// <param name="parentId">the parent navigation id</param>
        /// <returns></returns>
        NavigationModel GetNavigations(string parentId = null);

        #endregion

        /// <summary>
        /// Get breadcrumbs
        /// </summary>
        /// <param name="controller">the current controller name</param>
        /// <param name="action">the current action name</param>
        /// <returns></returns>
        BreadcrumbModel GetBreadcrumbs(string controller, string action);
    }
}