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
using EzCMS.Core.Models.NoticeTypes;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.NoticeTypes
{
    public class NoticeTypeService : ServiceHelper, INoticeTypeService
    {
        private readonly IRepository<NoticeType> _noticeTypeRepository;

        public NoticeTypeService(IRepository<NoticeType> noticeTypeRepository)
        {
            _noticeTypeRepository = noticeTypeRepository;
        }

        #region Validation

        /// <summary>
        /// Check if type exists
        /// </summary>
        /// <param name="noticeTypeId"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public bool IsTypeExisted(int? noticeTypeId, string typeName)
        {
            return Fetch(u => u.Name.Equals(typeName) && u.Id != noticeTypeId).Any();
        }

        #endregion

        /// <summary>
        /// Get list of notice type
        /// </summary>
        /// <param name="noticeTypeIds"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetNoticeTypes(List<int> noticeTypeIds = null)
        {
            if (noticeTypeIds == null) noticeTypeIds = new List<int>();
            return GetAll().Select(g => new SelectListItem
            {
                Text = g.Name,
                Value = SqlFunctions.StringConvert((double) g.Id).Trim(),
                Selected = noticeTypeIds.Any(id => id == g.Id)
            });
        }

        #region Base

        public IQueryable<NoticeType> GetAll()
        {
            return _noticeTypeRepository.GetAll();
        }

        public IQueryable<NoticeType> Fetch(Expression<Func<NoticeType, bool>> expression)
        {
            return _noticeTypeRepository.Fetch(expression);
        }

        public NoticeType FetchFirst(Expression<Func<NoticeType, bool>> expression)
        {
            return _noticeTypeRepository.FetchFirst(expression);
        }

        public NoticeType GetById(object id)
        {
            return _noticeTypeRepository.GetById(id);
        }

        internal ResponseModel Insert(NoticeType noticeType)
        {
            return _noticeTypeRepository.Insert(noticeType);
        }

        internal ResponseModel Update(NoticeType noticeType)
        {
            return _noticeTypeRepository.Update(noticeType);
        }

        internal ResponseModel Delete(NoticeType noticeType)
        {
            return _noticeTypeRepository.Delete(noticeType);
        }

        internal ResponseModel Delete(object id)
        {
            return _noticeTypeRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _noticeTypeRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the notice types.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchNoticeTypes(JqSearchIn si)
        {
            var data = GetAll();

            var noticeType = Maps(data);

            return si.Search(noticeType);
        }

        /// <summary>
        /// Export notice types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var noticeTypes = Maps(data);

            var exportData = si.Export(noticeTypes, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="noticeType"></param>
        /// <returns></returns>
        private IQueryable<NoticeTypeModel> Maps(IQueryable<NoticeType> noticeType)
        {
            return noticeType.Select(m => new NoticeTypeModel
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
        /// Get notice type manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NoticeTypeManageModel GetNoticeTypeManageModel(int? id = null)
        {
            var noticeType = GetById(id);
            if (noticeType != null)
            {
                return new NoticeTypeManageModel(noticeType);
            }
            return new NoticeTypeManageModel();
        }

        /// <summary>
        /// Save notice type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveNoticeType(NoticeTypeManageModel model)
        {
            ResponseModel response;
            var noticeType = GetById(model.Id);
            if (noticeType != null)
            {
                noticeType.Name = model.Name;
                response = Update(noticeType);
                return response.SetMessage(response.Success
                    ? T("NoticeType_Message_UpdateSuccessfully")
                    : T("NoticeType_Message_UpdateFailure"));
            }
            Mapper.CreateMap<NoticeTypeManageModel, NoticeType>();
            noticeType = Mapper.Map<NoticeTypeManageModel, NoticeType>(model);
            response = Insert(noticeType);
            return
                response.SetMessage(response.Success
                    ? T("NoticeType_Message_CreateSuccessfully")
                    : T("NoticeType_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete notice type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteNoticeType(int id)
        {
            var noticeType = GetById(id);
            if (noticeType != null)
            {
                if (noticeType.Notices.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("NoticeType_Message_DeleteFailureBasedOnRelatedNotices")
                    };
                }
                var response = Delete(noticeType);
                return response.SetMessage(response.Success
                    ? T("NoticeType_Message_DeleteSuccessfully")
                    : T("NoticeType_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("NoticeType_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get notice type detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NoticeTypeDetailModel GetNoticeTypeDetailModel(int id)
        {
            var noticeType = GetById(id);
            return noticeType != null ? new NoticeTypeDetailModel(noticeType) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateNoticeTypeData(XEditableModel model)
        {
            var noticeType = GetById(model.Pk);
            if (noticeType != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (NoticeTypeManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new NoticeTypeManageModel(noticeType);
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

                    noticeType.SetProperty(model.Name, value);

                    var response = Update(noticeType);
                    return response.SetMessage(response.Success
                        ? T("NoticeType_Message_UpdateNoticeTypeInfoSuccessfully")
                        : T("NoticeType_Message_UpdateNoticeTypeInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("NoticeType_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("NoticeType_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}