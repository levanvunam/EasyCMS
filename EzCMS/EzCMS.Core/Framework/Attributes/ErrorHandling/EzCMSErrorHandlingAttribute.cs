using Ez.Framework.Configurations;
using Ez.Framework.Core.Attributes.ErrorHandling;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Logging;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Files;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Exceptions.Sites;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.SiteSettings;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace EzCMS.Core.Framework.Attributes.ErrorHandling
{
    public class EzCMSErrorHandlingAttribute : EzErrorHandlingAttribute
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

            //Log the error
            var logger = HostContainer.GetInstance<ILogger>();
            var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();

            if (context.Exception != null)
            {
                var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();

                var requestType = GetRequestType(context);

                var exceptionType = context.Exception.GetType();

                var errorHandlingSeting = siteSettingService.LoadSetting<ErrorHandlingSetting>();

                //Check unauthorized exception
                if (exceptionType == typeof(EzCMSUnauthorizeException))
                {
                    //Clear error response
                    ClearError(context);

                    //Generate login url
                    var unauthorizeUrl = UrlUtilities.GenerateUrl(context.RequestContext, "Account", "Login", new
                    {
                        area = "Admin",
                        returnUrl = context.HttpContext.Request.RawUrl
                    });

                    var unauthorizedMessage = context.Exception.Message;
                    if (string.IsNullOrEmpty(unauthorizedMessage))
                    {
                        if (WorkContext.CurrentUser == null)
                        {
                            unauthorizedMessage =
                                localizedResourceService.T("System_Message_AccessDenied");
                        }
                        else
                        {
                            unauthorizedMessage =
                                localizedResourceService.T("System_Message_AccessDeniedForLoggedInUser");
                        }
                    }

                    if (requestType == RequestType.ChildRequest)
                    {
                        context.Controller.TempData[FrameworkConstants.EzErrorMessage] = unauthorizedMessage;
                        context.Result = new HttpNotFoundResult();
                    }
                    else if (requestType == RequestType.AjaxRequest)
                    {
                        context.Result = new JsonResult
                        {
                            Data = new ResponseModel
                            {
                                Success = false,
                                ResponseStatus = ResponseStatusEnums.AccessDenied,
                                Message = string.Format("{0} {1}", unauthorizedMessage, localizedResourceService.T("System_Message_AccessDeniedReturnUrl")),
                                Data = unauthorizeUrl
                            },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    else
                    {
                        context.Controller.TempData[FrameworkConstants.EzErrorMessage] = unauthorizedMessage;
                        context.Result = new RedirectResult(unauthorizeUrl);
                    }
                }
                else if (exceptionType == typeof(EzCMSNotFoundException))
                {
                    if (requestType == RequestType.AjaxRequest || requestType == RequestType.ChildRequest)
                    {
                        context.Result = new HttpNotFoundResult();
                    }
                    else
                    {
                        //Clear error response
                        ClearError(context);

                        if (context.HttpContext.Request.Url != null)
                        {
                            var requestUrl = context.HttpContext.Request.Url.LocalPath;

                            // This maybe is local resouce
                            if (string.IsNullOrEmpty(requestUrl.GetExtension()))
                            {
                                var pageNotFoundUrl = errorHandlingSeting.PageNotFoundUrl;
                                if (!pageNotFoundUrl.Equals(requestUrl))
                                {
                                    var exception = (EzCMSNotFoundException)context.Exception;
                                    pageNotFoundUrl = pageNotFoundUrl.AddQueryParam("url", exception.Url);

                                    /* Remove error logging for 404
                                    
                                    // Check if 404 error had been logged for this application
                                    var notFoundHandledUrls = EzWorkContext.EzWorkContext.NotFoundHandledUrls;
                                    if (!notFoundHandledUrls.Contains(requestUrl))
                                    {
                                        // Add 404 error url to handled 404 list in Application
                                        notFoundHandledUrls.Add(requestUrl);
                                       logger.Error(context.Exception);

                                        EzWorkContext.EzWorkContext.NotFoundHandledUrls = notFoundHandledUrls;
                                    }
                                    
                                    */

                                    context.Result = new RedirectResult(pageNotFoundUrl);

                                    //Set current page status code to 404
                                    Ez.Framework.Core.Context.EzWorkContext.CurrentControllerContext.Controller.TempData[EzCMSContants.CurrentPageStatusCode] = (int)HttpStatusCode.NotFound;
                                    return;
                                }
                            }
                        }

                        context.Result = new HttpNotFoundResult();
                    }
                }
                else
                {
                    logger.Error(context.Exception);
                    if (requestType == RequestType.AjaxRequest)
                    {
                        //Clear error response
                        ClearError(context);

                        // Generate exception list
                        var exceptionInformation = new List<ExceptionInformation>();
                        for (var exception = context.Exception; exception != null; exception = exception.InnerException)
                        {
                            exceptionInformation.Add(new ExceptionInformation(exception));
                        }

                        context.Result = new JsonResult
                        {
                            Data = new ResponseModel
                            {
                                Success = false,
                                ResponseStatus = ResponseStatusEnums.Error,
                                Message =
                                    localizedResourceService.T("System_Message_JsonError"),
                                DetailMessage =
                                    string.Join("<br />", exceptionInformation.Select(ResponseModel.BuildDetailMessage))
                            }
                        };
                    }
                    else
                    {
                        if (context.HttpContext.Request.Url != null)
                        {
                            var requestUrl = context.HttpContext.Request.Url.LocalPath;
                            var enableLiveSite = siteSettingService.GetSetting<bool>(SettingNames.EnableLiveSiteMode);
                            if (enableLiveSite)
                            {
                                var errorUrl = errorHandlingSeting.PageErrorUrl;
                                if (!errorUrl.Equals(requestUrl))
                                {
                                    //Clear error response
                                    ClearError(context);

                                    context.Result = new RedirectResult(errorUrl);
                                    return;
                                }
                            }
                        }

                        base.OnException(context);
                    }
                }
            }
        }
    }
}
