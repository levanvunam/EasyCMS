using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Ez.Framework.Configurations;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.SlideInHelps;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.SlideInHelps;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.SlideInHelps
{
    public class SlideInHelpService : ServiceHelper, ISlideInHelpService
    {
        private readonly IRepository<Language> _languageRepository;
        private readonly ISiteSettingService _siteSettingService;
        private readonly ISlideInHelpRepository _slideInHelpRepository;

        public SlideInHelpService(ISlideInHelpRepository slideInHelpRepository, IRepository<Language> languageRepository,
            ISiteSettingService siteSettingService)
        {
            _slideInHelpRepository = slideInHelpRepository;
            _languageRepository = languageRepository;
            _siteSettingService = siteSettingService;
        }

        #region Initialize

        /// <summary>
        /// Refresh slide in help dictionaries
        /// </summary>
        public void RefreshDictionary()
        {
            var data = GetAll().ToList();
            WorkContext.SlideInHelpDictionary = data.Select(s => new SlideInHelpDictionaryItem(s)).ToList();
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check if the key exists
        /// </summary>
        /// <param name="key"></param>
        /// <param name="languageId"></param>
        /// <param name="slideInHelpId"></param>
        /// <returns></returns>
        public bool IsKeyExisted(string key, int languageId, int? slideInHelpId = null)
        {
            return
                Fetch(
                    s =>
                        s.TextKey.Equals(key) && s.LanguageId == languageId && s.Id != slideInHelpId).Any();
        }

        #endregion

        #region Base

        public IQueryable<SlideInHelp> GetAll()
        {
            return _slideInHelpRepository.GetAll();
        }

        public IQueryable<SlideInHelp> Fetch(Expression<Func<SlideInHelp, bool>> expression)
        {
            return _slideInHelpRepository.Fetch(expression);
        }

        public SlideInHelp FetchFirst(Expression<Func<SlideInHelp, bool>> expression)
        {
            return _slideInHelpRepository.FetchFirst(expression);
        }

        public SlideInHelp GetById(object id)
        {
            return _slideInHelpRepository.GetById(id);
        }

        public ResponseModel Insert(SlideInHelp slideInHelp)
        {
            return _slideInHelpRepository.Insert(slideInHelp);
        }

        public ResponseModel Update(SlideInHelp slideInHelp)
        {
            return _slideInHelpRepository.Update(slideInHelp);
        }

        internal ResponseModel Delete(SlideInHelp slideInHelp)
        {
            return _slideInHelpRepository.Delete(slideInHelp);
        }

        internal ResponseModel Delete(object id)
        {
            return _slideInHelpRepository.Delete(id);
        }

        internal ResponseModel SetRecordDeleted(int id)
        {
            return _slideInHelpRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid

        /// <summary>
        /// Search the slide in helps
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchSlideInHelps(JqSearchIn si, int languageId)
        {
            var data = SearchSlideInHelps(languageId);

            var slideInHelps = Maps(data);

            return si.Search(slideInHelps);
        }

        /// <summary>
        /// Export the slide in helps
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int languageId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchSlideInHelps(languageId);

            var slideInHelps = Maps(data);

            var exportData = si.Export(slideInHelps, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search slide in helps
        /// </summary>
        /// <param name="languageId"></param>
        /// <returns></returns>
        private IQueryable<SlideInHelp> SearchSlideInHelps(int languageId)
        {
            return Fetch(l => l.LanguageId == languageId);
        }

        /// <summary>
        /// Search slide in helps
        /// </summary>
        /// <param name="slideInHelps"></param>
        /// <returns></returns>
        private IQueryable<SlideInHelpModel> Maps(IQueryable<SlideInHelp> slideInHelps)
        {
            return slideInHelps.Select(s => new SlideInHelpModel
            {
                Id = s.Id,
                TextKey = s.TextKey,
                Language = s.Language.Key,
                HelpTitle = s.HelpTitle,
                MasterHelpContent = s.MasterHelpContent,
                LocalHelpContent = s.LocalHelpContent,
                RecordOrder = s.RecordOrder,
                Created = s.Created,
                CreatedBy = s.CreatedBy,
                LastUpdate = s.LastUpdate,
                LastUpdateBy = s.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get slide in help manage model
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public SlideInHelpManageModel GetSlideInHelpManageModel(int? languageId, int? id = null)
        {
            var language = _languageRepository.GetById(languageId);
            var slideInHelp = GetById(id);
            if (slideInHelp != null)
            {
                return new SlideInHelpManageModel(slideInHelp);
            }
            if (language != null) return new SlideInHelpManageModel(languageId);

            return new SlideInHelpManageModel();
        }

        /// <summary>
        /// Save slide in help
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveSlideInHelpManageModel(SlideInHelpManageModel model)
        {
            ResponseModel response;
            var slideInHelp = GetById(model.Id);
            if (slideInHelp != null)
            {
                slideInHelp.HelpTitle = model.Title;
                slideInHelp.LocalHelpContent = model.Content;
                slideInHelp.Active = model.Active;

                response = Update(slideInHelp);

                if (response.Success)
                {
                    RefreshDictionary();
                }

                return response.SetMessage(response.Success
                    ? T("SlideInHelp_Message_UpdateSuccessfully")
                    : T("SlideInHelp_Message_UpdateFailure"));
            }
            Mapper.CreateMap<SlideInHelpManageModel, SlideInHelp>();
            slideInHelp = Mapper.Map<SlideInHelpManageModel, SlideInHelp>(model);
            response = Insert(slideInHelp);
            if (response.Success)
            {
                RefreshDictionary();
            }
            return response.SetMessage(response.Success
                ? T("SlideInHelp_Message_CreateSuccessfully")
                : T("SlideInHelp_Message_CreateFailure"));
        }

        /// <summary>
        /// Change status slide in help
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ResponseModel ChangeSlideInHelpStatus(int id, bool status)
        {
            var slideInHelp = GetById(id);
            if (slideInHelp != null)
            {
                slideInHelp.Active = !status;
                var response = Update(slideInHelp);

                RefreshDictionary();

                return response.Success
                    ? response.SetMessage(T("SlideInHelp_Message_ChangeStatusSuccessfully"))
                    : response.SetMessage(T("SlideInHelp_Message_ChangeStatusFailure"));
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("SlideInHelp_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Get slide in help

        /// <summary>
        /// Get slider in helpe
        /// </summary>
        /// <param name="textKey"></param>
        /// <returns></returns>
        public SlideInHelpDictionaryItem GetSlideInHelp(string textKey)
        {
            var langKey = WorkContext.CurrentCulture;
            var key = BuildKey(langKey, textKey);
            var dictionary = WorkContext.SlideInHelpDictionary;
            if (dictionary == null || !dictionary.Any(l => l.Key.Equals(key) && l.Language.Equals(langKey)))
            {
                return GetDefaultValue(langKey, textKey);
            }
            var slideInHelp = dictionary.FirstOrDefault(d => d.Key.Equals(key));
            if (slideInHelp != null)
            {
                return slideInHelp;
            }

            return null;
        }

        /// <summary>
        /// Gets the default value of a slide in help in case this is not available in dictionary
        /// </summary>
        /// <param name="langKey">Language key of the slide in help to get</param>
        /// <param name="textKey">Key of the string/text to get slide in help</param>
        /// <returns></returns>
        private SlideInHelpDictionaryItem GetDefaultValue(string langKey, string textKey)
        {
            var slideInHelp = _slideInHelpRepository.Get(langKey, textKey);

            if (slideInHelp != null)
            {
                var model = new SlideInHelpModel(slideInHelp);

                return new SlideInHelpDictionaryItem(model);
            }

            //Adding data only when no service required
            var slideInHelpSetting = _siteSettingService.LoadSetting<EzCMSHelpConfigurationSetting>();
            if (slideInHelpSetting.DisabledSlideInHelpService)
            {
                return UpdateDictionaryToDb(langKey, textKey);
            }

            return null;
        }

        /// <summary>
        /// Update new value to database
        /// </summary>
        /// <param name="langKey"></param>
        /// <param name="textKey"></param>
        private SlideInHelpDictionaryItem UpdateDictionaryToDb(string langKey, string textKey)
        {
            var values = textKey.Split(new[] {FrameworkConstants.LocalizeResourceSeperator},
                StringSplitOptions.RemoveEmptyEntries);

            //Convert field to camel friendly text
            var defaultTitle = values.Last().CamelFriendly();

            var existedSlideInHelpIds = Fetch(l => l.TextKey.Equals(textKey)).Select(l => l.LanguageId).ToList();
            var languages = _languageRepository.Fetch(l => !existedSlideInHelpIds.Contains(l.Id)).ToList();

            foreach (var language in languages)
            {
                var slideInHelp = new SlideInHelp
                {
                    TextKey = textKey,
                    LanguageId = language.Id,
                    HelpTitle = defaultTitle,
                    LocalHelpContent = string.Empty,
                    Active = false
                };
                Insert(slideInHelp);
            }

            RefreshDictionary();
            var returnSlideInHelp = FetchFirst(s => s.TextKey.Equals(textKey) && s.Language.Key.Equals(langKey));
            return new SlideInHelpDictionaryItem(returnSlideInHelp);
        }

        #endregion
    }
}