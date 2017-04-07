using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.Mvc.Helpers;
using EzCMS.Core.Services.Localizes;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.ProtectedDocuments
{
    public class DocumentFeedbackModel : IValidatableObject
    {
        public DocumentFeedbackModel()
        {

        }

        public DocumentFeedbackModel(string encryptedPath)
            : this()
        {
            EncryptedPath = encryptedPath;
            RelativePath = encryptedPath.GetUniqueLinkInput();
        }

        #region Public Properties

        [Required]
        [LocalizedDisplayName("ProtectedDocument_Field_EncryptedPath")]
        public string EncryptedPath { get; set; }

        [Required]
        [LocalizedDisplayName("ProtectedDocument_Field_RelativePath")]
        public string RelativePath { get; set; }

        [Required]
        [LocalizedDisplayName("ProtectedDocument_Field_Comment")]
        public string Comment { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (!EncryptedPath.Equals(RelativePath.GetUniqueLink()))
            {
                yield return new ValidationResult(localizedResourceService.T("ProtectedDocument_Message_InvalidPathMapping"), new[] { "Email" });
            }
        }
    }
}
