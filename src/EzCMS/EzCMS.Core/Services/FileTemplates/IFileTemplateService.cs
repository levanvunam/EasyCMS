using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.FileTemplates;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.FileTemplates
{
    [Register(Lifetime.PerInstance)]
    public interface IFileTemplateService : IBaseService<FileTemplate>
    {
        #region Validation

        /// <summary>
        /// Check if file template exists
        /// </summary>
        /// <param name="fileTemplateId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsFileTemplateNameExisted(int? fileTemplateId, string name);

        #endregion

        /// <summary>
        /// Get file template detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FileTemplateDetailModel GetFileTemplateDetailModel(int id);

        /// <summary>
        /// Get file template master
        /// </summary>
        /// <returns></returns>
        string GetFileTemplateMaster(int id);

        /// <summary>
        /// Get file template select list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetFileTemplates(int? id = null);

        #region Grid Search

        /// <summary>
        /// Search file templates
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchFileTemplates(JqSearchIn si);

        /// <summary>
        /// Export file templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get page template manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FileTemplateManageModel GetTemplateManageModel(int? id = null);

        /// <summary>
        /// Save file template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveFileTemplate(FileTemplateManageModel model);

        /// <summary>
        /// Update template data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateFileTemplateData(XEditableModel model);

        /// <summary>
        /// Delete file template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteFileTemplate(int id);

        #endregion
    }
}