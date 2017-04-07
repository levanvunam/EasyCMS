using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.ClientNavigations;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.ClientNavigations.WidgetManageModels
{
    public class NavigationsWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public NavigationsWidgetManageModel()
        {
        }

        public NavigationsWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var clientNavigationService = HostContainer.GetInstance<IClientNavigationService>();
            Parents = clientNavigationService.GetPossibleParents();

            ParseParams(parameters);
        }

        #endregion

        #region Parse Params

        /// <summary>
        /// Parse params from parameters
        /// </summary>
        /// <param name="parameters"></param>
        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Parent Id
             * * Show Parent
             * * Parent Template 
             */

            //ParentId
            if (parameters.Length > 1 && !string.IsNullOrEmpty(parameters[1]))
            {
                ParentId = parameters[1].ToNullableInt();
            }

            //ShowParentNavigation
            if (parameters.Length > 2 && !string.IsNullOrEmpty(parameters[2]) && ParentId.HasValue)
            {
                ShowParentNavigation = parameters[2].ToBool(false);
            }

            //TemplateName
            if (parameters.Length > 3 && !string.IsNullOrEmpty(parameters[3]))
            {
                Template = parameters[3];
            }

        }
        #endregion

        #region Public Properties

        [LocalizedDisplayName("Widget_Navigations_Field_ParentId")]
        public int? ParentId { get; set; }

        public IEnumerable<SelectListItem> Parents { get; set; }

        [LocalizedDisplayName("Widget_Navigations_Field_ShowParentNavigation")]
        public bool ShowParentNavigation { get; set; }

        #endregion
    }
}
