using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.NoticeTypes;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.NoticeTypes
{
    public class NoticeTypeManageModel : IValidatableObject
    {
        #region Constructors

        public NoticeTypeManageModel()
        {
        }

        public NoticeTypeManageModel(NoticeType noticeType)
            : this()
        {
            Id = noticeType.Id;
            Name = noticeType.Name;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("NoticeType_Field_Name")]
        public string Name { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var noticeTypeService = HostContainer.GetInstance<INoticeTypeService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (noticeTypeService.IsTypeExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("NoticeType_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}
