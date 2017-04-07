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
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.FileTemplates;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.FileTemplates
{
    public class FileTemplateService : ServiceHelper, IFileTemplateService
    {
        private readonly IRepository<FileTemplate> _fileTemplateRepository;

        public FileTemplateService(IRepository<FileTemplate> fileTemplateRepository)
        {
            _fileTemplateRepository = fileTemplateRepository;
        }

        /// <summary>
        /// Gets the user groups.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetFileTemplates(int? id = null)
        {
            return GetAll().Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = SqlFunctions.StringConvert((double) t.Id).Trim(),
                Selected = id == t.Id
            });
        }

        /// <summary>
        /// Check if template exists.
        /// </summary>
        /// <param name="fileTemplateId">the template id</param>
        /// <param name="name">the template name</param>
        /// <returns></returns>
        public bool IsFileTemplateNameExisted(int? fileTemplateId, string name)
        {
            return Fetch(t => t.Id != fileTemplateId && t.Name.Equals(name)).Any();
        }

        /// <summary>
        /// Get master template for file Template 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetFileTemplateMaster(int id)
        {
            var fileTemplate = GetById(id);
            if (fileTemplate != null)
            {
                return fileTemplate.PageTemplateId.HasValue
                    ? string.Format("{0}.{1}", EzCMSContants.DbTemplate, fileTemplate.PageTemplate.Name)
                    : string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// Get FileTemplate detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileTemplateDetailModel GetFileTemplateDetailModel(int id)
        {
            var item = GetById(id);
            return item != null ? new FileTemplateDetailModel(item) : null;
        }

        /// <summary>
        /// Update file template data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateFileTemplateData(XEditableModel model)
        {
            var fileTemplate = GetById(model.Pk);
            if (fileTemplate != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (FileTemplateManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new FileTemplateManageModel(fileTemplate);
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

                    fileTemplate.SetProperty(model.Name, value);

                    var response = Update(fileTemplate);
                    return response.SetMessage(response.Success
                        ? T("FileTemplate_Message_UpdateFileTemplateInfoSuccessfully")
                        : T("FileTemplate_Message_UpdateFileTemplateInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("FileTemplate_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("FileTemplate_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete file template by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteFileTemplate(int id)
        {
            var fileTemplate = GetById(id);

            if (fileTemplate != null)
            {
                // Stop deletion if related with pages
                if (fileTemplate.Pages.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("FileTemplate_Message_DeleteFailureFileTemplateOnRelatedPages")
                    };
                }

                // Delete the file template
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("FileTemplate_Message_DeleteSuccessfully")
                    : T("FileTemplate_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("FileTemplate_Message_DeleteSuccessfully")
            };
        }

        #region Base

        public IQueryable<FileTemplate> GetAll()
        {
            return _fileTemplateRepository.GetAll();
        }

        public IQueryable<FileTemplate> Fetch(Expression<Func<FileTemplate, bool>> expression)
        {
            return _fileTemplateRepository.Fetch(expression);
        }

        public FileTemplate FetchFirst(Expression<Func<FileTemplate, bool>> expression)
        {
            return _fileTemplateRepository.FetchFirst(expression);
        }

        public FileTemplate GetById(object id)
        {
            return _fileTemplateRepository.GetById(id);
        }

        internal ResponseModel Insert(FileTemplate fileTemplate)
        {
            return _fileTemplateRepository.Insert(fileTemplate);
        }

        internal ResponseModel Update(FileTemplate fileTemplate)
        {
            return _fileTemplateRepository.Update(fileTemplate);
        }

        internal ResponseModel Delete(FileTemplate fileTemplate)
        {
            return _fileTemplateRepository.Delete(fileTemplate);
        }

        internal ResponseModel Delete(object id)
        {
            return _fileTemplateRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _fileTemplateRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the file templates.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchFileTemplates(JqSearchIn si)
        {
            var data = GetAll();

            var fileTemplates = Maps(data);

            return si.Search(fileTemplates);
        }

        /// <summary>
        /// Export file templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var fileTemplates = Maps(data);

            var exportData = si.Export(fileTemplates, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }


        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="fileTemplates"></param>
        /// <returns></returns>
        private IQueryable<FileTemplateModel> Maps(IQueryable<FileTemplate> fileTemplates)
        {
            return fileTemplates.Select(u => new FileTemplateModel
            {
                Id = u.Id,
                Name = u.Name,
                Controller = u.Controller,
                Action = u.Action,
                Area = u.Area,
                Parameters = u.Parameters,
                PageTemplateId = u.PageTemplateId,
                PageTemplateName = u.PageTemplateId.HasValue ? u.PageTemplate.Name : string.Empty,
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
        /// Get file template manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileTemplateManageModel GetTemplateManageModel(int? id = null)
        {
            var template = GetById(id);
            if (template != null)
            {
                return new FileTemplateManageModel(template);
            }

            return new FileTemplateManageModel();
        }

        /// <summary>
        /// Save file template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveFileTemplate(FileTemplateManageModel model)
        {
            ResponseModel response;
            var fileTemplate = GetById(model.Id);
            if (fileTemplate != null)
            {
                fileTemplate.Name = model.Name;
                fileTemplate.Action = model.Action;
                fileTemplate.Controller = model.Controller;
                fileTemplate.Parameters = model.Parameters;
                fileTemplate.Area = model.Area;
                fileTemplate.PageTemplateId = model.PageTemplateId;

                response = Update(fileTemplate);
                return response.SetMessage(response.Success
                    ? T("FileTemplate_Message_UpdateSuccessfully")
                    : T("FileTemplate_Message_UpdateFailure"));
            }
            Mapper.CreateMap<FileTemplateManageModel, FileTemplate>();
            fileTemplate = Mapper.Map<FileTemplateManageModel, FileTemplate>(model);
            response = Insert(fileTemplate);
            return response.SetMessage(response.Success
                ? T("FileTemplate_Message_CreateSuccessfully")
                : T("FileTemplate_Message_CreateFailure"));
        }

        #endregion
    }
}