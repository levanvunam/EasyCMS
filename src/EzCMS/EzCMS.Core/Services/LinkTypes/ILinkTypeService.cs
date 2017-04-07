using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.LinkTypes;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.LinkTypes
{
    [Register(Lifetime.PerInstance)]
    public interface ILinkTypeService : IBaseService<LinkType>
    {
        #region Validation

        bool IsTypeExisted(int? linkTypeId, string typeName);

        #endregion

        LinkTypeDetailModel GetLinkTypeDetailModel(int? id = null);

        #region Grid Search

        /// <summary>
        /// Search the link types
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchLinkTypes(JqSearchIn si);

        /// <summary>
        /// Export the link types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        IEnumerable<SelectListItem> GetLinkTypes(List<int> linkTypeIds = null);

        LinkTypeManageModel GetLinkTypeManageModel(int? id = null);

        ResponseModel SaveLinkType(LinkTypeManageModel model);

        ResponseModel DeleteLinkType(int id);

        ResponseModel UpdateLinkTypeData(XEditableModel model);

        #endregion
    }
}