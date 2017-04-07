using System.Collections.Generic;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.Users.Emails;

namespace EzCMS.Core.Models.Contacts.Emails
{
    public class ContactFormEmailModel : EmailTemplateSetupModel<ContactFormEmailModel>
    {
        #region Public Properties

        public string FullName { get; set; }

        public string Email { get; set; }

        public List<ContactInformation> ContactInformation { get; set; }

        #endregion

        #region Method

        public override ContactFormEmailModel GetMockData()
        {
            return new ContactFormEmailModel
            {
                FullName = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.FullName : "Test Name",
                Email = WorkContext.CurrentUser != null ? WorkContext.CurrentUser.Email : "Test Email",
                ContactInformation = new List<ContactInformation>
                {
                    new ContactInformation
                    {
                        Key = "1",
                        Value = "Test"
                    }
                }
            };
        }

        #endregion
    }

    public class ContactInformation
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
