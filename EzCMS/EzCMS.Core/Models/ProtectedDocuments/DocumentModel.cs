using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Files;
using Ez.Framework.Utilities.Time;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Models.ProtectedDocumentLogs;
using EzCMS.Entity.Core.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EzCMS.Core.Models.ProtectedDocuments
{
    public class DocumentModel
    {
        public DocumentModel()
        {
        }

        /// <summary>
        /// Get document from folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="logs"></param>
        public DocumentModel(DirectoryInfo folder, IEnumerable<ProtectedDocumentLogModel> logs = null)
            : this()
        {

            var mediaPath = folder.FullName.ToRelativePath(EzCMSContants.ProtectedDocumentPath);

            Text = folder.FullName.GetPathName();
            DocumentType = ProtectedDocumentEnums.DocumentType.Folder;
            Data = new DocumentReport
            {
                Id = mediaPath.ToIdStringByHash(),
                Path = mediaPath.GetUniqueLink(),
                IsRead = false,
                DataIcon = string.Empty,
                HasChildren = folder.GetFiles().Any() || folder.GetDirectories().Any(),
                LastModified = DateTimeUtilities.ToDateTimeFormat(folder.LastWriteTimeUtc),
                LastWriteTimeUtc = folder.LastWriteTimeUtc,
                TotalFiles = folder.GetFiles().Count(),
                TotalFolders = folder.GetDirectories().Count()
            };

            if (logs != null)
            {
                Data.TotalRead =
                    folder.GetFiles()
                        .Count(
                            f => logs.Any(l => l.Path.Equals(f.FullName.ToRelativePath(EzCMSContants.ProtectedDocumentPath), StringComparison.CurrentCultureIgnoreCase)));
            }

            Data.TotalNotRead = Data.TotalFiles - Data.TotalRead;
        }

        /// <summary>
        /// Get document from file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="logs"></param>
        /// <param name="mappings"></param>
        public DocumentModel(FileInfo file, IEnumerable<ProtectedDocumentLogModel> logs = null, IEnumerable<DocumentClassMapping> mappings = null)
            : this()
        {

            var mediaPath = file.FullName.ToRelativePath(EzCMSContants.ProtectedDocumentPath);

            Text = file.FullName.GetPathName();
            DocumentType = ProtectedDocumentEnums.DocumentType.Item;
            Data = new DocumentReport
            {
                Id = mediaPath.ToIdStringByHash(),
                //Path = string.Format("{0}{1}", mediaPath.GetUniqueLink(), mediaPath.GetExtension()),
                Path = mediaPath.GetUniqueLink(),
                Mime = mediaPath.GetMimeMapping(),
                Extension = mediaPath.GetExtension(),
                IsRead = logs == null || logs.Any(l => l.Path.Equals(mediaPath)),
                HasChildren = false,
                DataIcon = GetImageClass(mappings, file.FullName),
                LastModified = DateTimeUtilities.ToDateTimeFormat(file.LastWriteTimeUtc),
                LastWriteTimeUtc = file.LastWriteTimeUtc
            };
        }

        /// <summary>
        /// Get image class for path
        /// </summary>
        /// <param name="mappings"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetImageClass(IEnumerable<DocumentClassMapping> mappings, string path)
        {
            var fileExtension = path.GetExtension();

            if (!string.IsNullOrEmpty(fileExtension) && mappings != null)
            {
                fileExtension = fileExtension.Replace(".", "");
                var mapping =
                    mappings.FirstOrDefault(
                        m => m.Extension.Equals(fileExtension, StringComparison.CurrentCultureIgnoreCase));

                if (mapping != null)
                    return mapping.ClassName;
            }

            return "fa fa-file";
        }

        #region Public Properties

        public string Text { get; set; }

        public ProtectedDocumentEnums.DocumentType DocumentType { get; set; }

        public string Type
        {
            get
            {
                return DocumentType.GetEnumName();
            }
        }


        public DocumentReport Data { get; set; }

        #endregion
    }

    public class DocumentReport
    {
        public string Id { get; set; }

        public string Path { get; set; }

        public string Url
        {
            get
            {
                return UrlUtilities.GenerateUrl(HttpContext.Current.Request.RequestContext, "ProtectedDocument", "SiteApi", new { area = "", id = Path });
            }
        }

        #region Folder information

        public bool HasChildren { get; set; }

        public int TotalFiles { get; set; }

        public int TotalFolders { get; set; }

        public int TotalRead { get; set; }

        public int TotalNotRead { get; set; }

        #endregion

        #region File information

        public string CssClass { get; set; }

        public string DataIcon { get; set; }

        public string Extension { get; set; }

        public bool IsRead { get; set; }

        public string Mime { get; set; }

        #endregion

        public string LastModified { get; set; }

        public DateTime LastWriteTimeUtc { get; set; }
    }
}