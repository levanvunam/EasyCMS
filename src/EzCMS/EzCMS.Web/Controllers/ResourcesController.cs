using Ez.Framework.Configurations;
using Ez.Framework.Utilities.Files;
using Ez.Framework.Utilities.Web;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Services.Scripts;
using EzCMS.Core.Services.Styles;
using System.Web.Mvc;

namespace EzCMS.Web.Controllers
{
    public class ResourcesController : ClientController
    {
        private readonly IScriptService _scriptService;
        private readonly IStyleService _styleService;
        private readonly string _cssMime = FrameworkConstants.CssMIME.GetMimeMapping();
        private readonly string _javascriptMime = FrameworkConstants.JavascriptMIME.GetMimeMapping();

        public ResourcesController(IScriptService scriptService, IStyleService styleService)
        {
            _scriptService = scriptService;
            _styleService = styleService;
        }

        /// <summary>
        /// Render dynamic style sheet file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult Style(string fileName)
        {
            var model = _styleService.GetStyleByName(fileName);

            if (model != null)
            {
                if (!WebUtilities.OutputNeedReCache(HttpContext.ApplicationInstance.Context, model.LastUpdate))
                {
                    return Content("");
                }

                return Content(model.Content, _cssMime);
            }

            var requestPath = string.Format("/Resources/{0}.css", fileName);
            if (System.IO.File.Exists(HttpContext.Server.MapPath(requestPath)))
            {
                return new FilePathResult(requestPath, _cssMime);
            }

            return HttpNotFound();
        }

        /// <summary>
        /// Render dynamic script file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult Script(string fileName)
        {
            var model = _scriptService.GetScriptByName(fileName);
            if (model != null)
            {
                if (!WebUtilities.OutputNeedReCache(HttpContext.ApplicationInstance.Context, model.LastUpdate))
                {
                    return Content("");
                }

                return Content(model.Content, _javascriptMime);
            }

            var requestPath = string.Format("/Resources/{0}.js", fileName);
            if (System.IO.File.Exists(HttpContext.Server.MapPath(requestPath)))
            {
                return new FilePathResult(requestPath, _javascriptMime);
            }

            return HttpNotFound();
        }
    }
}
