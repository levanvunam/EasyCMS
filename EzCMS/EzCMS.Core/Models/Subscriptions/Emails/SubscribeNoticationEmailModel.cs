using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.Users.Emails;

namespace EzCMS.Core.Models.Subscriptions.Emails
{
    public class SubscribeNoticationEmailModel : EmailTemplateSetupModel<SubscribeNoticationEmailModel>
    {
        #region Public Properties

        public string Email { get; set; }

        public string Content { get; set; }

        #endregion

        #region Method

        public override SubscribeNoticationEmailModel GetMockData()
        {
            return new SubscribeNoticationEmailModel
            {
                Email = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.Email : "Test Email",
                Content = "Test Content"
            };
        }

        #endregion
    }
}
