using Ez.Framework.Core.IoC;
using Ez.Framework.Utilities.Html;
using EzCMS.Core.Services.CampaignCodes;
using EzCMS.Core.Services.ProductOfInterests;

namespace EzCMS.Core.Models.Forms.Render
{
    public class FormElementsModel
    {
        public FormElementsModel()
        {
            var campaignCodeService = HostContainer.GetInstance<ICampaignCodeService>();
            var productOfInterestService = HostContainer.GetInstance<IProductOfInterestService>();

            var campaignCodes = campaignCodeService.GetCampaignCodes();
            CampaignCode = campaignCodes.BuildOptionsHtml();

            var productOfInterests = productOfInterestService.GetProductOfInterests();
            ProductOfInterest = productOfInterests.BuildOptionsHtml();
        }

        #region Public Properties

        public string CampaignCode { get; set; }

        public string ProductOfInterest { get; set; }

        public string SubscriberType { get; set; }

        #endregion
    }
}