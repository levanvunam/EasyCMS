using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Models.UserGroups;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;
using System.Linq;

namespace EzCMS.Core.Models.ClientNavigations.Widgets
{
    public class NavigationNodeModel
    {
        public NavigationNodeModel()
        {
            ViewableGroups = new List<GroupItem>();
            EditableGroups = new List<GroupItem>();
        }

        public NavigationNodeModel(ClientNavigation navigation)
            : this()
        {
            Id = navigation.Id;
            PageId = navigation.PageId;
            ViewableGroups = (navigation.PageId.HasValue && navigation.Page.PageSecurities.Any()) ? navigation.Page.PageSecurities.Where(s => s.CanView).Select(s => new GroupItem
            {
                Id = s.GroupId,
                Name = s.UserGroup.Name
            }).ToList() : new List<GroupItem>();
            EditableGroups = (navigation.PageId.HasValue && navigation.Page.PageSecurities.Any()) ? navigation.Page.PageSecurities.Where(s => s.CanEdit).Select(s => new GroupItem
            {
                Id = s.GroupId,
                Name = s.UserGroup.Name
            }).ToList() : new List<GroupItem>();
            Title = (navigation.PageId.HasValue ? navigation.Page.Title : navigation.Title).Trim();
            Url = navigation.PageId.HasValue ? navigation.Page.FriendlyUrl.ToPageFriendlyUrl(navigation.Page.IsHomePage) : navigation.Url;
            UrlTarget = string.IsNullOrEmpty(navigation.UrlTarget) ? "_self" : navigation.UrlTarget;
            ParentId = navigation.ParentId;
            IncludeInSiteNavigation = navigation.PageId.HasValue ? navigation.Page.IncludeInSiteNavigation : navigation.IncludeInSiteNavigation;
            Hierarchy = navigation.Hierarchy;
            RecordOrder = navigation.PageId.HasValue ? navigation.Page.RecordOrder * EzCMSContants.OrderMultipleTimes : navigation.RecordOrder;

            Status = navigation.PageId.HasValue ? navigation.Page.Status : PageEnums.PageStatus.Online;
        }

        #region Public Properties

        public int Id { get; set; }

        public int? PageId { get; set; }

        public int? ParentId { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public List<GroupItem> ViewableGroups { get; set; }

        public List<GroupItem> EditableGroups { get; set; }

        public bool IncludeInSiteNavigation { get; set; }

        public PageEnums.PageStatus Status { get; set; }

        public bool CanView { get; set; }

        public bool CanEdit { get; set; }

        public string UrlTarget { get; set; }

        public string Hierarchy { get; set; }

        public int RecordOrder { get; set; }

        #endregion
    }
}
