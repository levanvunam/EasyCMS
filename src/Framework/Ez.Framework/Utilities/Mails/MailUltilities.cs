using Ez.Framework.Configurations;
using Ez.Framework.Utilities.Mails.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Ez.Framework.Utilities.Mails
{
    /// <summary>
    /// Mail utilities
    /// </summary>
    public class MailUtilities
    {
        private readonly EmailSetting _mailSettings;

        public MailUtilities(EmailSetting mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public void SendEmail(string from, string fromName, string to, string cc, string bcc, bool isHtml, string subject, string body)
        {
            var toList = ParseEmailList(to);
            var ccList = ParseEmailList(cc);
            var bccList = ParseEmailList(bcc);

            SendEmail(from, fromName, toList, ccList, bccList, isHtml, subject, body);
        }

        public void SendEmail(string from, string fromName, List<string> to, List<string> cc, List<string> bcc, bool isHtml, string subject, string body)
        {
            var mail = new MailMessage();
            var client = new SmtpClient
            {
                Port = _mailSettings.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = _mailSettings.Host,
                EnableSsl = _mailSettings.EnableSsl,
                Timeout = _mailSettings.Timeout,
                Credentials = new NetworkCredential(_mailSettings.User, _mailSettings.Password)
            };

            if (client.Timeout <= 0)
            {
                client.Timeout = FrameworkConstants.DefaultEmailSendTimeout;
            }

            mail.From = new MailAddress(from, !string.IsNullOrEmpty(fromName) ? fromName : from);

            #region Mail To

            foreach (var email in to)
            {
                try
                {
                    mail.To.Add(email);
                }
                catch (System.Exception)
                {
                    //Do nothing
                }
            }

            #endregion

            #region CC

            foreach (var email in cc)
            {
                try
                {
                    mail.CC.Add(email);
                }
                catch (System.Exception)
                {
                    //Do nothing
                }
            }

            #endregion

            #region BCC

            foreach (var email in bcc)
            {
                try
                {
                    mail.Bcc.Add(email);
                }
                catch (System.Exception)
                {
                    //Do nothing
                }
            }

            #endregion

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = isHtml;
            client.Send(mail);
        }

        public void SendEmail(EmailModel model)
        {
            SendEmail(model.From, model.FromName, model.To, model.CC, model.Bcc, true, model.Subject, model.Body);
        }

        /// <summary>
        /// Parse email list from email string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<string> ParseEmailList(string input)
        {
            input = FormatEmailList(input);

            if (string.IsNullOrEmpty(input)) return new List<string>();

            var toList = input.Split(';').Select(email => email.Trim()).ToList();

            return toList;
        }

        /// <summary>
        /// Format email list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FormatEmailList(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            //Replace separate character
            input = input.Replace(',', ';');

            return input;
        }
    }
}
