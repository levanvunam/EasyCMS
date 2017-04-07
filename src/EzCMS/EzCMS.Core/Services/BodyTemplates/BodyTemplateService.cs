using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Routing;
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
using Ez.Framework.Utilities.Web;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Models.BodyTemplates;
using EzCMS.Core.Models.BodyTemplates.HelpServices;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.BodyTemplates
{
    public class BodyTemplateService : ServiceHelper, IBodyTemplateService
    {
        private readonly IRepository<BodyTemplate> _bodyTemplateRepository;
        private readonly ISiteSettingService _siteSettingService;

        public BodyTemplateService(IRepository<BodyTemplate> bodyTemplateRepository,
            ISiteSettingService siteSettingService)
        {
            _bodyTemplateRepository = bodyTemplateRepository;
            _siteSettingService = siteSettingService;
        }

        #region Validation

        /// <summary>
        /// Check if body template name existed
        /// </summary>
        /// <param name="bodyTemplateId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsNameExisted(int? bodyTemplateId, string name)
        {
            return Fetch(t => t.Id != bodyTemplateId && t.Name.Equals(name)).Any();
        }

        #endregion

        /// <summary>
        /// Get body template list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<SelectListItem> GetBodyTemplates(int? id = null)
        {
            return GetAll().Select(b => new SelectListItem
            {
                Text = b.Name,
                Value = SqlFunctions.StringConvert((double) b.Id).Trim(),
                Selected = id != null && b.Id == id
            }).ToList();
        }

        /// <summary>
        /// Get body template content
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetBodyTemplateContent(int? id)
        {
            var bodyTemplate = GetById(id);
            if (bodyTemplate != null)
            {
                return bodyTemplate.Content;
            }
            return string.Empty;
        }

        /// <summary>
        /// Get chosen body template model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ChosenBodyTemplateModel GetChosenBodyTemplateModel(int id)
        {
            var bodyTemplate = GetById(id);

            if (bodyTemplate != null)
            {
                return new ChosenBodyTemplateModel
                {
                    Content = bodyTemplate.Content,
                    BodyTemplates = GetBodyTemplates(id)
                };
            }

            return null;
        }

        #region Base

        public IQueryable<BodyTemplate> GetAll()
        {
            return _bodyTemplateRepository.GetAll();
        }

        public IQueryable<BodyTemplate> Fetch(Expression<Func<BodyTemplate, bool>> expression)
        {
            return _bodyTemplateRepository.Fetch(expression);
        }

        public BodyTemplate FetchFirst(Expression<Func<BodyTemplate, bool>> expression)
        {
            return _bodyTemplateRepository.FetchFirst(expression);
        }

        public BodyTemplate GetById(object id)
        {
            return _bodyTemplateRepository.GetById(id);
        }

        internal ResponseModel Insert(BodyTemplate bodyTemplate)
        {
            return _bodyTemplateRepository.Insert(bodyTemplate);
        }

        internal ResponseModel Update(BodyTemplate bodyTemplate)
        {
            return _bodyTemplateRepository.Update(bodyTemplate);
        }

        internal ResponseModel Delete(BodyTemplate bodyTemplate)
        {
            return _bodyTemplateRepository.Delete(bodyTemplate);
        }

        internal ResponseModel Delete(object id)
        {
            return _bodyTemplateRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _bodyTemplateRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Export body templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var bodyTemplates = Maps(data);

            var exportData = si.Export(bodyTemplates, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Search the body templates.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchBodyTemplates(JqSearchIn si)
        {
            var data = GetAll();

            var bodyTemplates = Maps(data);

            return si.Search(bodyTemplates);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="bodyTemplates"></param>
        /// <returns></returns>
        private IQueryable<BodyTemplateModel> Maps(IQueryable<BodyTemplate> bodyTemplates)
        {
            return bodyTemplates.Select(m => new BodyTemplateModel
            {
                Id = m.Id,
                Name = m.Name,
                ImageUrl = m.ImageUrl,
                Description = m.Description,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get body template manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BodyTemplateManageModel GetBodyTemplateManageModel(int? id = null)
        {
            var bodyTemplate = GetById(id);
            if (bodyTemplate != null)
            {
                return new BodyTemplateManageModel(bodyTemplate);
            }
            return new BodyTemplateManageModel();
        }

        /// <summary>
        /// Save BodyTemplate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveBodyTemplate(BodyTemplateManageModel model)
        {
            ResponseModel response;
            var bodyTemplate = GetById(model.Id);
            if (bodyTemplate != null)
            {
                bodyTemplate.Name = model.Name;
                bodyTemplate.ImageUrl = model.ImageUrl;
                bodyTemplate.Description = model.Description;
                bodyTemplate.Content = model.Content;
                response = Update(bodyTemplate);
                return response.SetMessage(response.Success
                    ? T("BodyTemplate_Message_UpdateSuccessfully")
                    : T("BodyTemplate_Message_UpdateFailure"));
            }
            Mapper.CreateMap<BodyTemplateManageModel, BodyTemplate>();
            bodyTemplate = Mapper.Map<BodyTemplateManageModel, BodyTemplate>(model);
            response = Insert(bodyTemplate);
            return response.SetMessage(response.Success
                ? T("BodyTemplate_Message_CreateSuccessfully")
                : T("BodyTemplate_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete body template by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteBodyTemplate(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                // Stop deletion if related with pages
                if (item.Pages.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("BodyTemplate_Message_DeleteFailureBodyTemplateOnRelatedPageData")
                    };
                }

                // Delete the body template
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("BodyTemplate_Message_DeleteSuccessfully")
                    : T("BodyTemplate_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("BodyTemplate_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get body template detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BodyTemplateDetailModel GetBodyTemplateDetailModel(int id)
        {
            var bodyTemplate = GetById(id);
            return bodyTemplate != null ? new BodyTemplateDetailModel(bodyTemplate) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateBodyTemplateData(XEditableModel model)
        {
            var bodyTemplate = GetById(model.Pk);
            if (bodyTemplate != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (BodyTemplateManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new BodyTemplateManageModel(bodyTemplate);
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

                    bodyTemplate.SetProperty(model.Name, value);
                    var response = Update(bodyTemplate);
                    return response.SetMessage(response.Success
                        ? T("BodyTemplate_Message_UpdateBodyTemplateInfoSuccessfully")
                        : T("BodyTemplate_Message_UpdateBodyTemplateInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("BodyTemplate_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("BodyTemplate_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Select Body Template 

        /// <summary>
        /// Get body template selector model
        /// </summary>
        /// <returns></returns>
        public BodyTemplateSelectorModel GetBodyTemplateSelector(BodyTemplateEnums.Mode mode)
        {
            var helpServiceSetting = _siteSettingService.LoadSetting<EzCMSHelpConfigurationSetting>();

            return new BodyTemplateSelectorModel
            {
                EnableOnlineTemplate = !helpServiceSetting.DisabledOnlineBodyTemplateService,
                Mode = mode,
                BodyTemplates = GetInstalledBodyTemplates()
            };
        }

        /// <summary>
        /// Get installed body templates
        /// </summary>
        /// <returns></returns>
        public List<BodyTemplateSelectModel> GetInstalledBodyTemplates()
        {
            return GetAll().Select(b => new BodyTemplateSelectModel
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                ImageUrl = b.ImageUrl
            }).ToList();
        }

        /// <summary>
        /// Search body template
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public BodyTemplateSelectorModel SearchBodyTemplates(string keyword, int? pageIndex, int? pageSize)
        {
            var helpServiceSetting = _siteSettingService.LoadSetting<EzCMSHelpConfigurationSetting>();
            var model = WebUtilities.SendApiRequest<BodyTemplateSelectorModel>(
                helpServiceSetting.HelpServiceUrl, helpServiceSetting.AuthorizeCode,
                "api/Helps/SearchBodyTemplates",
                HttpMethod.Get, new RouteValueDictionary(new
                {
                    keyword,
                    pageIndex,
                    pageSize
                }));

            return model;
        }

        /// <summary>
        /// Download online template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BodyTemplateDownloadModel GetOnlineTemplate(int id)
        {
            var helpServiceSetting = _siteSettingService.LoadSetting<EzCMSHelpConfigurationSetting>();
            var model = WebUtilities.SendApiRequest<BodyTemplateDownloadModel>(
                helpServiceSetting.HelpServiceUrl, helpServiceSetting.AuthorizeCode,
                "api/Helps/DownloadBodyTemplate",
                HttpMethod.Get, new RouteValueDictionary(new
                {
                    id
                }));

            return model;
        }

        /// <summary>
        /// Save online template
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ResponseModel DownloadedOnlineTemplate(int id, string name)
        {
            var template = GetOnlineTemplate(id);

            if (template != null)
            {
                var model = new BodyTemplateManageModel
                {
                    Name = name,
                    Description = template.Description,
                    ImageUrl = template.ImageUrl,
                    Content = template.Content
                };

                #region Validate

                var validationResults = model.ValidateModel();

                if (validationResults.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = validationResults.BuildValidationMessages()
                    };
                }

                #endregion

                var response = SaveBodyTemplate(model);

                return response.SetMessage(response.Success
                    ? T("BodyTemplate_Message_DownloadOnlineBodyTemplateSuccessfully")
                    : T("BodyTemplate_Message_DownloadOnlineBodyTemplateFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("BodyTemplate_Message_OnlineTemplateNotFound")
            };
        }

        #endregion
    }
}