using Ez.Framework.Models;
using Ez.Framework.Utilities;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.SubscriptionTemplates
{
    public class SubscriptionTemplateModel : BaseGridModel
    {
        #region Constructors
        public SubscriptionTemplateModel()
        {

        }

        public SubscriptionTemplateModel(SubscriptionTemplate subscriptionTemplate)
            : this()
        {
            Id = subscriptionTemplate.Id;
            Module = subscriptionTemplate.Module;
            Name = subscriptionTemplate.Name;

            RecordOrder = subscriptionTemplate.RecordOrder;
            Created = subscriptionTemplate.Created;
            CreatedBy = subscriptionTemplate.CreatedBy;
            LastUpdate = subscriptionTemplate.LastUpdate;
            LastUpdateBy = subscriptionTemplate.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public string Name { get; set; }

        public SubscriptionEnums.SubscriptionModule Module { get; set; }

        public string ModuleName
        {
            get { return Module.GetEnumName(); }
        }

        #endregion
    }
}
