using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Core.Enums;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Services.LinkTypes;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Links
{
    public class LinkManageModel
    {
        #region Constructors

        private readonly ILinkTypeService _linkTypeService;
        public LinkManageModel()
        {
            _linkTypeService = HostContainer.GetInstance<ILinkTypeService>();

            LinkTypes = _linkTypeService.GetLinkTypes();
            UrlTargets = EnumUtilities.GenerateSelectListItems<CommonEnums.UrlTarget>(GenerateEnumType.DescriptionValueAndDescriptionText);
        }

        public LinkManageModel(int? linkTypeId)
            : this()
        {
            if (linkTypeId.HasValue)
            {
                LinkTypeId = linkTypeId.Value;
            }
        }

        public LinkManageModel(Link link)
            : this()
        {
            Id = link.Id;
            DateStart = link.DateStart;
            DateEnd = link.DateEnd;
            LinkTypeId = link.LinkTypeId;
            Name = link.Name;
            Description = link.Description;
            Url = link.Url;
            UrlTarget = link.UrlTarget;

            LinkTypes = _linkTypeService.GetLinkTypes(new List<int>(link.Id));
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [StringLength(512)]
        [LocalizedDisplayName("Link_Field_Name")]
        public string Name { get; set; }

        [Required]
        [LocalizedDisplayName("Link_Field_Url")]
        public string Url { get; set; }

        [LocalizedDisplayName("Link_Field_UrlTarget")]
        public string UrlTarget { get; set; }

        public IEnumerable<SelectListItem> UrlTargets { get; set; }

        [LocalizedDisplayName("Link_Field_Description")]
        public string Description { get; set; }

        [RequiredInteger("Link_Field_LinkTypeId")]
        [LocalizedDisplayName("Link_Field_LinkTypeId")]
        public int LinkTypeId { get; set; }

        public IEnumerable<SelectListItem> LinkTypes { get; set; }

        [LocalizedDisplayName("Link_Field_DateStart")]
        public DateTime? DateStart { get; set; }

        [DateGreaterThan("DateStart")]
        [LocalizedDisplayName("Link_Field_DateEnd")]
        public DateTime? DateEnd { get; set; }

        #endregion
    }
}
