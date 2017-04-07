using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.ProtectedDocuments;
using EzCMS.Core.Models.ProtectedDocuments.Widgets;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Services.ProtectedDocuments
{
    [Register(Lifetime.PerInstance)]
    public interface IDocumentService : IBaseService<ProtectedDocument>
    {
        ProtectedDocument GetByEncryptPath(string encryptedPath);

        #region Search Tree

        List<DocumentModel> GetDocuments(string encryptPath,
            ProtectedDocumentEnums.DocumentSearchType type = ProtectedDocumentEnums.DocumentSearchType.All,
            int? total = null);

        List<DocumentModel> SearchDocuments(string path, string keyword);

        #endregion

        #region Manage

        DocumentPermissionManageModel GetDocumentPermissionManageModel(string encryptedPath);

        ResponseModel SaveDocumentPermissionManageModel(DocumentPermissionManageModel model);

        #endregion

        #region Feedback

        DocumentFeedbackModel GetDocumentFeedback(string encryptedPath);

        ResponseModel SaveCustomerFeedback(DocumentFeedbackModel model);

        #endregion

        #region Permissions

        bool CanCurrentUserAccessPath(string path, DocumentPermissionModel parentPermissions = null);

        DocumentPermissionModel GetDocumentPermissions(string path, DocumentPermissionModel parentPermissions);

        #endregion

        #region Widgets

        /// <summary>
        /// Get model for protected document widget
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        ProtectedDocumentWidget GetProtectedDocumentWidget(string path);

        /// <summary>
        /// Get model for protected document widget for files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        ProtectedDocumentFilesWidget GetProtectedDocumentFilesWidget(string path, int total);

        #endregion
    }
}