using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Models.BodyTemplates;
using EzCMS.Core.Models.BodyTemplates.HelpServices;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.BodyTemplates
{
    [Register(Lifetime.PerInstance)]
    public interface IBodyTemplateService : IBaseService<BodyTemplate>
    {
        #region Validation

        /// <summary>
        /// Check name is existed or not
        /// </summary>
        /// <param name="bodyTemplateId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsNameExisted(int? bodyTemplateId, string name);

        #endregion

        /// <summary>
        /// Get list of body templates
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<SelectListItem> GetBodyTemplates(int? id = null);

        #region Grid Search

        /// <summary>
        /// Search the body templates
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchBodyTemplates(JqSearchIn si);

        /// <summary>
        /// Export body templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get body template manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BodyTemplateManageModel GetBodyTemplateManageModel(int? id = null);

        /// <summary>
        /// Save body template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveBodyTemplate(BodyTemplateManageModel model);

        /// <summary>
        /// Delete body template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteBodyTemplate(int id);

        #endregion

        #region Details

        /// <summary>
        /// Get body template detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BodyTemplateDetailModel GetBodyTemplateDetailModel(int id);

        /// <summary>
        /// Update body template data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateBodyTemplateData(XEditableModel model);

        #endregion

        #region Select Body Template 

        /// <summary>
        /// Get body template selector
        /// </summary>
        /// <returns></returns>
        BodyTemplateSelectorModel GetBodyTemplateSelector(BodyTemplateEnums.Mode mode);

        /// <summary>
        /// Get installed body templates
        /// </summary>
        /// <returns></returns>
        List<BodyTemplateSelectModel> GetInstalledBodyTemplates();

        /// <summary>
        /// Get body template content
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetBodyTemplateContent(int? id);

        /// <summary>
        /// Get chosen body template model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ChosenBodyTemplateModel GetChosenBodyTemplateModel(int id);

        #region Online Body Templates

        /// <summary>
        /// Search online body templates
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        BodyTemplateSelectorModel SearchBodyTemplates(string keyword, int? pageIndex, int? pageSize);

        /// <summary>
        /// Download online template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BodyTemplateDownloadModel GetOnlineTemplate(int id);

        /// <summary>
        /// Download and save online template
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        ResponseModel DownloadedOnlineTemplate(int id, string name);

        #endregion

        #endregion
    }
}