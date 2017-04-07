using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.Users;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.UserLoginHistories
{
    public class UserLoginHistorySearchModel
    {
        private readonly IUserService _userService;
        public UserLoginHistorySearchModel()
        {
            _userService = HostContainer.GetInstance<IUserService>();

            Users = _userService.GetUsers();
        }

        public UserLoginHistorySearchModel(int? userId)
            : this()
        {
            if (userId.HasValue)
            {
                UserId = userId.Value;
                Users = _userService.GetUsers(UserId);
            }
        }

        #region Public Properties

        public string Keyword { get; set; }

        public int? UserId { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }

        #endregion
    }
}
