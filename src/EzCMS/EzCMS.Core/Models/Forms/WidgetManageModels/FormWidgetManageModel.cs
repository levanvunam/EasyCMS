using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.Forms;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Forms.WidgetManageModels
{
    public class FormWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public FormWidgetManageModel()
        {
        }

        public FormWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var formService = HostContainer.GetInstance<IFormService>();
            Forms = formService.GetForms();

            ParseParams(parameters);
        }

        #region Private Methods

        /// <summary>
        /// Parse params
        /// </summary>
        /// <param name="parameters"></param>
        private void ParseParams(string[] parameters)
        {
            //Form Id
            if (parameters.Length > 1)
            {
                FormId = parameters[1].ToInt();
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

        [LocalizedDisplayName("Widget_Form_Field_FormId")]
        public int FormId { get; set; }

        public IEnumerable<SelectListItem> Forms { get; set; }

        #endregion
    }
}
