using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Subscriptions.ModuleParameters
{
    public class SubscriptionPageParameterModel
    {
        public SubscriptionPageParameterModel()
        {

        }

        public SubscriptionPageParameterModel(Page page)
            : this()
        {
            Id = page.Id;
        }

        #region Public Properties

        public int Id { get; set; }

        #endregion
    }
}
