using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Files;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Exceptions.Sites;
using EzCMS.Core.Models.ProtectedDocuments;
using EzCMS.Core.Models.ProtectedDocuments.Widgets;
using EzCMS.Core.Models.ProtectedDocuments.Emails;
using EzCMS.Core.Services.EmailLogs;
using EzCMS.Core.Services.EmailTemplates;
using EzCMS.Core.Services.ProtectedDocumentLogs;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.ProtectedDocumentCompanies;
using EzCMS.Entity.Repositories.ProtectedDocumentCompanyTypes;
using EzCMS.Entity.Repositories.ProtectedDocumentGroups;

namespace EzCMS.Core.Services.ProtectedDocuments
{
    public class DocumentService : ServiceHelper, IDocumentService
    {
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IProtectedDocumentCompanyRepository _protectedDocumentCompanyRepository;
        private readonly IProtectedDocumentCompanyTypeRepository _protectedDocumentCompanyTypeRepository;
        private readonly IProtectedDocumentGroupRepository _protectedDocumentGroupRepository;
        private readonly IProtectedDocumentLogService _protectedDocumentLogService;
        private readonly IRepository<ProtectedDocument> _protectedDocumentRepository;
        private readonly ISiteSettingService _siteSettingService;

        public DocumentService(IRepository<ProtectedDocument> protectedDocumentRepository,
            IProtectedDocumentGroupRepository protectedDocumentGroupRepository,
            IProtectedDocumentCompanyRepository protectedDocumentCompanyRepository,
            IProtectedDocumentCompanyTypeRepository protectedDocumentCompanyTypeRepository,
            ISiteSettingService siteSettingService,
            IProtectedDocumentLogService protectedDocumentLogService, IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService)
        {
            _protectedDocumentRepository = protectedDocumentRepository;
            _protectedDocumentGroupRepository = protectedDocumentGroupRepository;
            _protectedDocumentCompanyRepository = protectedDocumentCompanyRepository;
            _protectedDocumentCompanyTypeRepository = protectedDocumentCompanyTypeRepository;
            _siteSettingService = siteSettingService;
            _protectedDocumentLogService = protectedDocumentLogService;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
        }

        #region Base

        public IQueryable<ProtectedDocument> GetAll()
        {
            return _protectedDocumentRepository.GetAll();
        }

        public IQueryable<ProtectedDocument> Fetch(Expression<Func<ProtectedDocument, bool>> expression)
        {
            return _protectedDocumentRepository.Fetch(expression);
        }

        public ProtectedDocument FetchFirst(Expression<Func<ProtectedDocument, bool>> expression)
        {
            return _protectedDocumentRepository.FetchFirst(expression);
        }

        internal ResponseModel Insert(ProtectedDocument protectedDocument)
        {
            return _protectedDocumentRepository.Insert(protectedDocument);
        }

        internal ResponseModel Update(ProtectedDocument protectedDocument)
        {
            return _protectedDocumentRepository.Update(protectedDocument);
        }

        internal ResponseModel Delete(ProtectedDocument protectedDocument)
        {
            return _protectedDocumentRepository.Delete(protectedDocument);
        }

        internal ResponseModel Delete(object id)
        {
            return _protectedDocumentRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _protectedDocumentRepository.SetRecordDeleted(id);
        }

        public ProtectedDocument GetById(object id)
        {
            return _protectedDocumentRepository.GetById(id);
        }

        public ProtectedDocument GetByPath(string path)
        {
            return FetchFirst(d => d.Path.Equals(path));
        }

        public ProtectedDocument GetByEncryptPath(string encryptedPath)
        {
            var path = encryptedPath.GetUniqueLinkInput();
            return FetchFirst(d => d.Path.Equals(path));
        }

        #endregion

        #region Search

        /// <summary>
        /// Search the protected documents
        /// </summary>
        /// <returns></returns>
        public List<DocumentModel> GetDocuments(string encryptPath,
            ProtectedDocumentEnums.DocumentSearchType type = ProtectedDocumentEnums.DocumentSearchType.All,
            int? total = null)
        {
            // Convert encrypt path to real path
            var path = encryptPath.GetUniqueLinkInput();
            var documents = new List<DocumentModel>();

            var physicalPath = HttpContext.Current.Server.MapPath(path);
            if (Directory.Exists(physicalPath))
            {
                var folderPermissions = GetDocumentPermissions(path);
                var documentTypeMapping = _siteSettingService.LoadSetting<DocumentTypeMappingSetting>();
                var directory = new DirectoryInfo(physicalPath);

                var documentLogs = _protectedDocumentLogService.GetCurrentUserRelatedLogs(path);

                if (type == ProtectedDocumentEnums.DocumentSearchType.All ||
                    type == ProtectedDocumentEnums.DocumentSearchType.Folder)
                {
                    // Loop through all directories and add if current user can see the documents
                    foreach (var childDirectory in directory.GetDirectories())
                    {
                        var mediaPath =
                            childDirectory.FullName.ToRelativePath(EzCMSContants.ProtectedDocumentPath);
                        if (CanCurrentUserAccessPath(mediaPath, folderPermissions))
                        {
                            documents.Add(new DocumentModel(childDirectory, documentLogs));
                        }
                    }
                }

                if (type == ProtectedDocumentEnums.DocumentSearchType.All ||
                    type == ProtectedDocumentEnums.DocumentSearchType.File)
                {
                    // If user can access folder then they can view the files
                    var canAccessPath = CanCurrentUserAccessPath(path);
                    if (canAccessPath)
                    {
                        //Loop through all files in directory
                        documents.AddRange(
                            directory.GetFiles()
                                .Select(file => new DocumentModel(file, documentLogs, documentTypeMapping.Mappings)));
                    }
                }
            }

            // Get number of documents
            if (total.HasValue && total.Value > 0)
            {
                return documents.Take(total.Value).ToList();
            }

            return documents;
        }

        /// <summary>
        /// Search the protected documents
        /// </summary>
        /// <returns></returns>
        public List<DocumentModel> SearchDocuments(string path, string keyword)
        {
            // Convert encrypt path to real path
            path = path.GetUniqueLinkInput();

            var documentLogs = _protectedDocumentLogService.GetCurrentUserRelatedLogs(path);

            var documentTypeMapping = _siteSettingService.LoadSetting<DocumentTypeMappingSetting>();

            // Get protected document full path
            var protectedDocumentFolder =
                new DirectoryInfo(HttpContext.Current.Server.MapPath(path));

            // Search all files contains keyword
            var files = protectedDocumentFolder.GetFiles(string.Format("*{0}*", keyword), SearchOption.AllDirectories);

            var fileInfos = new List<FileInfo>();
            foreach (var item in files)
            {
                if (CanCurrentUserAccessPath(item.FullName.ToRelativePath(EzCMSContants.ProtectedDocumentPath)))
                {
                    fileInfos.Add(item);
                }
            }

            return fileInfos.Select(f => new DocumentModel(f, documentLogs, documentTypeMapping.Mappings)).ToList();
        }

        #endregion

        #region Manage Permissions

        /// <summary>
        /// Get document permissions
        /// </summary>
        /// <param name="encryptedPath"></param>
        /// <returns></returns>
        public DocumentPermissionManageModel GetDocumentPermissionManageModel(string encryptedPath)
        {
            return new DocumentPermissionManageModel(encryptedPath);
        }

        /// <summary>
        /// Save document permissions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveDocumentPermissionManageModel(DocumentPermissionManageModel model)
        {
            var protectedDocument = GetByEncryptPath(model.EncryptedPath);

            if (protectedDocument != null)
            {
                #region User Groups

                if (model.GroupIds == null) model.GroupIds = new List<int>();

                var currentGroups = protectedDocument.ProtectedDocumentGroups.Select(p => p.UserGroupId).ToList();

                // Remove groups
                var removedGroupIds = currentGroups.Where(g => !model.GroupIds.Contains(g)).ToList();
                _protectedDocumentGroupRepository.Delete(protectedDocument.Id, removedGroupIds);

                // Add new groups
                var addedGroupIds = model.GroupIds.Where(id => !currentGroups.Contains(id)).ToList();
                _protectedDocumentGroupRepository.Insert(protectedDocument.Id, addedGroupIds);

                #endregion

                #region Company

                if (model.CompanyIds == null) model.CompanyIds = new List<int>();

                var currentCompanies = protectedDocument.ProtectedDocumentCompanies.Select(p => p.CompanyId).ToList();

                // Remove companies
                var removedCompanyIds = currentCompanies.Where(id => !model.CompanyIds.Contains(id)).ToList();
                _protectedDocumentCompanyRepository.Delete(protectedDocument.Id, removedCompanyIds);

                // Add new companies
                var addedCompanyIds = model.CompanyIds.Where(id => !currentCompanies.Contains(id)).ToList();
                _protectedDocumentCompanyRepository.Insert(protectedDocument.Id, addedCompanyIds);

                #endregion

                #region Company Types

                if (model.CompanyTypeIds == null) model.CompanyTypeIds = new List<int>();

                var currentCompanyTypes =
                    protectedDocument.ProtectedDocumentCompanyTypes.Select(p => p.CompanyTypeId).ToList();

                // Remove company types
                var removedCompanyTypeIds = currentCompanyTypes.Where(id => !model.CompanyTypeIds.Contains(id)).ToList();
                _protectedDocumentCompanyTypeRepository.Delete(protectedDocument.Id, removedCompanyTypeIds);

                // Add new company types
                var addedCompanyTypeIds = model.CompanyTypeIds.Where(id => !currentCompanyTypes.Contains(id)).ToList();
                _protectedDocumentCompanyTypeRepository.Insert(protectedDocument.Id, addedCompanyTypeIds);

                #endregion

                return new ResponseModel
                {
                    Success = true,
                    Message = T("ProtectedDocument_Message_UpdateSuccessfully")
                };
            }

            protectedDocument = new ProtectedDocument
            {
                Path = model.EncryptedPath.GetUniqueLinkInput()
            };

            var response = Insert(protectedDocument);
            if (response.Success)
            {
                #region User Groups

                if (model.GroupIds == null) model.GroupIds = new List<int>();

                _protectedDocumentGroupRepository.Insert(protectedDocument.Id, model.GroupIds);

                #endregion

                #region Company

                if (model.CompanyIds == null) model.CompanyIds = new List<int>();

                _protectedDocumentCompanyRepository.Insert(protectedDocument.Id, model.CompanyIds);

                #endregion

                #region Company Types

                if (model.CompanyTypeIds == null) model.CompanyTypeIds = new List<int>();

                _protectedDocumentCompanyTypeRepository.Insert(protectedDocument.Id, model.CompanyTypeIds);

                #endregion

                return response.SetMessage(response.Success
                    ? T("ProtectedDocument_Message_UpdateSuccessfully")
                    : T("ProtectedDocument_Message_UpdateFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("ProtectedDocument_Message_UpdateFailure")
            };
        }

        #endregion

        #region Feedback

        /// <summary>
        /// Get document feedback model
        /// </summary>
        /// <param name="encryptedPath"></param>
        /// <returns></returns>
        public DocumentFeedbackModel GetDocumentFeedback(string encryptedPath)
        {
            return new DocumentFeedbackModel(encryptedPath);
        }

        /// <summary>
        /// Save document feedback
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveCustomerFeedback(DocumentFeedbackModel model)
        {
            if (WorkContext.CurrentUser == null)
            {
                throw new EzCMSUnauthorizeException();
            }

            var protectedDocumentEmailTo =
                _siteSettingService.GetSetting<string>(SettingNames.ProtectedDocumentEmailTo);

            var emailModel = new ProtectedDocumentEmailModel
            {
                Email = WorkContext.CurrentUser.Email,
                FullName = WorkContext.CurrentUser.FullName,
                Comment = model.Comment,
                Path = model.RelativePath
            };

            var emailResponse = _emailTemplateService.ParseEmail(EmailEnums.EmailTemplateType.ProtectedDocumentForm,
                emailModel);

            if (emailResponse == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("ProtectedDocument_Message_MissingProtectedDocumentFeedbackEmailTemplate")
                };
            }

            var emailLog = new EmailLog
            {
                To = protectedDocumentEmailTo,
                ToName = protectedDocumentEmailTo,
                From = WorkContext.CurrentUser.Email,
                FromName = WorkContext.CurrentUser.FullName,
                CC = emailResponse.CC,
                Bcc = emailResponse.BCC,
                Subject = emailResponse.Subject,
                Body = emailResponse.Body,
                Priority = EmailEnums.EmailPriority.Medium
            };

            var response = _emailLogService.CreateEmail(emailLog, true);

            return response.Success
                ? response.SetMessage(T("ProtectedDocument_Message_FeedbackSuccessfully"))
                : response.SetMessage(T("ProtectedDocument_Message_FeedbackFailure"));
        }

        #endregion

        #region Permissions

        /// <summary>
        /// Check if current user can access folder or not
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parentPermissions"></param>
        /// <returns></returns>
        public bool CanCurrentUserAccessPath(string path, DocumentPermissionModel parentPermissions = null)
        {
            if (WorkContext.CurrentUser != null)
            {
                if (WorkContext.CurrentUser.IsSystemAdministrator ||
                    WorkContext.CurrentUser.HasPermissions(Permission.ManageProtectedDocuments))
                {
                    return true;
                }

                var documentPermissions = GetDocumentPermissions(path, parentPermissions);
                if (documentPermissions != null)
                {
                    if (WorkContext.CurrentUser.GroupIds.Any(g => documentPermissions.GroupIds.Contains(g))
                        || WorkContext.CurrentUser.CompanyIds.Any(c => documentPermissions.CompanyIds.Contains(c))
                        ||
                        WorkContext.CurrentUser.CompanyTypeIds.Any(c => documentPermissions.CompanyTypeIds.Contains(c)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Get permissions setup for the path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parentPermissions"></param>
        /// <returns></returns>
        public DocumentPermissionModel GetDocumentPermissions(string path,
            DocumentPermissionModel parentPermissions = null)
        {
            var document = GetByPath(path);
            if (document != null &&
                (document.ProtectedDocumentGroups.Any() || document.ProtectedDocumentCompanyTypes.Any() ||
                 document.ProtectedDocumentCompanies.Any()))
            {
                return new DocumentPermissionModel(document);
            }

            if (parentPermissions != null)
            {
                return parentPermissions;
            }

            var parentSetupPermissions = Fetch(p => path.Contains(p.Path)).OrderByDescending(p => p.Path);
            foreach (var protectedDocument in parentSetupPermissions)
            {
                if (protectedDocument.ProtectedDocumentGroups.Any() ||
                    protectedDocument.ProtectedDocumentCompanyTypes.Any() ||
                    protectedDocument.ProtectedDocumentCompanies.Any())
                {
                    return new DocumentPermissionModel(protectedDocument);
                }
            }

            return null;
        }

        #endregion

        #region Widgets

        /// <summary>
        /// Get model for protected document widget
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ProtectedDocumentWidget GetProtectedDocumentWidget(string path)
        {
            return new ProtectedDocumentWidget(path);
        }

        /// <summary>
        /// Get model for protected document widget for files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public ProtectedDocumentFilesWidget GetProtectedDocumentFilesWidget(string path, int total)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = EzCMSContants.ProtectedDocumentPath;
            }

            var model = new ProtectedDocumentFilesWidget(path);

            // If path is invalid / not found
            if (string.IsNullOrEmpty(model.Folder))
            {
                return null;
            }

            // Get all documents from folder
            var protectedDocuments = GetDocuments(model.EncryptedPath, ProtectedDocumentEnums.DocumentSearchType.File,
                total);

            var totalNotRead = protectedDocuments.Count(x => !x.Data.IsRead);

            model.Documents = protectedDocuments;
            model.TotalNotRead = totalNotRead;

            return model;
        }

        #endregion
    }
}