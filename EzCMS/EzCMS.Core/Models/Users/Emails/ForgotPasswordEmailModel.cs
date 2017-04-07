using EzCMS.Core.Framework.Context;

namespace EzCMS.Core.Models.Users.Emails
{
    public class ForgotPasswordEmailModel : EmailTemplateSetupModel<ForgotPasswordEmailModel>
    {
        #region Public Properties

        public string Username { get; set; }

        public string FullName { get; set; }

        public string ResetLink { get; set; }

        public int EffectiveTimeInMinutes { get; set; }

        #endregion

        #region Method

        public override ForgotPasswordEmailModel GetMockData()
        {
            return new ForgotPasswordEmailModel
            {
                Username = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.Username : "Test Username",
                FullName = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.FullName : "Test Name",
                ResetLink = string.Empty,
                EffectiveTimeInMinutes = 10
            };
        }

        #endregion
    }
}
