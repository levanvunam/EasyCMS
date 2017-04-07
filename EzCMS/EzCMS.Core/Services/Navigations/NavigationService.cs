using Ez.Framework.Configurations;
using Ez.Framework.Core.Context;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.Navigations;
using Ez.Framework.Core.Navigations.Attributes;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Utilities;
using EzCMS.Core.Models.Navigations;
using EzCMS.Core.Services.FavouriteNavigations;
using EzCMS.Core.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace EzCMS.Core.Services.Navigations
{
    public class NavigationService : ServiceHelper, INavigationService
    {
        private readonly IFavouriteNavigationService _favouriteNavigationService;
        private readonly IUserService _userService;

        public NavigationService(IFavouriteNavigationService favouriteNavigationService, IUserService userService)
        {
            _favouriteNavigationService = favouriteNavigationService;
            _userService = userService;
        }

        /// <summary>
        /// Get all available navigations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Navigation> GetAll()
        {
            if (EzWorkContext.EzCMSNavigations == null)
            {
                List<Navigation> navigations = new List<Navigation>();
                var assemblies = EzCMSUtilities.GetEzCMSAssemblies();

                var actions = assemblies
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => typeof(Controller).IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(method => method.IsPublic && !method.IsDefined(typeof(NonActionAttribute)) && method.GetCustomAttributes<NavigationAttribute>(true) != null);

                foreach (var action in actions)
                {
                    if (action.DeclaringType != null)
                    {
                        string controller = action.DeclaringType.Name;
                        var attributes = action.GetCustomAttributes<NavigationAttribute>(true);
                        foreach (var attribute in attributes)
                        {
                            if (attribute != null)
                            {
                                var navigation = new Navigation
                                {
                                    Id = attribute.Id,
                                    ParentId = attribute.ParentId,
                                    Name = LocalizedResourceService.T(attribute.Name),
                                    Visible = attribute.Visible,
                                    Order = attribute.Order,
                                    Icon = attribute.Icon,
                                    Controller = attribute.IsCategory ? "" : controller.Replace("Controller", ""),
                                    Action = attribute.IsCategory ? "" : action.Name
                                };

                                //Setup hierarchy
                                if (string.IsNullOrEmpty(navigation.ParentId))
                                {
                                    navigation.Hierarchy = navigation.Id;
                                }
                                else
                                {
                                    navigation.Hierarchy = string.Format("{0}{1}{2}", navigation.ParentId,
                                        FrameworkConstants.IdSeparator, navigation.Id);
                                }

                                navigations.Add(navigation);
                            }
                        }
                    }
                }

                foreach (var navigation in navigations)
                {
                    foreach (var item in navigations)
                    {
                        if (navigation.Id != item.Id)
                        {
                            item.Hierarchy = item.Hierarchy.Replace(navigation.Id, navigation.Hierarchy);
                        }
                    }
                }

                EzWorkContext.EzCMSNavigations = navigations;
                InitializePermissions();
            }
            return EzWorkContext.EzCMSNavigations;
        }

        /// <summary>
        /// Get navigation by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Navigation GetById(string id)
        {
            return GetAll().FirstOrDefault(navigation => navigation.Id.Equals(id));
        }

        /// <summary>
        /// Get breadcrumbs
        /// </summary>
        /// <param name="controller">the current controller name</param>
        /// <param name="action">the current action name</param>
        /// <returns></returns>
        public BreadcrumbModel GetBreadcrumbs(string controller, string action)
        {
            var navigation =
                GetAll()
                    .FirstOrDefault(n => n.Controller.Equals(controller) && n.Action.Equals(action));
            if (navigation != null)
            {
                return new BreadcrumbModel
                {
                    Breadcrumbs = GetParents(navigation.Id, false).Select(n => new Breadcrumb(n)).ToList(),
                    CurrentBreadcrumb = new Breadcrumb(navigation)
                };
            }
            return new BreadcrumbModel();
        }

        /// <summary>
        /// Get all parents of item (and include child if needed)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeChild"></param>
        /// <param name="orderAsc"></param>
        /// <returns></returns>
        public IEnumerable<Navigation> GetParents(string id, bool includeChild, bool orderAsc = true)
        {
            var entity = GetById(id);
            if (entity == null)
            {
                return null;
            }
            var suffix = entity.Hierarchy;
            var parents = GetAll().Where(navigation => suffix.StartsWith(navigation.Hierarchy) && !suffix.Equals(navigation.Hierarchy));
            if (orderAsc) return parents.OrderBy(p => p.Hierarchy);
            return parents.OrderByDescending(p => p.Hierarchy);
        }

        #region Navigation Permissions

        /// <summary>
        /// Initialize the Navigation permissions when application start
        /// </summary>
        public void InitializePermissions()
        {
            var navigations = GetAll().ToList();
            foreach (var navigation in navigations)
            {
                var permissions = new List<Permission>();
                if (string.IsNullOrEmpty(navigation.Controller))
                {
                    navigation.Permissions = string.Empty;
                }
                else
                {
                    // Get currently loaded assemblies
                    var assemblies = EzCMSUtilities.GetEzCMSAssemblies();

                    var controllerType = assemblies
                        .SelectMany(assembly => assembly.GetTypes())
                        .FirstOrDefault(type => type != null
                            && type.IsPublic
                            && type.Name.Equals(string.Format("{0}Controller", navigation.Controller), StringComparison.OrdinalIgnoreCase)
                            && !type.IsAbstract
                            && typeof(IController).IsAssignableFrom(type));
                    if (controllerType != null)
                    {
                        //Get permission from controller
                        var controllerAuthorizationAttributes = controllerType.GetCustomAttributes(typeof(EzCMSAuthorizeAttribute), false)
                                        .Cast<EzCMSAuthorizeAttribute>();
                        foreach (var attribute in controllerAuthorizationAttributes)
                        {
                            if (attribute.Permissions != null && attribute.Permissions.Any())
                                permissions.AddRange(attribute.Permissions);
                        }

                        //Get permission from actions
                        var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            .Where(m => typeof(ActionResult).IsAssignableFrom(m.ReturnType)
                                        && m.Name.Equals(navigation.Action)).ToList();
                        if (methods.Any())
                        {
                            foreach (var methodInfo in methods)
                            {
                                var actionAuthorizationAttributes = methodInfo
                                        .GetCustomAttributes(typeof(EzCMSAuthorizeAttribute), false)
                                        .Cast<EzCMSAuthorizeAttribute>();
                                foreach (var attribute in actionAuthorizationAttributes)
                                {
                                    if (attribute.Permissions != null)
                                        permissions.AddRange(attribute.Permissions);
                                }
                            }
                        }
                    }
                }
                navigation.Permissions = permissions.Any() ? string.Join(",", permissions.Select(permission => (int)permission)) : string.Empty;
            }

            EzWorkContext.EzCMSNavigations = navigations;
        }

        #endregion

        #region Render Back-End Navigations

        /// <summary>
        /// Get Navigations tree by parent NavigationId
        /// </summary>
        /// <param name="parentId">the parent NavigationId</param>
        /// <returns></returns>
        public NavigationModel GetNavigations(string parentId)
        {
            var navigations = GetAccessibleNavigations().ToList();

            var currentFavouriteNavigations = _favouriteNavigationService.GetCurrentUserFavouriteNavigations().ToList();

            var adminNavigations = RenderNavigations(navigations, parentId);

            var favourite = adminNavigations.FirstOrDefault(m =>
                !string.IsNullOrEmpty(m.Controller) && m.Controller.Equals("FavouriteNavigations")
                && !string.IsNullOrEmpty(m.Action) && m.Action.Equals("Index"));
            if (favourite != null)
            {
                favourite.Id = "0";
                favourite.Children = currentFavouriteNavigations.Select(c => new NavigationItemModel(c)).ToList();
            }

            return new NavigationModel
            {
                Navigations = adminNavigations,
                SearchNavigations = navigations.Where(navigation => !string.IsNullOrEmpty(navigation.Url) ||
                                                                    (!string.IsNullOrEmpty(navigation.Action) &&
                                                                     !string.IsNullOrEmpty(navigation.Controller)))
                    .Select(m => new NavigationItemModel(m))
                    .ToList()
            };
        }

        #region Private Methods

        /// <summary>
        /// Recursive render Navigations
        /// </summary>
        /// <param name="data"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private List<NavigationItemModel> RenderNavigations(IEnumerable<Navigation> data, string parentId = null)
        {
            var currentData = data.Where(navigation => !string.IsNullOrEmpty(parentId)
                ? parentId.Equals(navigation.ParentId)
                : string.IsNullOrEmpty(navigation.ParentId))
                .OrderBy(m => m.Order);
            return currentData.Select(navigation => new NavigationItemModel
            {
                Id = navigation.Id,
                Name = navigation.Name,
                Url = navigation.Url,
                Controller = navigation.Controller,
                Action = navigation.Action,
                Icon = navigation.Icon,
                Hierarchy = navigation.Hierarchy,
                Order = navigation.Order,
                IsFavouriteNavigation = _favouriteNavigationService.Fetch(f => f.NavigationId == navigation.Id && f.UserId == WorkContext.CurrentUser.Id).Any(),
                Children = navigation.Id == parentId
                    ? new List<NavigationItemModel>()
                    : RenderNavigations(data, navigation.Id)
            }).ToList();
        }

        /// <summary>
        /// Get all visible Navigations in the system
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Navigation> GetVisibleNavigations()
        {
            return GetAll().Where(navigation => navigation.Visible);
        }

        /// <summary>
        /// Get all visible Navigations in the system
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Navigation> GetAccessibleNavigations(IEnumerable<Navigation> data = null)
        {
            if (data == null)
            {
                data = GetVisibleNavigations();
            }

            // Get current user permissions
            var userPermissions = _userService.GetUserPermissions(WorkContext.CurrentUser.Id);

            return data.ToList().Where(navigation => string.IsNullOrEmpty(navigation.Permissions) ||
                                                 navigation.Permissions.Split(',')
                                                     .Select(int.Parse)
                                                     .Intersect(userPermissions).Count() == navigation.Permissions.Split(',').Length);
        }

        #endregion

        #endregion
    }
}