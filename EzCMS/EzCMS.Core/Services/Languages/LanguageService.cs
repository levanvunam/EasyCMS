using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.Languages;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Languages
{
    public class LanguageService : ServiceHelper, ILanguageService
    {
        private readonly IRepository<Language> _languageRepository;

        public LanguageService(IRepository<Language> languageRepository)
        {
            _languageRepository = languageRepository;
        }

        #region Validation

        /// <summary>
        /// Check key is existed or not
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyExisted(int? languageId, string key)
        {
            return Fetch(u => u.Key.Equals(key) && u.Id != languageId).Any();
        }

        #endregion

        /// <summary>
        /// Get language by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Language GetByKey(string key)
        {
            return _languageRepository.FetchFirst(i => i.Key == key);
        }

        /// <summary>
        /// Get default language
        /// </summary>
        /// <returns></returns>
        public Language GetDefault()
        {
            return FetchFirst(l => l.IsDefault);
        }

        /// <summary>
        /// Get languages
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetLanguages(string key = null)
        {
            return GetAll().Select(language => new SelectListItem
            {
                Text = language.Name + " - " + language.Culture,
                Value = language.Key,
                Selected =
                    string.IsNullOrEmpty(key) ? language.IsDefault : key.Equals(language.Key) || language.IsDefault
            });
        }

        #region Base

        public IQueryable<Language> GetAll()
        {
            return _languageRepository.GetAll();
        }

        public IQueryable<Language> Fetch(Expression<Func<Language, bool>> expression)
        {
            return _languageRepository.Fetch(expression);
        }

        public Language FetchFirst(Expression<Func<Language, bool>> expression)
        {
            return _languageRepository.FetchFirst(expression);
        }

        public Language GetById(object id)
        {
            return _languageRepository.GetById(id);
        }

        internal ResponseModel Insert(Language language)
        {
            return _languageRepository.Insert(language);
        }

        internal ResponseModel Update(Language language)
        {
            return _languageRepository.Update(language);
        }

        internal ResponseModel Delete(Language language)
        {
            return _languageRepository.Delete(language);
        }

        internal ResponseModel Delete(object id)
        {
            return _languageRepository.Delete(id);
        }

        internal ResponseModel SetRecordDeleted(int id)
        {
            return _languageRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid

        /// <summary>
        /// Search the languages
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchLanguages(JqSearchIn si)
        {
            var data = GetAll();

            var languages = Maps(data);

            return si.Search(languages);
        }

        /// <summary>
        /// Export languages
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var languages = Maps(data);

            var exportData = si.Export(languages, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search languages
        /// </summary>
        /// <returns></returns>
        private IQueryable<LanguageModel> Maps(IQueryable<Language> languages)
        {
            return languages.Select(u => new LanguageModel
            {
                Id = u.Id,
                Key = u.Key,
                Name = u.Name,
                Culture = u.Culture,
                IsDefault = u.IsDefault,
                RecordOrder = u.RecordOrder,
                Created = u.Created,
                CreatedBy = u.CreatedBy,
                LastUpdate = u.LastUpdate,
                LastUpdateBy = u.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get language manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LanguageManageModel GetLanguageManageModel(int? id = null)
        {
            var language = GetById(id);
            if (language != null)
            {
                return new LanguageManageModel(language);
            }
            return new LanguageManageModel();
        }

        /// <summary>
        /// Save language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveLanguage(LanguageManageModel model)
        {
            ResponseModel response;
            var language = GetById(model.Id);
            if (language != null)
            {
                language.Key = model.Key;
                language.Name = model.Name;
                language.Culture = model.Culture;
                language.RecordOrder = model.RecordOrder;
                response = Update(language);
                return response.SetMessage(response.Success
                    ? T("Language_Message_UpdateSuccessfully")
                    : T("Language_Message_UpdateFailure"));
            }
            Mapper.CreateMap<LanguageManageModel, Language>();
            language = Mapper.Map<LanguageManageModel, Language>(model);
            response = Insert(language);
            return response.SetMessage(response.Success
                ? T("Language_Message_CreateSuccessfully")
                : T("Language_Message_CreateFailure"));
        }

        #endregion
    }
}