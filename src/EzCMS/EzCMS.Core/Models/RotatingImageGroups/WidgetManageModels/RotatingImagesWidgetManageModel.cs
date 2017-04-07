using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.RotatingImageGroups;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.RotatingImageGroups.WidgetManageModels
{
    public class RotatingImagesWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public RotatingImagesWidgetManageModel()
        {
        }

        public RotatingImagesWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var rotatingImageGroupService = HostContainer.GetInstance<IRotatingImageGroupService>();
            Groups = rotatingImageGroupService.GetRotatingImageGroups();

            ParseParams(parameters);
        }

        #endregion

        #region Parse Params

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Group Id
             * * Template 
             */

            //Count
            if (parameters.Length > 1)
            {
                GroupId = parameters[1].ToInt(0);
            }

            //Template 
            if (parameters.Length > 2)
            {
                Template = parameters[2];
            }
        }

        #endregion

        #region Public Properties

        [RequiredInteger("Widget_RotatingImages_Field_GroupId")]
        [LocalizedDisplayName("Widget_RotatingImages_Field_GroupId")]
        public int GroupId { get; set; }

        public IEnumerable<SelectListItem> Groups { get; set; }

        #endregion
    }
}
