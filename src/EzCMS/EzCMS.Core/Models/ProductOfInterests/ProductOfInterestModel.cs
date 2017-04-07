using Ez.Framework.Models;

namespace EzCMS.Core.Models.ProductOfInterests
{
    public class ProductOfInterestModel : BaseGridModel
    {
        #region Public Properties

        public string Name { get; set; }

        public int? TargetCount { get; set; }

        #endregion
    }
}
