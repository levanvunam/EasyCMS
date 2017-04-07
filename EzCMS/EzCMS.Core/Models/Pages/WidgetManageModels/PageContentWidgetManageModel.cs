using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.Pages;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Pages.WidgetManageModels
{
    public class PageContentWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public PageContentWidgetManageModel()
        {
        }

        public PageContentWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var pageService = HostContainer.GetInstance<IPageService>();
            Pages = pageService.GetAccessablePageSelectList();

            ParseParams(parameters);
        }

        #region Private Methods

        /// <summary>
        /// Parse params
        /// </summary>
        /// <param name="parameters"></param>
        private void ParseParams(string[] parameters)
        {
            //Page Id
            if (parameters.Length > 1)
            {
                PageId = parameters[1].ToNullableInt();
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

        [Required]
        [LocalizedDisplayName("Widget_PageContent_Field_PageId")]
        public int? PageId { get; set; }

        public IEnumerable<SelectListItem> Pages { get; set; }

        #endregion
    }
}
