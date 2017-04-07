using EzCMS.Core.Framework.Context;

namespace EzCMS.Core.Models.Users.Emails
{
    public class UserCreatedEmailModel : EmailTemplateSetupModel<UserCreatedEmailModel>
    {
        #region Public Properties

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }

        public string SiteUrl { get; set; }

        public string LoginLink { get; set; }

        public string ResetPasswordLink { get; set; }

        #endregion

        #region Method

        public override UserCreatedEmailModel GetMockData()
        {
            return new UserCreatedEmailModel
            {
                Username = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.Username : "Test Username",
                FullName = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.FullName : "Test Name",
                Password = "1234",
                Email = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.Email : "Test Email",
                SiteUrl = !string.IsNullOrEmpty(WorkContext.SiteUrl) ? WorkContext.SiteUrl : "Test Site",
                LoginLink = string.Empty,
                ResetPasswordLink = string.Empty
            };
        }

        #endregion
    }
}
