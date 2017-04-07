using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.ContactGroups;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Services.Contacts;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.ContactGroupContacts;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.ContactGroups
{
    public class ContactGroupService : ServiceHelper, IContactGroupService
    {
        private readonly ContactGroupContactRepository _contactGroupContactRepository;
        private readonly IRepository<ContactGroup> _contactGroupRepository;
        private readonly IContactService _contactService;

        public ContactGroupService(IRepository<ContactGroup> contactGroupRepository,
            ContactGroupContactRepository contactGroupContactRepository, IContactService contactService)
        {
            _contactGroupRepository = contactGroupRepository;
            _contactGroupContactRepository = contactGroupContactRepository;
            _contactService = contactService;
        }

        #region Validation

        /// <summary>
        /// Check if contact group exists
        /// </summary>
        /// <param name="id">the contact group id</param>
        /// <param name="name">the contact group name</param>
        /// <returns></returns>
        public bool IsNameExisted(int? id, string name)
        {
            return Fetch(u => u.Name.Equals(name) && u.Id != id).Any();
        }

        #endregion

        /// <summary>
        /// Count contacts currently in the group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int CountContacts(int id)
        {
            var contactGroup = GetById(id);
            if (contactGroup != null)
            {
                return contactGroup.ContactGroupContacts.Count;
            }

            return 0;
        }

        /// <summary>
        /// Calculate number of contacts in this group queries
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int CalculateContacts(int id)
        {
            var contactGroup = GetById(id);
            if (contactGroup != null)
            {
                return GetContacts(contactGroup.Queries).Count();
            }

            return 0;
        }

        #region Base

        public IQueryable<ContactGroup> GetAll()
        {
            return _contactGroupRepository.GetAll();
        }

        public IQueryable<ContactGroup> Fetch(Expression<Func<ContactGroup, bool>> expression)
        {
            return _contactGroupRepository.Fetch(expression);
        }

        public ContactGroup FetchFirst(Expression<Func<ContactGroup, bool>> expression)
        {
            return _contactGroupRepository.FetchFirst(expression);
        }

        public ContactGroup GetById(object id)
        {
            return _contactGroupRepository.GetById(id);
        }

        public ContactGroup GetByName(string name)
        {
            return _contactGroupRepository.FetchFirst(x => x.Name.Equals(name));
        }

        internal ResponseModel Insert(ContactGroup contactGroup)
        {
            return _contactGroupRepository.Insert(contactGroup);
        }

        internal ResponseModel Update(ContactGroup contactGroup)
        {
            return _contactGroupRepository.Update(contactGroup);
        }

        internal ResponseModel Delete(ContactGroup contactGroup)
        {
            return _contactGroupRepository.Delete(contactGroup);
        }

        internal ResponseModel Delete(object id)
        {
            return _contactGroupRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _contactGroupRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the contact groups
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchContactGroups(JqSearchIn si)
        {
            var data = GetAll();

            var contactGroups = Maps(data);

            return si.Search(contactGroups);
        }

        /// <summary>
        /// Export contact groups
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var contactGroups = Maps(data);

            var exportData = si.Export(contactGroups, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="contactGroups"></param>
        /// <returns></returns>
        private IQueryable<ContactGroupModel> Maps(IQueryable<ContactGroup> contactGroups)
        {
            return contactGroups.Select(m => new ContactGroupModel
            {
                Id = m.Id,
                Name = m.Name,
                Queries = m.Queries,
                ContactCount = m.ContactGroupContacts.Count,
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
        /// Get contact group manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ContactGroupManageModel GetContactGroupManageModel(int? id = null)
        {
            var contactGroup = GetById(id);
            if (contactGroup != null)
            {
                return new ContactGroupManageModel(contactGroup);
            }
            return new ContactGroupManageModel();
        }

        /// <summary>
        /// Get all contacts that fit queries
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
        public IQueryable<Contact> GetContacts(string queries)
        {
            // If queries is empty, return empty list
            if (string.IsNullOrEmpty(queries))
            {
                return new List<Contact>().AsQueryable();
            }

            var contactSearchModels = SerializeUtilities.Deserialize<List<ContactSearchModel>>(queries);
            return _contactService.SearchContacts(contactSearchModels);
        }

        /// <summary>
        /// Get active contact groups
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetActiveContactGroups()
        {
            return GetAll().Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = SqlFunctions.StringConvert((double) m.Id).Trim()
            });
        }

        /// <summary>
        /// Save contact group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveContactGroup(ContactGroupManageModel model)
        {
            ResponseModel response;

            var contactGroup = GetById(model.Id);
            if (contactGroup != null)
            {
                contactGroup.Name = model.Name;
                contactGroup.Queries = model.Queries;
                contactGroup.Active = model.Active;

                response = Update(contactGroup);
                if (response.Success)
                {
                    RefreshContactGroup((int) response.Data);
                }
                return response.SetMessage(response.Success
                    ? T("ContactGroup_Message_UpdateSuccessfully")
                    : T("ContactGroup_Message_UpdateFailure"));
            }

            Mapper.CreateMap<ContactGroupManageModel, ContactGroup>();
            contactGroup = Mapper.Map<ContactGroupManageModel, ContactGroup>(model);

            response = Insert(contactGroup);
            if (response.Success)
            {
                RefreshContactGroup((int) response.Data);
            }
            return response.SetMessage(response.Success
                ? T("ContactGroup_Message_CreateSuccessfully")
                : T("ContactGroup_Message_CreateFailure"));
        }

        /// <summary>
        /// Refresh contact group static contacts
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel RefreshContactGroup(int id)
        {
            var contactGroup = GetById(id);
            if (contactGroup != null)
            {
                // Delete old contact group contacts
                var response = _contactGroupContactRepository.DeleteByContactGroupId(id);

                // Save new contact group contacts
                var contacts = GetContacts(contactGroup.Queries);
                if (response.Success && contacts != null && contacts.Any())
                {
                    response = _contactGroupContactRepository.InsertForGroup(contactGroup.Id,
                        contacts.Select(contact => contact.Id));
                }

                return response.SetMessage(response.Success
                    ? T("ContactGroup_Message_RefreshSuccessfully")
                    : T("ContactGroup_Message_RefreshFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("ContactGroup_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Add contact search to group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel AddToGroup(AddToGroupModel model)
        {
            var contactGroupManageModel = new ContactGroupManageModel();

            var contactSearchModel = SerializeUtilities.Deserialize<ContactSearchModel>(model.ContactSearchModel);
            var contactSearchModels = new List<ContactSearchModel>();

            switch (model.AddToGroupType)
            {
                case ContactGroupEnums.AddToGroupType.New:
                    contactSearchModels.Add(contactSearchModel);

                    contactGroupManageModel.Name = model.Name;
                    contactGroupManageModel.Queries = SerializeUtilities.Serialize(contactSearchModels);
                    break;
                case ContactGroupEnums.AddToGroupType.Existing:
                    var contactGroup = GetById(model.Id);
                    if (contactGroup != null)
                    {
                        contactSearchModels =
                            SerializeUtilities.Deserialize<List<ContactSearchModel>>(contactGroup.Queries);
                        contactSearchModels.Add(contactSearchModel);

                        contactGroupManageModel.Id = contactGroup.Id;
                        contactGroupManageModel.Name = contactGroup.Name;
                        contactGroupManageModel.Queries = SerializeUtilities.Serialize(contactSearchModels);
                        contactGroupManageModel.Active = contactGroup.Active;
                    }
                    break;
            }

            var response = SaveContactGroup(contactGroupManageModel);
            return response.SetMessage(response.Success
                ? T("ContactGroup_Message_AddToGroupSuccessfully")
                : T("ContactGroup_Message_AddToGroupFailure"));
        }

        /// <summary>
        /// Delete contact group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteContactGroup(int id)
        {
            var contactGroup = GetById(id);
            if (contactGroup != null)
            {
                // Delete old contact group contacts
                var response = _contactGroupContactRepository.DeleteByContactGroupId(id);

                if (response.Success)
                {
                    response = Delete(contactGroup);
                }

                return response.SetMessage(response.Success
                    ? T("ContactGroup_Message_DeleteSuccessfully")
                    : T("ContactGroup_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("ContactGroup_Message_DeleteSuccessfully")
            };
        }

        /// <summary>
        /// Change contact group active state
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel ChangeActiveState(int id)
        {
            var contactGroup = GetById(id);

            if (contactGroup == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("ContactGroup_Message_ObjectNotFound")
                };
            }

            contactGroup.Active = !contactGroup.Active;

            var response = Update(contactGroup);

            if (response.Success)
            {
                response.Data = contactGroup.Active;
                return response.SetMessage(T("ContactGroup_Message_UpdateSuccessfully"));
            }
            return response.SetMessage(T("ContactGroup_Message_UpdateFailure"));
        }

        #endregion

        #region Details

        /// <summary>
        /// Get contact group detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ContactGroupDetailModel GetContactGroupDetailModel(int? id = null)
        {
            var contactGroup = GetById(id);
            return contactGroup != null ? new ContactGroupDetailModel(contactGroup) : null;
        }

        /// <summary>
        /// Update contact group data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateContactGroupData(XEditableModel model)
        {
            var contactGroup = GetById(model.Pk);
            if (contactGroup != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (ContactGroupManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new ContactGroupManageModel(contactGroup);
                    manageModel.SetProperty(model.Name, value);

                    var validationResults = manageModel.ValidateModel();

                    if (validationResults.Any())
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = validationResults.BuildValidationMessages()
                        };
                    }

                    #endregion

                    contactGroup.SetProperty(model.Name, value);
                    var response = Update(contactGroup);
                    return response.SetMessage(response.Success
                        ? T("ContactGroup_Message_UpdateCompanyInfoSuccessfully")
                        : T("ContactGroup_Message_UpdateCompanyInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("ContactGroup_Message_PropertyNotFound")
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("ContactGroup_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}