using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Subscriptions.ModuleParameters;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Subscriptions
{
    public class SubscriptionManageModel
    {
        #region Constructors

        public SubscriptionManageModel()
        {
            SubscriptionType = SubscriptionEnums.SubscriptionType.Midnight;
            Types = EnumUtilities.GenerateSelectListItems<SubscriptionEnums.SubscriptionType>();
        }

        public SubscriptionManageModel(Subscription subscription)
            : this()
        {
            Id = subscription.Id;
            Email = subscription.Email;
            Parameters = subscription.Parameters;
            Module = subscription.Module;
        }

        public SubscriptionManageModel(Page page)
            : this()
        {
            var parameterModel = new SubscriptionPageParameterModel(page);
            Parameters = SerializeUtilities.Serialize(parameterModel);

            Module = SubscriptionEnums.SubscriptionModule.Page;
            SubscriptionType = SubscriptionEnums.SubscriptionType.Midnight;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("Subscription_Field_Email")]
        public string Email { get; set; }

        [LocalizedDisplayName("Subscription_Field_ContactId")]
        public int? ContactId { get; set; }

        [LocalizedDisplayName("Subscription_Field_Parameters")]
        public string Parameters { get; set; }

        [LocalizedDisplayName("Subscription_Field_Type")]
        public SubscriptionEnums.SubscriptionType SubscriptionType { get; set; }

        public IEnumerable<SelectListItem> Types { get; set; }

        [LocalizedDisplayName("Subscription_Field_Module")]
        public SubscriptionEnums.SubscriptionModule Module { get; set; }

        #endregion
    }
}
