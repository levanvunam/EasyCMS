using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Tags;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Tags
{
    [Register(Lifetime.PerInstance)]
    public interface ITagService : IBaseService<Tag>
    {
        /// <summary>
        /// Get tags
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        IEnumerable<Select2Model> GetTags(int? pageId = null);

        /// <summary>
        /// Get tags
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetTagSelectList(int? pageId = null);

        /// <summary>
        /// Get tag details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TagDetailModel GetTagDetailModel(int id);

        /// <summary>
        /// Update tag data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateTagData(XEditableModel model);

        #region Grid Search

        /// <summary>
        /// Search tags
        /// </summary>
        /// <param name="si"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchTags(JqSearchIn si, int? pageId);

        /// <summary>
        /// Export tags
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? pageId);

        #endregion

        #region Manage

        /// <summary>
        /// Get Tag
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TagManageModel GetTagManageModel(int? id = null);

        /// <summary>
        /// Save Tag
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveTag(TagManageModel model);

        /// <summary>
        /// Delete Tag
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteTag(int id);

        /// <summary>
        /// Remove Page and Tag reference
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        ResponseModel DeletePageTagMapping(int tagId, int pageId);

        #endregion
    }
}