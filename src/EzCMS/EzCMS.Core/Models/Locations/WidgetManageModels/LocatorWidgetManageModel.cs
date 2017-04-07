using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;

namespace EzCMS.Core.Models.Locations.WidgetManageModels
{
    public class LocatorWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public LocatorWidgetManageModel()
        {
        }

        public LocatorWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            ParseParams(parameters);
        }

        #region Private Methods

        /// <summary>
        /// Parse params
        /// </summary>
        /// <param name="parameters"></param>
        private void ParseParams(string[] parameters)
        {
            /*
            * Params:
            * * Country
            * * Template 
            */

            // Country
            if (parameters.Length > 1)
            {
                Country = parameters[1];
            }

            // Template 
            if (parameters.Length > 2)
            {
                Template = parameters[2];
            }
        }

        #endregion

        #endregion

        #region Public Properties

        [LocalizedDisplayName("Widget_Location_Field_LocatorCountry")]
        public string Country { get; set; }

        #endregion
    }
}
