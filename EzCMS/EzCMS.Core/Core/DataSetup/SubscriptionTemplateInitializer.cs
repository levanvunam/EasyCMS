using System.Data.Entity;
using Ez.Framework.Core.Entity.Extensions;
using Ez.Framework.Core.Entity.Intialize;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Core.DataSetup
{
    public class SubscriptionTemplateInitializer : IDataInitializer
    {
        /// <summary>
        /// Priority of initializer
        /// </summary>
        /// <returns></returns>
        public DataInitializerPriority Priority()
        {
            return DataInitializerPriority.High;
        }

        #region Initialize

        /// <summary>
        /// Initialize default settings
        /// </summary>
        public void Initialize(DbContext ezContext)
        {
            var context = ezContext as EzCMSEntities;
            if (context != null)
            {
                var subscriptionTemplates = new[]
                {
                    new SubscriptionTemplate
                    {
                        Name = SubscriptionEnums.SubscriptionModule.Page.GetEnumName(),
                        Module = SubscriptionEnums.SubscriptionModule.Page,
                        Body = DataInitializeHelper.GetResourceContent("PageSubscribeNotification.cshtml",
                            DataSetupResourceType.SubscriptionTemplate)
                    }
                };

                context.SubscriptionTemplates.AddIfNotExist(t => t.Name, subscriptionTemplates);
            }
        }

        #endregion
    }
}
