using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.FormComponents;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.FormComponents
{
    [Register(Lifetime.PerInstance)]
    public interface IFormComponentService : IBaseService<FormComponent>
    {
        /// <summary>
        /// Get form components
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetFormComponents(int? id = null);

        #region Grid Search

        /// <summary>
        /// Search the form components
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchFormComponents(JqSearchIn si, FormComponentSearchModel model);

        /// <summary>
        /// Export the form components
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, FormComponentSearchModel model);

        #endregion

        #region Manage

        /// <summary>
        /// Get form component manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FormComponentManageModel GetFormComponentManageModel(int? id = null);

        /// <summary>
        /// Save form component
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveFormComponent(FormComponentManageModel model);

        /// <summary>
        /// Delete form component
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteFormComponent(int id);

        #endregion

        #region Details

        /// <summary>
        /// Get form component details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FormComponentDetailModel GetFormComponentDetailModel(int? id = null);

        /// <summary>
        /// Update form component data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateFormComponentData(XEditableModel model);

        #endregion
    }
}