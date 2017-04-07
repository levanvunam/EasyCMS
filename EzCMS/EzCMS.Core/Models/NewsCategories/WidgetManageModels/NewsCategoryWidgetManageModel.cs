using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.NewsCategories;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.NewsCategories.WidgetManageModels
{
    public class NewsCategoryWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public NewsCategoryWidgetManageModel()
        {
        }

        public NewsCategoryWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var newsCategoryService = HostContainer.GetInstance<INewsCategoryService>();
            NewsCategories = newsCategoryService.GetNewsCategories();

            ParseParams(parameters);
        }

        #region Private Methods

        /// <summary>
        /// Parse params
        /// </summary>
        /// <param name="parameters"></param>
        private void ParseParams(string[] parameters)
        {
            //Category Id
            if (parameters.Length > 1)
            {
                CategoryId = parameters[1].ToInt(0);
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

        [LocalizedDisplayName("Widget_NewsCategoryDetails_Field_CategoryId")]
        public int? CategoryId { get; set; }

        public IEnumerable<SelectListItem> NewsCategories { get; set; }

        #endregion
    }
}
