using Ez.Framework.Models;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Web;
using EzCMS.Entity.Entities.Models;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EzCMS.Core.Models.UserGroups
{
    public class UserGroupModel : BaseGridModel
    {
        public UserGroupModel()
        {
        }

        public UserGroupModel(UserGroup userGroup)
            : this()
        {
            Id = userGroup.Id;
            Name = userGroup.Name;
            Description = userGroup.Description;

            if (!string.IsNullOrEmpty(userGroup.RedirectUrl))
            {
                RedirectUrl = userGroup.RedirectUrl;
                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                if (urlHelper.IsLocalUrl(RedirectUrl))
                {
                    RedirectUrl = RedirectUrl.ToAbsoluteUrl();
                }

                var responseCode = RedirectUrl.GetResponseCode();

                IsValidUrl = responseCode == HttpStatusCode.OK ? responseCode.ToString() : responseCode.ToString().CamelFriendly();
            }

            ToolbarId = userGroup.ToolbarId;
            ToolbarName = userGroup.ToolbarId.HasValue ? userGroup.Toolbar.Name : string.Empty;

            RecordOrder = userGroup.RecordOrder;
            Created = userGroup.Created;
            CreatedBy = userGroup.CreatedBy;
            LastUpdate = userGroup.LastUpdate;
            LastUpdateBy = userGroup.LastUpdateBy;
        }

        #region Public Properties

        public string Name { get; set; }

        public string Description { get; set; }

        public string RedirectUrl { get; set; }

        public int? ToolbarId { get; set; }

        public string ToolbarName { get; set; }

        public string IsValidUrl { get; set; }

        #endregion
    }
}
