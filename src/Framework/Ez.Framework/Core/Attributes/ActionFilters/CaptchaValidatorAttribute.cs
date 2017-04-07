using Ez.Framework.Configurations;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Services;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using System.Web;
using System.Web.Mvc;

namespace Ez.Framework.Core.Attributes.ActionFilters
{
    public class CaptchaValidatorAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Name of captcha to validation
        /// </summary>
        private string Name { get; set; }

        /// <summary>
        /// Private key
        /// </summary>
        private string _privateKey;

        /// <summary>
        /// Public key
        /// </summary>
        private string _publicKey;

        #region Contructors

        public CaptchaValidatorAttribute()
        {
        }

        public CaptchaValidatorAttribute(string name)
            : this()
        {
            Name = name;
        }

        #endregion

        /// <summary>
        /// Set captcha keys
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="publicKey"></param>
        public void SetKeys(string privateKey, string publicKey)
        {
            _privateKey = privateKey;
            _publicKey = publicKey;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrEmpty(_publicKey) && !string.IsNullOrEmpty(_privateKey))
            {
                var localizedResourceService = HostContainer.GetInstance<ILocalizedResourceService>();

                var controller = (Controller)HttpContext.Current.Items[FrameworkConstants.EzCurrentController];
                RecaptchaVerificationHelper recaptchaHelper = controller.GetRecaptchaVerificationHelper(_privateKey);

                if (string.IsNullOrEmpty(recaptchaHelper.Response))
                {
                    filterContext.Controller.ViewData.ModelState.AddModelError(Name,
                        localizedResourceService.T("CaptchaValidation_Message_EmptyCaptcha"));
                }
                RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();

                if (recaptchaResult != RecaptchaVerificationResult.Success)
                {
                    filterContext.Controller.ViewData.ModelState.AddModelError(Name,
                        localizedResourceService.T("CaptchaValidation_Message_InvalidCaptchaAnswer"));
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
