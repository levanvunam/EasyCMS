using EzCMS.Core.Framework.Context;

namespace EzCMS.Core.Models.Users.Emails
{
    public class NotifyAccountDeactivatedEmailModel : EmailTemplateSetupModel<NotifyAccountDeactivatedEmailModel>
    {
        #region Public Properties

        public string Username { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        #endregion

        #region Method

        public override NotifyAccountDeactivatedEmailModel GetMockData()
        {
            return new NotifyAccountDeactivatedEmailModel
            {
                Username = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.Username : "Test Username",
                FullName = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.FullName : "Test Name",
                Email = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.Email : "Test Email"
            };
        }

        #endregion
    }
}
