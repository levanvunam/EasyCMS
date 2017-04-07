using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Services;
using EzCMS.Core.Models.Services.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Services
{
    [Register(Lifetime.PerInstance)]
    public interface IServiceService : IBaseService<Service>
    {
        /// <summary>
        /// Get service status
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetStatus();

        /// <summary>
        /// Check if service exists.
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        bool IsNameExisted(int? serviceId, string title);

        /// <summary>
        /// Get services
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        List<ServiceWidget> GetServices(int number);

        /// <summary>
        /// Get service detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ServiceDetailModel GetServiceDetailModel(int id);

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateServiceData(XEditableModel model);

        #region Grid Search

        /// <summary>
        /// Search service
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchService(JqSearchIn si);

        /// <summary>
        /// Export service
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get service manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ServiceManageModel GetServiceManageModel(int? id = null);

        /// <summary>
        /// Save service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveServiceManageModel(ServiceManageModel model);

        /// <summary>
        /// Delete service
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteService(int id);

        #endregion
    }
}