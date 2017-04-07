using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.Forms;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Forms.Setup
{
    public class BuildFormSetupModel : FormSetupModel
    {
        public BuildFormSetupModel()
        {
            var formService = HostContainer.GetInstance<IFormService>();

            Step = ForNavigationms.FormSetupStep.SetupForm;
            ComponentTemplates = formService.GetFormTemplates();
        }

        public BuildFormSetupModel(Form form)
            : this()
        {
            Id = form.Id;
            Content = form.Content;
            JsonContent = form.JsonContent;
        }

        #region Public Properties

        public string Content { get; set; }

        public string JsonContent { get; set; }

        public string ComponentTemplates { get; set; }

        #endregion
    }
}
