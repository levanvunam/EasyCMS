using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.NewsCategories;
using EzCMS.Entity.Core.Enums;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.News.WidgetManageModels
{
    public class NewsListingWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public NewsListingWidgetManageModel()
        {
        }

        public NewsListingWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var newsCategoryService = HostContainer.GetInstance<INewsCategoryService>();
            NewsCategories = newsCategoryService.GetNewsCategories();
            NewsTypes = EnumUtilities.GenerateSelectListItems<NewsEnums.NewsType>();
            NewsStatusList = EnumUtilities.GenerateSelectListItems<NewsEnums.NewsStatus>();

            ParseParams(parameters);
        }

        #endregion

        #region Parse Params

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * NewsCategoryId
             * * Total
             * * PageSize
             * * NewsType
             * * Template 
             */

            //NewsCategoryId
            if (parameters.Length > 1)
            {
                NewsCategoryId = parameters[1].ToInt(0);
            }

            //Total
            if (parameters.Length > 2)
            {
                Total = parameters[2].ToInt(0);
            }

            //PageSize
            if (parameters.Length > 3)
            {
                PageSize = parameters[3].ToInt(0);
            }

            //NewsType
            if (parameters.Length > 4)
            {
                NewsType = parameters[4].ToInt((int)NewsEnums.NewsType.All);
            }

            //News Status
            if (parameters.Length > 5)
            {
                NewsStatus = parameters[5].ToInt((int)NewsEnums.NewsStatus.Active);
            }

            //Template 
            if (parameters.Length > 6)
            {
                Template = parameters[6];
            }
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("Widget_NewsListing_Field_NewsCategoryId")]
        public int? NewsCategoryId { get; set; }

        public IEnumerable<SelectListItem> NewsCategories { get; set; }

        [LocalizedDisplayName("Widget_NewsListing_Field_Total")]
        public int Total { get; set; }

        [LocalizedDisplayName("Widget_NewsListing_Field_PageSize")]
        public int PageSize { get; set; }

        [LocalizedDisplayName("Widget_NewsListing_Field_NewsType")]
        public int NewsType { get; set; }

        public IEnumerable<SelectListItem> NewsTypes { get; set; }

        [LocalizedDisplayName("Widget_NewsListing_Field_NewsStatus")]
        public int NewsStatus { get; set; }

        public IEnumerable<SelectListItem> NewsStatusList { get; set; }
        #endregion
    }
}
