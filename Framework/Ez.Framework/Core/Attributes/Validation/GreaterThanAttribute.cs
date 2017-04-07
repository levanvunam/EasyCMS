using System;
using System.ComponentModel.DataAnnotations;

namespace Ez.Framework.Core.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GreaterThanAttribute : ValidationAttribute
    {
        private string CompareToFieldName { get; set; }

        public GreaterThanAttribute(string compareToFieldName)
        {
            CompareToFieldName = compareToFieldName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var greaterValue = (int?)value;
            var smallerValue = (int?)validationContext.ObjectType.GetProperty(CompareToFieldName).GetValue(validationContext.ObjectInstance, null);

            if (!greaterValue.HasValue || !smallerValue.HasValue)
                return ValidationResult.Success;

            if (greaterValue > smallerValue)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage);
        }
    }
}
