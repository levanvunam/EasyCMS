using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Ez.Framework.Utilities
{
    /// <summary>
    /// Validation utilities
    /// </summary>
    public static class ValidationUtilities
    {
        /// <summary>
        /// Build validation messages from model state
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static string BuildValidationMessages(this ModelStateDictionary modelState)
        {
            var errors = GetAllValidationResults(modelState);
            return string.Join("<br />", errors.Select(e => e.Message));
        }

        /// <summary>
        /// Build validation result
        /// </summary>
        /// <returns></returns>
        public static List<ValidationResult> ValidateModel<TModel>(this TModel model)
        {
            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, results, true);

            return results;
        }

        /// <summary>
        /// Build validation messages from model state
        /// </summary>
        /// <param name="validations"></param>
        /// <returns></returns>
        public static string BuildValidationMessages(this List<ValidationResult> validations)
        {
            return string.Join("<br />", validations.Select(e => e.ErrorMessage));
        }

        /// <summary>
        /// Build validation messages from model state
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static List<ValidationResultModel> GetAllValidationResults(this ModelStateDictionary modelState)
        {
            if (modelState.IsValid) return null;
            var result = new List<ValidationResultModel>();
            foreach (var key in modelState.Keys)
            {
                if (modelState[key].Errors.Any())
                {
                    result.Add(new ValidationResultModel
                    {
                        PropertyName = key,
                        Message = string.Join(",", modelState[key].Errors.Select(e => e.ErrorMessage))
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// Get first validation result from model state
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static ValidationResultModel GetFirstValidationResults(this ModelStateDictionary modelState)
        {
            return GetAllValidationResults(modelState).FirstOrDefault();
        }

    }

    public class ValidationResultModel
    {
        public string PropertyName { get; set; }

        public string Message { get; set; }
    }
}
