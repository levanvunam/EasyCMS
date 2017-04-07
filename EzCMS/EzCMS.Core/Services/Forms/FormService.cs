using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using Ez.Framework.Core.Entity.Models;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Html;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Contacts.Emails;
using EzCMS.Core.Models.Forms;
using EzCMS.Core.Models.Forms.Widgets;
using EzCMS.Core.Models.Forms.FormConfigurations;
using EzCMS.Core.Models.Forms.Render;
using EzCMS.Core.Models.Forms.Setup;
using EzCMS.Core.Models.Forms.Setup.BuildFormSetup;
using EzCMS.Core.Services.Contacts;
using EzCMS.Core.Services.EmailLogs;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Forms
{
    public class FormService : ServiceHelper, IFormService
    {
        private readonly IContactService _contactService;
        private readonly IEmailLogService _emailLogService;
        private readonly IRepository<FormComponentTemplate> _formComponentTemplateRepository;
        private readonly IRepository<FormDefaultComponent> _formDefaultComponentRepository;
        private readonly IRepository<Form> _formRepository;
        private readonly IRepository<FormTab> _formTabRepository;
        private readonly ISiteSettingService _siteSettingService;

        public FormService(IRepository<Form> formRepository, IRepository<FormTab> formTabRepository,
            IRepository<FormComponentTemplate> formComponentTemplateRepository,
            IRepository<FormDefaultComponent> formDefaultComponentRepository, ISiteSettingService siteSettingService,
            IContactService contactService, IEmailLogService emailLogService)
        {
            _formRepository = formRepository;
            _formTabRepository = formTabRepository;
            _formDefaultComponentRepository = formDefaultComponentRepository;
            _formComponentTemplateRepository = formComponentTemplateRepository;
            _siteSettingService = siteSettingService;
            _contactService = contactService;
            _emailLogService = emailLogService;
        }

        /// <summary>
        /// Load form
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="formSubmitted"></param>
        /// <returns></returns>
        public FormIframeLoaderModel LoadForm(string formId, bool formSubmitted = false)
        {
            try
            {
                var id = PasswordUtilities.ComplexDecrypt(formId).ToInt();
                var form = GetById(id);

                if (form != null)
                {
                    return new FormIframeLoaderModel(form, formSubmitted);
                }
            }
            catch
            {
                //Invalid parameters
            }

            return null;
        }

        /// <summary>
        /// Delete form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteForm(int id)
        {
            var response = _formRepository.SetRecordDeleted(id);

            return
                response.SetMessage(response.Success
                    ? T("Form_Message_DeleteSuccessfully")
                    : T("Form_Message_DeleteFailure"));
        }

        /// <summary>
        /// Get forms
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetForms()
        {
            return Fetch(f => f.Active).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = SqlFunctions.StringConvert((double) f.Id).Trim()
            });
        }

        #region Base

        public IQueryable<Form> GetAll()
        {
            return _formRepository.GetAll();
        }

        public IQueryable<Form> Fetch(Expression<Func<Form, bool>> expression)
        {
            return _formRepository.Fetch(expression);
        }

        public Form FetchFirst(Expression<Func<Form, bool>> expression)
        {
            return _formRepository.FetchFirst(expression);
        }

        public Form GetById(object id)
        {
            return _formRepository.GetById(id);
        }

        internal ResponseModel Insert(Form form)
        {
            return _formRepository.Insert(form);
        }

        internal ResponseModel Update(Form form)
        {
            return _formRepository.Update(form);
        }

        internal ResponseModel Delete(Form form)
        {
            return _formRepository.Delete(form);
        }

        internal ResponseModel Delete(object id)
        {
            return _formRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _formRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the forms
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchForms(JqSearchIn si)
        {
            var data = SearchForm();

            var forms = Maps(data);

            return si.Search(forms);
        }

        /// <summary>
        /// Export forms
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = SearchForm();

            var forms = Maps(data);

            var exportData = si.Export(forms, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Search form
        /// </summary>
        /// <returns></returns>
        public IQueryable<Form> SearchForm()
        {
            return Fetch(f => f.Active);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        private IQueryable<FormModel> Maps(IQueryable<Form> forms)
        {
            return forms.Select(m => new FormModel
            {
                Id = m.Id,
                Name = m.Name,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion

        #region Setup

        #region Build Form

        /// <summary>
        /// Get build form setup
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BuildFormSetupModel GetBuildFormSetupModel(int? id)
        {
            var form = GetById(id);

            if (form != null)
            {
                return new BuildFormSetupModel(form);
            }

            return new BuildFormSetupModel();
        }

        /// <summary>
        /// Save form content
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveFormSetupModel(BuildFormSetupModel model)
        {
            ResponseModel response;
            var form = GetById(model.Id);

            if (form != null)
            {
                form.Content = model.Content;
                form.JsonContent = model.JsonContent;

                response = Update(form);

                return response.SetMessage(response.Success
                    ? T("Form_Message_UpdateSuccessfully")
                    : T("Form_Message_UpdateFailure"));
            }

            Mapper.CreateMap<BuildFormSetupModel, Form>();
            form = Mapper.Map<BuildFormSetupModel, Form>(model);
            form.Active = false;

            // Create inactive form
            response = Insert(form);
            return response.SetMessage(response.Success
                ? T("Form_Message_CreateSuccessfully")
                : T("Form_Message_CreateFailure"));
        }

        #region Methods

        /// <summary>
        /// Get form component html templates
        /// </summary>
        /// <returns></returns>
        public string GetFormTemplates()
        {
            var templates = _formComponentTemplateRepository.GetAll();

            var stringBuilder = new StringBuilder();
            foreach (var template in templates)
            {
                var div = new TagBuilder("div");
                var attributes = new RouteValueDictionary
                {
                    {"data-template", template.Name},
                    {"data-content", template.Content}
                };

                div.MergeAttributes(attributes);

                stringBuilder.AppendLine(div.ToString());
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Get form configuration model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FormConfigurationModel GetConfigurations(int? id)
        {
            var form = GetById(id);

            if (form != null)
            {
                return new FormConfigurationModel
                {
                    FormComponents = form.JsonContent,
                    TabComponents = GetTabConfigurations()
                };
            }

            return new FormConfigurationModel
            {
                FormComponents = GetDefaultFormComponents(),
                TabComponents = GetTabConfigurations()
            };
        }

        #region Private Methods

        /// <summary>
        /// Get tab configurations
        /// </summary>
        /// <returns></returns>
        private string GetTabConfigurations()
        {
            var tabs =
                _formTabRepository.GetAll().Include(t => t.FormComponents.Select(c => c.FormComponentFields)).ToList();
            var data = tabs.Select(t => new FormTabModel
            {
                title = t.Name,
                data = t.FormComponents.Select(c => new FormData
                {
                    title = c.Name,
                    template = c.FormComponentTemplate.Name,
                    fields = GetFields(c.FormComponentFields)
                })
            });

            return SerializeUtilities.Serialize(data);
        }

        /// <summary>
        /// Get default components
        /// </summary>
        /// <returns></returns>
        private string GetDefaultFormComponents()
        {
            var components = _formDefaultComponentRepository.GetAll().ToList();
            var data = components.Select(c => new FormData
            {
                title = c.Name,
                template = c.FormComponentTemplate.Name,
                fields = GetFields(c.FormDefaultComponentFields)
            });

            return SerializeUtilities.Serialize(data);
        }

        /// <summary>
        /// Get fields
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private dynamic GetFields(IEnumerable<FormComponentFieldBase> fields)
        {
            //TODO: update here using http://stackoverflow.com/questions/26874014/serialize-data-to-json-string-with-dynamic-property-names
            const string jsonString = @"
""{0}"" : {1}";
            var json = string.Join(",", fields.Select(field => string.Format(jsonString, field.Name, field.Attributes)));

            json = string.Format(@"
{{{0}}}", json);

            return SerializeUtilities.Deserialize<dynamic>(json);
        }

        #endregion

        #endregion

        #endregion

        #region Configurations

        /// <summary>
        /// Get form configurations
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ConfigurationSetupModel GetConfigurationSetupModel(int id)
        {
            var form = GetById(id);

            if (form != null)
            {
                return new ConfigurationSetupModel(form);
            }

            return null;
        }

        /// <summary>
        /// Save form configurations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveConfiguration(ConfigurationSetupModel model)
        {
            var form = GetById(model.Id);

            if (form != null)
            {
                form.Name = model.Name;
                form.FromName = model.FromName;
                form.FromEmail = model.FromEmail;
                form.ThankyouMessage = model.ThankyouMessage;
                form.AllowAjaxSubmit = model.AllowAjaxSubmit;
                form.StyleId = model.StyleId;

                form.SendSubmitFormEmail = model.SendSubmitFormEmail;
                form.EmailTo = model.EmailTo;

                form.SendNotificationEmail = model.SendNotificationEmail;
                form.NotificationSubject = model.NotificationSubject;
                form.NotificationBody = model.NotificationBody;
                form.NotificationEmailTo = model.NotificationEmailTo;

                form.SendAutoResponse = model.SendAutoResponse;
                form.AutoResponseSubject = model.AutoResponseSubject;
                form.AutoResponseBody = model.AutoResponseBody;

                // Active form
                form.Active = true;

                var response = Update(form);

                return response.SetMessage(response.Success
                    ? T("Form_Message_UpdateSuccessfully")
                    : T("Form_Message_UpdateFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("Form_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Embedded

        /// <summary>
        /// Get embedded script model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EmbeddedScriptSetupModel GetEmbeddedScriptModel(int id)
        {
            var form = GetById(id);

            if (form != null)
            {
                return new EmbeddedScriptSetupModel(form);
            }

            return null;
        }

        #endregion

        #region Preview

        /// <summary>
        /// Save draft
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ResponseModel SaveDraft(int? id, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                if (id.HasValue)
                {
                    return new ResponseModel
                    {
                        Success = true
                    };
                }
            }
            else
            {
                var token = StringUtilities.GetUniqueString();

                HttpContext.Current.Session[token] = content;

                return new ResponseModel
                {
                    Success = true,
                    Data = token
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("Form_Message_InvalidParameters")
            };
        }

        /// <summary>
        /// Load preview version for form
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public FormIframeLoaderModel LoadPreview(int? id, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                var form = GetById(id);
                if (form != null)
                {
                    return new FormIframeLoaderModel(form);
                }
            }
            else
            {
                var draftContent = HttpContext.Current.Session[token];
                if (draftContent != null)
                {
                    return new FormIframeLoaderModel
                    {
                        Content = ParseForm(token, draftContent.ToString())
                    };
                }
            }

            return null;
        }

        #endregion

        #endregion

        #region Form Apis

        /// <summary>
        /// Load embedded script
        /// </summary>
        /// <param name="formEncryptedId"></param>
        /// <returns></returns>
        public string LoadEmbeddedScript(string formEncryptedId)
        {
            try
            {
                var id = PasswordUtilities.ComplexDecrypt(formEncryptedId).ToInt();

                return LoadEmbeddedScript(id);
            }
            catch
            {
                //Invalid parameters
            }

            return HtmlUtilities.BuildScript(ScriptAction.Alert, T("Form_Message_InvalidFormId"));
        }

        /// <summary>
        /// Load embedded script
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        public string LoadEmbeddedScript(int formId)
        {
            var form = GetById(formId);

            if (form != null)
            {
                var formRenderModel = new FormWidget(form);

                var formBuilderSetting = _siteSettingService.LoadSetting<FormBuilderSetting>();
                var cacheName = SettingNames.FormBuilderSetting.GetTemplateCacheName(formBuilderSetting.EmbeddedTemplate);

                return RazorEngineHelper.CompileAndRun(formBuilderSetting.EmbeddedTemplate, formRenderModel, null,
                    cacheName);
            }
            return HtmlUtilities.BuildScript(ScriptAction.Alert, T("Form_Message_InvalidFormId"));
        }

        /// <summary>
        /// Load form loader script
        /// </summary>
        /// <param name="formEncryptedId"></param>
        /// <returns></returns>
        public string LoadLoaderScript(string formEncryptedId)
        {
            try
            {
                var id = PasswordUtilities.ComplexDecrypt(formEncryptedId).ToInt();

                return LoadLoaderScript(id);
            }
            catch
            {
                //Invalid parameters
            }

            return HtmlUtilities.BuildScript(ScriptAction.Alert, T("Form_Message_InvalidFormId"), false);
        }

        /// <summary>
        /// Load form loader script
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        public string LoadLoaderScript(int formId)
        {
            var form = GetById(formId);

            if (form != null)
            {
                var formRenderModel = new FormLoaderScriptModel(form);

                var formBuilderSetting = _siteSettingService.LoadSetting<FormBuilderSetting>();
                var cacheName =
                    SettingNames.FormBuilderSetting.GetTemplateCacheName(formBuilderSetting.RenderScriptTemplate);

                return RazorEngineHelper.CompileAndRun(formBuilderSetting.RenderScriptTemplate, formRenderModel,
                    null, cacheName);
            }

            return HtmlUtilities.BuildScript(ScriptAction.Alert, T("Form_Message_InvalidFormId"), false);
        }

        /// <summary>
        /// Submit form
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="contact"></param>
        /// <param name="communication"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ResponseModel SubmitForm(string formId, Contact contact, ContactCommunication communication,
            NameValueCollection collection)
        {
            try
            {
                var id = PasswordUtilities.ComplexDecrypt(formId).ToInt();

                var form = GetById(id);

                if (form != null)
                {
                    var formData = collection.AllKeys.SelectMany(collection.GetValues, (k, v) => new ContactInformation
                    {
                        Key = k,
                        Value = v
                    }).ToList();

                    //Save contact and communication
                    contact = _contactService.SaveForm(contact, communication, formData);

                    #region Form Data

                    var formEmailModel =
                        collection.AllKeys.SelectMany(collection.GetValues, (k, v) => new ContactInformation
                        {
                            Key = k.CamelFriendly(),
                            Value = v
                        }).ToList();

                    var formBuilderSetting = _siteSettingService.LoadSetting<FormBuilderSetting>();

                    var cacheName =
                        SettingNames.FormBuilderSetting.GetTemplateCacheName(formBuilderSetting.SubmitFormBodyTemplate);

                    var formDataBody = RazorEngineHelper.CompileAndRun(formBuilderSetting.SubmitFormBodyTemplate,
                        formEmailModel, null, cacheName);

                    #endregion

                    if (form.SendSubmitFormEmail && !string.IsNullOrEmpty(form.EmailTo))
                    {
                        var email = new EmailLog
                        {
                            From = form.FromEmail,
                            FromName = form.FromName,
                            To = form.EmailTo,
                            ToName = form.EmailTo,
                            Subject = formBuilderSetting.SubmitFormSubject,
                            Body = formDataBody
                        };

                        _emailLogService.CreateEmail(email, true);
                    }

                    if (form.SendNotificationEmail && !string.IsNullOrEmpty(form.NotificationEmailTo))
                    {
                        var notificationBodyStringBuilder = new StringBuilder();
                        notificationBodyStringBuilder.AppendLine(form.NotificationBody);
                        notificationBodyStringBuilder.AppendLine(formDataBody);

                        var email = new EmailLog
                        {
                            From = form.FromEmail,
                            FromName = form.FromName,
                            To = form.NotificationEmailTo,
                            ToName = form.NotificationEmailTo,
                            Subject = form.NotificationSubject,
                            Body = notificationBodyStringBuilder.ToString()
                        };

                        _emailLogService.CreateEmail(email, true);
                    }

                    if (form.SendAutoResponse)
                    {
                        // Get email from form data
                        var emailAddress =
                            formData.FirstOrDefault(
                                f => f.Key.Contains("Email", StringComparison.CurrentCultureIgnoreCase));

                        if (emailAddress != null && !string.IsNullOrEmpty(emailAddress.Value))
                        {
                            var toName = contact.FullName;
                            var email = new EmailLog
                            {
                                From = form.FromEmail,
                                FromName = form.FromName,
                                To = emailAddress.Value,
                                ToName = string.IsNullOrWhiteSpace(toName) ? emailAddress.Value : toName,
                                Subject = form.AutoResponseSubject,
                                Body = form.AutoResponseBody
                            };

                            _emailLogService.CreateEmail(email, true);
                        }
                    }

                    return new ResponseModel
                    {
                        Success = true,
                        Message = form.ThankyouMessage
                    };
                }
            }
            catch (Exception)
            {
                // Form parameters invalid
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("Form_Message_InvalidFormId")
            };
        }

        #region Parse form

        /// <summary>
        /// Reformat form content
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string ParseForm(string name, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }

            content = content.ParseProperties(typeof (FormElementsModel), true);
            var cacheName = name.GetTemplateCacheName(content);
            var formElementsModel = new FormElementsModel();

            content = RazorEngineHelper.CompileAndRun(content, formElementsModel, null, cacheName);

            // Remove hidden html
            content = content.Replace(EzCMSContants.AddElementClass, string.Empty);
            return content.RemoveElementsHasClass(EzCMSContants.RemoveElementClass);
        }

        #endregion

        #endregion
    }
}