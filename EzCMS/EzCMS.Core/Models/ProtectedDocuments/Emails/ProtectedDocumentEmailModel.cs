using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.Users.Emails;

namespace EzCMS.Core.Models.ProtectedDocuments.Emails
{
    public class ProtectedDocumentEmailModel : EmailTemplateSetupModel<ProtectedDocumentEmailModel>
    {
        #region Public Properties

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Path { get; set; }

        public string Comment { get; set; }

        #endregion

        #region Method

        public override ProtectedDocumentEmailModel GetMockData()
        {
            return new ProtectedDocumentEmailModel
            {
                FullName = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.FullName : "Test Name",
                Email = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.Email : "Test Email",
                Path = "Test Path",
                Comment = "Test Comment"
            };
        }

        #endregion
    }
}
