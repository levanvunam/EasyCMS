using System.Collections.Generic;

namespace EzCMS.Core.Models.BodyTemplates.HelpServices
{
    public class BodyTemplateResponseModel
    {
        #region Public Properties

        public List<BodyTemplateSelectModel> BodyTemplates { get; set; }

        public int Total { get; set; }

        public bool HasMore { get; set; }

        #endregion
    }
}
