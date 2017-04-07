using Ez.Framework.Configurations;
using Ez.Framework.Core.Context;
using Ez.Framework.Core.Locale.Services;
using Ez.Framework.IoC;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Ez.Framework.Core.IoC;

namespace Ez.Framework.Core.Mvc.Controllers
{
    /// <summary>
    /// Base controller
    /// </summary>
    public class EzBaseController : Controller
    {
        public readonly ILocalizedResourceService EzLocalizedResourceService;
        public EzBaseController()
        {
            EzLocalizedResourceService = HostContainer.GetInstance<ILocalizedResourceService>();
        }

        #region Protected Method

        /// <summary>
        /// Creates the temp data provider. Push current controller to Temp data
        /// </summary>
        /// <returns></returns>
        protected override ITempDataProvider CreateTempDataProvider()
        {
            ITempDataProvider iTempDataProvider = base.CreateTempDataProvider();
            HttpContext.Items[FrameworkConstants.EzCurrentController] = this;
            return iTempDataProvider;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            // Save the controller context
            EzWorkContext.CurrentControllerContext = ControllerContext;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Save the controller action
            var action = ControllerContext.RouteData.Values["action"].ToString();
            EzWorkContext.CurrentControllerAction = action;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get text by key
        /// </summary>
        /// <param name="textKey"></param>
        /// <returns></returns>
        protected string T(string textKey)
        {
            return EzLocalizedResourceService.T(textKey);
        }

        /// <summary>
        /// Get text by key, if not found set default value
        /// </summary>
        /// <param name="textKey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        [NonAction]
        protected string T(string textKey, string defaultValue)
        {
            return EzLocalizedResourceService.T(textKey, defaultValue);
        }

        /// <summary>
        /// Get text by key
        /// </summary>
        /// <param name="textKey"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected string TFormat(string textKey, params object[] parameters)
        {
            return EzLocalizedResourceService.TFormat(textKey, parameters);
        }

        [NonAction]
        public bool TryUpdateModelWithType(object model, string prefix = null,
            string[] includeProperties = null, string[] excludeProperties = null,
            IValueProvider valueProvider = null)
        {
            if (model == null) return false;

            var methodInfo = GetType()
                .GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(x => x.Name == "TryUpdateModel" && x.GetParameters().Count() == 5)
                .MakeGenericMethod(new[] { model.GetType() });

            return (bool)methodInfo.Invoke(this, new[]
            {
                model, prefix, includeProperties, excludeProperties, valueProvider ?? ValueProvider
            });
        }

        #endregion

        /// <summary>
        /// Render partial to string
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [NonAction]
        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = System.Web.Mvc.ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
