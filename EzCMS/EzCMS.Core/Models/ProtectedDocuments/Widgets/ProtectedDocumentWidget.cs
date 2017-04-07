using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Files;
using EzCMS.Core.Framework.Configuration;
using System.IO;
using System.Web;

namespace EzCMS.Core.Models.ProtectedDocuments.Widgets
{
    public class ProtectedDocumentWidget
    {
        #region Constructors

        public ProtectedDocumentWidget()
        {

        }

        public ProtectedDocumentWidget(string path)
            : this()
        {
            if (!string.IsNullOrEmpty(path))
            {
                var relativePath = path.ToRelativePath(EzCMSContants.ProtectedDocumentPath);
                var physicalPath = HttpContext.Current.Server.MapPath(relativePath);

                if (Directory.Exists(physicalPath))
                {
                    EncryptedPath = relativePath.GetUniqueLink();
                }
            }
            else
            {
                EncryptedPath = EzCMSContants.ProtectedDocumentPath.GetUniqueLink();
            }
        }

        #endregion

        #region Public Properties

        public string DocumentId
        {
            get
            {
                return EncryptedPath.ToIdStringByHash();
            }
        }

        public string EncryptedPath { get; set; }

        #endregion
    }
}
