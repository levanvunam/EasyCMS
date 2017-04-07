using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.FormComponentFields;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.FormComponentFields
{
    [Register(Lifetime.PerInstance)]
    public interface IFormComponentFieldService : IBaseService<FormComponentField>
    {
        #region Grid Search

        /// <summary>
        /// Search the form component fields
        /// </summary>
        /// <param name="si"></param>
        /// <param name="formComponentId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchFormComponentFields(JqSearchIn si, int? formComponentId);

        /// <summary>
        /// Export form component fields
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="formComponentId"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? formComponentId);

        #endregion

        #region Manage

        /// <summary>
        /// Get form component field manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FormComponentFieldManageModel GetFormComponentFieldManageModel(int? id = null);

        /// <summary>
        /// Get form component field detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FormComponentFieldDetailModel GetFormComponentFieldDetailModel(int? id = null);

        /// <summary>
        /// Save form component field
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveFormComponentField(FormComponentFieldManageModel model);

        /// <summary>
        /// Delete form component field
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteFormComponentField(int id);

        /// <summary>
        /// Update form component field data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateFormComponentFieldData(XEditableModel model);

        #endregion
    }
}