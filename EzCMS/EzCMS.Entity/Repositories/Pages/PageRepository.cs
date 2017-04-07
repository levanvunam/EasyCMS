using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.ClientNavigations;

namespace EzCMS.Entity.Repositories.Pages
{
    public class PageRepository : HierarchyRepository<Page>, IPageRepository
    {
        private readonly IClientNavigationRepository _clientNavigationRepository;
        private readonly IRepository<PageLog> _pageLogRepository;

        public PageRepository(EzCMSEntities entities, IClientNavigationRepository clientNavigationRepository,
            IRepository<PageLog> pageLogRepository)
            : base(entities)
        {
            _clientNavigationRepository = clientNavigationRepository;
            _pageLogRepository = pageLogRepository;
        }

        public override ResponseModel HierarchyInsert(Page entity)
        {
            var response = base.HierarchyInsert(entity);

            if (response.Success)
            {
                var page = GetById(entity.Id);

                _clientNavigationRepository.SaveFromPage(page);
            }

            return response;
        }

        public override ResponseModel HierarchyUpdate(Page entity)
        {
            var response = base.HierarchyUpdate(entity);

            if (response.Success)
            {
                var page = GetById(entity.Id);
                _clientNavigationRepository.SaveFromPage(page);
            }

            return response;
        }

        /// <summary>
        /// Delete page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override ResponseModel Delete(object id)
        {
            var page = GetById(id);

            _clientNavigationRepository.DeleteNavigationsOfPage(page);

            _pageLogRepository.Delete(page.PageLogs);

            return base.Delete(id);
        }

        public override ResponseModel Delete(Page entity)
        {
            _clientNavigationRepository.DeleteNavigationsOfPage(entity);

            return base.Delete(entity);
        }

        public override ResponseModel Delete(IEnumerable<Page> entities)
        {
            foreach (var entity in entities.ToList())
            {
                _clientNavigationRepository.DeleteNavigationsOfPage(entity);
            }

            return base.Delete(entities);
        }

        public override ResponseModel Delete(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return new ResponseModel
                {
                    Success = true
                };
            }

            var entities = Fetch(e => ids.Contains(e.Id));
            foreach (var entity in entities.ToList())
            {
                _clientNavigationRepository.DeleteNavigationsOfPage(entity);
            }

            return base.Delete(entities);
        }
    }
}