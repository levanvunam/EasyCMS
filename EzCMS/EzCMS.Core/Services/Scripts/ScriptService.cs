using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Models.ScriptLogs;
using EzCMS.Core.Models.Scripts;
using EzCMS.Core.Models.Scripts.Logs;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Services.ScriptLogs;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Scripts
{
    public class ScriptService : ServiceHelper, IScriptService
    {
        private readonly IRepository<ScriptLog> _scriptLogRepository;
        private readonly IScriptLogService _scriptLogService;
        private readonly IRepository<Script> _scriptRepository;
        private readonly ISiteSettingService _siteSettingService;

        public ScriptService(IScriptLogService scriptLogService,
            ISiteSettingService siteSettingService,
            IRepository<Script> scriptRepository,
            IRepository<ScriptLog> scriptLogRepository)
        {
            _scriptLogService = scriptLogService;
            _siteSettingService = siteSettingService;
            _scriptRepository = scriptRepository;
            _scriptLogRepository = scriptLogRepository;
        }

        #region Logs

        /// <summary>
        /// Get page log model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="total"> </param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ScriptLogListingModel GetLogs(int id, int total = 0, int index = 1)
        {
            var pageSize = _siteSettingService.GetSetting<int>(SettingNames.LogsPageSize);
            var script = GetById(id);
            if (script != null)
            {
                var logs = script.ScriptLogs.OrderByDescending(l => l.Created)
                    .GroupBy(l => l.SessionId)
                    .Skip((index - 1)*pageSize)
                    .Take(pageSize)
                    .ToList()
                    .Select(l => new ScriptLogsModel
                    {
                        SessionId = l.First().SessionId,
                        Creator = new SimpleUserModel(l.First().CreatedBy),
                        From = l.Last().Created,
                        To = l.First().Created,
                        Total = l.Count(),
                        Logs = l.Select(i => new ScriptLogItem(i)).ToList()
                    }).ToList();
                total = total + logs.Sum(l => l.Logs.Count);
                var model = new ScriptLogListingModel
                {
                    Id = script.Id,
                    Name = script.Name,
                    Total = total,
                    Logs = logs,
                    LoadComplete = total == script.ScriptLogs.Count
                };
                return model;
            }
            return null;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check if Script exists.
        /// </summary>
        /// <param name="scriptId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsScriptNameExisted(int? scriptId, string name)
        {
            return Fetch(t => t.Id != scriptId && t.Name.Equals(name)).Any();
        }

        #endregion

        /// <summary>
        /// Get Script by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ScriptRenderModel GetScriptByName(string name)
        {
            var script = GetByName(name);
            if (script != null)
            {
                return new ScriptRenderModel(script);
            }
            return null;
        }

        /// <summary>
        /// Get url for script
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetScriptUrl(int? id)
        {
            var script = GetById(id);

            return GetScriptUrl(script);
        }

        /// <summary>
        /// Get script url by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetScriptUrl(string name)
        {
            var script = GetByName(name);

            return GetScriptUrl(script);
        }

        /// <summary>
        /// Get script url
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public string GetScriptUrl(Script script)
        {
            if (script != null)
            {
                return string.Format(EzCMSContants.ScriptResourceUrl, script.Name);
            }
            return string.Empty;
        }

        /// <summary>
        /// Get script by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Script GetByName(string name)
        {
            return FetchFirst(s => s.Name.Equals(name));
        }

        #region Base

        public IQueryable<Script> GetAll()
        {
            return _scriptRepository.GetAll();
        }

        public IQueryable<Script> Fetch(Expression<Func<Script, bool>> expression)
        {
            return _scriptRepository.Fetch(expression);
        }

        public Script FetchFirst(Expression<Func<Script, bool>> expression)
        {
            return _scriptRepository.FetchFirst(expression);
        }

        public Script GetById(object id)
        {
            return _scriptRepository.GetById(id);
        }

        public ResponseModel CreateScript(Script script)
        {
            return _scriptRepository.Insert(script);
        }

        internal ResponseModel Update(Script script)
        {
            return _scriptRepository.Update(script);
        }

        internal ResponseModel Delete(Script script)
        {
            return _scriptRepository.Delete(script);
        }

        internal ResponseModel Delete(object id)
        {
            return _scriptRepository.Delete(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the scripts
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchScripts(JqSearchIn si)
        {
            var data = GetAll();

            var scripts = Maps(data);

            return si.Search(scripts);
        }

        /// <summary>
        /// Export scripts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var scripts = Maps(data);

            var exportData = si.Export(scripts, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="scripts"></param>
        /// <returns></returns>
        private IQueryable<ScriptModel> Maps(IQueryable<Script> scripts)
        {
            return scripts.Select(u => new ScriptModel
            {
                Id = u.Id,
                Name = u.Name,
                RecordOrder = u.RecordOrder,
                Created = u.Created,
                CreatedBy = u.CreatedBy,
                LastUpdate = u.LastUpdate,
                LastUpdateBy = u.LastUpdateBy
            });
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get Script manage model by log id
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        public ScriptManageModel GetScriptManageModelByLogId(int? logId)
        {
            var log = _scriptLogRepository.GetById(logId);
            return log != null ? new ScriptManageModel(log) : new ScriptManageModel();
        }

        /// <summary>
        /// Get Script manage model from id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ScriptManageModel GetScriptManageModel(int? id = null)
        {
            var script = GetById(id);
            return script != null ? new ScriptManageModel(script) : new ScriptManageModel();
        }

        /// <summary>
        /// Save Script
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveScriptManageModel(ScriptManageModel model)
        {
            ResponseModel response;
            var script = GetById(model.Id);
            if (script != null)
            {
                var log = new ScriptLogManageModel(script);
                script.Name = model.Name;
                script.Content = model.Content;

                response = Update(script);
                if (response.Success)
                {
                    _scriptLogService.SaveScriptLog(log);
                }
                return response.SetMessage(response.Success
                    ? T("Script_Message_UpdateSuccessfully")
                    : T("Script_Message_UpdateFailure"));
            }
            Mapper.CreateMap<ScriptManageModel, Script>();
            script = Mapper.Map<ScriptManageModel, Script>(model);
            response = CreateScript(script);
            return response.SetMessage(response.Success
                ? T("Script_Message_CreateSuccessfully")
                : T("Script_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete Script
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteScript(int id)
        {
            var script = GetById(id);
            if (script != null)
            {
                _scriptLogRepository.Delete(script.ScriptLogs);

                var response = Delete(script);
                return response.SetMessage(response.Success
                    ? T("Script_Message_DeleteSuccessfully")
                    : T("Script_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("Script_Message_DeleteSuccessfully")
            };
        }

        #endregion
    }
}