using Ez.Framework.Core.Attributes;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.Banners;
using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Banners.WidgetManageModels
{
    public class BannerWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public BannerWidgetManageModel()
        {
        }

        public BannerWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var bannerService = HostContainer.GetInstance<IBannerService>();
            Banners = bannerService.GetBanners();

            ParseParams(parameters);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parse params
        /// </summary>
        /// <param name="parameters"></param>
        private void ParseParams(string[] parameters)
        {
            //Banner Id
            if (parameters.Length > 1)
            {
                BannerId = parameters[1].ToInt();
            }

            //Template 
            if (parameters.Length > 2)
            {
                Template = parameters[2];
            }
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("Widget_Banner_Field_BannerId")]
        public int BannerId { get; set; }

        public IEnumerable<SelectListItem> Banners { get; set; }

        #endregion
    }
}
