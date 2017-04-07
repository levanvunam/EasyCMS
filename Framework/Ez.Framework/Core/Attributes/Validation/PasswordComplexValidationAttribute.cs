using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ez.Framework.Core.Attributes.Validation
{
    /// <summary>
    /// Defines the <see cref="PasswordComplexValidationAttribute" /> object
    /// </summary>
    public class PasswordComplexValidationAttribute : ValidationAttribute
    {
        private int _passwordMinLengthRequired;
        private bool _passwordMustHaveDigit;
        private bool _passwordMustHaveSymbol;
        private bool _passwordMustHaveUpperAndLowerCaseLetters;

        public const string StartRegexPattern = "^.*";
        public const string EndRegexPattern = ".*$";
        public const string DigitPattern = "(?=.*\\d)";
        public const string UpperLowerCaseMixedPattern = "(?=.*[a-z])(?=.*[A-Z])";
        public const string SymbolPattern = "(?=.*[!@#$%^\\*])";

        public void Config(int passwordMinLengthRequired, bool passwordMustHaveDigit, bool passwordMustHaveSymbol, bool passwordMustHaveUpperAndLowerCaseLetters)
        {
            _passwordMinLengthRequired = passwordMinLengthRequired;
            _passwordMustHaveDigit = passwordMustHaveDigit;
            _passwordMustHaveSymbol = passwordMustHaveSymbol;
            _passwordMustHaveUpperAndLowerCaseLetters = passwordMustHaveUpperAndLowerCaseLetters;
        }

        public override bool IsValid(object value)
        {
            if (string.IsNullOrEmpty((string)value))
            {
                return true;
            }

            var errorMessage = new List<string>();

            var regexPatterns = Enumerable.Empty<string>();
            regexPatterns = regexPatterns.Concat(new[] { StartRegexPattern });

            if (_passwordMustHaveDigit)
            {
                errorMessage.Add("1 digit");
                regexPatterns = regexPatterns.Concat(new[] { DigitPattern });
            }

            if (_passwordMustHaveSymbol)
            {
                errorMessage.Add("1 symbol");
                regexPatterns = regexPatterns.Concat(new[] { SymbolPattern });
            }

            if (_passwordMustHaveUpperAndLowerCaseLetters)
            {
                errorMessage.Add("both upper and lower case characters");
                regexPatterns = regexPatterns.Concat(new[] { UpperLowerCaseMixedPattern });
            }
            regexPatterns = regexPatterns.Concat(new[] { EndRegexPattern });

            var regexPatternString = string.Join("", regexPatterns);
            var valid = new Regex(regexPatternString).IsMatch(((string)value));
            if (!valid)
            {
                ErrorMessage = "The password must have " + string.Join(", ", errorMessage);
                return false;
            }

            var pwdLength = _passwordMinLengthRequired > 0 ? _passwordMinLengthRequired : 8;
            if (((string)value).Length < pwdLength)
            {
                ErrorMessage = string.Format("Password must have at least {0} characters in length", pwdLength);
                return false;
            }

            return true;
        }
    }
}
