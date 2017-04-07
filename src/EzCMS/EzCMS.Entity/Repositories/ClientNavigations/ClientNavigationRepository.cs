using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.ClientNavigations
{
    public class ClientNavigationRepository : HierarchyRepository<ClientNavigation>, IClientNavigationRepository
    {
        public ClientNavigationRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Save client menu from page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ResponseModel SaveFromPage(Page page)
        {
            ResponseModel response;
            var clientNavigation = FetchFirst(c => c.PageId == page.Id);
            if (clientNavigation != null)
            {
                clientNavigation.PageId = page.Id;
                if (page.ParentId.HasValue && page.Parent.ClientNavigations.Any())
                {
                    clientNavigation.ParentId = page.Parent.ClientNavigations.First().Id;
                }
                else
                {
                    clientNavigation.ParentId = null;
                }
                clientNavigation.RecordDeleted = page.RecordDeleted;
                response = HierarchyUpdate(clientNavigation);
            }
            else
            {
                clientNavigation = new ClientNavigation
                {
                    PageId = page.Id,
                    RecordDeleted = page.RecordDeleted
                };
                if (page.ParentId.HasValue && page.Parent.ClientNavigations.Any())
                {
                    clientNavigation.ParentId = page.Parent.ClientNavigations.First().Id;
                }
                else
                {
                    clientNavigation.ParentId = null;
                }
                response = HierarchyInsert(clientNavigation);
            }

            return response;
        }

        /// <summary>
        /// Delete all menus of page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ResponseModel DeleteNavigationsOfPage(Page page)
        {
            if (page == null)
                return new ResponseModel
                {
                    Success = true
                };

            var menu = FetchFirst(m => m.PageId == page.Id);

            var deletedNavigations = GetHierarchies(menu, true, false).ToList();

            foreach (var deletedNavigation in deletedNavigations)
            {
                Delete(deletedNavigation);
            }

            return new ResponseModel
            {
                Success = true
            };
        }
    }
}