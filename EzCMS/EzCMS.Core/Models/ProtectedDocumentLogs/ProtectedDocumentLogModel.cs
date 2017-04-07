using Ez.Framework.Models;

namespace EzCMS.Core.Models.ProtectedDocumentLogs
{
    public class ProtectedDocumentLogModel : BaseGridModel
    {
        #region Public Properties

        public string Path { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        #endregion
    }
}
