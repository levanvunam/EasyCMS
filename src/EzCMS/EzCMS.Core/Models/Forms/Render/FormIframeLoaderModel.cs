using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Forms.Render
{
    public class FormIframeLoaderModel : FormRenderModel
    {
        public FormIframeLoaderModel()
        {

        }

        public FormIframeLoaderModel(Form form, bool formSubmitted = false)
            : base(form)
        {
            FormSubmitted = formSubmitted;
        }

        #region Public Properties

        public bool FormSubmitted { get; set; }

        #endregion
    }
}