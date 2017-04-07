using Ez.Framework.Core.Locale.Services;
using Ez.Framework.IoC;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;

namespace Ez.Framework.Core.Attributes.Validation
{
    /// <summary>
    /// There are 3 available usages for the attribute:
    /// Pass null as targetValue to constructor: required only when dependent field is null.
    /// Pass any value as tagetValue: required only when dependent field equals passed in any value.
    /// Pass "*" as tagetValue: required only when dependent field is populated.
    /// </summary>
    public class RequiredIfAttribute : ValidationAttribute, IClientValidatable
    {
        protected RequiredAttribute InnerAttribute;

        public string DependentProperty { get; set; }
        public object TargetValue { get; set; }
        public string RequiredMessage { get; set; }

        public bool AllowEmptyStrings
        {
            get
            {
                return InnerAttribute.AllowEmptyStrings;
            }
            set
            {
                InnerAttribute.AllowEmptyStrings = value;
            }
        }

        public RequiredIfAttribute(string dependentProperty, object targetValue)
        {
            InnerAttribute = new RequiredAttribute();
            DependentProperty = dependentProperty;
            TargetValue = targetValue;
        }

        public RequiredIfAttribute(string dependentProperty, object targetValue, string key)
        {
            InnerAttribute = new RequiredAttribute();
            DependentProperty = dependentProperty;
            TargetValue = targetValue;
            var localizedResourceService = HostContainer.GetInstance<ILocalizedResourceService>();
            RequiredMessage = localizedResourceService.T(key);
        }

        public RequiredIfAttribute(string dependentProperty, object targetValue, string key, string defaultValue)
        {
            InnerAttribute = new RequiredAttribute();
            DependentProperty = dependentProperty;
            TargetValue = targetValue;
            var localizedResourceService = HostContainer.GetInstance<ILocalizedResourceService>();
            RequiredMessage = localizedResourceService.T(key, defaultValue);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // get a reference to the property this validation depends upon
            var containerType = validationContext.ObjectInstance.GetType();
            var field = containerType.GetProperty(DependentProperty);

            if (field != null)
            {
                // get the value of the dependent property
                var dependentValue = field.GetValue(validationContext.ObjectInstance, null);
                // trim spaces of dependent value
                if (dependentValue != null && dependentValue is string)
                {
                    dependentValue = (dependentValue as string).Trim();

                    if (!AllowEmptyStrings && (dependentValue as string).Length == 0)
                    {
                        dependentValue = null;
                    }
                }

                // compare the value against the target value
                if ((dependentValue == null && TargetValue == null) ||
                    (dependentValue != null && (TargetValue == "*" || dependentValue.Equals(TargetValue))))
                {
                    // match => means we should try validating this field
                    if (!InnerAttribute.IsValid(value))
                    {
                        if (!string.IsNullOrEmpty(RequiredMessage))
                        {
                            return new ValidationResult(RequiredMessage);
                        }
                        // validation failed - return an error
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName });
                    }

                }
            }

            return ValidationResult.Success;
        }

        public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var message = RequiredMessage;
            if (string.IsNullOrEmpty(message))
            {
                message = FormatErrorMessage(metadata.GetDisplayName());
            }
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = message,
                ValidationType = "requiredif",
            };

            string depProp = BuildDependentPropertyId(metadata, context as ViewContext);

            // find the value on the control we depend on;
            // if it's a bool, format it javascript style 
            // (the default is True or False!)
            string targetValue = (TargetValue ?? "").ToString();
            if (TargetValue is bool)
                targetValue = targetValue.ToLower();

            rule.ValidationParameters.Add("dependentproperty", depProp);
            rule.ValidationParameters.Add("targetvalue", targetValue);

            yield return rule;
        }

        private string BuildDependentPropertyId(ModelMetadata metadata, ViewContext viewContext)
        {
            // build the ID of the property
            string depProp = viewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(DependentProperty);
            // unfortunately this will have the name of the current field appended to the beginning,
            // because the TemplateInfo's context has had this fieldname appended to it. Instead, we
            // want to get the context as though it was one level higher (i.e. outside the current property,
            // which is the containing object, and hence the same level as the dependent property.
            var thisField = metadata.PropertyName + "_";
            if (depProp.StartsWith(thisField))
                // strip it off again
                depProp = depProp.Substring(thisField.Length);
            return depProp;
        }
    }
}
