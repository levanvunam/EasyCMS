using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.ProductOfInterests
{
    public class ProductOfInterestManageModel
    {
        #region Constructors

        public ProductOfInterestManageModel()
        {
        }

        public ProductOfInterestManageModel(ProductOfInterest productOfInterest)
            : this()
        {
            Id = productOfInterest.Id;
            Name = productOfInterest.Name;
            TargetCount = productOfInterest.TargetCount;
            RecordOrder = productOfInterest.RecordOrder;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("ProductOfInterest_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("ProductOfInterest_Field_TargetCount")]
        public int? TargetCount { get; set; }

        [LocalizedDisplayName("ProductOfInterest_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion
    }
}
