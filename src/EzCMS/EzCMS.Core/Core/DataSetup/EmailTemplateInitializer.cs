using Ez.Framework.Core.Entity.Extensions;
using Ez.Framework.Core.Entity.Intialize;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Contacts.Emails;
using EzCMS.Core.Models.ProtectedDocuments.Emails;
using EzCMS.Core.Models.Subscriptions.Emails;
using EzCMS.Core.Models.Users.Emails;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;
using System.Data.Entity;

namespace EzCMS.Core.Core.DataSetup
{
    public class EmailTemplateInitializer : IDataInitializer
    {
        /// <summary>
        /// Priority of initializer
        /// </summary>
        /// <returns></returns>
        public DataInitializerPriority Priority()
        {
            return DataInitializerPriority.High;
        }

        #region Initialize

        /// <summary>
        /// Initialize default settings
        /// </summary>
        public void Initialize(DbContext ezContext)
        {
            var context = ezContext as EzCMSEntities;
            if (context != null)
            {
                var emailTemplates = new[]
                {
                    new EmailTemplate
                    {
                        Name = "Forgot Password Notification",
                        Subject = "Forgot Password Notification",
                        Body =
                            DataInitializeHelper.GetResourceContent("ForgotPasswordTemplate.cshtml",
                                DataSetupResourceType.EmailTemplate),
                        DataType = typeof (ForgotPasswordEmailModel).AssemblyQualifiedName,
                        Type = EmailEnums.EmailTemplateType.ForgotPassword,
                    },
                    new EmailTemplate
                    {
                        Name = "Register Email Notification",
                        Subject = "Register Email Notification",
                        Body =
                            DataInitializeHelper.GetResourceContent("RegisterUserTemplate.cshtml",
                                DataSetupResourceType.EmailTemplate),
                        DataType = typeof (RegisterEmailModel).AssemblyQualifiedName,
                        Type = EmailEnums.EmailTemplateType.RegisterUser,
                    },
                    new EmailTemplate
                    {
                        Name = "User Created Notification Template",
                        Subject = "User Created Notification",
                        Body = DataInitializeHelper.GetResourceContent("UserCreatedTemplate.cshtml",
                            DataSetupResourceType.EmailTemplate),
                        DataType = typeof (UserCreatedEmailModel).AssemblyQualifiedName,
                        Type = EmailEnums.EmailTemplateType.UserCreatedNotification
                    },
                    new EmailTemplate
                    {
                        Name = "Contact Form Notification",
                        Subject = "Contact Form Notification",
                        Body =
                            DataInitializeHelper.GetResourceContent("ContactFormTemplate.cshtml",
                                DataSetupResourceType.EmailTemplate),
                        DataType = typeof (ContactFormEmailModel).AssemblyQualifiedName,
                        Type = EmailEnums.EmailTemplateType.ContactForm,
                    },
                    new EmailTemplate
                    {
                        Name = "Protected Document Feedback",
                        Subject = "Protected Document Feedback",
                        Body =
                            DataInitializeHelper.GetResourceContent("ProtectedDocumentFeedbackTemplate.cshtml",
                                DataSetupResourceType.EmailTemplate),
                        DataType = typeof (ProtectedDocumentEmailModel).AssemblyQualifiedName,
                        Type = EmailEnums.EmailTemplateType.ProtectedDocumentForm,
                    },
                    new EmailTemplate
                    {
                        Name = "Subscribe Notification Template",
                        Subject = "Subscribe Notification",
                        Body = DataInitializeHelper.GetResourceContent("SubscribeNotificationTemplate.cshtml",
                            DataSetupResourceType.EmailTemplate),
                        DataType = typeof (SubscribeNoticationEmailModel).AssemblyQualifiedName,
                        Type = EmailEnums.EmailTemplateType.SubscribeNotification
                    },
                    new EmailTemplate
                    {
                        Name = "Account Expired Notification Template",
                        Subject = "Account Expired Notification",
                        Body = DataInitializeHelper.GetResourceContent("AccountExpiredNotificationTemplate.cshtml",
                            DataSetupResourceType.EmailTemplate),
                        DataType = typeof (NotifyAccountExpiredEmailModel).AssemblyQualifiedName,
                        Type = EmailEnums.EmailTemplateType.AccountExpiredNotification
                    },
                    new EmailTemplate
                    {
                        Name = "Deactivate Expired Account Notification Template",
                        Subject = "Deactivate Expired Account Notification",
                        Body =
                            DataInitializeHelper.GetResourceContent("DeactiveExpiredAccountNotificationTemplate.cshtml",
                                DataSetupResourceType.EmailTemplate),
                        DataType = typeof (NotifyAccountDeactivatedEmailModel).AssemblyQualifiedName,
                        Type = EmailEnums.EmailTemplateType.DeactivateAccountNotification
                    }
                };

                context.EmailTemplates.AddIfNotExist(t => t.Name, emailTemplates);
            }
        }

        #endregion
    }

}
