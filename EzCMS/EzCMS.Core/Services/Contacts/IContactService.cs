using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Models.Contacts.Emails;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Contacts
{
    [Register(Lifetime.PerInstance)]
    public interface IContactService : IBaseService<Contact>
    {
        #region Contact Form

        /// <summary>
        /// Save contact form
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="communication"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        ResponseModel SaveContactForm(Contact contact, ContactCommunication communication, NameValueCollection form);

        #endregion

        #region Form Submit

        /// <summary>
        /// Save form
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="communication"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        Contact SaveForm(Contact contact, ContactCommunication communication, List<ContactInformation> formData);

        #endregion

        /// <summary>
        /// Check if contact is valid for creating new user
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        ResponseModel IsContactValidForCreateNewUser(int contactId);

        /// <summary>
        /// Create contact if not exists
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        Contact CreateContactIfNotExists(Contact contact);

        #region Grid Search

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        JqGridSearchOut SearchContacts(JqSearchIn si, ContactSearchModel searchModel);

        /// <summary>
        /// Export contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, ContactSearchModel searchModel);

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchContactsByUser(JqSearchIn si, int userId);

        /// <summary>
        /// Export contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        HSSFWorkbook ExportsContactsByUser(JqSearchIn si, GridExportMode gridExportMode, int userId);

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="contactGroupId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchContactsByContactGroup(JqSearchIn si, int contactGroupId);

        /// <summary>
        /// Export contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="contactGroupId"></param>
        /// <returns></returns>
        HSSFWorkbook ExportsContactsByContactGroup(JqSearchIn si, GridExportMode gridExportMode, int contactGroupId);

        #endregion

        #region Search Contacts

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        IQueryable<Contact> SearchContacts(ContactSearchModel searchModel);

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IQueryable<Contact> SearchContactsByUser(int userId);

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="contactGroupId"></param>
        /// <returns></returns>
        IQueryable<Contact> SearchContactsByContactGroup(int contactGroupId);

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        IQueryable<Contact> SearchContactsByEmail(string email);

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="searchModels"></param>
        /// <returns></returns>
        IQueryable<Contact> SearchContacts(IEnumerable<ContactSearchModel> searchModels);

        #endregion

        #region Manage

        /// <summary>
        /// Get contact details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ContactDetailModel GetContactDetailsModel(int? id = null);

        /// <summary>
        /// Get contact manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ContactManageModel GetContactManageModel(int? id = null);

        /// <summary>
        /// Get contact search details models
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
        IEnumerable<ContactSearchDetailsModel> GetContactSearchDetailsModels(string queries);

        /// <summary>
        /// Save contact
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveContact(ContactManageModel model);

        /// <summary>
        /// Search contacts by keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        List<AutoCompleteModel> SearchContacts(string keyword);

        /// <summary>
        /// Update contact data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateContactData(XEditableModel model);

        /// <summary>
        /// Delete contact
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteContact(int id);

        /// <summary>
        /// Delete contact - user mapping
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteContactUserMapping(int id);

        #endregion

        #region Company Admin

        /// <summary>
        /// Search company user contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchCompanyUserContacts(JqSearchIn si, int userId);

        /// <summary>
        /// Get contact information for company
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ContactDetailModel GetCompanyContactDetailsModel(int? id = null);

        #endregion
    }
}