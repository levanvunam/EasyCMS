using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.FormTabs;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.FormTabs
{
    [Register(Lifetime.PerInstance)]
    public interface IFormTabService : IBaseService<FormTab>
    {
        /// <summary>
        /// Get form tab
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetFormTabs(int? id = null);

        #region Grid Search

        JqGridSearchOut SearchFormTabs(JqSearchIn si);

        /// <summary>
        /// Export FormTabs
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        FormTabManageModel GetFormTabManageModel(int? id = null);

        FormTabDetailModel GetFormTabDetailModel(int? id = null);

        ResponseModel SaveFormTab(FormTabManageModel model);

        ResponseModel DeleteFormTab(int id);

        ResponseModel UpdateFormTabData(XEditableModel model);

        #endregion
    }
}