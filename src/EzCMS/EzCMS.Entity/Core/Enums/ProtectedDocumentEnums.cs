using System.ComponentModel;

namespace EzCMS.Entity.Core.Enums
{
    public class ProtectedDocumentEnums
    {
        public enum DocumentType
        {
            [Description("folder")]
            Folder = 1,
            [Description("item")]
            Item = 2
        }

        public enum DocumentSearchType
        {
            All = 0,
            Folder = 1,
            File = 2
        }
    }
}
