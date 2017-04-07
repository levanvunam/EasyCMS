using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Framework.SiteSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class DocumentTypeMappingSetting : ComplexSetting
    {
        #region Setup

        /// <summary>
        /// Get complex setting setup
        /// </summary>
        /// <returns></returns>
        public override ComplexSettingSetup GetSetup()
        {
            var mappingsContent = DataInitializeHelper.GetResourceContent("ProtectedDocuments.DocumentTypeMappings.csv", DataSetupResourceType.SiteSetting);

            List<string> countryList = mappingsContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var mappings = new List<DocumentClassMapping>();
            foreach (var item in countryList)
            {
                var parts = item.Split(',');
                if (parts.Count() < 2)
                    continue;
                var mapping = new DocumentClassMapping
                {
                    Extension = parts[0].Trim(),
                    ClassName = parts[1].Trim()
                };

                mappings.Add(mapping);
            }

            return new ComplexSettingSetup
            {
                Name = SettingNames.DocumentTypeMapping,
                SettingType = "system",
                DefaultValue = new DocumentTypeMappingSetting
                {
                    Mappings = mappings
                }
            };
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SiteSetting_DocumentTypeMappingSetting_Field_Mappings")]
        public List<DocumentClassMapping> Mappings { get; set; }

        #endregion
    }

    public class DocumentClassMapping
    {
        public string Extension { get; set; }

        public string ClassName { get; set; }
    }
}
