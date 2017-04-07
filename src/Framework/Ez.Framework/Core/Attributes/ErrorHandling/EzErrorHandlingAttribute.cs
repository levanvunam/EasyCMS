using Ez.Framework.Core.Logging;
using Ez.Framework.IoC;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;

namespace Ez.Framework.Core.Attributes.ErrorHandling
{
    /// <summary>
    /// Ez error handling attribute
    /// </summary>
    public class EzErrorHandlingAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// Logic for exception handler
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled)
            {
                return;
            }

            if (context.Exception != null)
            {
                //Log the error
                var logger = HostContainer.GetInstance<ILogger>();
                logger.Error(context.Exception);
            }

            base.OnException(context);
        }

        /// <summary>
        /// Clear the error so IIS will not catch it
        /// </summary>
        /// <param name="exceptionContext"></param>
        protected void ClearError(ExceptionContext exceptionContext)
        {
            // Clear error response
            exceptionContext.HttpContext.Response.Clear();
            exceptionContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            exceptionContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            exceptionContext.ExceptionHandled = true;
        }

        /// <summary>
        /// Get the request type of request
        /// </summary>
        /// <param name="exceptionContext"></param>
        /// <returns></returns>
        protected RequestType GetRequestType(ExceptionContext exceptionContext)
        {
            if (exceptionContext.HttpContext.Request.IsAjaxRequest())
            {
                return RequestType.AjaxRequest;
            }

            if (exceptionContext.Controller.ControllerContext.IsChildAction)
            {
                return RequestType.ChildRequest;
            }

            return RequestType.Other;
        }
    }

    /// <summary>
    /// Request Type
    /// </summary>
    public enum RequestType
    {
        AjaxRequest = 1,
        ChildRequest = 2,
        Other = 3
    }
}
