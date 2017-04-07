using Ez.Framework.Core.Logging;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Html;
using Ez.Framework.Utilities.Mails.Models;
using Ez.Framework.IoC;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Services.EmailAccounts;
using EzCMS.Core.Services.SiteSettings;
using log4net;
using System;
using System.Web;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Framework.Logging
{
    public class Logger : ILogger
    {
        private readonly ILog _logger;

        private ErrorHandlingSetting _errorHandlingSetting;
        private ErrorHandlingSetting ErrorHandlingSetting
        {
            get
            {
                if (_errorHandlingSetting != null)
                {
                    return _errorHandlingSetting;
                }

                var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
                _errorHandlingSetting = siteSettingService.LoadSetting<ErrorHandlingSetting>();
                return _errorHandlingSetting;
            }
        }

        public Logger(Type type)
        {
            _logger = LogManager.GetLogger(type);
        }

        #region Info Log

        public void Info(string message)
        {
            if (ErrorHandlingSetting.LoggingOption == LoggingOption.LogAndSendEmail ||
                ErrorHandlingSetting.LoggingOption == LoggingOption.LogOnly)
            {
                _logger.Info(message);
            }
        }

        #endregion

        #region Warning Log

        public void Warn(string message)
        {
            if (ErrorHandlingSetting.LoggingOption == LoggingOption.LogAndSendEmail ||
                ErrorHandlingSetting.LoggingOption == LoggingOption.LogOnly)
            {
                _logger.Warn(message);
            }
        }

        public void Warn(Exception exception)
        {
            Warn(exception.Message, exception);
        }

        public void Warn(string message, Exception exception)
        {
            if (ErrorHandlingSetting.LoggingOption == LoggingOption.LogAndSendEmail ||
                ErrorHandlingSetting.LoggingOption == LoggingOption.LogOnly)
            {
                _logger.Warn(message, exception);
            }
        }

        #endregion

        #region Debug Log

        public void Debug(string message)
        {
            if (ErrorHandlingSetting.LoggingOption == LoggingOption.LogAndSendEmail ||
                ErrorHandlingSetting.LoggingOption == LoggingOption.LogOnly)
            {
                _logger.Debug(message);
            }
        }

        #endregion

        #region Error Log

        public void Error(string message)
        {
            message = message.BuildMessageBody();
            if (ErrorHandlingSetting.LoggingOption == LoggingOption.LogAndSendEmail ||
                ErrorHandlingSetting.LoggingOption == LoggingOption.LogOnly)
            {
                _logger.Error(message);
            }

            SendEmail(message, message);
        }

        public void Error(Exception exception)
        {
            Error(exception.Message, exception);
        }

        public void Error(string message, Exception exception)
        {
            if (ErrorHandlingSetting.LoggingOption == LoggingOption.LogAndSendEmail ||
                ErrorHandlingSetting.LoggingOption == LoggingOption.LogOnly)
            {
                _logger.Error(message, exception);
            }

            SendEmail(message, exception.BuildFullExceptionMessage());
        }

        #endregion

        #region Fatal Log

        public void Fatal(string message)
        {
            message = message.BuildMessageBody();

            if (ErrorHandlingSetting.LoggingOption == LoggingOption.LogAndSendEmail ||
                ErrorHandlingSetting.LoggingOption == LoggingOption.LogOnly)
            {
                _logger.Fatal(message);
            }

            SendEmail(message, message);
        }

        public void Fatal(Exception exception)
        {
            Fatal(exception.Message, exception);
        }

        public void Fatal(string message, Exception exception)
        {
            if (ErrorHandlingSetting.LoggingOption == LoggingOption.LogAndSendEmail ||
                ErrorHandlingSetting.LoggingOption == LoggingOption.LogOnly)
            {
                Fatal(exception.BuildFullExceptionMessage());
            }

            SendEmail(message, exception.BuildFullExceptionMessage());
        }

        #endregion

        /// <summary>
        /// Send logging email
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public void SendEmail(string subject, string body)
        {
            if (ErrorHandlingSetting.LoggingOption == LoggingOption.LogAndSendEmail ||
                ErrorHandlingSetting.LoggingOption == LoggingOption.SendEmailOnly)
            {
                try
                {
                    var emailAccountService = HostContainer.GetInstance<IEmailAccountService>();
                    var account = emailAccountService.GetDefaultAccount();

                    if (account != null)
                    {
                        var siteDomain = string.Empty;
                        if (HttpContext.Current != null)
                        {
                            siteDomain = "/".ToAbsoluteUrl();
                        }
                        subject = string.Format("EzCMS Error From site {0} - {1}", siteDomain, subject);

                        emailAccountService.SendEmailDirectly(account, new EmailModel
                        {
                            From = ErrorHandlingSetting.EmailFrom,
                            FromName = ErrorHandlingSetting.EmailFrom,
                            To = ErrorHandlingSetting.EmailTo,
                            ToName = ErrorHandlingSetting.EmailTo,
                            Subject = subject,
                            Body = body.Nl2Br()
                        });
                    }
                }
                catch (Exception)
                {
                    // Cannot send email
                    _logger.Error("Cannot sending error email.");
                }
            }
        }
    }
}
