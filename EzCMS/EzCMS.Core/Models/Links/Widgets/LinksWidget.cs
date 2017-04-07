using System.Collections.Generic;

namespace EzCMS.Core.Models.Links.Widgets
{
    public class LinksWidget
    {
        #region Constructors

        public LinksWidget()
        {
            Links = new List<LinkWidget>();
        }

        #endregion

        #region Public Properties

        public List<LinkWidget> Links { get; set; }

        #endregion
    }
}
