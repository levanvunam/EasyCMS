using System;
using EzCMS.Core.Framework.Context;

namespace EzCMS.Core.Models.Users.Emails
{
    public class NotifyAccountExpiredEmailModel : EmailTemplateSetupModel<NotifyAccountExpiredEmailModel>
    {
        #region Public Properties

        public string Username { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string ExtendExpirationDateLink { get; set; }

        #endregion

        #region Method

        public override NotifyAccountExpiredEmailModel GetMockData()
        {
            return new NotifyAccountExpiredEmailModel
            {
                Username = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.Username : "Test Username",
                FullName = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.FullName : "Test Name",
                Email = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.Email : "Test Email",
                ExpirationDate = DateTime.UtcNow,
                ExtendExpirationDateLink = string.Empty
            };
        }

        #endregion

        
    }
}
