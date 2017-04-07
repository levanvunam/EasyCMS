using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.RotatingImageGroups;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.RotatingImages
{
    public class RotatingImageManageModel
    {
        private readonly IRotatingImageGroupService _rotatingImageGroupService;
        public RotatingImageManageModel()
        {
            _rotatingImageGroupService = HostContainer.GetInstance<IRotatingImageGroupService>();
            Groups = _rotatingImageGroupService.GetRotatingImageGroups();
            UrlTargets = EnumUtilities.GenerateSelectListItems<CommonEnums.UrlTarget>(GenerateEnumType.DescriptionValueAndDescriptionText);
            Url = "#";
        }

        public RotatingImageManageModel(RotatingImage rotatingImage)
            : this()
        {
            Id = rotatingImage.Id;
            Title = rotatingImage.Title;
            ImageUrl = rotatingImage.ImageUrl;
            Text = rotatingImage.Text;
            Url = rotatingImage.Url;
            UrlTarget = rotatingImage.UrlTarget;
            GroupId = rotatingImage.GroupId;
        }

        public RotatingImageManageModel(int groupId)
            : this()
        {
            GroupId = groupId;
            Groups = _rotatingImageGroupService.GetRotatingImageGroups(GroupId);
        }

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("RotatingImage_Field_ImageUrl")]
        public string ImageUrl { get; set; }

        [LocalizedDisplayName("RotatingImage_Field_Title")]
        public string Title { get; set; }

        [LocalizedDisplayName("RotatingImage_Field_Text")]
        public string Text { get; set; }

        [LocalizedDisplayName("RotatingImage_Field_Url")]
        public string Url { get; set; }

        [Required]
        [LocalizedDisplayName("RotatingImage_Field_UrlTarget")]
        public string UrlTarget { get; set; }

        public IEnumerable<SelectListItem> UrlTargets { get; set; }

        [Required]
        [LocalizedDisplayName("RotatingImage_Field_GroupId")]
        public int GroupId { get; set; }

        public IEnumerable<SelectListItem> Groups { get; set; }

        public int RecordOrder { get; set; }

        #endregion
    }
}
