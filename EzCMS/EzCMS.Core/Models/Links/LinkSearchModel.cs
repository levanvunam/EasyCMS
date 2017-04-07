using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.LinkTypes;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Links
{
    public class LinkSearchModel
    {
        #region Constructors

        public LinkSearchModel()
        {
            var linkTypeServive = HostContainer.GetInstance<ILinkTypeService>();
            LinkTypes = linkTypeServive.GetLinkTypes();
        }

        #endregion

        #region Public Properties

        public string Keyword { get; set; }

        public int? LinkTypeId { get; set; }

        public IEnumerable<SelectListItem> LinkTypes { get; set; }

        #endregion
    }
}
