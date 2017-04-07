using Ez.Framework.IoC;
using EzCMS.Core.Services.SiteSettings;
using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Models.SiteSettings
{
    public class SiteSettingSearchModel
    {
        public SiteSettingSearchModel()
        {
            var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();

            SettingTypeNames = new List<string>();
            SettingTypes = siteSettingService.GetSettingTypes();
        }

        #region Public Properties

        public string Keyword { get; set; }

        public List<string> SettingTypeNames { get; set; }

        public IEnumerable<SelectListItem> SettingTypes { get; set; }

        #endregion
    }
}