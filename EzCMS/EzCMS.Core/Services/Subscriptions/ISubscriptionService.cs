using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Subscriptions;
using EzCMS.Core.Models.Subscriptions.Widgets;
using EzCMS.Core.Models.Subscriptions.Widgets.ContentUpdates;
using EzCMS.Core.Models.Subscriptions.Subscribers;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Subscriptions
{
    [Register(Lifetime.PerInstance)]
    public interface ISubscriptionService : IBaseService<Subscription>
    {
        ResponseModel Register(SubscriptionManageModel model);

        ResponseModel RemoveRegistration(SubscriptionManageModel model);

        ResponseModel TriggerSubscriptionAction(SubscriptionEnums.SubscriptionAction subscriptionAction,
            int subscriptionId);

        #region Background Tasks

        string GetSubscriptionEmailContent(IEnumerable<SubscribeModule> subscriberInfo,
            IEnumerable<SubscriptionLog> subscriptionLogs, SubscriptionTemplate template, ref bool isAnythingChanged);

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the subscription
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchSubscriptions(JqSearchIn si);

        /// <summary>
        /// Export the subscription
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Widgets

        /// <summary>
        /// Generate subscription widget
        /// </summary>
        /// <param name="module"></param>
        /// <param name="email"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        SubscriptionWidget GenerateSubscription(SubscriptionEnums.SubscriptionModule module, string email,
            dynamic parameters);

        /// <summary>
        /// Get content update for current user
        /// </summary>
        /// <returns></returns>
        ContentUpdateWidget GetContentUpdates();

        #endregion
    }
}