using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Mails;
using Ez.Framework.Utilities.Mails.Models;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.EmailAccounts;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.EmailAccounts
{
    public class EmailAccountService : ServiceHelper, IEmailAccountService
    {
        private readonly IRepository<EmailAccount> _emailAccountRepository;

        public EmailAccountService(IRepository<EmailAccount> emailAccountRepository)
        {
            _emailAccountRepository = emailAccountRepository;
        }

        /// <summary>
        /// Mark email account as default
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel MarkAsDefault(int id)
        {
            var image = GetById(id);
            if (image != null)
            {
                var query = "Update EmailAccounts Set IsDefault = 0";
                var response = _emailAccountRepository.ExcuteSql(query);
                if (response.Success)
                {
                    image.IsDefault = true;
                    response = Update(image);
                    return response.SetMessage(response.Success
                        ? T("EmailAccount_Message_MarkDefaultSuccessfully")
                        : T("EmailAccount_Message_MarkDefaultFailure"));
                }
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("EmailAccount_Message_ObjectNotFound")
            };
        }

        public ResponseModel SendTestEmail(TestEmailModel model)
        {
            var emailAccount = GetById(model.EmailAccountId);
            if (emailAccount == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("EmailAccount_Message_ObjectNotFound")
                };
            }
            var emailModel = new EmailModel
            {
                From = emailAccount.Email,
                FromName = "Email Test Service",
                To = model.Email,
                ToName = model.Email,
                Bcc = string.Empty,
                CC = string.Empty,
                Subject = "Test Email",
                Body = "This is test email.",
                Attachment = string.Empty
            };

            return SendEmailDirectly(emailAccount, emailModel);
        }

        /// <summary>
        /// Send the email directly
        /// </summary>
        /// <param name="emailAccount"></param>
        /// <param name="emailModel"></param>
        /// <returns></returns>
        public ResponseModel SendEmailDirectly(EmailAccount emailAccount, EmailModel emailModel)
        {
            var mailSetting = new EmailSetting
            {
                Host = emailAccount.Host,
                Port = emailAccount.Port,
                Timeout = emailAccount.TimeOut,
                EnableSsl = emailAccount.EnableSsl,
                User = emailAccount.Username,
                Password = emailAccount.Password
            };

            var mailUtilities = new MailUtilities(mailSetting);

            try
            {
                mailUtilities.SendEmail(emailModel);
                return new ResponseModel
                {
                    Success = true,
                    Message = T("EmailAccount_Message_SendEmailDirectlySuccessfully")
                };
            }
            catch (Exception exception)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("EmailAccount_Message_SendEmailDirectlyFailure"),
                    DetailMessage = exception.Message
                };
            }
        }

        /// <summary>
        /// Get email accounts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetEmailAccounts()
        {
            return GetAll().Select(l => new SelectListItem
            {
                Text = l.Email + " - " + l.Host,
                Value = SqlFunctions.StringConvert((double) l.Id).Trim()
            });
        }

        /// <summary>
        /// Get email account detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EmailAccountDetailModel GetEmailAccountDetailModel(int id)
        {
            var emailAccount = GetById(id);
            return emailAccount != null ? new EmailAccountDetailModel(emailAccount) : null;
        }

        #region Validation

        /// <summary>
        /// Check if setting type exists
        /// </summary>
        /// <param name="emailAccountId"></param>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public bool IsEmailAccountExisted(int? emailAccountId, string emailAddress)
        {
            return Fetch(u => u.Email.Equals(emailAddress) && u.Id != emailAccountId).Any();
        }

        #endregion

        #region Base

        public IQueryable<EmailAccount> GetAll()
        {
            return _emailAccountRepository.GetAll();
        }

        public IQueryable<EmailAccount> Fetch(Expression<Func<EmailAccount, bool>> expression)
        {
            return _emailAccountRepository.Fetch(expression);
        }

        public EmailAccount FetchFirst(Expression<Func<EmailAccount, bool>> expression)
        {
            return _emailAccountRepository.FetchFirst(expression);
        }

        public EmailAccount GetById(object id)
        {
            return _emailAccountRepository.GetById(id);
        }

        /// <summary>
        /// Get default account
        /// </summary>
        /// <returns></returns>
        public EmailAccount GetDefaultAccount()
        {
            return FetchFirst(e => e.IsDefault);
        }

        internal ResponseModel Insert(EmailAccount emailAccount)
        {
            return _emailAccountRepository.Insert(emailAccount);
        }

        internal ResponseModel Update(EmailAccount emailAccount)
        {
            return _emailAccountRepository.Update(emailAccount);
        }

        internal ResponseModel Delete(EmailAccount emailAccount)
        {
            return _emailAccountRepository.Delete(emailAccount);
        }

        internal ResponseModel Delete(object id)
        {
            return _emailAccountRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _emailAccountRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the email accounts
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchEmailAccounts(JqSearchIn si)
        {
            var data = GetAll();

            var emailAccounts = Maps(data);

            return si.Search(emailAccounts);
        }

        /// <summary>
        /// Export email accounts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var emailAccounts = Maps(data);

            var exportData = si.Export(emailAccounts, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="emailAccounts"></param>
        /// <returns></returns>
        private IQueryable<EmailAccountModel> Maps(IQueryable<EmailAccount> emailAccounts)
        {
            return emailAccounts.Select(e => new EmailAccountModel
            {
                Id = e.Id,
                Email = e.Email,
                DisplayName = e.DisplayName,
                IsDefault = e.IsDefault,
                RecordOrder = e.RecordOrder,
                Created = e.Created,
                CreatedBy = e.CreatedBy,
                LastUpdate = e.LastUpdate,
                LastUpdateBy = e.LastUpdateBy
            });
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get email account manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EmailAccountManageModel GetEmailAccountManageModel(int? id = null)
        {
            var emailAccount = GetById(id);
            if (emailAccount != null)
            {
                return new EmailAccountManageModel(emailAccount);
            }
            return new EmailAccountManageModel();
        }

        /// <summary>
        /// Save email account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveEmailAccount(EmailAccountManageModel model)
        {
            ResponseModel response;
            var emailAccount = GetById(model.Id);
            if (emailAccount != null)
            {
                emailAccount.Email = model.Email;
                emailAccount.DisplayName = model.DisplayName;
                emailAccount.Host = model.Host;
                emailAccount.Port = model.Port;
                emailAccount.Username = model.Username;
                emailAccount.Password = model.Password;
                emailAccount.EnableSsl = model.EnableSsl;
                emailAccount.UseDefaultCredentials = model.UseDefaultCredentials;
                emailAccount.IsDefault = model.IsDefault;
                emailAccount.TimeOut = model.Timeout;

                response = Update(emailAccount);
                return response.SetMessage(response.Success
                    ? T("EmailAccount_Message_UpdateSuccessfully")
                    : T("EmailAccount_Message_UpdateFailure"));
            }
            Mapper.CreateMap<EmailAccountManageModel, EmailAccount>();
            emailAccount = Mapper.Map<EmailAccountManageModel, EmailAccount>(model);
            response = Insert(emailAccount);
            return response.SetMessage(response.Success
                ? T("EmailAccount_Message_CreateSuccessfully")
                : T("EmailAccount_Message_CreateFailure"));
        }


        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateEmailAccountData(XEditableModel model)
        {
            var emailAccount = GetById(model.Pk);
            if (emailAccount != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (EmailAccountManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new EmailAccountManageModel(emailAccount);
                    manageModel.SetProperty(model.Name, value);

                    var validationResults = manageModel.ValidateModel();

                    if (validationResults.Any())
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = validationResults.BuildValidationMessages()
                        };
                    }

                    #endregion

                    emailAccount.SetProperty(model.Name, value);
                    var response = Update(emailAccount);
                    return response.SetMessage(response.Success
                        ? T("EmailAccount_Message_UpdateEmailAccountInfoSuccessfully")
                        : T("EmailAccount_Message_UpdateFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("EmailAccount_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("EmailAccount_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete email account by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteEmailAccount(int id)
        {
            // Stop deletion if ralation found
            var item = GetById(id);

            if (item != null)
            {
                if (item.EmailLogs.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("EmailAccount_Message_DeleteFailureBasedOnRelatedEmailLogs")
                    };
                }

                // Delete the Email Account
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("EmailAccount_Message_DeleteSuccessfully")
                    : T("EmailAccount_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("EmailAccount_Message_DeleteSuccessfully")
            };
        }

        #endregion
    }
}