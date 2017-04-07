namespace EzCMS.Core.Models.Pages
{
    public class PageOrderSetupModel
    {
        #region Public Properties

        public int CurrentId { get; set; }

        public int? ParentId { get; set; }

        public int? PreviousId { get; set; }

        public int? NextId { get; set; }

        #endregion
    }
}
