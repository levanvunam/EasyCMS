using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Core.Enums;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Subscriptions.Widgets
{
    public class SubscriptionWidget
    {
        #region Constructors

        public SubscriptionWidget()
        {
            Type = SubscriptionEnums.SubscriptionType.Midnight;
            Types = EnumUtilities.GenerateSelectListItems<SubscriptionEnums.SubscriptionType>();
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("Subscription_Field_Email")]
        public string Email { get; set; }

        [LocalizedDisplayName("Subscription_Field_Parameters")]
        public string Parameters { get; set; }

        [LocalizedDisplayName("Subscription_Field_Type")]
        public SubscriptionEnums.SubscriptionType Type { get; set; }

        public IEnumerable<SelectListItem> Types { get; set; }

        [LocalizedDisplayName("Subscription_Field_Module")]
        public SubscriptionEnums.SubscriptionModule Module { get; set; }

        #endregion
    }
}
