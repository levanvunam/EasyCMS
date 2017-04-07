using Ez.Framework.Core.Attributes.Validation;

namespace EzCMS.Core.Models.EmailAccounts
{
    public class TestEmailModel
    {
        public int EmailAccountId { get; set; }

        [EmailValidation]
        public string Email { get; set; }

    }
}
