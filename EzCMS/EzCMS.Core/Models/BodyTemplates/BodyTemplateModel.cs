using Ez.Framework.Models;

namespace EzCMS.Core.Models.BodyTemplates
{
    public class BodyTemplateModel : BaseGridModel
    {
        #region Public Properties

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        #endregion
    }
}
