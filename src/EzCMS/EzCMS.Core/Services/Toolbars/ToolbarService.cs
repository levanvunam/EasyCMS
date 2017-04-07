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
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.Toolbars;
using EzCMS.Core.Services.UserGroups;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Toolbars
{
    public class ToolbarService : ServiceHelper, IToolbarService
    {
        private readonly IRepository<Toolbar> _toolbarRepository;
        private readonly IUserGroupService _userGroupService;

        public ToolbarService(IRepository<Toolbar> toolbarRepository, IUserGroupService userGroupService)
        {
            _toolbarRepository = toolbarRepository;
            _userGroupService = userGroupService;
        }

        #region Validation

        /// <summary>
        /// Check if toolbar exists
        /// </summary>
        /// <param name="toolbarId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsToolbarExisted(int? toolbarId, string name)
        {
            return Fetch(u => u.Name.Equals(name) && u.Id != toolbarId).Any();
        }

        #endregion

        /// <summary>
        /// Get toolbar for current user
        /// </summary>
        /// <returns></returns>
        public ToolbarRenderModel GetCurrentUserToolbar()
        {
            if (WorkContext.CurrentUser != null)
            {
                var toolbars =
                    _userGroupService.Fetch(g => WorkContext.CurrentUser.GroupIds.Contains(g.Id) && g.ToolbarId.HasValue)
                        .Select(g => g.Toolbar);

                var basicTools = new List<string>();
                var pageTools = new List<string>();

                if (toolbars.Any())
                {
                    foreach (var toolbar in toolbars)
                    {
                        var basic = SerializeUtilities.Deserialize<List<string>>(toolbar.BasicToolbar);
                        basicTools.AddRange(basic ?? new List<string>());

                        var page = SerializeUtilities.Deserialize<List<string>>(toolbar.PageToolbar);
                        pageTools.AddRange(page ?? new List<string>());
                    }

                    return new ToolbarRenderModel
                    {
                        BasicTools = basicTools.Distinct().ToList(),
                        PageTools = pageTools.Distinct().ToList()
                    };
                }
            }

            var defaultToolbar = FetchFirst(t => t.IsDefault);
            if (defaultToolbar != null)
            {
                return new ToolbarRenderModel(defaultToolbar);
            }

            return new ToolbarRenderModel();
        }

        /// <summary>
        /// Get list of toolbar
        /// </summary>
        /// <param name="toolbarId"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetToolbars(int? toolbarId = null)
        {
            return GetAll().Select(toolbar => new SelectListItem
            {
                Text = toolbar.Name,
                Value = SqlFunctions.StringConvert((double) toolbar.Id).Trim(),
                Selected = toolbarId.HasValue && toolbarId == toolbar.Id
            });
        }

        #region Base

        public IQueryable<Toolbar> GetAll()
        {
            return _toolbarRepository.GetAll();
        }

        public IQueryable<Toolbar> Fetch(Expression<Func<Toolbar, bool>> expression)
        {
            return _toolbarRepository.Fetch(expression);
        }

        public Toolbar FetchFirst(Expression<Func<Toolbar, bool>> expression)
        {
            return _toolbarRepository.FetchFirst(expression);
        }

        public Toolbar GetById(object id)
        {
            return _toolbarRepository.GetById(id);
        }

        internal ResponseModel Insert(Toolbar toolbar)
        {
            return _toolbarRepository.Insert(toolbar);
        }

        internal ResponseModel Update(Toolbar toolbar)
        {
            return _toolbarRepository.Update(toolbar);
        }

        internal ResponseModel Delete(Toolbar toolbar)
        {
            return _toolbarRepository.Delete(toolbar);
        }

        internal ResponseModel Delete(object id)
        {
            return _toolbarRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _toolbarRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the toolbars.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchToolbars(JqSearchIn si)
        {
            var data = GetAll();

            var toolbar = Maps(data);

            return si.Search(toolbar);
        }

        /// <summary>
        /// Export toolbars
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var toolbars = Maps(data);

            var exportData = si.Export(toolbars, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="toolbar"></param>
        /// <returns></returns>
        private IQueryable<ToolbarModel> Maps(IQueryable<Toolbar> toolbar)
        {
            return toolbar.Select(m => new ToolbarModel
            {
                Id = m.Id,
                Name = m.Name,
                IsDefault = m.IsDefault,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get toolbar manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ToolbarManageModel GetToolbarManageModel(int? id = null)
        {
            var toolbar = GetById(id);
            if (toolbar != null)
            {
                return new ToolbarManageModel(toolbar);
            }
            return new ToolbarManageModel();
        }

        /// <summary>
        /// Save toolbar
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveToolbar(ToolbarManageModel model)
        {
            ResponseModel response;
            var toolbar = GetById(model.Id);
            if (toolbar != null)
            {
                toolbar.Name = model.Name;
                toolbar.BasicToolbar = model.BasicToolbar;
                toolbar.PageToolbar = model.PageToolbar;

                response = Update(toolbar);

                return response.SetMessage(response.Success
                    ? T("Toolbar_Message_UpdateSuccessfully")
                    : T("Toolbar_Message_UpdateFailure"));
            }
            Mapper.CreateMap<ToolbarManageModel, Toolbar>();
            toolbar = Mapper.Map<ToolbarManageModel, Toolbar>(model);

            //Cannot create toolbar
            toolbar.IsDefault = false;

            response = Insert(toolbar);

            return response.SetMessage(response.Success
                ? T("Toolbar_Message_CreateSuccessfully")
                : T("Toolbar_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete toolbar by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteToolbar(int id)
        {
            // Stop deletion if relation found
            var toolbar = GetById(id);
            if (toolbar != null)
            {
                if (toolbar.IsDefault)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Toolbar_Message_CanNotDeleteDefaultToolbar")
                    };
                }

                if (toolbar.UserGroups.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Toolbar_Message_DeleteFailureBasedOnRelatedUserGroups")
                    };
                }

                // Delete the toolbar
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("Toolbar_Message_DeleteSuccessfully")
                    : T("Toolbar_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("Toolbar_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get toolbar detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ToolbarDetailModel GetToolbarDetailModel(int id)
        {
            var toolbar = GetById(id);
            return toolbar != null ? new ToolbarDetailModel(toolbar) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateToolbarData(XEditableModel model)
        {
            var toolbar = GetById(model.Pk);
            if (toolbar != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (ToolbarManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    //Generate property value
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new ToolbarManageModel(toolbar);
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

                    toolbar.SetProperty(model.Name, value);

                    var response = Update(toolbar);
                    return response.SetMessage(response.Success
                        ? T("Toolbar_Message_UpdateToolbarInfoSuccessfully")
                        : T("Toolbar_Message_UpdateToolbarInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Toolbar_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("Toolbar_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}