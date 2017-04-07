namespace Ez.Framework.Core.SiteSettings.Models
{
    public class ComplexSettingSetup
    {
        #region Public Properties

        public string SettingType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public dynamic DefaultValue { get; set; }

        #endregion
    }
}