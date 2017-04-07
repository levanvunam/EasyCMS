using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Navigations;
using EzCMS.Core.Services.Navigations;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;

namespace EzCMS.Core.Models.Navigations
{
    public class NavigationItemModel
    {
        private readonly INavigationService _navigationService;
        public NavigationItemModel()
        {
            _navigationService = HostContainer.GetInstance<INavigationService>();
            Children = new List<NavigationItemModel>();
        }

        public NavigationItemModel(Navigation navigation)
            : this()
        {
            Id = navigation.Id;
            Icon = navigation.Icon;
            Url = navigation.Url;
            Controller = navigation.Controller;
            Action = navigation.Action;
            Name = navigation.Name;
            Hierarchy = navigation.Hierarchy;
            ParentId = navigation.ParentId;
            Order = navigation.Order;
        }

        public NavigationItemModel(FavouriteNavigation navigation)
            : this()
        {
            Id = navigation.NavigationId;
            var menu = _navigationService.GetById(navigation.NavigationId);
            if (menu != null)
            {
                Icon = menu.Icon;
                Url = menu.Url;
                Controller = menu.Controller;
                Action = menu.Action;
                Name = menu.Name;
                Hierarchy = menu.Hierarchy;
                ParentId = menu.ParentId;
            }
            Order = navigation.RecordOrder;
            IsFavouriteNavigation = true;
        }

        #region Public Properties

        public string Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string Url { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Hierarchy { get; set; }

        public string ParentId { get; set; }

        public int Order { get; set; }

        public bool IsFavouriteNavigation { get; set; }

        public List<NavigationItemModel> Children { get; set; }

        #endregion
    }
}
