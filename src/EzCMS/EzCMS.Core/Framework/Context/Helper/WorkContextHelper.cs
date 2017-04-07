using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.IoC;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Reflection;
using Ez.Framework.Utilities.Time;
using Ez.Framework.Utilities.Web;
using EzCMS.Core.Core.Resources.Languages;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.EmbeddedResource;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Framework.Plugins.Resources;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Services.Contacts;
using EzCMS.Core.Services.Languages;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Web;

namespace EzCMS.Core.Framework.Context.Helper
{
    public static class WorkContextHelper
    {
        #region Resource Manager


        /// <summary>
        /// Get available resource managers
        /// </summary>
        /// <returns></returns>
        public static List<ResourceManager> GetAvailableResourceManagers()
        {
            //Return stored resource manager if any
            var availableResourceManagers = StateManager.GetApplication<List<ResourceManager>>(EzCMSContants.AvailableResourceManagers);

            if (availableResourceManagers == null)
            {
                List<ResourceManager> resourceManagers = new List<ResourceManager>();

                //Get core resource manager
                #region Core resource managers

                var resources = DataInitializeHelper.GetAllResourceNamesInFolder(string.Empty, DataSetupResourceType.Languages);

                availableResourceManagers = new List<ResourceManager>();
                var businessAssembly = ReflectionUtilities.GetAssembly(EzCMSContants.EzCMSCoreProject);
                foreach (var resource in resources)
                {
                    var resourceName = resource.Replace(".resources", string.Empty);
                    resourceManagers.Add(new ResourceManager(resourceName, businessAssembly));
                }

                #region Plugin resource managers

                var pluginResourceManagers = HostContainer.GetInstances<IResourceRegister>();
                foreach (var pluginResourceManager in pluginResourceManagers)
                {
                    resourceManagers.AddRange(pluginResourceManager.RegisterResources());
                }

                #endregion

                #endregion

                StateManager.SetApplication(EzCMSContants.AvailableResourceManagers, availableResourceManagers);
            }

            return availableResourceManagers;
        }

        /// <summary>
        /// Get current resource manager
        /// </summary>
        /// <param name="currentCulture"></param>
        /// <returns></returns>
        public static List<ResourceManager> GetCurrentResourceManager(string currentCulture)
        {
            var resourceNamespace = string.Format(EzCMSContants.LanguageResourceNamespaceFormat, currentCulture);

            if (GetAvailableResourceManagers().Any(r => r.BaseName.Equals(resourceNamespace)))
            {
                return GetAvailableResourceManagers().Where(r => r.BaseName.Equals(resourceNamespace)).ToList();
            }

            //Return default resource manager
            return new List<ResourceManager>()
            {
                Resources.ResourceManager
            };
        }

        #endregion

        #region Culture

        /// <summary>
        /// Get current culture
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentCulture()
        {
            var culture = StateManager.GetSession<string>(EzCMSContants.CurrentCuture);

            if (string.IsNullOrEmpty(culture))
            {
                // Set default language as current culture
                var languageService = HostContainer.GetInstance<ILanguageService>();
                var language = languageService.GetDefault();

                culture = language == null ? CultureInfo.CurrentCulture.Name : language.Key;

                StateManager.SetSession(EzCMSContants.CurrentCuture, culture);
            }

            return culture;
        }

        /// <summary>
        /// Set culture
        /// </summary>
        /// <param name="value"></param>
        public static void SetCurrentCulture(string value)
        {

            var languageService = HostContainer.GetInstance<ILanguageService>();
            var language = languageService.GetByKey(value) ?? languageService.GetDefault();

            var culture = language == null ? CultureInfo.CurrentCulture.Name : language.Key;

            StateManager.SetSession(EzCMSContants.CurrentCuture, culture);
        }

        #endregion

        #region Timezone

        /// <summary>
        /// Get current user timezone cookie
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static string GetCurrentTimezoneCookie(UserSessionModel currentUser)
        {
            if (HttpContext.Current != null)
            {
                if (currentUser != null && currentUser.UserSettingModel != null && !string.IsNullOrEmpty(currentUser.UserSettingModel.TimeZone))
                {
                    return currentUser.UserSettingModel.TimeZone.TimeZoneInfoToOlsonTimeZone();
                }
            }

            var timezone = HttpUtility.UrlDecode(StateManager.GetCookie<string>(EzCMSContants.CurrentTimezone));
            if (!string.IsNullOrEmpty(timezone))
            {
                return timezone;
            }

            return TimeZoneInfo.Utc.Id.TimeZoneInfoToOlsonTimeZone();
        }

        #endregion

        #region User & Contact

        /// <summary>
        /// Get current contact info
        /// </summary>
        /// <returns></returns>
        public static ContactCookieModel GetCurrentContact()
        {

            var currentContact = StateManager.GetCookie<ContactCookieModel>(EzCMSContants.CurrentContactInformation);

            /*
             * This is the first time user come to site and has no cookie. We will
             *  - Create new cookie
             *  - Create an anonymous contact for user
             */
            if (currentContact == null)
            {
                //Create new anonymous contact
                var annonymousContactRepository = HostContainer.GetInstance<IRepository<AnonymousContact>>();
                var annonymousContact = new AnonymousContact
                {
                    CookieKey = PasswordUtilities.GenerateUniqueKey(),
                    IpAddress = HttpContext.Current.GetUserAgentInformationFromRequest().IpAddress
                };
                annonymousContactRepository.Insert(annonymousContact);

                //Setup contact
                currentContact = new ContactCookieModel
                {
                    AnonymousContactId = annonymousContact.Id,
                    CookieKey = annonymousContact.CookieKey,
                    IpAddress = annonymousContact.IpAddress
                };

                //Set current contact to cookie
                StateManager.SetCookie(EzCMSContants.CurrentContactInformation, currentContact, DateTime.UtcNow.AddYears(1));
            }

            return currentContact;
        }

        /// <summary>
        /// Save contact info
        /// </summary>
        /// <param name="model"></param>
        public static void SetCurrentContact(ContactCookieModel model)
        {

            var annonymousContactRepository = HostContainer.GetInstance<IRepository<AnonymousContact>>();

            var anonymousContact = annonymousContactRepository.FetchFirst(c => c.CookieKey.Equals(model.CookieKey));

            /*
             * If we cannot find the anonymous contact of user, maybe because the data is removed, user log to different account or user remove cookie. We will
             *  - Create an anonymous contact for user
             */
            if (anonymousContact == null)
            {
                anonymousContact = new AnonymousContact
                {
                    ContactId = model.ContactId,
                    CookieKey = model.CookieKey,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Phone = model.Phone,
                    Address = model.Address,
                    IpAddress = HttpContext.Current.GetUserAgentInformationFromRequest().IpAddress
                };

                annonymousContactRepository.Insert(anonymousContact);

                model = new ContactCookieModel(anonymousContact);
            }
            else
            {
                if (model.ContactId.HasValue)
                {
                    /*
                     * If the contact ID in database has no model, then maybe user just have an account so we will link the anonymous contact to current contact
                     */
                    if (!anonymousContact.ContactId.HasValue)
                    {
                        anonymousContact.ContactId = model.ContactId;
                        anonymousContact.Email = model.Email;
                        anonymousContact.FirstName = model.FirstName;
                        anonymousContact.LastName = model.LastName;
                        anonymousContact.Phone = model.Phone;
                        anonymousContact.IpAddress = model.IpAddress;
                        anonymousContact.Address = model.Address;

                        annonymousContactRepository.Update(anonymousContact);

                        model = new ContactCookieModel(anonymousContact);
                    }
                    else
                    {
                        /*
                         * If the contact ID in database has model but different with current contact id, then user may change the account. So we will
                         *  - Create new anonymous for current user
                         */
                        if (model.ContactId.Value != anonymousContact.ContactId.Value)
                        {
                            anonymousContact = new AnonymousContact
                            {
                                ContactId = model.ContactId,
                                CookieKey = PasswordUtilities.GenerateUniqueKey(),
                                Email = model.Email,
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                Phone = model.Phone,
                                Address = model.Address,
                                IpAddress = HttpContext.Current.GetUserAgentInformationFromRequest().IpAddress
                            };

                            annonymousContactRepository.Insert(anonymousContact);

                            model = new ContactCookieModel(anonymousContact);
                        }
                        else
                        {
                            /*
                             * If the contact ID in database has model and equal with current contact id, then we will check if there are any contact information updates or not.
                             *  - If yes, then update anonymous contact information
                             */
                            if (!anonymousContact.FirstName.AreEqual(model.FirstName) ||
                                !anonymousContact.LastName.AreEqual(model.LastName) ||
                                !anonymousContact.Email.AreEqual(model.Email) ||
                                !anonymousContact.Phone.AreEqual(model.Phone) ||
                                !anonymousContact.IpAddress.AreEqual(model.IpAddress) ||
                                !anonymousContact.Address.AreEqual(model.Address))
                            {
                                anonymousContact.FirstName = model.FirstName;
                                anonymousContact.LastName = model.LastName;
                                anonymousContact.Email = model.Email;
                                anonymousContact.Phone = model.Phone;
                                anonymousContact.IpAddress = model.IpAddress;
                                anonymousContact.Address = model.Address;

                                annonymousContactRepository.Update(anonymousContact);

                                model = new ContactCookieModel(anonymousContact);
                            }
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(anonymousContact.FirstName)
                        && !string.IsNullOrEmpty(anonymousContact.LastName)
                        && (!string.IsNullOrEmpty(anonymousContact.Email) || !string.IsNullOrEmpty(anonymousContact.Phone)))
                    {
                        var contactService = HostContainer.GetInstance<IContactService>();
                        var contact = contactService.CreateContactIfNotExists(new Contact
                        {
                            FirstName = anonymousContact.FirstName,
                            LastName = anonymousContact.LastName,
                            Email = anonymousContact.Email,
                            PreferredPhoneNumber = anonymousContact.Phone,
                            AddressLine1 = anonymousContact.Address,
                        });

                        anonymousContact.ContactId = contact.Id;

                        model = new ContactCookieModel(anonymousContact);
                    }
                }
            }

            StateManager.SetCookie(EzCMSContants.CurrentContactInformation, model, DateTime.UtcNow.AddYears(1));
        }
        #endregion

        #region Others

        /// <summary>
        /// Get git version of EzCMS
        /// </summary>
        /// <returns></returns>
        public static string GetEzCMSVersion()
        {

            var version = StateManager.GetApplication<string>(EzCMSContants.EzCMSVersion);

            if (string.IsNullOrEmpty(version))
            {
                version = EmbeddedResourceHelper.GetString("version.txt", "EzCMS.Web.App_Data");
                if (HttpContext.Current != null)
                {
                    StateManager.SetApplication(EzCMSContants.EzCMSVersion, version);
                }
            }
            return version;
        }

        #endregion
    }
}
