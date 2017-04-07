using System.Collections.Generic;

namespace EzCMS.Core.Models.Associates.Widgets
{
    public class AssociateListingWidget
    {
        #region Constructors

        public AssociateListingWidget()
        {
            AssociateTypes = new List<AssociateTypeWidget>();
            Associates = new List<AssociateWidget>();
        }

        #endregion

        #region Public Properties

        public List<AssociateTypeWidget> AssociateTypes { get; set; }

        public List<AssociateWidget> Associates { get; set; }

        #endregion
    }
}
