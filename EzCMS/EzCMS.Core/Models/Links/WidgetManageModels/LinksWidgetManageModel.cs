using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.LinkTypes;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Links.WidgetManageModels
{
    public class LinksWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public LinksWidgetManageModel()
        {
        }

        public LinksWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var linkTypeService = HostContainer.GetInstance<ILinkTypeService>();
            LinkTypes = linkTypeService.GetLinkTypes();

            ParseParams(parameters);
        }

        #endregion

        #region Parse Params

        /// <summary>
        /// Parse parameters
        /// </summary>
        /// <param name="parameters"></param>
        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * LinkTypeId
             * * Number
             * * Template 
             */

            //LinkTypeId
            if (parameters.Length > 1)
            {
                LinkTypeId = parameters[1].ToInt(0);
            }

            //Number
            if (parameters.Length > 2)
            {
                Number = parameters[2].ToInt(0);
            }

            //Template 
            if (parameters.Length > 3)
            {
                Template = parameters[3];
            }
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("Widget_Links_Field_LinkTypeId")]
        public int LinkTypeId { get; set; }

        public IEnumerable<SelectListItem> LinkTypes { get; set; }

        [LocalizedDisplayName("Widget_Links_Field_Number")]
        public int Number { get; set; }

        #endregion
    }
}
