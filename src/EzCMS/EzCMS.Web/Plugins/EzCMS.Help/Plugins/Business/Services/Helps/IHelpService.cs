using Ez.Framework.Core.IoC.Attributes;
using EzCMS.Core.Models.BodyTemplates.HelpServices;
using EzCMS.Core.Models.SlideInHelps.HelpServices;
using System;
using System.Collections.Generic;

namespace EzCMS.Help.Plugins.Business.Services.Helps
{
    [Register(Lifetime.PerInstance)]
    public interface IHelpService
    {
        #region Slide in helps

        /// <summary>
        /// Get all updates for all slide in help after last updating time
        /// </summary>
        /// <param name="lastUpdatingTime"></param>
        /// <returns></returns>
        List<SlideInHelpResponseModel> GetSlideInHelps(DateTime? lastUpdatingTime);

        #endregion

        #region Body Templates

        /// <summary>
        /// Get available body templates
        /// </summary>
        /// <returns></returns>
        BodyTemplateResponseModel GetBodyTemplates(string keyword, int? pageIndex, int? pageSize);

        /// <summary>
        /// Download body template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BodyTemplateDownloadModel DownloadBodyTemplate(int id);

        #endregion
    }
}
