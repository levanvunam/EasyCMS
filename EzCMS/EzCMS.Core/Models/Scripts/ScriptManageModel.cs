using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.Scripts;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Scripts
{
    public class ScriptManageModel : IValidatableObject
    {
        public ScriptManageModel()
        {
            
        }

        public ScriptManageModel(Script script)
        {
            Id = script.Id;
            Name = script.Name;
            Content = script.Content;
        }

        public ScriptManageModel(ScriptLog log)
            : this()
        {
            Id = log.ScriptId;
            Name = log.Name;
            Content = log.Content;
        }

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("Script_Field_Name")]
        public string Name { get; set; }

        [Required]
        [LocalizedDisplayName("Script_Field_Content")]

        public string Content { get; set; }
        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var scriptService = HostContainer.GetInstance<IScriptService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (scriptService.IsScriptNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("Script_Message_ExistingName"), new[]{ "Name"});
            }
        }
    }
}
