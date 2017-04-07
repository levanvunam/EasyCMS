using Ez.Framework.Models;

namespace EzCMS.Core.Models.SiteSettings
{
    public class SiteSettingModel : BaseGridModel
    {
        #region Public Properties

        public string Name { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }

        public string SettingType { get; set; }

        #endregion
    }
}
