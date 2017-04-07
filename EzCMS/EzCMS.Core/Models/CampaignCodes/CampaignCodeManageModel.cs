using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.CampaignCodes
{
    public class CampaignCodeManageModel
    {
        #region Constructors

        public CampaignCodeManageModel()
        {
        }

        public CampaignCodeManageModel(CampaignCode campaignCode)
            : this()
        {
            Id = campaignCode.Id;
            Name = campaignCode.Name;
            Description = campaignCode.Description;
            TargetCount = campaignCode.TargetCount;
            RecordOrder = campaignCode.RecordOrder;
        }

        #endregion

        #region Public Properties
        public int? Id { get; set; }

        [LocalizedDisplayName("CampaignCode_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("CampaignCode_Field_Description")]
        public string Description { get; set; }

        [LocalizedDisplayName("CampaignCode_Field_TargetCount")]
        public int? TargetCount { get; set; }

        [LocalizedDisplayName("CampaignCode_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion
    }
}
