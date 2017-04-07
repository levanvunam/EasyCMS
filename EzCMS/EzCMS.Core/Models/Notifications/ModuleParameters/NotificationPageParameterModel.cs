using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Notifications.ModuleParameters
{
    public class NotificationPageParameterModel
    {
        public NotificationPageParameterModel()
        {

        }

        public NotificationPageParameterModel(Page page)
            : this()
        {
            Id = page.Id;
        }

        #region Public Properties

        public int Id { get; set; }

        #endregion
    }
}
