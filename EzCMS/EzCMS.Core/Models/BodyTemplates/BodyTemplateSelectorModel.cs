using System.Collections.Generic;
using Ez.Framework.Core.Enums;
using EzCMS.Core.Framework.Enums;

namespace EzCMS.Core.Models.BodyTemplates
{
    public class BodyTemplateSelectorModel
    {
        #region Public Properties

        public List<BodyTemplateSelectModel> BodyTemplates { get; set; }

        public bool EnableOnlineTemplate { get; set; }

        public BodyTemplateEnums.Mode Mode { get; set; }

        #endregion
    }
}
