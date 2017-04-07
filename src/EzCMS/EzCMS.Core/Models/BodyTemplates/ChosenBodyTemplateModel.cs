using System.Collections.Generic;
using System.Web.Mvc;

namespace EzCMS.Core.Models.BodyTemplates
{
    public class ChosenBodyTemplateModel
    {
        #region Public Properties

        public string Content { get; set; }

        public IEnumerable<SelectListItem> BodyTemplates { get; set; }

        #endregion
    }
}
