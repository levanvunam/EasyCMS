using Ez.Framework.Core.Locale.Services;
using Ez.Framework.IoC;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;

namespace Ez.Framework.Core.Attributes.Validation
{
    public class EmailValidationAttribute : RegularExpressionAttribute
    {
        public EmailValidationAttribute()
            : base(@"^([\w\-\.]+)@((\[([0-9]{1,3}\.){3}[0-9]{1,3}\])|(([\w\-]+\.)+)([a-zA-Z]{2,4}))$")
        {
            var localizedResourceServies = HostContainer.GetInstance<ILocalizedResourceService>();
            ErrorMessage = localizedResourceServies.T("System_Message_InvalidEmailFormat");
        }

        static EmailValidationAttribute()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(EmailValidationAttribute), typeof(RegularExpressionAttributeAdapter));
        }
    }
}