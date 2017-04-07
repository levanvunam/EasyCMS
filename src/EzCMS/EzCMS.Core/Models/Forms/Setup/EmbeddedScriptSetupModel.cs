using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.Forms;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Forms.Setup
{
    public class EmbeddedScriptSetupModel : FormSetupModel
    {
        public EmbeddedScriptSetupModel()
        {
            Step = ForNavigationms.FormSetupStep.Finish;
        }

        public EmbeddedScriptSetupModel(Form form)
            : this()
        {
            var formService = HostContainer.GetInstance<IFormService>();

            Id = form.Id;
            EmbeddedScript = formService.LoadEmbeddedScript(form.Id);
        }

        #region Public Properties

        public string EmbeddedScript { get; set; }

        #endregion
    }
}
