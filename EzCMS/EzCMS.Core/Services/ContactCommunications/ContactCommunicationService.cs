using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.ContactCommunications;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.ContactCommunications
{
    public class ContactCommunicationService : ServiceHelper, IContactCommunicationService
    {
        private readonly IRepository<ContactCommunication> _contactCommunicationRepository;

        public ContactCommunicationService(IRepository<ContactCommunication> contactCommunicationRepository)
        {
            _contactCommunicationRepository = contactCommunicationRepository;
        }

        #region Base

        public IQueryable<ContactCommunication> GetAll()
        {
            return _contactCommunicationRepository.GetAll();
        }

        public IQueryable<ContactCommunication> Fetch(Expression<Func<ContactCommunication, bool>> expression)
        {
            return _contactCommunicationRepository.Fetch(expression);
        }

        public ContactCommunication FetchFirst(Expression<Func<ContactCommunication, bool>> expression)
        {
            return _contactCommunicationRepository.FetchFirst(expression);
        }

        public ContactCommunication GetById(object id)
        {
            return _contactCommunicationRepository.GetById(id);
        }

        public ResponseModel Insert(ContactCommunication contactCommunication)
        {
            return _contactCommunicationRepository.Insert(contactCommunication);
        }

        public ResponseModel Update(ContactCommunication contactCommunication)
        {
            return _contactCommunicationRepository.Update(contactCommunication);
        }

        internal ResponseModel Delete(ContactCommunication contactCommunication)
        {
            return _contactCommunicationRepository.Delete(contactCommunication);
        }

        internal ResponseModel Delete(object id)
        {
            return _contactCommunicationRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _contactCommunicationRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the contact communications
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchContactCommunications(JqSearchIn si, int contactId)
        {
            var data = Fetch(c => c.ContactId == contactId);

            var contactCommunications = Maps(data);

            return si.Search(contactCommunications);
        }

        /// <summary>
        /// Export contact communications
        /// </summary>
        /// <returns></returns>
        public HSSFWorkbook Exports(int contactId)
        {
            var data = Fetch(c => c.ContactId == contactId);

            var contactCommunications = Maps(data);

            return ExcelUtilities.CreateWorkBook(contactCommunications);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="contactCommunications"></param>
        /// <returns></returns>
        private IQueryable<ContactCommunicationModel> Maps(IQueryable<ContactCommunication> contactCommunications)
        {
            return contactCommunications.Select(m => new ContactCommunicationModel
            {
                Id = m.Id,
                CurrentlyOwn = m.CurrentlyOwn,
                InterestedInOwning = m.InterestedInOwning,
                ProductOfInterest = m.ProductOfInterest,
                PurchaseDate = m.PurchaseDate,
                CampaignCode = m.CampaignCode,
                Certification = m.Certification,
                SubscriberType = m.SubscriberType,
                ReferredBy = m.ReferredBy,
                TimeFrameToOwn = m.TimeFrameToOwn,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get Contact Communication manage model for creating / editing
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ContactCommunicationManageModel GetContactCommunicationManageModel(int? contactId, int? id = null)
        {
            var contactCommunication = GetById(id);
            if (contactCommunication != null)
            {
                return new ContactCommunicationManageModel(contactCommunication);
            }
            if (contactId != null)
            {
                return new ContactCommunicationManageModel(contactId.Value);
            }
            return null;
        }

        /// <summary>
        /// Save ContactCommunication
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveContactCommunication(ContactCommunicationManageModel model)
        {
            ResponseModel response;
            var contactCommunication = GetById(model.Id);
            if (contactCommunication != null)
            {
                contactCommunication.CurrentlyOwn = model.CurrentlyOwn;
                contactCommunication.InterestedInOwning = model.InterestedInOwning;
                contactCommunication.ProductOfInterest = model.ProductOfInterest;
                contactCommunication.PurchaseDate = model.PurchaseDate;
                contactCommunication.CampaignCode = model.CampaignCode;
                contactCommunication.Certification = model.Certification;
                contactCommunication.SubscriberType = model.SubscriberType;
                contactCommunication.ReferredBy = model.ReferredBy;
                contactCommunication.TimeFrameToOwn = model.TimeFrameToOwn;
                contactCommunication.Comments = model.Comments;
                response = Update(contactCommunication);
                return response.SetMessage(response.Success
                    ? T("ContactCommunication_Message_UpdateSuccessfully")
                    : T("ContactCommunication_Message_UpdateFailure"));
            }
            Mapper.CreateMap<ContactCommunicationManageModel, ContactCommunication>();
            contactCommunication = Mapper.Map<ContactCommunicationManageModel, ContactCommunication>(model);
            response = Insert(contactCommunication);
            return response.SetMessage(response.Success
                ? T("ContactCommunication_Message_CreateSuccessfully")
                : T("ContactCommunication_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete contact communication
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteContactCommunication(int id)
        {
            var communication = GetById(id);

            if (communication != null)
            {
                var response = Delete(communication);
                return response.SetMessage(response.Success
                    ? T("ContactCommunication_Message_DeleteSuccessfully")
                    : T("ContactCommunication_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message =
                    T("ContactCommunication_Message_DeleteSuccessfully")
            };
        }

        #endregion
    }
}