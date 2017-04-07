using System.Collections.Generic;

namespace EzCMS.Core.Models.Associates.Widgets
{
    public class AssociateTypeWidget
    {
        #region Constructors

        public AssociateTypeWidget()
        {
            Associates = new List<AssociateWidget>();
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public List<AssociateWidget> Associates { get; set; }

        #endregion
    }
}