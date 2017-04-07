using Ez.Framework.Configurations;
using Ez.Framework.Core.Exceptions.Common;
using Ez.Framework.Utilities;
using System;
using System.Linq;
using System.Web;

namespace Ez.Framework.Core.Mvc.Helpers
{
    /// <summary>
    /// Unique link helper
    /// </summary>
    public static class UniqueLinkHelper
    {
        /// <summary>
        /// Generate unique link
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetUniqueLink(this string input)
        {
            return
                PasswordUtilities.Base64Encode(string.Format("{0}{1}{2}", HttpContext.Current.Session.SessionID,
                    FrameworkConstants.UniqueLinkSeparator, input));
        }

        /// <summary>
        /// Get back the result
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string GetUniqueLinkInput(this string encryptString)
        {
            if (!string.IsNullOrEmpty(encryptString))
            {
                var parts = PasswordUtilities.Base64Decode(encryptString).Split(new[] { FrameworkConstants.UniqueLinkSeparator }, StringSplitOptions.None);

                if (parts.Count() == 2)
                {
                    if (HttpContext.Current.Session != null && parts[0].Equals(HttpContext.Current.Session.SessionID))
                    {
                        return parts[1];
                    }
                }
            }

            // Link is invalid
            throw new InvalidUniqueLinkException();
        }
    }
}
