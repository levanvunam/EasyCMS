using EzCMS.Core.Models.Subscriptions.ModuleParameters;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.Utilities;
using System;
using Ez.Framework.Models;

namespace EzCMS.Core.Models.Subscriptions
{
    public class SubscriptionModel : BaseGridModel
    {
        public SubscriptionModel()
        {
        }

        public SubscriptionModel(Subscription subscription)
            : this()
        {
            Id = subscription.Id;
            Email = subscription.Email;
            Module = subscription.Module;
            switch (Module)
            {
                case SubscriptionEnums.SubscriptionModule.Page:
                    Parameters = SerializeUtilities.Deserialize<SubscriptionPageParameterModel>(subscription.Parameters);
                    break;
            }

            DeactivatedDate = subscription.DeactivatedDate;

            RecordOrder = subscription.RecordOrder;
            Created = subscription.Created;
            CreatedBy = subscription.CreatedBy;
            LastUpdate = subscription.LastUpdate;
            LastUpdateBy = subscription.LastUpdateBy;
        }

        #region Public Properties

        public string Email { get; set; }

        public SubscriptionEnums.SubscriptionModule Module { get; set; }

        public dynamic Parameters { get; set; }

        public DateTime? DeactivatedDate { get; set; }

        public string ModuleName
        {
            get { return Module.GetEnumName(); }
        }
        #endregion
    }
}
