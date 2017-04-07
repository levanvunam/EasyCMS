using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;

namespace EzCMS.Core.Models.ProtectedDocuments.WidgetManageModels
{
    public class ProtectedDocumentWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public ProtectedDocumentWidgetManageModel()
        {
        }

        public ProtectedDocumentWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            ParseParams(parameters);
        }

        #endregion

        #region Parse Params

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Relative Path
             * * Template 
             */

            //Count
            if (parameters.Length > 1)
            {
                Path = parameters[1];
            }

            //Template 
            if (parameters.Length > 2)
            {
                Template = parameters[2];
            }
        }
        #endregion

        #region Public Properties

        [LocalizedDisplayName("Widget_ProtectedDocuments_Field_Path")]
        public string Path { get; set; }

        #endregion
    }
}
