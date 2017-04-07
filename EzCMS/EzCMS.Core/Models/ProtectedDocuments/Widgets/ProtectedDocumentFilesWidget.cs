using Ez.Framework.Utilities.Files;
using EzCMS.Core.Framework.Configuration;
using System.Collections.Generic;
using System.Web;

namespace EzCMS.Core.Models.ProtectedDocuments.Widgets
{
    public class ProtectedDocumentFilesWidget : ProtectedDocumentWidget
    {
        #region Constructors

        public ProtectedDocumentFilesWidget()
        {
            Documents = new List<DocumentModel>();
        }

        public ProtectedDocumentFilesWidget(string path)
            : base(path)
        {
            var relativePath = path.ToRelativePath(EzCMSContants.ProtectedDocumentPath);
            var physicalPath = HttpContext.Current.Server.MapPath(relativePath);
            Folder = physicalPath.GetPathName();
        }

        #endregion

        #region Public Properties

        public string Folder { get; set; }

        public int TotalNotRead { get; set; }

        public List<DocumentModel> Documents { get; set; }

        #endregion
    }
}
