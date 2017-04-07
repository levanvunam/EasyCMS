using Ez.Framework.Configurations;
using Ez.Framework.Core.Locale.Services;
using Ez.Framework.IoC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;

namespace Ez.Framework.Core.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIntegerAttribute : RangeAttribute, IClientValidatable
    {
        #region Constructors

        public RequiredIntegerAttribute()
            : base(FrameworkConstants.Zero, int.MaxValue)
        {
            var localizedResourceService = HostContainer.GetInstance<ILocalizedResourceService>();
            ErrorMessage = localizedResourceService.T("System_Message_Required");
        }

        public RequiredIntegerAttribute(string key)
            : this()
        {
            var localizedResourceService = HostContainer.GetInstance<ILocalizedResourceService>();
            var fieldName = localizedResourceService.T(key);

            ErrorMessage = string.Format(localizedResourceService.T("System_Message_RequiredFormat"), fieldName);
        }

        public RequiredIntegerAttribute(string key, string messageKey)
            : this()
        {
            var localizedResourceService = HostContainer.GetInstance<ILocalizedResourceService>();
            var fieldName = localizedResourceService.T(key);
            var message = localizedResourceService.T(messageKey);

            ErrorMessage = string.Format(message, fieldName);
        }

        public RequiredIntegerAttribute(string key, string defaultValue, string messageKey, string defaultMessageValue)
            : this()
        {
            var localizedResourceService = HostContainer.GetInstance<ILocalizedResourceService>();
            var fieldName = localizedResourceService.T(key, defaultValue);
            var message = localizedResourceService.T(messageKey, defaultMessageValue);

            ErrorMessage = string.Format(message, fieldName);
        }

        #endregion

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "requiredinteger",
            };

            yield return rule;
        }
    }
}
