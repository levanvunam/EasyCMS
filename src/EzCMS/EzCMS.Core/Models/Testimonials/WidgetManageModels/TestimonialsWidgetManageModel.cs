using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Testimonials.WidgetManageModels
{
    public class TestimonialsWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public TestimonialsWidgetManageModel()
        {
        }

        public TestimonialsWidgetManageModel(string[] parameters)
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
             * * Number
             * * Template 
             */
            Number = DefaultRandomNumber;

            //Number
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

        [LocalizedDisplayName("Widget_Testimonials_Field_Number")]
        public int Number { get; set; }

        #endregion
    }
}
