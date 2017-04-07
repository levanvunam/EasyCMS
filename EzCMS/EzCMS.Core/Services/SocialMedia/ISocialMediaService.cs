using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.SocialMedia;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.SocialMedia
{
    [Register(Lifetime.PerInstance)]
    public interface ISocialMediaService : IBaseService<Entity.Entities.Models.SocialMedia>
    {
        /// <summary>
        /// Get social media list
        /// </summary>
        /// <param name="socialMediaId"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetSocialMediaList(int? socialMediaId = null);

        #region Details

        /// <summary>
        /// Get social media details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SocialMediaDetailModel GetSocialMediaDetailModel(int id);

        #endregion

        #region Grid Search

        /// <summary>
        /// Search social media
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchSocialMedia(JqSearchIn si);

        /// <summary>
        /// Export social media
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get social media manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SocialMediaManageModel GetSocialMediaManageModel(int id);

        /// <summary>
        /// Save social media
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveSocialMedia(SocialMediaManageModel model);

        #endregion
    }
}