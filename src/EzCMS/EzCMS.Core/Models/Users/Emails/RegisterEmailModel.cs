using EzCMS.Core.Framework.Context;

namespace EzCMS.Core.Models.Users.Emails
{
    public class RegisterEmailModel : EmailTemplateSetupModel<RegisterEmailModel>
    {
        #region Public Properties

        public string Username { get; set; }

        public string FullName { get; set; }

        public string SiteUrl { get; set; }

        public string LoginLink { get; set; }

        #endregion

        #region Method

        public override RegisterEmailModel GetMockData()
        {
            return new RegisterEmailModel
            {
                Username = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.Username : "Test Username",
                FullName = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.FullName : "Test Name",
                SiteUrl = !string.IsNullOrEmpty(WorkContext.SiteUrl) ? WorkContext.SiteUrl : "Test Site",
                LoginLink = string.Empty
            };
        }

        #endregion
    }
}
