using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.IoC;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Pages.Widgets;
using EzCMS.Core.Services.Pages;
using EzCMS.Entity.Entities.Models;
using System;
using System.IO;
using System.Text;
using System.Web.Hosting;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Framework.Mvc.ViewEngines
{
    public class EzCMSVirtualFile : VirtualFile
    {
        private readonly byte[] _data;

        internal byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Generate virtual file from template
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="template"></param>
        public EzCMSVirtualFile(string virtualPath, PageTemplate template)
            : base(virtualPath)
        {
            var pageService = HostContainer.GetInstance<IPageService>();

            var pageId = WorkContext.ActivePageId;
            var model = pageService.RenderContent(pageId);

            // Get raw page template
            var rawMasterHtml = template.Content;
            if (template.ParentId.HasValue)
            {
                rawMasterHtml = rawMasterHtml.InsertMasterPage(template.Parent.CacheName).ParseProperties(typeof(PageRenderModel));
            }

            //Replace the @RenderBody() with markup to disable rendering body
            rawMasterHtml = rawMasterHtml.Replace(EzCMSContants.RenderBody, EzCMSContants.RenderBodyWidget);

            //Render master html
            var masterCacheName = template.Name.GetTemplateCacheName(rawMasterHtml);
            var masterHtml = EzRazorEngineHelper.CompileAndRun(rawMasterHtml, model, null, masterCacheName);

            // Parse the html
            masterHtml = masterHtml.ResolveContent();

            //Adding the @RenderBody() back to use the template as Master
            masterHtml = masterHtml.Replace(EzCMSContants.RenderBodyWidget,
                EzCMSContants.RenderBody);

            //Convert content to Unicode
            //using (var memoryStream = new MemoryStream())
            //{
            //    using (var writer = new StreamWriter(memoryStream, new UnicodeEncoding()))
            //    {
            //        writer.Write(content);
            //        _data = memoryStream.ToArray();
            //    }
            //}

            _data = Encoding.UTF8.GetBytes(masterHtml);
            var firstBytes = new byte[]
            {
                239, 187, 191
            };
            int newSize = firstBytes.Length + _data.Length;
            var ms = new MemoryStream(new byte[newSize], 0, newSize, true, true);
            ms.Write(firstBytes, 0, firstBytes.Length);
            ms.Write(_data, 0, _data.Length);
            _data = ms.GetBuffer();
        }

        public override Stream Open()
        {
            return new MemoryStream(_data);
        }
    }
}