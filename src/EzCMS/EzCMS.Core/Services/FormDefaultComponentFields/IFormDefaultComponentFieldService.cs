using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.FormDefaultComponentFields;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.FormDefaultComponentFields
{
    [Register(Lifetime.PerInstance)]
    public interface IFormDefaultComponentFieldService : IBaseService<FormDefaultComponentField>
    {
        #region Grid Search

        /// <summary>
        /// Search the form default component fields
        /// </summary>
        /// <param name="si"></param>
        /// <param name="formDefaultComponentId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchFormDefaultComponentFields(JqSearchIn si, int? formDefaultComponentId);

        /// <summary>
        /// Export form default component fields
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="formDefaultComponentId"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? formDefaultComponentId);

        #endregion

        #region Manage

        /// <summary>
        /// Get form default component field manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FormDefaultComponentFieldManageModel GetFormDefaultComponentFieldManageModel(int? id = null);

        /// <summary>
        /// Save form default component field
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveFormDefaultComponentField(FormDefaultComponentFieldManageModel model);

        /// <summary>
        /// Delete form default component field
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteFormDefaultComponentField(int id);

        #endregion

        #region Details

        /// <summary>
        /// Get form default component field detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FormDefaultComponentFieldDetailModel GetFormDefaultComponentFieldDetailModel(int? id = null);

        /// <summary>
        /// Update form default component field data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateFormDefaultComponentFieldData(XEditableModel model);

        #endregion
    }
}