using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.SiteSettings.Models;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.SiteSettings;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace EzCMS.Core.Services.SiteSettings
{
    public class SiteSettingService : ServiceHelper, ISiteSettingService
    {
        private readonly IRepository<SiteSetting> _siteSettingRepository;

        public SiteSettingService(IRepository<SiteSetting> siteSettingRepository)
        {
            _siteSettingRepository = siteSettingRepository;
        }

        #region Initialize

        /// <summary>
        /// Load settings to Application
        /// </summary>
        public void Initialize()
        {
            var data = GetAll();
            var siteSettings = Maps(data).ToList();

            WorkContext.EzCMSSettings = siteSettings;
        }

        #endregion

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SiteSetting GetByKey(string key)
        {
            return FetchFirst(s => s.Name.Equals(key));
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SiteSettingModel GetSetting(string key)
        {
            if (WorkContext.EzCMSSettings != null)
            {
                return WorkContext.EzCMSSettings.FirstOrDefault(s => s.Name.Equals(key));
            }

            return null;
        }

        /// <summary>
        /// Load object setting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadSetting<T>() where T : EzComplexSetting
        {
            var type = typeof(T);
            var classInstance = (EzComplexSetting)Activator.CreateInstance(type);
            var result = classInstance.LoadSetting<T>();
            return result;
        }

        /// <summary>
        /// Get available setting types
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetSettingTypes()
        {
            IEnumerable<string> settingTypes = GetAll().Select(siteSetting => siteSetting.SettingType).Distinct();
            return settingTypes.Select(settingType => new SelectListItem
            {
                Text = settingType,
                Value = settingType
            });
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetSetting<T>(string key)
        {
            var setting = GetSetting(key);
            if (setting == null)
            {
                return default(T);
            }
            return setting.Value.ToType<T>();
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"> </param>
        /// <returns></returns>
        public T GetSetting<T>(int id)
        {
            var setting = GetById(id);
            if (setting == null)
            {
                return default(T);
            }
            return setting.Value.ToType<T>();
        }

        #region Base

        public IQueryable<SiteSetting> GetAll()
        {
            return _siteSettingRepository.GetAll();
        }

        public IQueryable<SiteSetting> Fetch(Expression<Func<SiteSetting, bool>> expression)
        {
            return _siteSettingRepository.Fetch(expression);
        }

        public SiteSetting FetchFirst(Expression<Func<SiteSetting, bool>> expression)
        {
            return _siteSettingRepository.FetchFirst(expression);
        }

        public SiteSetting GetById(object id)
        {
            return _siteSettingRepository.GetById(id);
        }

        public ResponseModel Delete(SiteSetting siteSetting)
        {
            return _siteSettingRepository.Delete(siteSetting);
        }

        internal ResponseModel Delete(object id)
        {
            return _siteSettingRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _siteSettingRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid

        /// <summary>
        /// Search the site settings
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchSiteSettings(JqSearchIn si, SiteSettingSearchModel model)
        {
            var data = SearchSettings(model);

            var siteSettings = Maps(data);

            return si.Search(siteSettings);

            //#region Column model options
            //model.ModelOptions = new List<ModelOptions>();
            //var modelOption = new ModelOptions(GridColumnType.Select, "SettingTypeName",
            //    _settingTypeService.GetSettingTypes(null, true));
            //model.ModelOptions.Add(modelOption);

            //#endregion
        }


        /// <summary>
        /// Export site settings
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, SiteSettingSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchSettings(model);

            var siteSettings = Maps(data);

            var exportData = si.Export(siteSettings, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search site settings
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<SiteSetting> SearchSettings(SiteSettingSearchModel model)
        {
            if (model.SettingTypeNames == null) model.SettingTypeNames = new List<string>();

            var data = Fetch(s =>
                (string.IsNullOrEmpty(model.Keyword) || s.Name.ToLower().Contains(model.Keyword.ToLower()) ||
                 s.Value.ToLower().Contains(model.Keyword.ToLower()))
                && (!model.SettingTypeNames.Any() || model.SettingTypeNames.Contains(s.SettingType)));

            return data;
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="siteSettings"></param>
        /// <returns></returns>
        private IQueryable<SiteSettingModel> Maps(IQueryable<SiteSetting> siteSettings)
        {
            return siteSettings.Select(s => new SiteSettingModel
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Value = s.Value,
                SettingType = s.SettingType,
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
        /// Get site setting manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SiteSettingManageModel GetSettingManageModel(int? id)
        {
            var setting = GetById(id);
            if (setting != null)
            {
                return new SiteSettingManageModel(setting);
            }
            return null;
        }

        /// <summary>
        /// Get setting manage model by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SiteSettingManageModel GetSettingManageModel(string name)
        {
            var setting = GetByKey(name);
            if (setting != null)
            {
                return new SiteSettingManageModel(setting);
            }
            return null;
        }

        /// <summary>
        /// Save site setting
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveSettingManageModel(SiteSettingManageModel model)
        {
            var setting = GetByKey(model.SettingName);
            if (setting != null)
            {
                setting.SettingType = model.SettingType;
                setting.Description = model.Description;
                setting.Value = model.Value;
                var response = UpdateSetting(setting);

                // Re-init site settings
                if (response.Success) Initialize();

                return response.SetMessage(response.Success
                    ? T("SiteSetting_Message_UpdateSuccessfully")
                    : T("SiteSetting_Message_UpdateFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("SiteSetting_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Insert Setting
        /// </summary>
        /// <param name="siteSetting"></param>
        /// <returns></returns>
        public ResponseModel InsertSetting(SiteSetting siteSetting)
        {
            var response = _siteSettingRepository.Insert(siteSetting);
            if (response.Success) Initialize();

            return response;
        }

        /// <summary>
        /// Update setting
        /// </summary>
        /// <param name="siteSetting"></param>
        /// <returns></returns>
        public ResponseModel UpdateSetting(SiteSetting siteSetting)
        {
            var response = _siteSettingRepository.Update(siteSetting);
            if (response.Success) Initialize();

            return response;
        }

        #endregion
    }
}