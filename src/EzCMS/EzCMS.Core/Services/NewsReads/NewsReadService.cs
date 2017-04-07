using System;
using System.Linq;
using System.Linq.Expressions;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.NewsReads;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.NewsReads
{
    public class NewsReadService : ServiceHelper, INewsReadService
    {
        private readonly IRepository<Contact> _contactRepository;
        private readonly IRepository<NewsRead> _newsReadRepository;

        public NewsReadService(IRepository<NewsRead> newsReadRepository, IRepository<Contact> contactRepository)
        {
            _newsReadRepository = newsReadRepository;
            _contactRepository = contactRepository;
        }

        #region Base

        public IQueryable<NewsRead> GetAll()
        {
            return _newsReadRepository.GetAll();
        }

        public IQueryable<NewsRead> Fetch(Expression<Func<NewsRead, bool>> expression)
        {
            return _newsReadRepository.Fetch(expression);
        }

        public NewsRead FetchFirst(Expression<Func<NewsRead, bool>> expression)
        {
            return _newsReadRepository.FetchFirst(expression);
        }

        public NewsRead GetById(object id)
        {
            return _newsReadRepository.GetById(id);
        }

        internal ResponseModel Insert(NewsRead newsRead)
        {
            return _newsReadRepository.Insert(newsRead);
        }

        internal ResponseModel Update(NewsRead newsRead)
        {
            return _newsReadRepository.Update(newsRead);
        }

        internal ResponseModel Delete(NewsRead newsRead)
        {
            return _newsReadRepository.Delete(newsRead);
        }

        internal ResponseModel Delete(object id)
        {
            return _newsReadRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _newsReadRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the news reads.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchNewsReads(JqSearchIn si, NewsReadSearchModel model)
        {
            var data = SearchNewsReads(model);

            var newsReads = Maps(data);

            return si.Search(newsReads);
        }

        /// <summary>
        /// Export the news reads
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, NewsReadSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchNewsReads(model);

            var newsReads = Maps(data);

            var exportData = si.Export(newsReads, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search news reads
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<NewsRead> SearchNewsReads(NewsReadSearchModel model)
        {
            var data = Fetch(newsRead => !model.NewsId.HasValue || newsRead.NewsId == model.NewsId);

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
                        newsRead =>
                            !model.ContactId.HasValue || anonymousContactIds.Contains(newsRead.AnonymousContactId));
            }

            return data;
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="newsReads"></param>
        /// <returns></returns>
        private IQueryable<NewsReadModel> Maps(IQueryable<NewsRead> newsReads)
        {
            return newsReads.Select(m => new NewsReadModel
            {
                Id = m.Id,
                NewsId = m.NewsId,
                Title = m.News.Title,
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