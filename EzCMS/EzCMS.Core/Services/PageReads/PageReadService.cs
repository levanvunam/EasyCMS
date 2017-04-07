using System;
using System.Linq;
using System.Linq.Expressions;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.PageReads;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.PageReads
{
    public class PageReadService : ServiceHelper, IPageReadService
    {
        private readonly IRepository<Contact> _contactRepository;
        private readonly IRepository<PageRead> _pageReadRepository;

        public PageReadService(IRepository<PageRead> pageReadRepository, IRepository<Contact> contactRepository)
        {
            _pageReadRepository = pageReadRepository;
            _contactRepository = contactRepository;
        }

        #region Base

        public IQueryable<PageRead> GetAll()
        {
            return _pageReadRepository.GetAll();
        }

        public IQueryable<PageRead> Fetch(Expression<Func<PageRead, bool>> expression)
        {
            return _pageReadRepository.Fetch(expression);
        }

        public PageRead FetchFirst(Expression<Func<PageRead, bool>> expression)
        {
            return _pageReadRepository.FetchFirst(expression);
        }

        public PageRead GetById(object id)
        {
            return _pageReadRepository.GetById(id);
        }

        internal ResponseModel Insert(PageRead pageRead)
        {
            return _pageReadRepository.Insert(pageRead);
        }

        internal ResponseModel Update(PageRead pageRead)
        {
            return _pageReadRepository.Update(pageRead);
        }

        internal ResponseModel Delete(PageRead pageRead)
        {
            return _pageReadRepository.Delete(pageRead);
        }

        internal ResponseModel Delete(object id)
        {
            return _pageReadRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _pageReadRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the page reads.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchPageReads(JqSearchIn si, PageReadSearchModel model)
        {
            var data = SearchPageReads(model);

            var pageReads = Maps(data);

            return si.Search(pageReads);
        }

        /// <summary>
        /// Export the page reads
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, PageReadSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchPageReads(model);

            var pageReads = Maps(data);

            var exportData = si.Export(pageReads, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search page reads
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<PageRead> SearchPageReads(PageReadSearchModel model)
        {
            var data = Fetch(pageRead => !model.PageId.HasValue || pageRead.PageId == model.PageId);

            if (model.ContactId.HasValue)
            {
                //Get all anonymous contacts of current contact
                var anonymousContactIds =
                    _contactRepository.Fetch(c => c.Id == model.ContactId)
                        .Select(c => c.AnonymousContacts.Select(ac => ac.Id))
                        .ToList()
                        .SelectMany(id => id)
                        .ToList();


                data =
                    data.Where(
                        pageRead =>
                            !model.ContactId.HasValue || anonymousContactIds.Contains(pageRead.AnonymousContactId));
            }

            return data;
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="pageReads"></param>
        /// <returns></returns>
        private IQueryable<PageReadModel> Maps(IQueryable<PageRead> pageReads)
        {
            return pageReads.Select(m => new PageReadModel
            {
                Id = m.Id,
                PageId = m.PageId,
                Title = m.Page.Title,
                FriendlyUrl = m.Page.FriendlyUrl,
                ContactId = m.AnonymousContact.ContactId,
                CookieKey = m.AnonymousContact.CookieKey,
                Email = m.AnonymousContact.Email,
                FirstName = m.AnonymousContact.FirstName,
                LastName = m.AnonymousContact.LastName,
                Phone = m.AnonymousContact.Phone,
                Address = m.AnonymousContact.Address,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion

        #endregion
    }
}