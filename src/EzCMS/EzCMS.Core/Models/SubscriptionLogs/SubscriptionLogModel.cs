using Ez.Framework.Models;
using EzCMS.Entity.Core.Enums;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.SubscriptionLogs
{
    public class SubscriptionLogModel : BaseGridModel
    {
        #region Public Properties

        public string Parameters { get; set; }

        public SubscriptionEnums.SubscriptionModule Module { get; set; }

        public string ModuleName
        {
            get { return Module.GetEnumName(); }
        }

        public string ChangeLog { get; set; }

        public bool Active { get; set; }

        #endregion
    }
}
