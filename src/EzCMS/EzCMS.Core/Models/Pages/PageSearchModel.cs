using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.BodyTemplates;
using EzCMS.Core.Services.FileTemplates;
using EzCMS.Core.Services.PageTemplates;
using EzCMS.Core.Services.Tags;
using EzCMS.Entity.Core.Enums;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Pages
{
    public class PageSearchModel
    {
        public PageSearchModel()
        {
            var bodyTemplateService = HostContainer.GetInstance<IBodyTemplateService>();
            var pageTemplateService = HostContainer.GetInstance<IPageTemplateService>();
            var fileTemplateService = HostContainer.GetInstance<IFileTemplateService>();
            var tagService = HostContainer.GetInstance<ITagService>();

            BodyTemplates = bodyTemplateService.GetBodyTemplates();
            PageTemplates = pageTemplateService.GetPageTemplateSelectList();
            FileTemplates = fileTemplateService.GetFileTemplates();

            TagIds = new List<int>();
            Tags = tagService.GetTagSelectList();

            StatusList = EnumUtilities.GenerateSelectListItems<PageEnums.PageStatus>();
        }

        #region Public Properties

        public string Keyword { get; set; }

        public PageEnums.PageStatus? Status { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }

        public List<int> TagIds { get; set; }

        public IEnumerable<SelectListItem> Tags { get; set; }

        public int? BodyTemplateId { get; set; }

        public IEnumerable<SelectListItem> BodyTemplates { get; set; }

        public int? PageTemplateId { get; set; }

        public IEnumerable<SelectListItem> PageTemplates { get; set; }

        public int? FileTemplateId { get; set; }

        public IEnumerable<SelectListItem> FileTemplates { get; set; }

        #endregion
    }
}
