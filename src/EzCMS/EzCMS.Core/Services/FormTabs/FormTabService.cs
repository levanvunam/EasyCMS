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
using EzCMS.Core.Models.FormTabs;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.FormTabs
{
    public class FormTabService : ServiceHelper, IFormTabService
    {
        private readonly IRepository<FormTab> _formTabRepository;

        public FormTabService(IRepository<FormTab> formTabRepository)
        {
            _formTabRepository = formTabRepository;
        }

        /// <summary>
        /// Get form tab
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetFormTabs(int? id = null)
        {
            return GetAll().Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = SqlFunctions.StringConvert((double) t.Id).Trim(),
                Selected = id.HasValue && id == t.Id
            });
        }

        #region Base

        public IQueryable<FormTab> GetAll()
        {
            return _formTabRepository.GetAll();
        }

        public IQueryable<FormTab> Fetch(Expression<Func<FormTab, bool>> expression)
        {
            return _formTabRepository.Fetch(expression);
        }

        public FormTab FetchFirst(Expression<Func<FormTab, bool>> expression)
        {
            return _formTabRepository.FetchFirst(expression);
        }

        public FormTab GetById(object id)
        {
            return _formTabRepository.GetById(id);
        }

        internal ResponseModel Insert(FormTab formTab)
        {
            return _formTabRepository.Insert(formTab);
        }

        internal ResponseModel Update(FormTab formTab)
        {
            return _formTabRepository.Update(formTab);
        }

        internal ResponseModel Delete(FormTab formTab)
        {
            return _formTabRepository.Delete(formTab);
        }

        internal ResponseModel Delete(object id)
        {
            return _formTabRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _formTabRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the form tabs
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchFormTabs(JqSearchIn si)
        {
            var data = GetAll();
            var formTabs = Maps(data);
            return si.Search(formTabs);
        }

        /// <summary>
        /// Export form tabs
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var formTabs = Maps(data);

            var exportData = si.Export(formTabs, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="formTabs"></param>
        /// <returns></returns>
        private IQueryable<FormTabModel> Maps(IQueryable<FormTab> formTabs)
        {
            return formTabs.Select(m => new FormTabModel
            {
                Id = m.Id,
                Name = m.Name,
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
        /// Get form tab manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FormTabManageModel GetFormTabManageModel(int? id = null)
        {
            var formTab = GetById(id);
            if (formTab != null)
            {
                return new FormTabManageModel(formTab);
            }
            return new FormTabManageModel();
        }

        /// <summary>
        /// Get form tab detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FormTabDetailModel GetFormTabDetailModel(int? id = null)
        {
            var formTab = GetById(id);
            return formTab != null ? new FormTabDetailModel(formTab) : null;
        }

        /// <summary>
        /// Save form tab
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveFormTab(FormTabManageModel model)
        {
            ResponseModel response;
            var formTab = GetById(model.Id);
            if (formTab != null)
            {
                formTab.Name = model.Name;
                formTab.RecordOrder = model.RecordOrder;
                response = Update(formTab);
                return response.SetMessage(response.Success
                    ? T("FormTab_Message_UpdateSuccessfully")
                    : T("FormTab_Message_UpdateFailure"));
            }
            Mapper.CreateMap<FormTabManageModel, FormTab>();
            formTab = Mapper.Map<FormTabManageModel, FormTab>(model);
            response = Insert(formTab);
            return response.SetMessage(response.Success
                ? T("FormTab_Message_CreateSuccessfully")
                : T("FormTab_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete form tab
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteFormTab(int id)
        {
            // Stop deletion if form component found
            var item = GetById(id);

            if (item != null)
            {
                if (item.FormComponents.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("FormTab_Message_DeleteFailureBasedOnRelatedFormComponents")
                    };
                }

                // Delete form tab
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("FormTab_Message_DeleteSuccessfully")
                    : T("FormTab_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("FormTab_Message_DeleteSuccessfully")
            };
        }

        /// <summary>
        /// Update form tab data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateFormTabData(XEditableModel model)
        {
            var formTab = GetById(model.Pk);
            if (formTab != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (FormTabManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new FormTabManageModel(formTab);
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

                    formTab.SetProperty(model.Name, value);

                    var response = Update(formTab);
                    return response.SetMessage(response.Success
                        ? T("FormTab_Message_UpdateFormTabInfoSuccessfully")
                        : T("FormTab_Message_UpdateFormTabInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("FormTab_Message_PropertyNotFound")
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("FormTab_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}