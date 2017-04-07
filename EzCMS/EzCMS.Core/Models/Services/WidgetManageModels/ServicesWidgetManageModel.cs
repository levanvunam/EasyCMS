using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Services.WidgetManageModels
{
    public class ServicesWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public ServicesWidgetManageModel()
        {
        }

        public ServicesWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            ParseParams(parameters);
        }

        #endregion

        #region Parse Params

        private const int DefaultRandomNumber = 5;

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Count
             * * Template 
             */

            //Count
            if (parameters.Length > 1)
            {
                Number = parameters[1].ToInt(DefaultRandomNumber);
            }

            //Template 
            if (parameters.Length > 2)
            {
                Template = parameters[2];
            }
        }
        #endregion

        #region Public Properties

        [LocalizedDisplayName("Widget_Services_Field_Number")]
        public int Number { get; set; }

        #endregion
    }
}
