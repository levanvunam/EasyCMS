using Ez.Framework.IoC;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Services.PageTemplates;
using EzCMS.Entity.Entities.Models;
using System;
using System.Globalization;
using System.Web.Caching;
using System.Web.Hosting;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Framework.Mvc.ViewEngines
{
    public class EzCMSVirtualPathProvider : VirtualPathProvider
    {
        private IPageTemplateService _pageTemplateService;

        #region Private Methods

        /// <summary>
        /// Get file template from virtual path
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        private PageTemplate FindTemplate(string virtualPath)
        {
            _pageTemplateService = HostContainer.GetInstance<IPageTemplateService>();
            return _pageTemplateService.FindTemplate(virtualPath);
        }

        #endregion

        /// <summary>
        /// Check if template is modified to re-cache
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="virtualPathDependencies"></param>
        /// <returns></returns>
        public override string GetFileHash(string virtualPath, System.Collections.IEnumerable virtualPathDependencies)
        {
            if (virtualPath.IsDBTemplateMaster())
            {
                //No Cache
                return DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

                //var template = FindTemplate(virtualPath);
                //return template.Name.GetTemplateCacheName(Template .CacheName + DateTime.UtcNow);
            }
            return base.GetFileHash(virtualPath, virtualPathDependencies);
        }

        /// <summary>
        /// Return cache version of razor file
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="virtualPathDependencies"></param>
        /// <param name="utcStart"></param>
        /// <returns></returns>
        public override CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (base.FileExists(virtualPath))
            {
                return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            }
            return null;
        }

        /// <summary>
        /// Check if file exist in physical path or not, if not search in db template
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public override bool FileExists(string virtualPath)
        {
            _pageTemplateService = HostContainer.GetInstance<IPageTemplateService>();
            if (virtualPath.IsDBTemplateMaster())
            {
                return _pageTemplateService.IsPageTemplateExisted(virtualPath);
            }
            return base.FileExists(virtualPath);
        }

        /// <summary>
        /// Get file by virtual path
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public override VirtualFile GetFile(string virtualPath)
        {
            if (virtualPath.IsDBTemplateMaster())
            {
                var template = FindTemplate(virtualPath);
                if (template != null)
                    return new EzCMSVirtualFile(virtualPath, template);
            }
            return base.GetFile(virtualPath);
        }
    }
}