using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.NoticeTypes;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.NoticeTypes
{
    [Register(Lifetime.PerInstance)]
    public interface INoticeTypeService : IBaseService<NoticeType>
    {
        #region Validation

        bool IsTypeExisted(int? noticeTypeId, string typeName);

        #endregion

        NoticeTypeDetailModel GetNoticeTypeDetailModel(int id);

        #region Grid Search

        JqGridSearchOut SearchNoticeTypes(JqSearchIn si);

        /// <summary>
        /// Export notice types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        IEnumerable<SelectListItem> GetNoticeTypes(List<int> noticeTypeIds = null);

        NoticeTypeManageModel GetNoticeTypeManageModel(int? id = null);

        ResponseModel SaveNoticeType(NoticeTypeManageModel model);

        ResponseModel DeleteNoticeType(int id);

        ResponseModel UpdateNoticeTypeData(XEditableModel model);

        #endregion
    }
}