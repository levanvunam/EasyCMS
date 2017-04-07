using Ez.Framework.Core.Entity.Intialize;
using System.Data.Entity;

namespace EzCMS.Help.Plugins.Business.DataSetup
{
    public class HelpSiteSettingInitializer : IDataInitializer
    {
        /// <summary>
        /// Priority of initializer
        /// </summary>
        /// <returns></returns>
        public DataInitializerPriority Priority()
        {
            return DataInitializerPriority.Medium;
        }

        #region Initialize

        /// <summary>
        /// Initialize default settings
        /// </summary>
        public void Initialize(DbContext context)
        {
        }

        #endregion
    }
}
