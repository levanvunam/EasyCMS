using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Pages
{
    public class PageSecurityModel
    {
        public PageSecurityModel()
        {
            CanView = false;
            CanEdit = false;
        }

        public PageSecurityModel(UserGroup group): this()
        {
            GroupId = group.Id;
            GroupName = group.Name;
        }

        #region Public Properties

        public int Id { get; set; }

        public int PageId { get; set; }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public bool CanView { get; set; }

        public bool CanEdit { get; set; }

        #endregion
    }
}
