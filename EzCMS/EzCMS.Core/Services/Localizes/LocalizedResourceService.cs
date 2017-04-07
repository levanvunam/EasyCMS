using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.LocalizedResources;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.LocalizedResources;
using NPOI.HSSF.UserModel;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EzCMS.Core.Services.Localizes
{
    /// <summary>
    /// Localized resource service
    /// </summary>
    public class LocalizedResourceService : IEzCMSLocalizedResourceService
    {
        private readonly IRepository<Language> _languageRepository;
        private readonly ILocalizedResourceRepository _localizedResourceRepository;

        public LocalizedResourceService(ILocalizedResourceRepository localizedResourceRepository,
            IRepository<Language> languageRepository)
        {
            _localizedResourceRepository = localizedResourceRepository;
            _languageRepository = languageRepository;
        }

        #region Initialize

        /// <summary>
        /// Refresh localize resource dictionaries
        /// </summary>
        public void RefreshDictionary()
        {
            WorkContext.LocalizedResourceDictionary = GetAll()
                .Select(l => new LocalizedResourceModel
                {
                    TextKey = l.TextKey,
                    DefaultValue = l.DefaultValue,
                    Language = l.Language.Key,
                    TranslatedValue = l.TranslatedValue
                }).ToList()
                .Select(l => new LocalizeDictionaryItem
                {
                    Language = l.Language,
                    Key = ServiceHelper.BuildKey(l.Language, l.TextKey),
                    HasClientSetup = !string.IsNullOrEmpty(l.TranslatedValue),
                    Value = string.IsNullOrEmpty(l.TranslatedValue) ? l.DefaultValue : l.TranslatedValue
                }).ToList();
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check text key is existed or not
        /// </summary>
        /// <param name="localizedResourceId"></param>
        /// <param name="languageId"></param>
        /// <param name="textKey"></param>
        /// <returns></returns>
        public bool IsTextKeyExisted(int? localizedResourceId, int languageId, string textKey)
        {
            return
                Fetch(u => u.TextKey.Equals(textKey) && u.LanguageId == languageId && u.Id != localizedResourceId).Any();
        }

        #endregion

        /// <summary>
        /// Update localize resource
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateLocalizedResource(XEditableModel model)
        {
            var currentCuture = WorkContext.CurrentCulture;
            var localizeResource = FetchFirst(l => l.Language.Key.Equals(currentCuture) && l.TextKey.Equals(model.Name));
            if (localizeResource != null)
            {
                localizeResource.TranslatedValue = model.Value;
                var response = Update(localizeResource);
                if (response.Success)
                    RefreshDictionary();
                return response.SetMessage(response.Success
                    ? T("LocalizedResource_Message_UpdateSuccessfully")
                    : T("LocalizedResource_Message_UpdateFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("LocalizedResource_Message_ObjectNotFound")
            };
        }

        #region Base

        public IQueryable<LocalizedResource> GetAll()
        {
            return _localizedResourceRepository.GetAll();
        }

        public IQueryable<LocalizedResource> Fetch(Expression<Func<LocalizedResource, bool>> expression)
        {
            return _localizedResourceRepository.Fetch(expression);
        }

        public LocalizedResource FetchFirst(Expression<Func<LocalizedResource, bool>> expression)
        {
            return _localizedResourceRepository.FetchFirst(expression);
        }

        public LocalizedResource GetById(object id)
        {
            return _localizedResourceRepository.GetById(id);
        }

        internal ResponseModel Insert(LocalizedResource localizedResource)
        {
            return _localizedResourceRepository.Insert(localizedResource);
        }

        internal ResponseModel Update(LocalizedResource localizedResource)
        {
            return _localizedResourceRepository.Update(localizedResource);
        }

        internal ResponseModel Delete(LocalizedResource localizedResource)
        {
            return _localizedResourceRepository.Delete(localizedResource);
        }

        internal ResponseModel Delete(object id)
        {
            return _localizedResourceRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _localizedResourceRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid

        /// <summary>
        /// Search the localized resources
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchLocalizedResources(JqSearchIn si, LocalizedResourceSearchModel model)
        {
            var data = SearchLocalizedResources(model);

            var localizedResources = Maps(data);

            return si.Search(localizedResources);
        }

        /// <summary>
        /// Export localized resources
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, LocalizedResourceSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchLocalizedResources(model);

            var localizedResources = Maps(data);

            var exportData = si.Export(localizedResources, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search localized resources
        /// </summary>
        /// <returns></returns>
        private IQueryable<LocalizedResource> SearchLocalizedResources(LocalizedResourceSearchModel model)
        {
            return Fetch(localizedResource => localizedResource.LanguageId == model.LanguageId
                                              && (string.IsNullOrEmpty(model.Keyword)
                                                  ||
                                                  (!string.IsNullOrEmpty(localizedResource.TextKey) &&
                                                   localizedResource.TextKey.ToLower().Contains(model.Keyword.ToLower()))
                                                  ||
                                                  (!string.IsNullOrEmpty(localizedResource.DefaultValue) &&
                                                   localizedResource.DefaultValue.ToLower()
                                                       .Contains(model.Keyword.ToLower()))
                                                  ||
                                                  (!string.IsNullOrEmpty(localizedResource.TranslatedValue) &&
                                                   localizedResource.TranslatedValue.ToLower()
                                                       .Contains(model.Keyword.ToLower()))));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="localizedResources"></param>
        /// <returns></returns>
        private IQueryable<LocalizedResourceModel> Maps(IQueryable<LocalizedResource> localizedResources)
        {
            return localizedResources.Select(l => new LocalizedResourceModel
            {
                Id = l.Id,
                TextKey = l.TextKey,
                LanguageId = l.LanguageId,
                Language = l.Language.Key,
                DefaultValue = l.DefaultValue,
                TranslatedValue = l.TranslatedValue,
                RecordOrder = l.RecordOrder,
                Created = l.Created,
                CreatedBy = l.CreatedBy,
                LastUpdate = l.LastUpdate,
                LastUpdateBy = l.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get localized resource manage model for editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LocalizedResourceManageModel GetLocalizedResourceManageModel(int? id = null)
        {
            var localizedResource = GetById(id);
            return localizedResource != null ? new LocalizedResourceManageModel(localizedResource) : null;
        }

        /// <summary>
        /// Save localized resource
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveLocalizedResource(LocalizedResourceManageModel model)
        {
            ResponseModel response;
            var localizedResource = GetById(model.Id);
            if (localizedResource != null)
            {
                localizedResource.LanguageId = model.LanguageId;
                localizedResource.TextKey = model.TextKey;
                localizedResource.DefaultValue = model.DefaultValue;
                localizedResource.TranslatedValue = model.TranslatedValue;
                response = Update(localizedResource);

                //Refresh the dictionary when success updating
                if (response.Success)
                {
                    RefreshDictionary();
                }

                response.SetMessage(response.Success
                    ? T("LocalizedResource_Message_UpdateSuccessfully")
                    : T("LocalizedResource_Message_UpdateFailure"));
            }
            else
            {
                Mapper.CreateMap<LocalizedResourceManageModel, LocalizedResource>();
                localizedResource = Mapper.Map<LocalizedResourceManageModel, LocalizedResource>(model);

                response = Insert(localizedResource);

                //Refresh the dictionary when success creating
                if (response.Success)
                {
                    RefreshDictionary();
                }
                response.SetMessage(response.Success
                    ? T("LocalizedResource_Message_CreateSuccessfully")
                    : T("LocalizedResource_Message_CreateFailure"));
            }
            return response;
        }

        /// <summary>
        /// Delete the LocalizedResource by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteLocalizedResource(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("LocalizedResource_Message_DeleteSuccessfully")
                    : T("LocalizedResource_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("LocalizedResource_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Get Localize Resources

        /// <summary>
        /// Get text by key, if not found set default value
        /// </summary>
        /// <param name="textKey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string T(string textKey, string defaultValue = null)
        {
            return GetLocalizedResource(textKey, defaultValue);
        }


        /// <summary>
        /// Get text by key, if not found set default value
        /// </summary>
        /// <param name="textKey"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string TFormat(string textKey, params object[] parameters)
        {
            var resource = T(textKey);

            if (!string.IsNullOrEmpty(resource) && parameters != null && parameters.Any())
            {
                return string.Format(resource, parameters);
            }

            return resource;
        }

        /// <summary>
        /// Get get localized resource by text key
        /// </summary>
        /// <param name="textKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string GetLocalizedResource(string textKey, string defaultValue, params object[] parameters)
        {
            if (string.IsNullOrEmpty(textKey)) throw new ArgumentNullException("textKey", @"Key cannot be null");

            // If no default value then get from localize resource
            if (defaultValue == null)
            {
                if (WorkContext.CurrentResourceManagers != null)
                {
                    foreach (var resourceManager in WorkContext.CurrentResourceManagers)
                    {
                        try
                        {
                            defaultValue = resourceManager.GetString(textKey);
                            break;
                        }
                        catch (Exception)
                        {
                            //No resource with key
                        }
                    }
                }

                // If found the resource then return
                if (defaultValue == null)
                {
#if DEBUG
                    //throw new MissingResourceException(textKey);
                    defaultValue = textKey;
#else
                    //_logger.Error(string.Format("The resource: {0} is missing.", textKey));
                    defaultValue = textKey;
#endif
                }
            }

            var langKey = WorkContext.CurrentCulture;

            // Build the unique key from language and text key
            var key = ServiceHelper.BuildKey(langKey, textKey);

            var dictionary = WorkContext.LocalizedResourceDictionary;

            // Check if resource exists
            if (dictionary == null || !dictionary.Any(l => l.Key.Equals(key)))
            {
                return GetDefaultValue(langKey, textKey, defaultValue, parameters);
            }

            // Get matching localized resource
            var resource = dictionary.First(l => l.Key.Equals(key));

            // Check if user had setup the localize resouce and the default value is change via code then will update the default value
            if (!string.IsNullOrEmpty(defaultValue) && !resource.Value.Equals(defaultValue))
            {
                var localizedResource = _localizedResourceRepository.Get(langKey, textKey);
                if (localizedResource != null)
                {
                    localizedResource.DefaultValue = defaultValue;
                    var response = Update(localizedResource);

                    if (response.Success)
                    {
                        // Refresh dictionary
                        RefreshDictionary();

                        if (!resource.HasClientSetup)
                        {
                            if (parameters != null && parameters.Any())
                            {
                                return string.Format(defaultValue, parameters);
                            }
                            return defaultValue;
                        }
                    }
                }
            }

            if (parameters != null && parameters.Any())
            {
                return string.Format(resource.Value, parameters);
            }
            return resource.Value;
        }

        /// <summary>
        /// Gets the default value of a localized text in case this is not available in dictionary
        /// </summary>
        /// <param name="langKey">Language key of the localized to get</param>
        /// <param name="textKey">Key of the string/text to get localized</param>
        /// <param name="defaultValue">Default value for the key</param>
        /// <param name="parameters">Parameters for passing to the output string</param>
        /// <returns></returns>
        private string GetDefaultValue(string langKey, string textKey, string defaultValue, params object[] parameters)
        {
            var localizeResource = _localizedResourceRepository.Get(langKey, textKey);

            if (localizeResource != null)
            {
                if (parameters != null)
                {
                    return string.Format(localizeResource.TranslatedValue, parameters);
                }

                return localizeResource.TranslatedValue;
            }

            return UpdateDictionaryToDb(textKey, defaultValue, parameters);
        }

        /// <summary>
        /// Update new value to database
        /// </summary>
        /// <param name="textKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="parameters"> </param>
        private string UpdateDictionaryToDb(string textKey, string defaultValue, params object[] parameters)
        {
            var existedResourceIds = Fetch(l => l.TextKey.Equals(textKey)).Select(l => l.LanguageId).ToList();
            var languages = _languageRepository.Fetch(l => !existedResourceIds.Contains(l.Id)).ToList();
            foreach (var language in languages)
            {
                var localizeResource = new LocalizedResource
                {
                    TextKey = textKey,
                    DefaultValue = defaultValue,
                    TranslatedValue = string.Empty,
                    LanguageId = language.Id
                };
                Insert(localizeResource);
            }
            RefreshDictionary();
            if (parameters != null && parameters.Any())
            {
                return string.Format(defaultValue, parameters);
            }
            return defaultValue;
        }

        #endregion
    }
}