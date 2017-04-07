using Ez.Framework.Models;

namespace EzCMS.Core.Models.CampaignCodes
{
    public class CampaignCodeModel : BaseGridModel
    {
        #region Public Properties

        public string Name { get; set; }

        public string Description { get; set; }

        public int? TargetCount { get; set; }

        #endregion
    }
}
