using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.Pages;

namespace EzCMS.Core.Services.StarterKits
{
    public class StarterKitService : IStarterKitService
    {
        private readonly IPageRepository _pageRepository;
        private readonly IHierarchyRepository<PageTemplate> _pageTemplateRepository;
        private readonly IRepository<Script> _scriptRepository;
        private readonly IRepository<Style> _styleRepository;
        private readonly IRepository<WidgetTemplate> _widgetTemplateRepository;

        public StarterKitService(IHierarchyRepository<PageTemplate> pageTemplateRepository,
            IRepository<Style> styleRepository, IRepository<Script> scriptRepository, IPageRepository pageRepository, IRepository<WidgetTemplate> widgetTemplateRepository)
        {
            _pageTemplateRepository = pageTemplateRepository;
            _styleRepository = styleRepository;
            _scriptRepository = scriptRepository;
            _pageRepository = pageRepository;
            _widgetTemplateRepository = widgetTemplateRepository;
        }

        /// <summary>
        /// Insert page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ResponseModel InsertPage(Page page)
        {
            var response = _pageRepository.HierarchyInsert(page);

            return response;
        }

        /// <summary>
        /// Insert page template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public ResponseModel InsertPageTemplate(PageTemplate template)
        {
            return _pageTemplateRepository.HierarchyInsert(template);
        }

        /// <summary>
        /// Insert script
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public ResponseModel InsertScript(Script script)
        {
            return _scriptRepository.Insert(script);
        }

        /// <summary>
        /// Insert style
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public ResponseModel InsertStyle(Style style)
        {
            return _styleRepository.Insert(style);
        }

        /// <summary>
        /// Insert widget template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public ResponseModel InsertWidgetTemplate(WidgetTemplate template)
        {
            return _widgetTemplateRepository.Insert(template);
        }
    }
}