using System;
using System.Text;
using System.Web;

namespace Ez.Framework.Utilities
{
    /// <summary>
    /// Exception utilities
    /// </summary>
    public static class ExceptionUtilities
    {
        /// <summary>
        /// Build full exception messages
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string BuildFullExceptionMessage(this Exception exception)
        {

            var logException = exception;
            if (exception.InnerException != null)
                logException = exception.InnerException;

            var sb = new StringBuilder();

            try
            {
                sb.AppendLine("\n Error in Path : " + HttpContext.Current.Request.Path.ToAbsoluteUrl());

                // Get the QueryString along with the Virtual Path
                sb.AppendLine("\n Raw Url : " + HttpContext.Current.Request.RawUrl.ToAbsoluteUrl());
            }
            catch
            {
                // No http context
            }

            // Get the error message
            sb.AppendLine("\n Message : " + logException.Message);

            // Source of the message
            sb.AppendLine("\n Source : " + logException.Source);

            // Stack Trace of the error

            sb.AppendLine("\n Stack Trace : " + logException.StackTrace);

            // Method where the error occurred
            sb.AppendLine("\n TargetSite : " + logException.TargetSite);
            return sb.ToString();
        }

        /// <summary>
        /// Build exception messages
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string BuildErrorMessage(this Exception exception)
        {
            var sb = new StringBuilder();

            sb.AppendLine(exception.Message);
            sb.AppendLine(exception.StackTrace);

            return sb.ToString();
        }

        /// <summary>
        /// Build the error message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string BuildMessageBody(this string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            try
            {
                sb.AppendLine("\n Error in Path : " + HttpContext.Current.Request.Path.ToAbsoluteUrl());

                // Get the QueryString along with the Virtual Path
                sb.AppendLine("\n Raw Url : " + HttpContext.Current.Request.RawUrl.ToAbsoluteUrl());
            }
            catch
            {
                // No http context
            }

            sb.AppendLine(message);

            return sb.ToString();
        }
    }
}
