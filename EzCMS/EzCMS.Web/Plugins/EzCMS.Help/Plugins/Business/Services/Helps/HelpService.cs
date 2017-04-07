using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.BodyTemplates;
using EzCMS.Core.Models.BodyTemplates.HelpServices;
using EzCMS.Core.Models.SlideInHelps.HelpServices;
using EzCMS.Core.Services.BodyTemplates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EzCMS.Help.Plugins.Business.Services.Helps
{
    public class HelpService : IHelpService
    {
        private readonly IBodyTemplateService _bodyTemplateService;

        public HelpService(IBodyTemplateService bodyTemplateService)
        {
            _bodyTemplateService = bodyTemplateService;
        }

        /// <summary>
        /// Get slide in helps
        /// </summary>
        /// <param name="lastUpdatingTime"></param>
        /// <returns></returns>
        public List<SlideInHelpResponseModel> GetSlideInHelps(DateTime? lastUpdatingTime)
        {
            if (lastUpdatingTime.HasValue)
            {
                return WorkContext.SlideInHelpDictionary.Where(s => s.LastUpdate >= lastUpdatingTime && s.Active).Select(s => new SlideInHelpResponseModel(s)).ToList();
            }

            return WorkContext.SlideInHelpDictionary.Select(s => new SlideInHelpResponseModel(s)).ToList();
        }

        /// <summary>
        /// Get body templates
        /// </summary>
        /// <returns></returns>
        public BodyTemplateResponseModel GetBodyTemplates(string keyword, int? pageIndex, int? pageSize)
        {
            var data = _bodyTemplateService.Fetch(bodyTemplate =>
                string.IsNullOrEmpty(keyword)
                || (!string.IsNullOrEmpty(bodyTemplate.Name) && bodyTemplate.Name.Contains(keyword))
                || (!string.IsNullOrEmpty(bodyTemplate.Description) && bodyTemplate.Description.Contains(keyword)));

            var response = new BodyTemplateResponseModel
            {
                HasMore = false,
                Total = data.Count()
            };

            //Check if request has page size and page index
            if (pageSize.HasValue && pageSize > 0 && pageIndex.HasValue && pageIndex >= 0)
            {
                data = data.OrderBy(b => b.RecordOrder).Skip(pageSize.Value * pageIndex.Value).Take(pageSize.Value);
                response.HasMore = response.Total > (pageSize.Value * (pageIndex.Value + 1));
            }

            response.BodyTemplates = data.ToList().Select(b => new BodyTemplateSelectModel
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                ImageUrl = b.ImageUrl.ToAbsoluteUrl(),
            }).ToList();

            return response;
        }

        /// <summary>
        /// Download body template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BodyTemplateDownloadModel DownloadBodyTemplate(int id)
        {
            var bodyTemplate = _bodyTemplateService.GetById(id);

            if (bodyTemplate != null)
            {
                return new BodyTemplateDownloadModel(bodyTemplate);
            }

            return null;
        }
    }
}
