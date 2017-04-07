using System.ComponentModel;

namespace EzCMS.Core.Framework.Enums
{
    public enum Permission
    {
        [Description("Manage Protected Document Permissions")]
        ManageProtectedDocuments = 1,

        [Description("Manage label override")]
        ManageLabelOverride = 2,

        [Description("Manage slide in help")]
        ManageSlideInHelp = 3
    }
}
