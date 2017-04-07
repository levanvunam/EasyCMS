using Ez.Framework.Core.Locale.Services;
using Ez.Framework.IoC;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;

namespace Ez.Framework.Core.Attributes.Validation
{
    public class UsernameValidationAttribute : RegularExpressionAttribute
    {
        public UsernameValidationAttribute()
            : base(@"^[0-9a-zA-Z]{3,}$")
        {
            var localizedResourceServies = HostContainer.GetInstance<ILocalizedResourceService>();
            ErrorMessage = localizedResourceServies.T("User_Message_InvalidUsernameFormat");
        }

        static UsernameValidationAttribute()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(UsernameValidationAttribute), typeof(RegularExpressionAttributeAdapter));
        }
    }
}