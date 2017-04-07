using System.Collections.Generic;

namespace EzCMS.Core.Models.Users.Settings
{
    public class UserSettingModel
    {
        public UserSettingModel()
        {
            AdminPageSize = 50;
            GridSettingItems = new List<GridSettingItem>();
        }

        #region Public Properties

        public int AdminPageSize { get; set; }

        public string TimeZone { get; set; }

        public string Culture { get; set; }

        public List<GridSettingItem> GridSettingItems { get; set; }

        #endregion
    }

    public class GridSettingItem
    {
        #region Public Properties

        public string Name { get; set; }

        public int[] Order { get; set; }

        public List<ColumnSetting> Columns { get; set; }

        #endregion
    }

    public class ColumnSetting
    {
        #region Public Properties

        public string Name { get; set; }

        public bool Show { get; set; }

        #endregion
    }
}
