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
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Models.StyleLogs;
using EzCMS.Core.Models.Styles;
using EzCMS.Core.Models.Styles.Logs;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Core.Services.StyleLogs;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Styles
{
    public class StyleService : ServiceHelper, IStyleService
    {
        private readonly ISiteSettingService _siteSettingService;
        private readonly IRepository<StyleLog> _styleLogRepository;
        private readonly IStyleLogService _styleLogService;
        private readonly IRepository<Style> _styleRepository;

        public StyleService(IStyleLogService styleLogService, ISiteSettingService siteSettingService,
            IRepository<Style> styleRepository, IRepository<StyleLog> styleLogRepository)
        {
            _styleLogService = styleLogService;
            _siteSettingService = siteSettingService;
            _styleRepository = styleRepository;
            _styleLogRepository = styleLogRepository;
        }

        /// <summary>
        /// Get style by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StyleRenderModel GetStyleByName(string name)
        {
            var style = GetByName(name);
            if (style != null)
            {
                return new StyleRenderModel(style);
            }
            return null;
        }

        /// <summary>
        /// Get style by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Style GetByName(string name)
        {
            return FetchFirst(s => s.Name.Equals(name));
        }

        /// <summary>
        /// Get url for style
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetStyleUrl(int? id)
        {
            var style = GetById(id);

            return GetStyleUrl(style);
        }

        /// <summary>
        /// Get style url by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetStyleUrl(string name)
        {
            var style = GetByName(name);

            return GetStyleUrl(style);
        }

        /// <summary>
        /// Get style url
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public string GetStyleUrl(Style style)
        {
            if (style != null)
            {
                return string.IsNullOrEmpty(style.CdnUrl)
                    ? string.Format(EzCMSContants.CssResourceUrl, style.Name).ToAbsoluteUrl()
                    : style.CdnUrl;
            }
            return string.Empty;
        }

        #region Base

        public IQueryable<Style> GetAll()
        {
            return _styleRepository.GetAll();
        }

        public IQueryable<Style> Fetch(Expression<Func<Style, bool>> expression)
        {
            return _styleRepository.Fetch(expression);
        }

        public Style FetchFirst(Expression<Func<Style, bool>> expression)
        {
            return _styleRepository.FetchFirst(expression);
        }

        public Style GetById(object id)
        {
            return _styleRepository.GetById(id);
        }

        internal ResponseModel Insert(Style style)
        {
            return _styleRepository.Insert(style);
        }

        internal ResponseModel Update(Style style)
        {
            return _styleRepository.Update(style);
        }

        internal ResponseModel Delete(Style style)
        {
            return _styleRepository.Delete(style);
        }

        internal ResponseModel Delete(object id)
        {
            return _styleRepository.Delete(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the styles
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchStyles(JqSearchIn si)
        {
            var data = GetAll();

            var styles = Maps(data);

            return si.Search(styles);
        }

        /// <summary>
        /// Export styles
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var styles = Maps(data);

            var exportData = si.Export(styles, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="styles"></param>
        /// <returns></returns>
        private IQueryable<StyleModel> Maps(IQueryable<Style> styles)
        {
            return styles.Select(s => new StyleModel
            {
                Id = s.Id,
                Name = s.Name,
                IncludeIntoEditor = s.IncludeIntoEditor,
                Url = string.IsNullOrEmpty(s.CdnUrl) ? "/Resources/" + s.Name + ".css" : s.CdnUrl,
                RecordOrder = s.RecordOrder,
                Created = s.Created,
                CreatedBy = s.CreatedBy,
                LastUpdate = s.LastUpdate,
                LastUpdateBy = s.LastUpdateBy
            });
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get Style manage model by log id
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        public StyleManageModel GetStyleManageModelByLogId(int? logId)
        {
            var log = _styleLogRepository.GetById(logId);
            return log != null ? new StyleManageModel(log) : new StyleManageModel();
        }

        /// <summary>
        /// Get Style manage model from id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StyleManageModel GetStyleManageModel(int? id = null)
        {
            var style = GetById(id);
            return style != null ? new StyleManageModel(style) : new StyleManageModel();
        }

        /// <summary>
        /// Save Style
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveStyleManageModel(StyleManageModel model)
        {
            ResponseModel response;
            var style = GetById(model.Id);
            if (style != null)
            {
                var log = new StyleLogManageModel(style);
                style.Name = model.Name;
                style.Content = model.Content;
                style.CdnUrl = model.CdnUrl;
                style.IncludeIntoEditor = model.IncludeIntoEditor;

                response = Update(style);
                if (response.Success)
                {
                    _styleLogService.SaveStyleLog(log);
                }
                return response.SetMessage(response.Success
                    ? T("Style_Message_UpdateSuccessfully")
                    : T("Style_Message_UpdateFailure"));
            }

            Mapper.CreateMap<StyleManageModel, Style>();
            style = Mapper.Map<StyleManageModel, Style>(model);
            response = Insert(style);
            return response.SetMessage(response.Success
                ? T("Style_Message_CreateSuccessfully")
                : T("Style_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete Style
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteStyle(int id)
        {
            var style = GetById(id);
            if (style != null)
            {
                if (style.Forms.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Style_Message_DeleteFailureBasedOnRelatedForms")
                    };
                }

                //Delete all logs
                _styleLogRepository.Delete(style.StyleLogs);

                var response = Delete(style);
                return response.SetMessage(response.Success
                    ? T("Style_Message_DeleteSuccessfully")
                    : T("Style_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("Style_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Logs

        /// <summary>
        /// Get page log model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="total"> </param>
        /// <param name="index"></param>
        /// <returns></returns>
        public StyleLogListingModel GetLogs(int id, int total = 0, int index = 1)
        {
            var pageSize = _siteSettingService.GetSetting<int>(SettingNames.LogsPageSize);
            var style = GetById(id);
            if (style != null)
            {
                var logs = style.StyleLogs.OrderByDescending(l => l.Created)
                    .GroupBy(l => l.SessionId)
                    .Skip((index - 1)*pageSize)
                    .Take(pageSize)
                    .ToList()
                    .Select(l => new StyleLogsModel
                    {
                        SessionId = l.First().SessionId,
                        Creator = new SimpleUserModel(l.First().CreatedBy),
                        From = l.Last().Created,
                        To = l.First().Created,
                        Total = l.Count(),
                        Logs = l.Select(i => new StyleLogItem(i)).ToList()
                    }).ToList();
                total = total + logs.Sum(l => l.Logs.Count);
                var model = new StyleLogListingModel
                {
                    Id = style.Id,
                    Name = style.Name,
                    Total = total,
                    Logs = logs,
                    LoadComplete = total == style.StyleLogs.Count
                };
                return model;
            }
            return null;
        }

        /// <summary>
        /// Generate style select list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetStyles(int? id = null)
        {
            return GetAll().Select(s => new SelectListItem
            {
                Text = s.Name + ".css",
                Value = SqlFunctions.StringConvert((double) s.Id).Trim(),
                Selected = id.HasValue && id == s.Id
            });
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check if Style exists.
        /// </summary>
        /// <param name="styleId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsStyleNameExisted(int? styleId, string name)
        {
            return Fetch(t => t.Id != styleId && t.Name.Equals(name)).Any();
        }

        /// <summary>
        /// Get all styles that need to be included into page editor
        /// </summary>
        /// <returns></returns>
        public List<string> GetIncludedStyles()
        {
            var includeStyles = Fetch(s => s.IncludeIntoEditor).ToList();

            return includeStyles.Select(s => string.IsNullOrEmpty(s.CdnUrl)
                ? string.Format(EzCMSContants.CssResourceUrl, s.Name)
                : s.CdnUrl).ToList();
        }

        #endregion
    }
}