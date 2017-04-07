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
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Pages.Widgets;
using EzCMS.Core.Models.PageTemplateLogs;
using EzCMS.Core.Models.PageTemplates;
using EzCMS.Core.Models.PageTemplates.Logs;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Services.PageTemplateLogs;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;
using RazorEngine.Templating;

namespace EzCMS.Core.Services.PageTemplates
{
    public class PageTemplateService : ServiceHelper, IPageTemplateService
    {
        private readonly IRepository<Page> _pageRepository;
        private readonly IRepository<PageTemplateLog> _pageTemplateLogRepository;
        private readonly IPageTemplateLogService _pageTemplateLogService;
        private readonly IHierarchyRepository<PageTemplate> _pageTemplateRepository;
        private readonly ISiteSettingService _siteSettingService;

        public PageTemplateService(IPageTemplateLogService pageTemplateLogService,
            ISiteSettingService siteSettingService,
            IRepository<Page> pageRepository,
            IRepository<PageTemplateLog> pageTemplateLogRepository,
            IHierarchyRepository<PageTemplate> pageTemplateRepository)
        {
            _pageTemplateLogService = pageTemplateLogService;
            _siteSettingService = siteSettingService;
            _pageTemplateRepository = pageTemplateRepository;
            _pageRepository = pageRepository;
            _pageTemplateLogRepository = pageTemplateLogRepository;
        }

        #region Initialize

        /// <summary>
        /// Reset cache name for all hierarchies templates of page
        /// If null then reset all the application
        /// </summary>
        /// <param name="pageTemplateId"></param>
        public void Initialize(int? pageTemplateId = null)
        {
            var currentPageTemplate = GetById(pageTemplateId);

            // Get all templates
            List<PageTemplate> pageTemplates;

            // If has current template, then only refresh the Template tree
            if (currentPageTemplate != null)
            {
                pageTemplates = _pageTemplateRepository.GetHierarchies(currentPageTemplate).ToList();
            }
            // If no current template then reload all trees
            else
            {
                pageTemplates = GetAll().OrderBy(t => t.Hierarchy).ToList();
            }

            // Reset all effected templates
            foreach (var pageTemplate in pageTemplates)
            {
                var template = pageTemplate.Content.ParseProperties(typeof (PageRenderModel));
                var parentTemplate = GetById(pageTemplate.ParentId);

                //If parent template cache name is null, then it has parsing errors and all children templates is not valid
                if (parentTemplate != null && string.IsNullOrEmpty(parentTemplate.CacheName))
                {
                    pageTemplate.IsValid = false;
                    pageTemplate.CacheName = string.Empty;
                    pageTemplate.CompileMessage = parentTemplate.CompileMessage;
                }
                //Generate child template with parent as master
                else if (parentTemplate != null)
                {
                    var parentCacheName = parentTemplate.CacheName;
                    template = template.InsertMasterPage(parentCacheName);

                    // Generate new template cache name base on parent Template and content
                    pageTemplate.CacheName = pageTemplate.Name.GetTemplateCacheName(template);
                }
                else
                {
                    pageTemplate.CacheName = pageTemplate.Name.GetTemplateCacheName(template);
                }

                if (!string.IsNullOrEmpty(pageTemplate.CacheName))
                {
                    var compileMessage = string.Empty;
                    try
                    {
                        var templateKey = EzRazorEngineHelper.TryCompileAndAddTemplate(template, pageTemplate.CacheName,
                            typeof (PageRenderModel), ResolveType.Layout);

                        pageTemplate.IsValid = true;
                        pageTemplate.CompileMessage = string.Empty;
                        pageTemplate.CacheName = templateKey.Name;
                    }
                    catch (TemplateParsingException exception)
                    {
                        compileMessage = exception.Message;
                    }
                    catch (TemplateCompilationException exception)
                    {
                        compileMessage = string.Join("\n", exception.CompilerErrors.Select(e => e.ErrorText));
                    }
                    catch (Exception exception)
                    {
                        compileMessage = exception.Message;
                    }
                    finally
                    {
                        if (!string.IsNullOrEmpty(compileMessage))
                        {
                            pageTemplate.IsValid = false;
                            pageTemplate.CompileMessage = TFormat("PageTemplate_Message_InvalidPageTemplate",
                                pageTemplate.Name, compileMessage);
                            pageTemplate.CacheName = string.Empty;
                        }
                    }
                }

                Update(pageTemplate);
            }
        }

        #endregion

        #region Logs

        /// <summary>
        /// Get page template logs
        /// </summary>
        /// <param name="id"></param>
        /// <param name="total"> </param>
        /// <param name="index"></param>
        /// <returns></returns>
        public PageTemplateLogListingModel GetLogs(int id, int total = 0, int index = 1)
        {
            var pageSize = _siteSettingService.GetSetting<int>(SettingNames.LogsPageSize);

            var pageTemplate = GetById(id);
            if (pageTemplate != null)
            {
                var logs = pageTemplate.PageTemplateLogs.OrderByDescending(pageTemplateLog => pageTemplateLog.Created)
                    .GroupBy(pageTemplateLog => pageTemplateLog.SessionId)
                    .Skip((index - 1)*pageSize)
                    .Take(pageSize)
                    .ToList()
                    .Select(pageTemplateLogs => new PageTemplateLogsModel
                    {
                        SessionId = pageTemplateLogs.First().SessionId,
                        Creator = new SimpleUserModel(pageTemplateLogs.First().CreatedBy),
                        From = pageTemplateLogs.Last().Created,
                        To = pageTemplateLogs.First().Created,
                        Total = pageTemplateLogs.Count(),
                        Logs =
                            pageTemplateLogs.Select(pageTemplateLog => new PageTemplateLogItem(pageTemplateLog))
                                .ToList()
                    }).ToList();
                total = total + logs.Sum(l => l.Logs.Count);

                var model = new PageTemplateLogListingModel
                {
                    Id = pageTemplate.Id,
                    Name = pageTemplate.Name,
                    Total = total,
                    Logs = logs,
                    LoadComplete = total == pageTemplate.PageTemplateLogs.Count
                };
                return model;
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Get possible parent Navigation
        /// </summary>
        /// <param name="id">the current Navigation id</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetPossibleParents(int? id = null)
        {
            int? parentId = null;
            var pageTemplates = GetAll();

            var template = GetById(id);
            if (template != null)
            {
                parentId = template.ParentId;
                pageTemplates = _pageTemplateRepository.GetPossibleParents(template);
            }

            var data = pageTemplates.Select(pageTemplate => new HierarchyDropdownModel
            {
                Id = pageTemplate.Id,
                Name = pageTemplate.Name,
                Hierarchy = pageTemplate.Hierarchy,
                RecordOrder = pageTemplate.RecordOrder,
                Selected = parentId.HasValue && parentId.Value == pageTemplate.Id
            }).ToList();

            return _pageTemplateRepository.BuildSelectList(data);
        }

        /// <summary>
        /// Get children page templates by parent id
        /// </summary>
        /// <param name="parentId">the parent id</param>
        /// <returns></returns>
        public List<PageTemplate> GetPageTemplates(int? parentId = null)
        {
            return
                Fetch(
                    pageTemplate =>
                        parentId.HasValue ? pageTemplate.ParentId == parentId : !pageTemplate.ParentId.HasValue)
                    .OrderBy(m => m.RecordOrder).ToList();
        }

        /// <summary>
        /// Get page template select list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetPageTemplateSelectList(int? id = null)
        {
            int? templateId = null;
            var pageTemplates = GetAll();

            var page = _pageRepository.GetById(id);
            if (page != null)
            {
                templateId = page.PageTemplateId;
            }

            var data = pageTemplates.Select(pageTemplate => new HierarchyDropdownModel
            {
                Id = pageTemplate.Id,
                Name = pageTemplate.Name,
                Hierarchy = pageTemplate.Hierarchy,
                RecordOrder = pageTemplate.RecordOrder,
                Selected = templateId.HasValue && templateId.Value == pageTemplate.Id
            }).ToList();

            return _pageTemplateRepository.BuildSelectList(data);
        }

        /// <summary>
        /// Get page templates for file template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetPageTemplateSelectListForFileTemplate(int? id = null)
        {
            var pageTemplates = GetAll();
            var data = pageTemplates.Select(m => new HierarchyDropdownModel
            {
                Id = m.Id,
                Name = m.Name,
                Hierarchy = m.Hierarchy,
                RecordOrder = m.RecordOrder,
                Selected = m.FileTemplates.Any(t => t.Id == id)
            }).ToList();

            return _pageTemplateRepository.BuildSelectList(data);
        }

        /// <summary>
        /// Get page template for virtual path provider
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public PageTemplate FindTemplate(string filePath)
        {
            var templateName = filePath.GetDBTemplateMaster();
            if (string.IsNullOrEmpty(templateName)) return null;

            return FetchFirst(pageTemplate => pageTemplate.Name.Equals(templateName));
        }

        #region Base

        public IQueryable<PageTemplate> GetAll()
        {
            return _pageTemplateRepository.GetAll();
        }

        public IQueryable<PageTemplate> Fetch(Expression<Func<PageTemplate, bool>> expression)
        {
            return _pageTemplateRepository.Fetch(expression);
        }

        public PageTemplate FetchFirst(Expression<Func<PageTemplate, bool>> expression)
        {
            return _pageTemplateRepository.FetchFirst(expression);
        }

        public PageTemplate GetById(object id)
        {
            return _pageTemplateRepository.GetById(id);
        }

        internal ResponseModel Insert(PageTemplate pageTemplate)
        {
            return _pageTemplateRepository.Insert(pageTemplate);
        }

        internal ResponseModel Update(PageTemplate pageTemplate)
        {
            return _pageTemplateRepository.Update(pageTemplate);
        }

        internal ResponseModel HierarchyUpdate(PageTemplate pageTemplate)
        {
            return _pageTemplateRepository.HierarchyUpdate(pageTemplate);
        }

        public ResponseModel HierarchyInsert(PageTemplate pageTemplate)
        {
            return _pageTemplateRepository.HierarchyInsert(pageTemplate);
        }

        internal ResponseModel Delete(PageTemplate pageTemplate)
        {
            return _pageTemplateRepository.Delete(pageTemplate);
        }

        internal ResponseModel Delete(object id)
        {
            return _pageTemplateRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _pageTemplateRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the page templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="pageTemplateId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchPageTemplates(JqSearchIn si, int? pageTemplateId)
        {
            var data = SearchPageTemplates(pageTemplateId);

            var pageTemplates = Maps(data);

            return si.Search(pageTemplates);
        }

        /// <summary>
        /// Export page templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="pageTemplateId"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? pageTemplateId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchPageTemplates(pageTemplateId);

            var pageTemplates = Maps(data);

            var exportData = si.Export(pageTemplates, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search page templates
        /// </summary>
        /// <param name="pageTemplateId"></param>
        /// <returns></returns>
        private IQueryable<PageTemplate> SearchPageTemplates(int? pageTemplateId)
        {
            return Fetch(pageTemplate => !pageTemplateId.HasValue || pageTemplate.ParentId == pageTemplateId);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="pageTemplates"></param>
        /// <returns></returns>
        private IQueryable<PageTemplateModel> Maps(IQueryable<PageTemplate> pageTemplates)
        {
            return pageTemplates.Select(pageTemplate => new PageTemplateModel
            {
                Id = pageTemplate.Id,
                Name = pageTemplate.Name,
                ParentId = pageTemplate.ParentId,
                ParentName = pageTemplate.ParentId.HasValue ? pageTemplate.Parent.Name : string.Empty,
                IsDefaultTemplate = pageTemplate.IsDefaultTemplate,
                IsValid = pageTemplate.IsValid,
                CompileMessage = pageTemplate.CompileMessage,
                RecordOrder = pageTemplate.RecordOrder,
                Created = pageTemplate.Created,
                CreatedBy = pageTemplate.CreatedBy,
                LastUpdate = pageTemplate.LastUpdate,
                LastUpdateBy = pageTemplate.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get page template manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PageTemplateManageModel GetTemplateManageModel(int? id = null)
        {
            var template = GetById(id);
            return template != null ? new PageTemplateManageModel(template) : new PageTemplateManageModel();
        }

        /// <summary>
        /// Get page template manage model from log
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        public PageTemplateManageModel GetTemplateManageModelByLogId(int? logId = null)
        {
            var log = _pageTemplateLogRepository.GetById(logId);
            return log != null ? new PageTemplateManageModel(log) : new PageTemplateManageModel();
        }

        /// <summary>
        /// Save page template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SavePageTemplate(PageTemplateManageModel model)
        {
            ResponseModel response;

            var pageTemplate = GetById(model.Id);
            if (pageTemplate != null)
            {
                var log = new PageTemplateLogManageModel(pageTemplate);
                pageTemplate.Name = model.Name;
                pageTemplate.Content = model.Content;
                pageTemplate.ParentId = model.ParentId;
                pageTemplate.IsValid = true;
                pageTemplate.CompileMessage = string.Empty;

                response = HierarchyUpdate(pageTemplate);
                if (response.Success)
                {
                    _pageTemplateLogService.SavePageTemplateLog(log);
                }

                response.SetMessage(response.Success
                    ? T("PageTemplate_Message_UpdateSuccessfully")
                    : T("PageTemplate_Message_UpdateFailure"));
            }
            else
            {
                Mapper.CreateMap<PageTemplateManageModel, PageTemplate>();
                pageTemplate = Mapper.Map<PageTemplateManageModel, PageTemplate>(model);

                var sameLevelTemplates =
                    Fetch(t => pageTemplate.ParentId.HasValue ? t.ParentId == model.ParentId : !t.ParentId.HasValue);
                var recordOrder = sameLevelTemplates.Any() ? sameLevelTemplates.Max(t => t.RecordOrder) + 10 : 10;
                pageTemplate.RecordOrder = recordOrder;
                pageTemplate.IsValid = true;

                response = HierarchyInsert(pageTemplate);
                response.SetMessage(response.Success
                    ? T("PageTemplate_Message_CreateSuccessfully")
                    : T("PageTemplate_Message_CreateFailure"));
            }

            // Re-initialize templates
            if (response.Success)
            {
                Initialize(pageTemplate.Id);
            }

            return response;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check if page template exists
        /// </summary>
        /// <param name="pageTemplateId">the template id</param>
        /// <param name="name">the template name</param>
        /// <returns></returns>
        public bool IsPageTemplateNameExisted(int? pageTemplateId, string name)
        {
            return Fetch(t => t.Id != pageTemplateId && t.Name.Equals(name)).Any();
        }

        /// <summary>
        /// Check if page template existed for virtual path provider
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool IsPageTemplateExisted(string filePath)
        {
            var templateName = filePath.GetDBTemplateMaster();
            if (string.IsNullOrEmpty(templateName)) return false;

            return Fetch(pageTemplate => pageTemplate.Name.Equals(templateName)).Any();
        }

        #endregion

        #region Details

        /// <summary>
        /// Get page template details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PageTemplateDetailModel GetPageTemplateDetailModel(int id)
        {
            var item = GetById(id);
            return item != null ? new PageTemplateDetailModel(item) : null;
        }

        /// <summary>
        /// Update page template data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdatePageTemplateData(XEditableModel model)
        {
            var pageTemplate = GetById(model.Pk);
            if (pageTemplate != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (PageTemplateManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new PageTemplateManageModel(pageTemplate);
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

                    pageTemplate.SetProperty(model.Name, value);

                    var response = Update(pageTemplate);
                    return response.SetMessage(response.Success
                        ? T("PageTemplate_Message_UpdatePageTemplateInfoSuccessfully")
                        : T("PageTemplate_Message_UpdatePageTemplateInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("PageTemplate_Message_PropertyNotFound")
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("PageTemplate_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete the page template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeletePageTemplate(int id)
        {
            var pageTemplate = GetById(id);
            if (pageTemplate != null)
            {
                if (pageTemplate.IsDefaultTemplate)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("PageTemplate_Message_CannotDeleteDefaultTemplate")
                    };
                }

                if (pageTemplate.Pages.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("PageTemplate_Message_DeleteFailurePageTemplateOnRelatedPages")
                    };
                }

                // Delete logs
                _pageTemplateLogRepository.Delete(pageTemplate.PageTemplateLogs);

                var response = _pageTemplateRepository.SetRecordDeleted(id);
                return response.SetMessage(response.Success
                    ? T("PageTemplate_Message_DeleteSuccessfully")
                    : T("PageTemplate_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("PageTemplate_Message_DeleteSuccessfully")
            };
        }

        #endregion
    }
}