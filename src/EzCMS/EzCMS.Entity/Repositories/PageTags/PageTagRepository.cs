using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.PageTags
{
    public class PageTagRepository : Repository<PageTag>, IPageTagRepository
    {
        private readonly IRepository<Tag> _tagRepository;

        public PageTagRepository(EzCMSEntities entities, IRepository<Tag> tagRepository)
            : base(entities)
        {
            _tagRepository = tagRepository;
        }

        /// <summary>
        /// Delete all reference to tags for page
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="tagIds"></param>
        /// <returns></returns>
        public ResponseModel Delete(int pageId, List<int> tagIds)
        {
            var entities = Fetch(pageTag => pageTag.PageId == pageId && tagIds.Contains(pageTag.TagId));

            return Delete(entities);
        }

        /// <summary>
        /// Delete all reference to tags for page
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public ResponseModel Delete(int pageId, List<string> tags)
        {
            var entities = Fetch(pageTag => pageTag.PageId == pageId && tags.Contains(pageTag.Tag.Name));

            return Delete(entities);
        }

        /// <summary>
        /// Add new reference to tags for page
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="tagIds"></param>
        /// <returns></returns>
        public ResponseModel Insert(int pageId, List<int> tagIds)
        {
            var tags = new List<PageTag>();
            foreach (var tagId in tagIds)
            {
                tags.Add(new PageTag
                {
                    PageId = pageId,
                    TagId = tagId
                });
            }

            return Insert(tags);
        }

        /// <summary>
        /// Add new reference to tags for page
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="tags"> </param>
        /// <returns></returns>
        public ResponseModel Insert(int pageId, List<string> tags)
        {
            var tagIds = _tagRepository.Fetch(t => tags.Contains(t.Name)).Select(t => t.Id);
            var newTags = new List<PageTag>();
            foreach (var tagId in tagIds)
            {
                newTags.Add(new PageTag
                {
                    PageId = pageId,
                    TagId = tagId
                });
            }

            return Insert(newTags);
        }

        /// <summary>
        /// Save page tags
        /// </summary>
        /// <param name="page"> </param>
        /// <param name="tagString"></param>
        /// <returns></returns>
        public ResponseModel SavePageTags(Page page, string tagString)
        {
            var tags = string.IsNullOrEmpty(tagString) ? new List<string>() : tagString.Split(',').ToList();

            var currentTags = _tagRepository.GetAll().Select(t => t.Name);
            var newTags = tags.Where(t => !currentTags.Contains(t));
            _tagRepository.Insert(newTags.Select(t => new Tag
            {
                Name = t
            }));

            var currentTagReferences = page.PageTags == null
                ? new List<string>()
                : page.PageTags.Select(t => t.Tag.Name).ToList();

            // Remove deleted tags
            var removedTagReferences = currentTagReferences.Where(t => !tags.Contains(t)).ToList();
            Delete(page.Id, removedTagReferences);

            // Add new tags
            var addedTagReferences = currentTags.Where(t => tags.Contains(t)).ToList();
            Insert(page.Id, addedTagReferences);

            return new ResponseModel
            {
                Success = true
            };
        }
    }
}