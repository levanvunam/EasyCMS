using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.Pages;
using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Pages.WidgetManageModels
{
    public class PageLinkWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public PageLinkWidgetManageModel()
        {
        }

        public PageLinkWidgetManageModel(string[] parameters)
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
                PageId = parameters[1].ToInt();
            }

            //RenderHtmlLink
            if (parameters.Length > 2)
            {
                RenderHtmlLink = parameters[2].ToBool();
            }

            //ClassName
            if (parameters.Length > 3)
            {
                ClassName = parameters[3];
            }

            //Label
            if (parameters.Length > 4)
            {
                Title = parameters[4];
            }

            //Template 
            if (parameters.Length > 5)
            {
                Template = parameters[5];
            }
        }

        #endregion


        #endregion

        #region Public Properties

        [RequiredInteger("Widget_PageLink_Field_PageId")]
        [LocalizedDisplayName("Widget_PageLink_Field_PageId")]
        public int PageId { get; set; }

        public IEnumerable<SelectListItem> Pages { get; set; }

        [LocalizedDisplayName("Widget_PageLink_Field_RenderHtmlLink")]
        public bool RenderHtmlLink { get; set; }

        [LocalizedDisplayName("Widget_PageLink_Field_ClassName")]
        public string ClassName { get; set; }

        [LocalizedDisplayName("Widget_PageLink_Field_Title")]
        public string Title { get; set; }

        #endregion
    }
}
