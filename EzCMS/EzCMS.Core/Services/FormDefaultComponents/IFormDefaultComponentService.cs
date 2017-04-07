using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.FormDefaultComponents;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.FormDefaultComponents
{
    [Register(Lifetime.PerInstance)]
    public interface IFormDefaultComponentService : IBaseService<FormDefaultComponent>
    {
        /// <summary>
        /// Get form default component select list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetFormDefaultComponents(int? id = null);

        #region Grid Search

        /// <summary>
        /// Search the form default components
        /// </summary>
        /// <param name="si"></param>
        /// <param name="formComponentTemplateId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchFormDefaultComponents(JqSearchIn si, int? formComponentTemplateId);

        /// <summary>
        /// Export form default components
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="formComponentTemplateId"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? formComponentTemplateId);

        #endregion

        #region Manage

        /// <summary>
        /// Get form default component manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FormDefaultComponentManageModel GetFormDefaultComponentManageModel(int? id = null);

        /// <summary>
        /// Save form default component
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveFormDefaultComponent(FormDefaultComponentManageModel model);

        /// <summary>
        /// Delete form default component
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteFormDefaultComponent(int id);

        #endregion

        #region Details

        /// <summary>
        /// Get form default component detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FormDefaultComponentDetailModel GetFormDefaultComponentDetailModel(int? id = null);

        /// <summary>
        /// Update form default component data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateFormDefaultComponentData(XEditableModel model);

        #endregion
    }
}