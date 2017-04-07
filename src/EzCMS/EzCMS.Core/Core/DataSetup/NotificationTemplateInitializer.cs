using System.Data.Entity;
using Ez.Framework.Core.Entity.Extensions;
using Ez.Framework.Core.Entity.Intialize;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Core.DataSetup
{
    public class NotificationTemplateInitializer : IDataInitializer
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
                var notificationTemplates = new[]
                {
                    new NotificationTemplate
                    {
                        Name = "Default Page Notification Template",
                        Subject = "Page Notification",
                        Body =
                            DataInitializeHelper.GetResourceContent("Default.PageNotification.cshtml",
                                DataSetupResourceType.NotificationTemplate),
                        IsDefaultTemplate = true,
                        Module = NotificationEnums.NotificationModule.Page
                    }
                };

                context.NotificationTemplates.AddIfNotExist(t => t.Name, notificationTemplates);
            }
        }

        #endregion
    }
}
