using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.ProtectedDocuments.WidgetManageModels
{
    public class ProtectedDocumentFilesWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public ProtectedDocumentFilesWidgetManageModel()
        {
        }

        public ProtectedDocumentFilesWidgetManageModel(string[] parameters)
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
             * * Total
             * * Template 
             */

            //RelativePath
            if (parameters.Length > 1)
            {
                Path = parameters[1];
            }

            //Total
            if (parameters.Length > 2)
            {
                Total = parameters[2].ToInt(0);
            }

            //Template 
            if (parameters.Length > 3)
            {
                Template = parameters[3];
            }


        }
        #endregion

        #region Public Properties

        [LocalizedDisplayName("Widget_ProtectedDocumentFiles_Field_Path")]
        public string Path { get; set; }

        [LocalizedDisplayName("Widget_ProtectedDocumentFiles_Field_Total")]
        public int Total { get; set; }

        #endregion
    }
}
