using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.AnonymousContacts;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EzCMS.Core.Services.AnonymousContacts
{
    public class AnonymousContactService : ServiceHelper, IAnonymousContactService
    {
        private readonly IRepository<AnonymousContact> _anonymousContactRepository;

        public AnonymousContactService(IRepository<AnonymousContact> anonymousContactRepository)
        {
            _anonymousContactRepository = anonymousContactRepository;
        }

        #region Base

        public IQueryable<AnonymousContact> GetAll()
        {
            return _anonymousContactRepository.GetAll();
        }

        public IQueryable<AnonymousContact> Fetch(Expression<Func<AnonymousContact, bool>> expression)
        {
            return _anonymousContactRepository.Fetch(expression);
        }

        public AnonymousContact FetchFirst(Expression<Func<AnonymousContact, bool>> expression)
        {
            return _anonymousContactRepository.FetchFirst(expression);
        }

        public AnonymousContact GetById(object id)
        {
            return _anonymousContactRepository.GetById(id);
        }

        internal ResponseModel Insert(AnonymousContact anonymousContactt)
        {
            return _anonymousContactRepository.Insert(anonymousContactt);
        }

        internal ResponseModel Update(AnonymousContact anonymousContactt)
        {
            return _anonymousContactRepository.Update(anonymousContactt);
        }

        internal ResponseModel Delete(AnonymousContact anonymousContactt)
        {
            return _anonymousContactRepository.Delete(anonymousContactt);
        }

        internal ResponseModel Delete(object id)
        {
            return _anonymousContactRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _anonymousContactRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the anonymous contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchAnonymousContacts(JqSearchIn si, AnonymousContactSearchModel model)
        {
            var data = SearchAnonymousContacts(model);

            var anonymousContactts = Maps(data);

            return si.Search(anonymousContactts);
        }

        /// <summary>
        /// Export the anonymous contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, AnonymousContactSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchAnonymousContacts(model);

            var anonymousContactts = Maps(data);

            var exportData = si.Export(anonymousContactts, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search anonymous contacts
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<AnonymousContact> SearchAnonymousContacts(AnonymousContactSearchModel model)
        {
            return Fetch(anonymousContact => !model.ContactId.HasValue || anonymousContact.ContactId == model.ContactId);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="anonymousContacts"></param>
        /// <returns></returns>
        private IQueryable<AnonymousContactModel> Maps(IQueryable<AnonymousContact> anonymousContacts)
        {
            return anonymousContacts.Select(anonymousContact => new AnonymousContactModel
            {
                Id = anonymousContact.Id,
                CookieKey = anonymousContact.CookieKey,
                Email = anonymousContact.Email,
                FirstName = anonymousContact.FirstName,
                LastName = anonymousContact.LastName,
                Phone = anonymousContact.Phone,
                Address = anonymousContact.Address,
                RecordOrder = anonymousContact.RecordOrder,
                Created = anonymousContact.Created,
                CreatedBy = anonymousContact.CreatedBy,
                LastUpdate = anonymousContact.LastUpdate,
                LastUpdateBy = anonymousContact.LastUpdateBy
            });
        }

        #endregion

        #endregion
    }
}