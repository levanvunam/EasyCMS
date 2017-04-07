using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC;
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
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.EmailTemplates;
using EzCMS.Core.Services.EmailAccounts;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.EmailTemplates
{
    public class EmailTemplateService : ServiceHelper, IEmailTemplateService
    {
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;

        public EmailTemplateService(IRepository<EmailTemplate> emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
        }

        /// <summary>
        /// Parse the email
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="type"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailTemplateResponseModel ParseEmail<TModel>(EmailEnums.EmailTemplateType type, TModel model)
        {
            var emailTemlate = _emailTemplateRepository.FetchFirst(e => e.Type == type);
            return ParseEmail(emailTemlate, model);
        }

        /// <summary>
        /// Parse the email from template and model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailTemplateResponseModel ParseEmail<TModel>(EmailTemplate template, TModel model)
        {
            if (template == null) return null;

            var subjectCacheName = template.Type.GetEnumName().GetTemplateCacheName(template.Subject);
            var bodyCacheName = template.Type.GetEnumName().GetTemplateCacheName(template.Body);
            var response = new EmailTemplateResponseModel
            {
                From = template.From,
                FromName = template.FromName,
                CC = MailUtilities.FormatEmailList(template.CC),
                CCList = MailUtilities.ParseEmailList(template.CC),
                BCC = MailUtilities.FormatEmailList(template.BCC),
                BCCList = MailUtilities.ParseEmailList(template.BCC),
                Subject = EzRazorEngineHelper.CompileAndRun(template.Subject, model, subjectCacheName),
                Body = EzRazorEngineHelper.CompileAndRun(template.Body, model, bodyCacheName)
            };

            return response;
        }

        /// <summary>
        /// Get email template detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EmailTemplateDetailModel GetEmailTemplateDetailModel(int id)
        {
            var emailTemplate = GetById(id);
            return emailTemplate != null ? new EmailTemplateDetailModel(emailTemplate) : null;
        }

        #region Validation

        /// <summary>
        /// Check if email template exists
        /// </summary>
        /// <param name="emailTemplateId"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public bool IsEmailTemplateExisted(int? emailTemplateId, string subject)
        {
            return Fetch(u => u.Name.Equals(subject) && u.Id != emailTemplateId).Any();
        }

        #endregion

        /// <summary>
        /// Get select list email template by email Template type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetEmailTemplates(EmailEnums.EmailTemplateType type)
        {
            return Fetch(e => e.Type == type).Select(e => new SelectListItem
            {
                Text = e.Name,
                Value = SqlFunctions.StringConvert((double) e.Id).Trim()
            }
                );
        }

        #region Base

        public IQueryable<EmailTemplate> GetAll()
        {
            return _emailTemplateRepository.GetAll();
        }

        public IQueryable<EmailTemplate> Fetch(Expression<Func<EmailTemplate, bool>> expression)
        {
            return _emailTemplateRepository.Fetch(expression);
        }

        public EmailTemplate FetchFirst(Expression<Func<EmailTemplate, bool>> expression)
        {
            return _emailTemplateRepository.FetchFirst(expression);
        }

        public EmailTemplate GetById(object id)
        {
            return _emailTemplateRepository.GetById(id);
        }

        internal ResponseModel Insert(EmailTemplate emailTemplate)
        {
            return _emailTemplateRepository.Insert(emailTemplate);
        }

        internal ResponseModel Update(EmailTemplate emailTemplate)
        {
            return _emailTemplateRepository.Update(emailTemplate);
        }

        internal ResponseModel Delete(EmailTemplate emailTemplate)
        {
            return _emailTemplateRepository.Delete(emailTemplate);
        }

        internal ResponseModel Delete(object id)
        {
            return _emailTemplateRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _emailTemplateRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid

        /// <summary>
        /// Search the templates
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchEmailTemplates(JqSearchIn si)
        {
            var data = GetAll();

            var emailTemplates = Maps(data);

            return si.Search(emailTemplates);
        }

        /// <summary>
        /// Export templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var emailTemplates = Maps(data);

            var exportData = si.Export(emailTemplates, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search email template
        /// </summary>
        /// <returns></returns>
        private IQueryable<EmailTemplateModel> Maps(IQueryable<EmailTemplate> emailTemplates)
        {
            return emailTemplates.Select(emailTemplate => new EmailTemplateModel
            {
                Id = emailTemplate.Id,
                Subject = emailTemplate.Subject,
                Type = emailTemplate.Type,
                From = emailTemplate.From,
                FromName = emailTemplate.FromName,
                CC = emailTemplate.CC,
                BCC = emailTemplate.BCC,
                RecordOrder = emailTemplate.RecordOrder,
                Created = emailTemplate.Created,
                CreatedBy = emailTemplate.CreatedBy,
                LastUpdate = emailTemplate.LastUpdate,
                LastUpdateBy = emailTemplate.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get email template manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EmailTemplateManageModel GetEmailTemplateManageModel(int? id = null)
        {
            var emailTemplate = GetById(id);
            if (emailTemplate != null)
            {
                return new EmailTemplateManageModel(emailTemplate);
            }
            return new EmailTemplateManageModel();
        }

        /// <summary>
        /// Save email template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveEmailTemplate(EmailTemplateManageModel model)
        {
            var emailTemplate = GetById(model.Id);
            if (emailTemplate != null)
            {
                emailTemplate.Subject = model.Subject;
                emailTemplate.From = model.From;
                emailTemplate.FromName = model.FromName;
                emailTemplate.CC = model.CC;
                emailTemplate.BCC = model.BCC;
                emailTemplate.Body = model.Body;
                var response = Update(emailTemplate);
                return response.SetMessage(response.Success
                    ? T("EmailTemplate_Message_UpdateSuccessfully")
                    : T("EmailTemplate_Message_UpdateFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("EmailTemplate_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateEmailTemplateData(XEditableModel model)
        {
            var emailTemplate = GetById(model.Pk);
            if (emailTemplate != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (EmailTemplateManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new EmailTemplateManageModel(emailTemplate);
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

                    emailTemplate.SetProperty(model.Name, value);
                    var response = Update(emailTemplate);
                    return response.SetMessage(response.Success
                        ? T("EmailTemplate_Message_UpdateEmailTemplateInfoSuccessfully")
                        : T("EmailTemplate_Message_UpdateEmailTemplateInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("EmailTemplate_Message_PropertyNotFound")
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("EmailTemplate_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete email template by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteEmailTemplate(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                // Delete the email template
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("EmailTemplate_Message_DeleteSuccessfully")
                    : T("EmailTemplate_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("EmailTemplate_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Send Test Email Template 

        /// <summary>
        /// Send test email template
        /// </summary>
        /// <param name="emailTemplateSendTestModel"></param>
        /// <returns></returns>
        public ResponseModel SendTestEmailTemplate(EmailTemplateSendTestModel emailTemplateSendTestModel)
        {
            var emailTemplate = GetById(emailTemplateSendTestModel.Id);

            if (emailTemplate == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("EmailTemplate_Message_ObjectNotFound")
                };
            }
            var type = Type.GetType(emailTemplate.DataType);

            if (type != null && type.GetMethod("GetMockData") != null)
            {
                try
                {
                    var emailAccountService = HostContainer.GetInstance<IEmailAccountService>();

                    var defaultAccount = emailAccountService.GetDefaultAccount();
                    if (defaultAccount == null)
                    {
                        defaultAccount = emailAccountService.GetAll().FirstOrDefault();
                        if (defaultAccount == null)
                        {
                            return new ResponseModel
                            {
                                Success = false,
                                Message = T("EmailTemplate_Message_MissingDefaultAccount")
                            };
                        }
                    }

                    dynamic templateModel = Activator.CreateInstance(type);
                    var mockData = templateModel.GetMockData();
                    var emailResponse = ParseEmail(emailTemplate, mockData);

                    var emailModel = new EmailModel
                    {
                        To = emailTemplateSendTestModel.To,
                        From = !string.IsNullOrEmpty(emailTemplate.From) ? emailTemplate.From : defaultAccount.Email,
                        FromName = "Email Test Service",
                        Bcc = emailTemplateSendTestModel.BCC,
                        CC = emailTemplateSendTestModel.CC,
                        Subject = emailTemplateSendTestModel.Subject,
                        Body = emailResponse.Body,
                        Attachment = string.Empty
                    };
                    return emailAccountService.SendEmailDirectly(defaultAccount, emailModel);
                }
                catch (Exception exception)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("EmailTemplate_Message_SendTestEmailFailure"),
                        DetailMessage = exception.Message
                    };
                }
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("EmailTemplate_Message_SendTestEmailFailure")
            };
        }

        /// <summary>
        /// Get model for sending test email
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EmailTemplateSendTestModel GetEmailTemplateSendTestModel(int? id)
        {
            var emailTemplate = GetById(id);
            return new EmailTemplateSendTestModel(emailTemplate);
        }

        #endregion
    }
}