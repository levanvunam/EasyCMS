using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.Mvc.Models;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Models.Contacts.Emails;
using EzCMS.Core.Services.Companies;
using EzCMS.Core.Services.ContactCommunications;
using EzCMS.Core.Services.EmailLogs;
using EzCMS.Core.Services.EmailTemplates;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.ContactGroupContacts;
using EzCMS.Entity.Repositories.Users;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Contacts
{
    public class ContactService : ServiceHelper, IContactService
    {
        private readonly ICompanyService _companyService;
        private readonly IContactCommunicationService _contactCommunicationService;
        private readonly IContactGroupContactRepository _contactGroupContactRepository;
        private readonly IRepository<Contact> _contactRepository;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ISiteSettingService _siteSettingService;
        private readonly IUserRepository _userRepository;

        public ContactService(IRepository<Contact> contactRepository, ISiteSettingService siteSettingService,
            ICompanyService companyService, IContactGroupContactRepository contactGroupContactRepository,
            IEmailTemplateService emailTemplateService, IEmailLogService emailLogService,
            IContactCommunicationService contactCommunicationService, IUserRepository userRepository)
        {
            _siteSettingService = siteSettingService;
            _contactRepository = contactRepository;
            _companyService = companyService;
            _contactGroupContactRepository = contactGroupContactRepository;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _contactCommunicationService = contactCommunicationService;
            _userRepository = userRepository;
        }

        #region Contact Form

        /// <summary>
        /// Save contact form
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="communication"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public ResponseModel SaveContactForm(Contact contact, ContactCommunication communication,
            NameValueCollection form)
        {
            var formData = form.AllKeys.SelectMany(form.GetValues, (k, v) => new ContactInformation
            {
                Key = k,
                Value = v
            }).ToList();

            // Save contact
            SaveForm(contact, communication, formData);

            #region Generate emails

            var emailFrom = formData.FirstOrDefault(f => f.Key.Equals("EmailFrom"));
            var emailTo = formData.FirstOrDefault(f => f.Key.Equals("EmailTo"));

            if (emailTo != null && !string.IsNullOrEmpty(emailTo.Value))
            {
                var contactInformation = formData.Where(f => !f.Key.Equals("EmailFrom")
                                                             && !f.Key.Equals("EmailTo")).ToList();

                var model = new ContactFormEmailModel
                {
                    Email = contact.Email,
                    FullName = string.Format("{0} {1}", contact.FirstName, contact.LastName),
                    ContactInformation = contactInformation
                };

                var emailResponse = _emailTemplateService.ParseEmail(EmailEnums.EmailTemplateType.ContactForm, model);

                if (emailResponse == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Contact_Message_MissingContactFormEmailTemplate")
                    };
                }

                var emailLog = new EmailLog
                {
                    To = emailTo.Value,
                    ToName = emailTo.Value,
                    From = emailFrom == null ? string.Empty : emailFrom.Value,
                    FromName = emailFrom == null ? string.Empty : emailFrom.Value,
                    CC = emailResponse.CC,
                    Bcc = emailResponse.BCC,
                    Subject = emailResponse.Subject,
                    Body = emailResponse.Body,
                    Priority = EmailEnums.EmailPriority.Medium
                };

                var response = _emailLogService.CreateEmail(emailLog, true);

                return response.Success
                    ? response.SetMessage(T("Contact_Message_ContactFormActionSuccessfully"))
                    : response.SetMessage(T("Contact_Message_ContactFormActionFailure"));
            }

            #endregion

            return new ResponseModel
            {
                Success = true,
                Message = T("Contact_Message_ContactFormActionSuccessfully")
            };
        }

        #endregion

        #region Form Submit

        /// <summary>
        /// Save from
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="communication"></param>
        /// <param name="formData"></param>
        public Contact SaveForm(Contact contact, ContactCommunication communication, List<ContactInformation> formData)
        {
            #region Parse Data

            var fullname = formData.FirstOrDefault(k => k.Key.Equals("FullName"));
            if (fullname != null && string.IsNullOrEmpty(contact.FirstName) && string.IsNullOrEmpty(contact.LastName))
            {
                string firstName;
                string lastName;
                fullname.Value.GenerateName(out firstName, out lastName);
                contact.FirstName = firstName;
                contact.LastName = lastName;
            }

            #endregion

            #region Generate contact

            var currentContact = CreateContactIfNotExists(contact);

            if (currentContact != null)
            {
                Mapper.CreateMap<Contact, Contact>()
                    .ForMember(c => c.Id, opt => opt.Ignore())
                    .ForMember(c => c.Created, opt => opt.Ignore())
                    .ForAllMembers(opt => opt.Condition(srs => !srs.IsSourceValueNull));

                Mapper.Map(contact, currentContact);
                currentContact.Company = _companyService.SaveCompany(contact.Company);
                Update(currentContact);
            }

            if (currentContact != null && communication != null)
            {
                communication.ContactId = currentContact.Id;
                _contactCommunicationService.Insert(communication);
            }

            #endregion

            WorkContext.CurrentContact = new ContactCookieModel(currentContact);

            return currentContact;
        }

        #endregion

        /// <summary>
        /// Check if contact is valid for creating new user
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public ResponseModel IsContactValidForCreateNewUser(int contactId)
        {
            var contact = GetById(contactId);

            if (contact != null)
            {
                if (contact.UserId.HasValue)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Contact_Message_ContactBelongToUserAlready")
                    };
                }

                if (_userRepository.GetByEmail(contact.Email) != null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Contact_Message_EmailExists")
                    };
                }

                return new ResponseModel
                {
                    Success = true
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("Contact_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Create contact if not exists
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public Contact CreateContactIfNotExists(Contact contact)
        {
            var currentContact = FetchFirst(c => c.FirstName.Equals(contact.FirstName)
                                                 && c.LastName.Equals(contact.LastName)
                                                 && (c.PhoneHome.Equals(contact.PhoneHome)
                                                     || c.PhoneWork.Equals(contact.PhoneWork)
                                                     || c.PreferredPhoneNumber.Equals(contact.PreferredPhoneNumber)));
            if (currentContact == null)
            {
                currentContact = FetchFirst(c => c.FirstName.Equals(contact.FirstName)
                                                 && c.LastName.Equals(contact.LastName)
                                                 && c.Email.Equals(contact.Email));
                if (currentContact == null)
                {
                    contact.Company = _companyService.SaveCompany(contact.Company);
                    var response = Insert(contact);
                    if (response.Success)
                    {
                        currentContact = GetById(response.Data);
                    }
                }
            }

            return currentContact;
        }

        #region Base

        public IQueryable<Contact> GetAll()
        {
            return _contactRepository.GetAll();
        }

        public IQueryable<Contact> Fetch(Expression<Func<Contact, bool>> expression)
        {
            return _contactRepository.Fetch(expression);
        }

        public Contact FetchFirst(Expression<Func<Contact, bool>> expression)
        {
            return _contactRepository.FetchFirst(expression);
        }

        public Contact GetById(object id)
        {
            return _contactRepository.GetById(id);
        }

        internal ResponseModel Insert(Contact contact)
        {
            return _contactRepository.Insert(contact);
        }

        internal ResponseModel Update(Contact contact)
        {
            return _contactRepository.Update(contact);
        }

        internal ResponseModel Delete(Contact contact)
        {
            return _contactRepository.Delete(contact);
        }

        internal ResponseModel Delete(object id)
        {
            return _contactRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _contactRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchContacts(JqSearchIn si, ContactSearchModel searchModel)
        {
            var data = SearchContacts(searchModel);

            var contacts = Maps(data);

            return si.Search(contacts);
        }

        /// <summary>
        /// Export contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="searchModel"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, ContactSearchModel searchModel)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchContacts(searchModel);

            var contacts = Maps(data);

            var exportData = si.Export(contacts, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchContactsByUser(JqSearchIn si, int userId)
        {
            var data = SearchContactsByUser(userId);

            var contacts = Maps(data);

            return si.Search(contacts);
        }

        /// <summary>
        /// Export contacts of user
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public HSSFWorkbook ExportsContactsByUser(JqSearchIn si, GridExportMode gridExportMode, int userId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchContactsByUser(userId);

            var contacts = Maps(data);

            var exportData = si.Export(contacts, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="contactGroupId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchContactsByContactGroup(JqSearchIn si, int contactGroupId)
        {
            var data = SearchContactsByContactGroup(contactGroupId);

            var contacts = Maps(data);

            return si.Search(contacts);
        }

        /// <summary>
        /// Export contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="contactGroupId"></param>
        /// <returns></returns>
        public HSSFWorkbook ExportsContactsByContactGroup(JqSearchIn si, GridExportMode gridExportMode,
            int contactGroupId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchContactsByContactGroup(contactGroupId);

            var contacts = Maps(data);

            var exportData = si.Export(contacts, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="contacts"></param>
        /// <returns></returns>
        private IQueryable<ContactModel> Maps(IQueryable<Contact> contacts)
        {
            return contacts.Select(c => new ContactModel
            {
                Id = c.Id,
                BelongToUser = c.UserId.HasValue,
                UserId = c.UserId,
                Email = c.Email,
                Title = c.Title,
                IsCompanyAdministrator = c.IsCompanyAdministrator,
                FirstName = c.FirstName,
                LastName = c.LastName,
                AddressLine1 = c.AddressLine1,
                AddressLine2 = c.AddressLine2,
                Suburb = c.Suburb,
                State = c.State,
                Postcode = c.Postcode,
                Country = c.Country,
                Department = c.Department,
                PostalAddressLine1 = c.PostalAddressLine1,
                PostalAddressLine2 = c.PostalAddressLine2,
                PostalSuburb = c.PostalSuburb,
                PostalState = c.PostalState,
                PostalPostcode = c.PostalPostcode,
                PostalCountry = c.PostalCountry,
                PreferredPhoneNumber = c.PreferredPhoneNumber,
                PhoneWork = c.PhoneWork,
                PhoneHome = c.PhoneHome,
                MobilePhone = c.MobilePhone,
                Fax = c.Fax,
                Company = c.Company,
                GroupNames = c.ContactGroupContacts.Select(x => x.ContactGroup.Name),
                Occupation = c.Occupation,
                Website = c.Website,
                Sex = c.Sex,
                DateOfBirth = c.DateOfBirth,
                DontSendMarketing = c.DontSendMarketing,
                Unsubscribed = c.Unsubscribed,
                UnsubscribeDateTime = c.UnsubscribeDateTime,
                SubscriptionType = c.SubscriptionType,
                Confirmed = c.Confirmed,
                ConfirmDateTime = c.ConfirmDateTime,
                FromIPAddress = c.FromIPAddress,
                ValidatedOk = c.ValidatedOk,
                ValidateLevel = c.ValidateLevel,
                CRMID = c.CRMID,
                SalesPerson = c.SalesPerson,
                UnsubscribedIssueId = c.UnsubscribedIssueId,
                Facebook = c.Facebook,
                Twitter = c.Twitter,
                LinkedIn = c.LinkedIn,
                RecordDeleted = c.RecordDeleted,
                RecordOrder = c.RecordOrder,
                Created = c.Created,
                CreatedBy = c.CreatedBy,
                LastUpdate = c.LastUpdate,
                LastUpdateBy = c.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Search Contacts

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public IQueryable<Contact> SearchContacts(ContactSearchModel searchModel)
        {
            var dontSendMarketing = default(bool?);
            if (searchModel.MarketingMaterial == ContactEnums.DontSendMarketing.DontSend)
            {
                dontSendMarketing = true;
            }
            else if (searchModel.MarketingMaterial == ContactEnums.DontSendMarketing.Send)
            {
                dontSendMarketing = false;
            }

            var interestedInOwning = default(bool?);
            switch (searchModel.InterestedInOwning)
            {
                case ContactEnums.InterestInOwning.IsInterest:
                    interestedInOwning = true;
                    break;
                case ContactEnums.InterestInOwning.IsNotInterest:
                    interestedInOwning = false;
                    break;
            }
            searchModel.Sex = searchModel.Sex.Where(m => !string.IsNullOrEmpty(m)).ToList();

            // Company filter
            var companies = new List<string>();
            if (searchModel.Companies != null)
            {
                companies.AddRange(searchModel.Companies);
            }

            //Company type filter
            var companyNames = new List<string>();

            // Add current company of company type to company name list for searching
            if (searchModel.CompanyTypeIds.Any())
            {
                companyNames.AddRange(_companyService.Fetch(c => searchModel.CompanyTypeIds.Contains(c.Id))
                    .Select(ct => ct.Name).ToList());
            }

            var contacts =
                GetAll().Where(c =>
                    // Keyword filters
                    (string.IsNullOrEmpty(searchModel.Keyword) || string.IsNullOrEmpty(c.FirstName) ||
                     c.FirstName.Contains(searchModel.Keyword) || string.IsNullOrEmpty(c.LastName) ||
                     c.LastName.Contains(searchModel.Keyword) || string.IsNullOrEmpty(c.Email) ||
                     c.Email.Contains(searchModel.Keyword))

                        // State/Suburb/Country filters
                    && (string.IsNullOrEmpty(searchModel.State) || c.State.Contains(searchModel.State))
                    && (string.IsNullOrEmpty(searchModel.Suburb) || c.Suburb.Contains(searchModel.Suburb))
                    && (string.IsNullOrEmpty(searchModel.Country) || c.Country.Contains(searchModel.Country))

                        // Company filters
                    && (!companies.Any() || companies.Contains(c.Company))

                        // Company type filters
                    && (!companyNames.Any() || companyNames.Contains(c.Company))

                        // Contact group filters
                    &&
                    (!searchModel.ContactGroupId.HasValue ||
                     c.ContactGroupContacts.Any(x => x.ContactGroup.Id == searchModel.ContactGroupId))

                        // Other filters
                    && (!searchModel.SalesPerson.Any() || searchModel.SalesPerson.Contains(c.SalesPerson))
                    && (!dontSendMarketing.HasValue || dontSendMarketing == c.DontSendMarketing)
                    &&
                    (!interestedInOwning.HasValue ||
                     c.ContactCommunications.Any(
                         cc => cc.InterestedInOwning == interestedInOwning && cc.ContactId == c.Id))
                    && ((!searchModel.ReferredBy.Any()
                         && !searchModel.SubscriberTypes.Any()
                         && !searchModel.ProductOfInterests.Any()
                         && !searchModel.CampaignCodes.Any()
                         && !searchModel.CurrentlyOwns.Any()
                         && !searchModel.Certifications.Any())
                        || c.ContactCommunications.Any(cc =>
                            (!searchModel.ReferredBy.Any() || searchModel.ReferredBy.Contains(cc.ReferredBy))
                            && (!searchModel.SubscriberTypes.Any()
                                || searchModel.SubscriberTypes.Contains(cc.SubscriberType))
                            && (!searchModel.ProductOfInterests.Any()
                                || searchModel.ProductOfInterests.Contains(cc.ProductOfInterest))
                            && (!searchModel.CampaignCodes.Any()
                                || searchModel.CampaignCodes.Contains(cc.CampaignCode))
                            && (!searchModel.CurrentlyOwns.Any()
                                || searchModel.CurrentlyOwns.Contains(cc.CurrentlyOwn))
                            && (!searchModel.Certifications.Any()
                                || searchModel.Certifications.Contains(cc.Certification))))
                    );

            return contacts;
        }

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IQueryable<Contact> SearchContactsByUser(int userId)
        {
            return Fetch(contact => contact.UserId == userId);
        }

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="contactGroupId"></param>
        /// <returns></returns>
        public IQueryable<Contact> SearchContactsByContactGroup(int contactGroupId)
        {
            return Fetch(contact =>
                contact.ContactGroupContacts.Select(contactGroupContact => contactGroupContact.ContactGroupId)
                    .Contains(contactGroupId));
        }

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public IQueryable<Contact> SearchContactsByEmail(string email)
        {
            return Fetch(c => !string.IsNullOrEmpty(c.Email) && c.Email.Equals(email));
        }

        /// <summary>
        /// Search contacts
        /// </summary>
        /// <param name="searchModels"></param>
        /// <returns></returns>
        public IQueryable<Contact> SearchContacts(IEnumerable<ContactSearchModel> searchModels)
        {
            IQueryable<Contact> data = null;

            foreach (var searchModel in searchModels)
            {
                data = data == null ? SearchContacts(searchModel) : data.Union(SearchContacts(searchModel));
            }

            return data ?? new List<Contact>().AsQueryable();
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get contact details model from contact id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ContactDetailModel GetContactDetailsModel(int? id = null)
        {
            var contact = GetById(id);
            return contact != null ? new ContactDetailModel(contact) : null;
        }

        /// <summary>
        /// Get contact manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ContactManageModel GetContactManageModel(int? id = null)
        {
            var contact = GetById(id);
            return contact != null ? new ContactManageModel(contact) : new ContactManageModel();
        }

        /// <summary>
        /// Get contact search details models
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
        public IEnumerable<ContactSearchDetailsModel> GetContactSearchDetailsModels(string queries)
        {
            if (!string.IsNullOrEmpty(queries))
            {
                // Deserialize contact queries string to object
                var contactSearchModels = SerializeUtilities.Deserialize<List<ContactSearchModel>>(queries);
                return
                    contactSearchModels.Select(contactSearchModel => new ContactSearchDetailsModel(contactSearchModel));
            }

            return new List<ContactSearchDetailsModel>();
        }

        /// <summary>
        /// Save contact
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveContact(ContactManageModel model)
        {
            ResponseModel response;

            var contact = GetById(model.Id);
            if (contact != null)
            {
                contact.Company = _companyService.SaveCompany(model.Company);

                contact.IsCompanyAdministrator = model.IsCompanyAdministrator;
                contact.Email = model.Email;
                contact.Title = model.Title;
                contact.FirstName = model.FirstName;
                contact.LastName = model.LastName;
                contact.AddressLine1 = model.AddressLine1;
                contact.AddressLine2 = model.AddressLine2;
                contact.Suburb = model.Suburb;
                contact.State = model.State;
                contact.Postcode = model.Postcode;
                contact.Country = model.Country;
                contact.Department = model.Department;
                contact.PostalAddressLine1 = model.PostalAddressLine1;
                contact.PostalAddressLine2 = model.PostalAddressLine2;
                contact.PostalSuburb = model.PostalSuburb;
                contact.PostalState = model.PostalState;
                contact.PostalPostcode = model.PostalPostcode;
                contact.PostalCountry = model.PostalCountry;
                contact.PreferredPhoneNumber = model.PreferredPhoneNumber;
                contact.PhoneWork = model.PhoneWork;
                contact.PhoneHome = model.PhoneHome;
                contact.MobilePhone = model.MobilePhone;
                contact.Fax = model.Fax;
                contact.Occupation = model.Occupation;
                contact.Website = model.Website;
                contact.Sex = model.Sex;
                contact.DateOfBirth = model.DateOfBirth;
                contact.DontSendMarketing = model.DontSendMarketing;
                contact.Unsubscribed = model.Unsubscribed;
                contact.UnsubscribeDateTime = model.UnsubscribeDateTime;
                contact.SubscriptionType = model.SubscriptionType;
                contact.Confirmed = model.Confirmed;
                contact.ConfirmDateTime = model.ConfirmDateTime;
                contact.FromIPAddress = model.FromIPAddress;
                contact.ValidatedOk = model.ValidatedOk;
                contact.ValidateLevel = model.ValidateLevel;
                contact.CRMID = model.CRMID;
                contact.SalesPerson = model.SalesPerson;
                contact.UnsubscribedIssueId = model.UnsubscribedIssueId;
                contact.Facebook = model.Facebook;
                contact.Twitter = model.Twitter;
                contact.LinkedIn = model.LinkedIn;

                response = Update(contact);

                return response.SetMessage(response.Success
                    ? T("Contact_Message_UpdateSuccessfully")
                    : T("Contact_Message_UpdateFailure"));
            }

            Mapper.CreateMap<ContactManageModel, Contact>();
            contact = Mapper.Map<Contact>(model);

            contact.Company = _companyService.SaveCompany(contact.Company);

            response = Insert(contact);
            return response.SetMessage(response.Success
                ? T("Contact_Message_CreateSuccessfully")
                : T("Contact_Message_CreateFailure"));
        }

        /// <summary>
        /// Search contacts by keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<AutoCompleteModel> SearchContacts(string keyword)
        {
            var pageSize = _siteSettingService.GetSetting<int>(SettingNames.AutoCompleteSize);
            return _contactRepository.Fetch(c => c.Email.Contains(keyword)).Take(pageSize).ToList()
                .Select(c => new AutoCompleteModel
                {
                    id = c.Id,
                    value =
                        string.Format("{0} {1} | {2} | {3} | {4}", c.Title, c.FullName, c.Email,
                            c.PreferredPhoneNumber ?? c.PhoneHome ?? c.PhoneWork,
                            c.AddressLine1 ?? c.AddressLine2),
                    label =
                        string.Format("{0} {1} | {2} | {3} | {4}", c.Title, c.FullName, c.Email,
                            c.PreferredPhoneNumber ?? c.PhoneHome ?? c.PhoneWork,
                            c.AddressLine1 ?? c.AddressLine2)
                }).ToList();
        }

        /// <summary>
        /// Update contact property
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateContactData(XEditableModel model)
        {
            var contact = GetById(model.Pk);
            if (contact != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (ContactManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new ContactManageModel(contact);
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

                    contact.SetProperty(model.Name, value);

                    var response = Update(contact);
                    return response.SetMessage(response.Success
                        ? T("Contact_Message_UpdateContactInfoSuccessfully")
                        : T("Contact_Message_UpdateContactInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Contact_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("Contact_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete contact
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteContact(int id)
        {
            var contact = GetById(id);
            if (contact != null)
            {
                if (contact.ContactCommunications.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Contact_Message_DeleteFailureBasedOnRelatedCommunications")
                    };
                }

                if (contact.Subscriptions.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Contact_Message_DeleteFailureBasedOnRelatedSubscriptions")
                    };
                }

                // Delete contact group - contact mapping
                _contactGroupContactRepository.DeleteByContactId(contact.Id);

                // Delete contact - user mapping
                if (contact.UserId.HasValue)
                {
                    if (contact.User.Contacts.Count(c => !c.RecordDeleted) == 1)
                        _userRepository.SetRecordDeleted(contact.UserId.Value);
                    else
                    {
                        DeleteContactUserMapping(contact.Id);
                    }
                }

                var response = _contactRepository.SetRecordDeleted(id);
                return response.SetMessage(response.Success
                    ? T("Contact_Message_DeleteSuccessfully")
                    : T("Contact_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("Contact_Message_DeleteSuccessfully")
            };
        }

        /// <summary>
        /// Delete contact - user mapping
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteContactUserMapping(int id)
        {
            var contact = GetById(id);
            if (contact != null)
            {
                contact.UserId = null;

                var response = Update(contact);
                return response.SetMessage(response.Success
                    ? T("ContactUser_Message_DeleteMappingSuccessfully")
                    : T("ContactUser_Message_DeleteMappingFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("Contact_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Company Admin

        /// <summary>
        /// Search company user contacts
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchCompanyUserContacts(JqSearchIn si, int userId)
        {
            var data = SearchCompanyContacts().Where(x => x.UserId.HasValue && x.UserId == userId);

            var contacts = Maps(data);

            return si.Search(contacts);
        }

        /// <summary>
        /// Get contact information for company
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ContactDetailModel GetCompanyContactDetailsModel(int? id = null)
        {
            var data = SearchCompanyContacts();

            var contact = data.FirstOrDefault(c => c.Id == id);

            if (contact != null)
            {
                return new ContactDetailModel(contact);
            }

            return null;
        }

        #region Private Methods

        /// <summary>
        /// Search company contacts
        /// </summary>
        /// <returns></returns>
        private IQueryable<Contact> SearchCompanyContacts()
        {
            if (WorkContext.CurrentUser.IsCompanyAdministrator && !string.IsNullOrEmpty(WorkContext.CurrentUser.Company))
            {
                return Fetch(contact => contact.Company.Equals(WorkContext.CurrentUser.Company));
            }
            return new List<Contact>().AsQueryable();
        }

        #endregion

        #endregion
    }
}