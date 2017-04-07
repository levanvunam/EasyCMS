using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.Toolbars;
using EzCMS.Core.Services.UserGroups;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.UserGroups
{
    public class UserGroupManageModel : IValidatableObject
    {
        #region Constructors

        private readonly IToolbarService _toolbarService;
        public UserGroupManageModel()
        {
            _toolbarService = HostContainer.GetInstance<IToolbarService>();

            Toolbars = _toolbarService.GetToolbars();
        }

        public UserGroupManageModel(UserGroup userGroup)
            : this()
        {
            Id = userGroup.Id;
            Name = userGroup.Name;
            Description = userGroup.Description;
            RedirectUrl = userGroup.RedirectUrl;

            ToolbarId = userGroup.ToolbarId;
            Toolbars = _toolbarService.GetToolbars(ToolbarId);

            RecordOrder = userGroup.RecordOrder;
        }

        #endregion

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [StringLength(256)]
        [LocalizedDisplayName("UserGroup_Field_Name")]
        public string Name { get; set; }

        [StringLength(512)]
        [LocalizedDisplayName("UserGroup_Field_Description")]
        public string Description { get; set; }

        [StringLength(256)]
        [LocalizedDisplayName("UserGroup_Field_RedirectUrl")]
        public string RedirectUrl { get; set; }

        [LocalizedDisplayName("UserGroup_Field_ToolbarId")]
        public int? ToolbarId { get; set; }

        public IEnumerable<SelectListItem> Toolbars { get; set; }

        [LocalizedDisplayName("UserGroup_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            var userGroupService = HostContainer.GetInstance<IUserGroupService>();

            if (userGroupService.IsNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("UserGroup_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}