using System.Data.Entity;
using Ez.Framework.Core.Entity.Extensions;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Enums;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;
using System.Linq;
using Ez.Framework.Core.Entity.Intialize;

namespace EzCMS.Core.Core.DataSetup
{
    public class ToolbarInitializer : IDataInitializer
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
        /// Initialize default languages
        /// </summary>
        public void Initialize(DbContext ezContext)
        {
            var context = ezContext as EzCMSEntities;
            if (context != null)
            {
                var dbContext = new EzCMSEntities();

                var availableTools = EnumUtilities.GetAllItems<EditorTool>().Select(t => t.ToString());
                var availableToolsJsonString = SerializeUtilities.Serialize(availableTools);
                var toolbar = new Toolbar
                {
                    Name = "Default Toolbar",
                    IsDefault = true,
                    BasicToolbar = availableToolsJsonString,
                    PageToolbar = availableToolsJsonString
                };

                dbContext.Toolbars.AddIfConditionInvalid(t => t.IsDefault, toolbar);
                dbContext.SaveChanges();
            }
        }

        #endregion
    }
}
