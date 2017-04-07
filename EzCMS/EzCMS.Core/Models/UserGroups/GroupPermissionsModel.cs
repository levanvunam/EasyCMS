using System.Collections.Generic;

namespace EzCMS.Core.Models.UserGroups
{
    public class GroupPermissionsModel
    {
        public int UserGroupId { get; set; }
        public string Name { get; set; }
        public List<GroupPermissionItem> Permissions { get; set; } 
    }

    public class GroupPermissionItem
    {
        public int GroupPermissionId { get; set; }
        public string PermissionName { get; set; }
        public bool HasPermission { get; set; }
        public string HasPermissionString
        {
            get { return HasPermission ? "On" : "Off"; }
            set { HasPermission = value.Equals("On"); }
        }
    }
}
