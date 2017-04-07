using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.SubscriptionTemplates;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.SubscriptionTemplates
{
    [Register(Lifetime.PerInstance)]
    public interface ISubscriptionTemplateService : IBaseService<SubscriptionTemplate>
    {
        SubscriptionTemplateResponseModel ParseSubscription<TModel>(SubscriptionEnums.SubscriptionModule module,
            TModel model);

        /// <summary>
        /// Parse subscription
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        SubscriptionTemplateResponseModel ParseSubscription<TModel>(SubscriptionTemplate template, TModel model);

        /// <summary>
        /// Get notification email model assembly name
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        string GetNotificationEmailModelAssemblyName(SubscriptionEnums.SubscriptionModule module);

        #region Grid

        /// <summary>
        /// Search the subscription template
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchSubscriptionTemplates(JqSearchIn si);

        /// <summary>
        /// Export the subscription template
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        SubscriptionTemplateManageModel GetSubscriptionTemplateManageModel(int? id = null);

        ResponseModel SaveSubscriptionTemplate(SubscriptionTemplateManageModel model);

        #endregion
    }
}