using System;
using System.ComponentModel.DataAnnotations;

namespace Ez.Framework.Core.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private string DateToCompareToFieldName { get; set; }

        public DateGreaterThanAttribute(string dateToCompareToFieldName)
        {
            DateToCompareToFieldName = dateToCompareToFieldName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var laterDate = (DateTime?)value;
            var earlierDate = (DateTime?)validationContext.ObjectType.GetProperty(DateToCompareToFieldName).GetValue(validationContext.ObjectInstance, null);

            if (!laterDate.HasValue || !earlierDate.HasValue)
                return ValidationResult.Success;

            if (laterDate > earlierDate)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
