using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Mails.Models;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Models.EmailLogs;
using EzCMS.Core.Services.EmailAccounts;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.EmailLogs
{
    public class EmailLogService : ServiceHelper, IEmailLogService
    {
        private readonly IEmailAccountService _emailAccountService;
        private readonly IRepository<EmailLog> _emailLogRepository;

        public EmailLogService(IEmailAccountService emailAccountService, IRepository<EmailLog> emailLogRepository)
        {
            _emailAccountService = emailAccountService;
            _emailLogRepository = emailLogRepository;
        }

        /// <summary>
        /// Get emails waiting to be sent
        /// </summary>
        /// <returns></returns>
        public List<EmailLog> GetEmailSendingQueues()
        {
            var settingService = HostContainer.GetInstance<ISiteSettingService>();

            // Get max tries setup
            var maxTries = settingService.GetSetting<int>(SettingNames.EmailLogMaxTries);

            var now = DateTime.UtcNow;

            return Fetch(emailLog =>
                emailLog.SentTries < maxTries && !emailLog.SentOn.HasValue &&
                (!emailLog.SendLater.HasValue || now > emailLog.SendLater)).ToList();
        }

        #region Base

        public IQueryable<EmailLog> GetAll()
        {
            return _emailLogRepository.GetAll();
        }

        public IQueryable<EmailLog> Fetch(Expression<Func<EmailLog, bool>> expression)
        {
            return _emailLogRepository.Fetch(expression);
        }

        public EmailLog FetchFirst(Expression<Func<EmailLog, bool>> expression)
        {
            return _emailLogRepository.FetchFirst(expression);
        }

        public EmailLog GetById(object id)
        {
            return _emailLogRepository.GetById(id);
        }

        internal ResponseModel Insert(EmailLog emailLog)
        {
            return _emailLogRepository.Insert(emailLog);
        }

        internal ResponseModel Update(EmailLog emailLog)
        {
            return _emailLogRepository.Update(emailLog);
        }

        internal ResponseModel Delete(EmailLog emailLog)
        {
            return _emailLogRepository.Delete(emailLog);
        }

        internal ResponseModel Delete(object id)
        {
            return _emailLogRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _emailLogRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the email queues
        /// </summary>
        /// <param name="si"></param>
        /// <param name="emailAccountId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchEmailLogs(JqSearchIn si, int? emailAccountId = null)
        {
            var data = GetAll();

            // Get email queue by email account
            if (emailAccountId.HasValue)
            {
                data = data.Where(emailLog => emailLog.EmailAccountId == emailAccountId.Value);
            }

            var emailLogs = Maps(data);

            return si.Search(emailLogs);
        }

        /// <summary>
        /// Export email queues
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var emailLogs = Maps(data);

            var exportData = si.Export(emailLogs, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="emailLogs"></param>
        /// <returns></returns>
        private IQueryable<EmailLogModel> Maps(IQueryable<EmailLog> emailLogs)
        {
            return emailLogs.Select(emailLog => new EmailLogModel
            {
                Id = emailLog.Id,
                Priority = emailLog.Priority,
                FromName = emailLog.FromName,
                From = emailLog.From,
                ToName = emailLog.ToName,
                To = emailLog.To,
                Bcc = emailLog.Bcc,
                CC = emailLog.CC,
                Subject = emailLog.Subject,
                SendLater = emailLog.SendLater,
                SentTries = emailLog.SentTries,
                SentOn = emailLog.SentOn,
                IsSent = emailLog.SentOn.HasValue,
                EmailAccountId = emailLog.EmailAccountId,
                RecordOrder = emailLog.RecordOrder,
                Created = emailLog.Created,
                CreatedBy = emailLog.CreatedBy,
                LastUpdate = emailLog.LastUpdate,
                LastUpdateBy = emailLog.LastUpdateBy
            });
        }

        #endregion

        /// <summary>
        /// Search logs
        /// </summary>
        /// <param name="si"></param>
        /// <param name="emailLogId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchLogs(JqSearchIn si, int emailLogId)
        {
            var emailLog = GetById(emailLogId);
            var logs = new List<EmailSendingLog>();
            if (emailLog != null && !string.IsNullOrEmpty(emailLog.Message))
            {
                logs = SerializeUtilities.Deserialize<List<EmailSendingLog>>(emailLog.Message);
            }

            if (logs == null) logs = new List<EmailSendingLog>();

            return si.Search(logs.AsQueryable());
        }

        #endregion

        #region Manage

        /// <summary>
        /// Create email queue
        /// </summary>
        /// <param name="emailLog"></param>
        /// <param name="useDefaultAccount"></param>
        /// <returns></returns>
        public ResponseModel CreateEmail(EmailLog emailLog, bool useDefaultAccount = false)
        {
            if (useDefaultAccount || emailLog.EmailAccountId == 0)
            {
                var defaultAccount = _emailAccountService.GetDefaultAccount();
                if (defaultAccount == null)
                {
                    defaultAccount = _emailAccountService.GetAll().FirstOrDefault();
                    if (defaultAccount == null)
                        return new ResponseModel
                        {
                            Success = false,
                            Message = T("EmailLog_Message_MissingDefaultAccount")
                        };
                }

                if (string.IsNullOrEmpty(emailLog.From))
                {
                    emailLog.From = defaultAccount.Email;
                }
                if (string.IsNullOrEmpty(emailLog.FromName))
                {
                    emailLog.FromName = defaultAccount.Email;
                }
                emailLog.EmailAccountId = defaultAccount.Id;
            }

            var logs = new List<EmailSendingLog>
            {
                new EmailSendingLog
                {
                    Time = DateTime.Now,
                    Message = TFormat("EmailLog_Message_CreateEmailLog", DateTime.UtcNow)
                }
            };
            emailLog.Message = SerializeUtilities.Serialize(logs);

            var response = Insert(emailLog);
            return response.SetMessage(response.Success
                ? T("EmailLog_Message_CreateSuccessfully")
                : T("EmailLog_Message_CreateFailure"));
        }

        /// <summary>
        /// Update email
        /// </summary>
        /// <param name="emailLog"></param>
        /// <returns></returns>
        public ResponseModel UpdateMail(EmailLog emailLog)
        {
            var response = Update(emailLog);
            return response.SetMessage(response.Success
                ? T("EmailLog_Message_UpdateSuccessfully")
                : T("EmailLog_Message_UpdateFailure"));
        }

        /// <summary>
        /// Get email queue detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EmailLogDetailModel GetEmailLogDetailModel(int id)
        {
            var emailLog = GetById(id);
            return emailLog != null ? new EmailLogDetailModel(emailLog) : null;
        }

        /// <summary>
        /// Delete email by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteEmail(int id)
        {
            var item = GetById(id);

            if (item != null)
            {
                var response = Delete(id);
                return response.SetMessage(response.Success
                    ? T("EmailLog_Message_DeleteSuccessfully")
                    : T("EmailLog_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("EmailLog_Message_DeleteSuccessfully")
            };
        }

        /// <summary>
        /// Resend the email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel ResendEmail(ResendEmailModel model)
        {
            var emailLog = GetById(model.EmailLogId);
            if (emailLog == null)
                return new ResponseModel
                {
                    Success = false,
                    Message = T("EmailLog_Message_ObjectNotFound")
                };

            var emailAccount = _emailAccountService.GetById(model.EmailAccountId);
            if (emailAccount == null)
                return new ResponseModel
                {
                    Success = false,
                    Message = T("EmailAccount_Message_ObjectNotFound")
                };

            return _emailAccountService.SendEmailDirectly(emailAccount, new EmailModel
            {
                From = emailLog.From,
                FromName = emailLog.FromName,
                To = emailLog.To,
                ToName = emailLog.ToName,
                CC = emailLog.CC,
                Bcc = emailLog.Bcc,
                Subject = emailLog.Subject,
                Body = emailLog.From,
                Attachment = emailLog.Attachment
            });
        }

        /// <summary>
        /// Get resend email model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResendEmailModel GetResendEmailModel(int id)
        {
            var emailLog = GetById(id);
            return emailLog != null ? new ResendEmailModel(emailLog) : null;
        }

        #endregion
    }
}