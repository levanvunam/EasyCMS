using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using EzCMS.Core.Models.Notices;
using EzCMS.Core.Models.Notices.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Notices
{
    public class NoticeService : ServiceHelper, INoticeService
    {
        private readonly IRepository<Notice> _noticeRepository;

        public NoticeService(IRepository<Notice> noticeRepository)
        {
            _noticeRepository = noticeRepository;
        }

        #region Widget

        /// <summary>
        /// Get noticeboard widget
        /// </summary>
        /// <param name="noticeTypeId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public NoticeboardWidget GetNoticesWidgets(int noticeTypeId, int number)
        {
            var data = GetAll().Where(m =>
                (noticeTypeId == 0 || m.NoticeTypeId == noticeTypeId) &&
                (!m.DateEnd.HasValue || m.DateEnd >= DateTime.UtcNow) &&
                (!m.DateStart.HasValue || m.DateStart <= DateTime.UtcNow))
                .OrderByDescending(m => m.DateStart);

            var noticeWidgets = MapToNoticeboardWidget(data);

            List<NoticeWidget> notices;

            if (number > 0)
            {
                notices = noticeWidgets.Take(number).ToList();
            }
            else
            {
                notices = noticeWidgets.ToList();
            }

            return new NoticeboardWidget(notices);
        }

        #endregion

        /// <summary>
        /// Get Notice detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NoticeDetailModel GetNoticeDetailModel(int id)
        {
            var item = GetById(id);
            return item != null ? new NoticeDetailModel(item) : null;
        }

        #region Base

        public IQueryable<Notice> GetAll()
        {
            return _noticeRepository.GetAll();
        }

        public IQueryable<Notice> Fetch(Expression<Func<Notice, bool>> expression)
        {
            return _noticeRepository.Fetch(expression);
        }

        public Notice FetchFirst(Expression<Func<Notice, bool>> expression)
        {
            return _noticeRepository.FetchFirst(expression);
        }

        public Notice GetById(object id)
        {
            return _noticeRepository.GetById(id);
        }

        internal ResponseModel Insert(Notice notice)
        {
            return _noticeRepository.Insert(notice);
        }

        internal ResponseModel Update(Notice notice)
        {
            return _noticeRepository.Update(notice);
        }

        internal ResponseModel Delete(Notice notice)
        {
            return _noticeRepository.Delete(notice);
        }

        internal ResponseModel Delete(object id)
        {
            return _noticeRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _noticeRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the notices
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchNotices(JqSearchIn si, NoticeSearchModel model)
        {
            var data = SearchNotices(model);

            var notices = Maps(data);

            return si.Search(notices);
        }

        /// <summary>
        /// Export notices
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, NoticeSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchNotices(model);

            var notices = Maps(data);

            var exportData = si.Export(notices, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search notices
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<Notice> SearchNotices(NoticeSearchModel model)
        {
            return Fetch(notice => (string.IsNullOrEmpty(model.Keyword)
                                    || (!string.IsNullOrEmpty(notice.Message) && notice.Message.Contains(model.Keyword)))
                                   && (!model.NoticeTypeId.HasValue || notice.NoticeTypeId == model.NoticeTypeId));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="notices"></param>
        /// <returns></returns>
        private IQueryable<NoticeModel> Maps(IQueryable<Notice> notices)
        {
            return notices.Select(m => new NoticeModel
            {
                Id = m.Id,
                Message = m.Message,
                IsUrgent = m.IsUrgent,
                DateStart = m.DateStart,
                DateEnd = m.DateEnd,
                NoticeTypeId = m.NoticeTypeId,
                NoticeTypeName = m.NoticeType.Name,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="notices"></param>
        /// <returns></returns>
        private IQueryable<NoticeWidget> MapToNoticeboardWidget(IQueryable<Notice> notices)
        {
            return notices.Select(m => new NoticeWidget
            {
                DateStart = m.DateStart,
                DateEnd = m.DateEnd,
                Message = m.Message,
                IsUrgent = m.IsUrgent,
                NoticeTypeId = m.NoticeTypeId,
                NoticeTypeName = m.NoticeType.Name
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get notice manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <param name="noticeTypeId"></param>
        /// <returns></returns>
        public NoticeManageModel GetNoticeManageModel(int? id = null, int? noticeTypeId = null)
        {
            var notice = GetById(id);
            if (notice != null)
            {
                return new NoticeManageModel(notice);
            }

            return new NoticeManageModel(noticeTypeId);
        }

        /// <summary>
        /// Save notice
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveNotice(NoticeManageModel model)
        {
            ResponseModel response;
            var notice = GetById(model.Id);
            if (notice != null)
            {
                notice.DateStart = model.DateStart;
                notice.DateEnd = model.DateEnd;
                notice.Message = model.Message;
                notice.IsUrgent = model.IsUrgent;
                notice.NoticeTypeId = model.NoticeTypeId;
                response = Update(notice);
                return response.SetMessage(response.Success
                    ? T("Notice_Message_UpdateSuccessfully")
                    : T("Notice_Message_UpdateFailure"));
            }
            Mapper.CreateMap<NoticeManageModel, Notice>();
            notice = Mapper.Map<NoticeManageModel, Notice>(model);
            response = Insert(notice);
            return response.SetMessage(response.Success
                ? T("Notice_Message_CreateSuccessfully")
                : T("Notice_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete notice
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteNotice(int id)
        {
            var notice = GetById(id);
            if (notice != null)
            {
                var response = Delete(notice);
                return response.SetMessage(response.Success
                    ? T("Notice_Message_DeleteSuccessfully")
                    : T("Notice_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("Notice_Message_DeleteSuccessfully")
            };
        }

        /// <summary>
        /// Update Notice data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateNoticeData(XEditableModel model)
        {
            var notice = GetById(model.Pk);
            if (notice != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (NoticeManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new NoticeManageModel(notice);
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

                    notice.SetProperty(model.Name, value);

                    var response = Update(notice);
                    return response.SetMessage(response.Success
                        ? T("Notice_Message_UpdateNoticeInfoSuccessfully")
                        : T("Notice_Message_UpdateNoticeInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Notice_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("Notice_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}