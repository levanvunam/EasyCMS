using System.Linq;
using RazorEngine.Templating;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Mvc.Models;

namespace EzCMS.Core.Framework.Mvc.ViewEngines.Razor.RazorEngine.Configs
{
    public class RazorEngineTemplateManager : ITemplateManager
    {
        /// <summary>
        /// Resolve the template
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ITemplateSource Resolve(ITemplateKey key)
        {
            var template = WorkContext.TemplateCaches.First(t => t.Name.Equals(key.Name));
            return new LoadedTemplateSource(template.Content);
        }

        /// <summary>
        /// Generate key
        /// </summary>
        /// <param name="name"></param>
        /// <param name="resolveType"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public ITemplateKey GetKey(string name, ResolveType resolveType, ITemplateKey context)
        {
            return new NameOnlyTemplateKey(name, resolveType, context);
        }

        /// <summary>
        /// Add new template
        /// </summary>
        /// <param name="key"></param>
        /// <param name="source"></param>
        public void AddDynamic(ITemplateKey key, ITemplateSource source)
        {
            lock (WorkContext.TemplateCaches)
            {
                if (!WorkContext.TemplateCaches.Any(t => t.Name.Equals(key.Name)))
                {
                    var templateCache = new TemplateCache
                    {
                        Name = key.Name,
                        Type = key.TemplateType,
                        Content = source.Template
                    };

                    WorkContext.TemplateCaches.Add(templateCache);
                }
            }
        }
    }
}
