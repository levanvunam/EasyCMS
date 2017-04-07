using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.News;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.News.WidgetManageModels
{
    public class NewsWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public NewsWidgetManageModel()
        {
        }

        public NewsWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var newsService = HostContainer.GetInstance<INewsService>();
            NewsList = newsService.GetNewsList();

            ParseParams(parameters);
        }

        #region Private Methods

        /// <summary>
        /// Parse params
        /// </summary>
        /// <param name="parameters"></param>
        private void ParseParams(string[] parameters)
        {
            //News Id
            if (parameters.Length > 1)
            {
                NewsId = parameters[1].ToInt(0);
            }

            //Template 
            if (parameters.Length > 2)
            {
                Template = parameters[2];
            }
        }

        #endregion


        #endregion

        #region Public Properties

        [LocalizedDisplayName("Widget_NewsDetails_Field_NewsId")]
        public int? NewsId { get; set; }

        public IEnumerable<SelectListItem> NewsList { get; set; }

        #endregion
    }
}
