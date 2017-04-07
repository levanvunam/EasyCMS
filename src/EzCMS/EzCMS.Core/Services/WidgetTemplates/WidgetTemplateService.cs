using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Models.WidgetTemplateLogs;
using EzCMS.Core.Models.WidgetTemplates;
using EzCMS.Core.Models.WidgetTemplates.Logs;
using EzCMS.Core.Models.WidgetTemplates.TemplateBuilder;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Core.Services.WidgetTemplateLogs;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace EzCMS.Core.Services.WidgetTemplates
{
    public class WidgetTemplateService : ServiceHelper, IWidgetTemplateService
    {
        private readonly ISiteSettingService _siteSettingService;
        private readonly IRepository<WidgetTemplateLog> _templateLogRepository;
        private readonly IWidgetTemplateLogService _templateLogService;
        private readonly IRepository<WidgetTemplate> _templateRepository;

        public WidgetTemplateService(IWidgetTemplateLogService templateLogService, ISiteSettingService siteSettingService,
            IRepository<WidgetTemplate> templateRepository, IRepository<WidgetTemplateLog> templateLogRepository)
        {
            _templateLogService = templateLogService;
            _siteSettingService = siteSettingService;
            _templateRepository = templateRepository;
            _templateLogRepository = templateLogRepository;
        }

        #region Logs

        /// <summary>
        /// Get page log model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="total"> </param>
        /// <param name="index"></param>
        /// <returns></returns>
        public WidgetTemplateLogListingModel GetLogs(int id, int total = 0, int index = 1)
        {
            var pageSize = _siteSettingService.GetSetting<int>(SettingNames.LogsPageSize);
            var template = GetById(id);
            if (template != null)
            {
                var logs = template.WidgetTemplateLogs.OrderByDescending(l => l.Created)
                    .GroupBy(l => l.SessionId)
                    .Skip((index - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
                    .Select(l => new WidgetTemplateLogsModel
                    {
                        SessionId = l.First().SessionId,
                        Creator = new SimpleUserModel(l.First().CreatedBy),
                        From = l.Last().Created,
                        To = l.First().Created,
                        Total = l.Count(),
                        Logs = l.Select(i => new WidgetTemplateLogItem(i)).ToList()
                    }).ToList();
                total = total + logs.Sum(l => l.Logs.Count);
                var model = new WidgetTemplateLogListingModel
                {
                    Id = template.Id,
                    Name = template.Name,
                    Total = total,
                    Logs = logs,
                    LoadComplete = total == template.WidgetTemplateLogs.Count
                };
                return model;
            }
            return null;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check if template exists.
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsTemplateNameExisted(int? templateId, string name)
        {
            return Fetch(t => t.Id != templateId && t.Name.Equals(name)).Any();
        }

        #endregion

        /// <summary>
        /// Get template by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WidgetTemplateRenderModel GetTemplateByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var template = _templateRepository.FetchFirst(t => t.Name.Equals(name));
            if (template != null)
            {
                return new WidgetTemplateRenderModel(template);
            }
            return null;
        }

        /// <summary>
        /// Get template by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WidgetTemplate GetTemplate(string name)
        {
            return FetchFirst(t => t.Name.Equals(name));
        }

        /// <summary>
        /// Get all defined templates of the widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetTemplatesOfWidget(string widget)
        {
            return Fetch(t => t.Widget.Equals(widget)).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Name,
                Selected = c.IsDefaultTemplate
            });
        }

        /// <summary>
        /// Parse template
        /// </summary>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <param name="viewbag"></param>
        /// <returns></returns>
        public string ParseTemplate(WidgetTemplateRenderModel template, dynamic model, dynamic viewbag = null)
        {
            return EzRazorEngineHelper.CompileAndRun(template.Content, model, viewbag, template.CacheName);
        }

        /// <summary>
        /// Get full template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string GetFullTemplate(WidgetTemplateManageModel model)
        {
            return WidgetHelper.GetFullTemplate(model.Content, model.Style, model.Script, model.DataType,
                model.Shortcuts);
        }

        #region Base

        public IQueryable<WidgetTemplate> GetAll()
        {
            return _templateRepository.GetAll();
        }

        public IQueryable<WidgetTemplate> Fetch(Expression<Func<WidgetTemplate, bool>> expression)
        {
            return _templateRepository.Fetch(expression);
        }

        public WidgetTemplate FetchFirst(Expression<Func<WidgetTemplate, bool>> expression)
        {
            return _templateRepository.FetchFirst(expression);
        }

        public WidgetTemplate GetById(object id)
        {
            return _templateRepository.GetById(id);
        }

        public ResponseModel CreateTemplate(WidgetTemplate widgetTemplate)
        {
            return _templateRepository.Insert(widgetTemplate);
        }

        internal ResponseModel Update(WidgetTemplate widgetTemplate)
        {
            return _templateRepository.Update(widgetTemplate);
        }

        internal ResponseModel Delete(WidgetTemplate widgetTemplate)
        {
            return _templateRepository.Delete(widgetTemplate);
        }

        internal ResponseModel Delete(object id)
        {
            return _templateRepository.Delete(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the templates
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchTemplates(JqSearchIn si, WidgetTemplateSearchModel model)
        {
            var data = SearchTemplates(model);

            var templates = Maps(data);

            return si.Search(templates);
        }

        /// <summary>
        /// Export templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, WidgetTemplateSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchTemplates(model);

            var templates = Maps(data);

            var exportData = si.Export(templates, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search templates
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<WidgetTemplate> SearchTemplates(WidgetTemplateSearchModel model)
        {
            return Fetch(t => (string.IsNullOrEmpty(model.Keyword)
                               || (!string.IsNullOrEmpty(t.Name) && t.Name.Contains(model.Keyword))
                               || (!string.IsNullOrEmpty(t.Widget) && t.Widget.Contains(model.Keyword)))
                              && (string.IsNullOrEmpty(model.Widget) || model.Widget.Equals(t.Widget)));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="templates"></param>
        /// <returns></returns>
        private IQueryable<WidgetTemplateModel> Maps(IQueryable<WidgetTemplate> templates)
        {
            return templates.Select(u => new WidgetTemplateModel
            {
                Id = u.Id,
                Name = u.Name,
                IsDefaultTemplate = u.IsDefaultTemplate,
                DataType = u.DataType,
                Widget = u.Widget,
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
        /// Get template manage model by log id
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        public WidgetTemplateManageModel GetTemplateManageModelByLogId(int? logId)
        {
            var log = _templateLogRepository.GetById(logId);
            return log != null ? new WidgetTemplateManageModel(log) : new WidgetTemplateManageModel();
        }

        /// <summary>
        /// Get template manage model from id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WidgetTemplateManageModel GetTemplateManageModel(int? id = null)
        {
            var template = GetById(id);
            return template != null ? new WidgetTemplateManageModel(template) : new WidgetTemplateManageModel();
        }

        /// <summary>
        /// Get template manage model from name
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public WidgetTemplateManageModel GetTemplateManageModel(string widget, int? templateId)
        {
            var template = GetById(templateId);
            if (template != null)
            {
                return new WidgetTemplateManageModel(template)
                {
                    Id = 0,
                    IsDefaultTemplate = false,
                    Name = string.Empty
                };
            }

            var setup =
                WorkContext.Widgets.FirstOrDefault(
                    c => c.Widget.Equals(widget, StringComparison.CurrentCultureIgnoreCase));

            if (setup != null)
            {
                var model = new WidgetTemplateManageModel
                {
                    DataType = setup.Type.AssemblyQualifiedName,
                    Widget = widget,
                    IsDefaultTemplate = false
                };

                try
                {
                    if (!string.IsNullOrEmpty(setup.DefaultTemplate))
                    {
                        model.Content = DataInitializeHelper.GetTemplateContent(setup);
                        model.Script = DataInitializeHelper.GetTemplateScript(setup);
                        model.Style = DataInitializeHelper.GetTemplateStyle(setup);
                        model.Shortcuts =
                            SerializeUtilities.Deserialize<List<Shortcut>>(
                                DataInitializeHelper.GetTemplateWidgets(setup));
                        model.FullContent = DataInitializeHelper.GetTemplateFullContent(setup);
                    }
                }
                catch
                {
                    //The default template may not existed in current dll
                    // Or maybe it come from plugins
                    // So we will get the default template
                    var defaultTemplate = GetTemplate(setup.DefaultTemplate);
                    if (defaultTemplate != null)
                    {
                        model = new WidgetTemplateManageModel(defaultTemplate)
                        {
                            Id = 0,
                            IsDefaultTemplate = false,
                            Name = string.Empty
                        };
                    }
                }

                return model;
            }

            return null;
        }

        /// <summary>
        /// Save template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveTemplateManageModel(WidgetTemplateManageModel model)
        {
            ResponseModel response;
            var template = GetById(model.Id);
            if (template != null)
            {
                var log = new WidgetTemplateLogManageModel(template);
                template.Name = model.Name;
                template.Widgets = SerializeUtilities.Serialize(model.Shortcuts);
                template.Content = model.Content;
                template.Style = model.Style;
                template.Script = model.Script;
                template.FullContent = GetFullTemplate(model);

                response = Update(template);
                if (response.Success)
                {
                    _templateLogService.SaveTemplateLog(log);
                }
                return response.SetMessage(response.Success
                    ? T("WidgetTemplate_Message_UpdateSuccessfully")
                    : T("WidgetTemplate_Message_UpdateFailure"));
            }
            Mapper.CreateMap<WidgetTemplateManageModel, WidgetTemplate>();
            template = Mapper.Map<WidgetTemplateManageModel, WidgetTemplate>(model);
            template.FullContent = GetFullTemplate(model);
            response = CreateTemplate(template);
            return response.SetMessage(response.Success
                ? T("WidgetTemplate_Message_CreateSuccessfully")
                : T("WidgetTemplate_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteTemplate(int id)
        {
            var template = GetById(id);
            if (template != null)
            {
                if (template.IsDefaultTemplate)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("WidgetTemplate_Message_CanNotDeleteDefaultTemplate")
                    };
                }

                // Delete logs
                _templateLogRepository.Delete(template.WidgetTemplateLogs);

                var response = Delete(template);
                return response.SetMessage(response.Success
                    ? T("WidgetTemplate_Message_DeleteSuccessfully")
                    : T("WidgetTemplate_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("WidgetTemplate_Message_DeleteSuccessfully")
            };
        }

        #endregion
    }
}