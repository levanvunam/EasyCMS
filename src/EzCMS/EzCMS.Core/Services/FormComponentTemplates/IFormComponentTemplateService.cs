using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.FormComponentTemplates;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.FormComponentTemplates
{
    [Register(Lifetime.PerInstance)]
    public interface IFormComponentTemplateService : IBaseService<FormComponentTemplate>
    {
        /// <summary>
        /// Get form component templates
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetFormComponentTemplates(int? id = null);

        #region Grid Search

        /// <summary>
        /// Search the form component templates
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchFormComponentTemplates(JqSearchIn si);

        /// <summary>
        /// Export the form component templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        FormComponentTemplateManageModel GetFormComponentTemplateManageModel(int? id = null);

        FormComponentTemplateDetailModel GetFormComponentTemplateDetailModel(int? id = null);

        ResponseModel SaveFormComponentTemplate(FormComponentTemplateManageModel model);

        ResponseModel DeleteFormComponentTemplate(int id);

        ResponseModel UpdateFormComponentTemplateData(XEditableModel model);

        #endregion
    }
}