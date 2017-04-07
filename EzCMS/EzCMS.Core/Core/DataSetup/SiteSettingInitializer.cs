using System.Data.Entity;
using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.Entity.Enums;
using Ez.Framework.Core.Entity.Extensions;
using Ez.Framework.Core.Entity.Intialize;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Entity.Entities;

namespace EzCMS.Core.Core.DataSetup
{
    public class SiteSettingInitializer : IDataInitializer
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
                var initializeSettings = new[]
                {
                    new SiteSetting
                    {
                        Name = SettingNames.WidgetMaxLoop,
                        FieldName = "Max Loop",
                        Value = "5",
                        EditorType = SettingEnums.EditorType.Number,
                        SettingType = "system"
                    },
                    new SiteSetting
                    {
                        Name = SettingNames.AutoCompleteSize,
                        Description = "Auto complete size",
                        FieldName = "Size",
                        Value = "10",
                        EditorType = SettingEnums.EditorType.Number,
                        SettingType = "backend"
                    },
                    new SiteSetting
                    {
                        Name = SettingNames.DefaultGridPageSize,
                        Description = "Default grid page size in Admin module",
                        FieldName = "Grid Page Size",
                        Value = "50",
                        EditorType = SettingEnums.EditorType.Number,
                        SettingType = "backend"
                    },
                    new SiteSetting
                    {
                        Name = SettingNames.PhoneFormat,
                        Description = "Phone format",
                        FieldName = "Phone Format",
                        Value = "(999) 999-9999",
                        EditorType = SettingEnums.EditorType.Text,
                        SettingType = "backend"
                    },
                    new SiteSetting
                    {
                        Name = SettingNames.KeepAliveInterval,
                        Description = "Keep alive interval time (in second)",
                        FieldName = "Interval (second)",
                        Value = "60",
                        EditorType = SettingEnums.EditorType.Number,
                        SettingType = "system"
                    },
                    new SiteSetting
                    {
                        Name = SettingNames.LogsPageSize,
                        FieldName = "Log Size",
                        Value = "10",
                        EditorType = SettingEnums.EditorType.Number,
                        SettingType = "system"
                    },
                    new SiteSetting
                    {
                        Name = SettingNames.MaxSizeUploaded,
                        Description = "Max uploaded file size",
                        FieldName = "Max File Size",
                        Value = "10485760",
                        EditorType = SettingEnums.EditorType.Number,
                        SettingType = "system"
                    },
                    new SiteSetting
                    {
                        Name = SettingNames.EmailLogMaxTries,
                        FieldName = "Max Trires",
                        Value = "5",
                        EditorType = SettingEnums.EditorType.Number,
                        SettingType = "system"
                    },
                    new SiteSetting
                    {
                        Name = SettingNames.WordDocUploadsFolderPhysicalPath,
                        FieldName = "Word Doc Uploads Directory",
                        Value = "/Media/WordDocUploads",
                        EditorType = SettingEnums.EditorType.Text,
                        SettingType = "system"
                    },
                    new SiteSetting
                    {
                        Name = SettingNames.ProtectedDocumentEmailTo,
                        FieldName = "Email To",
                        Value = string.Empty,
                        EditorType = SettingEnums.EditorType.Text,
                        SettingType = "system"
                    },
                    new SiteSetting
                    {
                        Name = SettingNames.GoogleAnalyticConfiguration,
                        Description = "Google Analytic Tracking Script",
                        EditorType = SettingEnums.EditorType.TextArea,
                        FieldName = "Tracking Script",
                        Value = string.Empty,
                        SettingType = "system"
                    },
                    new SiteSetting
                    {
                        Name = SettingNames.EnableLiveSiteMode,
                        Description = "Enable / Disable live site mode",
                        EditorType = SettingEnums.EditorType.Checkbox,
                        FieldName = "Site Live",
                        Value = "false",
                        SettingType = "system"
                    },
                    new SiteSetting
                    {
                        Name = SettingNames.WebApiAuthorizeCodeConfiguration,
                        Description = "Web Api Authorize Code Configuration",
                        EditorType = SettingEnums.EditorType.Text,
                        FieldName = "Authorize Code",
                        Value = "",
                        SettingType = "system"
                    }
                };

                context.SiteSettings.AddIfNotExist(i => i.Name, initializeSettings);
            }
        }

        #endregion
    }
}
