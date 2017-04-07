using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.UserGroups;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Users
{
    public class UserSearchModel
    {
        public UserSearchModel()
        {
            var userGroupService = HostContainer.GetInstance<IUserGroupService>();

            UserGroups = userGroupService.GetUserGroups();
        }

        #region Public Properties

        public string Keyword { get; set; }

        public int? UserGroupId { get; set; }

        public IEnumerable<SelectListItem> UserGroups { get; set; }

        #endregion
    }
}
