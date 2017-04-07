using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using Ez.Framework.Utilities.Social.Enums;
using Ez.Framework.Utilities.Social.Models;
using EzCMS.Core.Models.SocialMedia.Widget;
using EzCMS.Core.Models.SocialMedia.Feed;
using EzCMS.Core.Models.SocialMediaTokens;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.SocialMediaTokens
{
    [Register(Lifetime.PerInstance)]
    public interface ISocialMediaTokenService : IBaseService<SocialMediaToken>
    {
        #region Check Token Status

        /// <summary>
        /// Check token status
        /// </summary>
        void CheckAndUpdateTokenStatus();

        #endregion

        /// <summary>
        /// Get active tokens
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetActiveTokens(int? socialMediaTokenId = null);

        #region Widget

        /// <summary>
        /// Get social media feed by token id
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        SocialFeedWidget GetFeed(int tokenId, int total);

        #endregion

        /// <summary>
        /// Set token as default
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel SetDefaultToken(int id);

        #region Details

        /// <summary>
        /// Get social media token detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SocialMediaTokenDetailModel GetSocialMediaTokenDetailModel(int id);

        #endregion

        #region Grid Search

        /// <summary>
        /// Search tokens
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchSocialMediaTokens(JqSearchIn si);

        /// <summary>
        /// Export social media tokens
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get social media token manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SocialMediaTokenManageModel GetSocialMediaTokenManageModel(int? id = null);

        /// <summary>
        /// Save token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveSocialMediaToken(SocialMediaTokenManageModel model);

        /// <summary>
        /// Save social media response
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveSocialMediaResponse(SocialMediaResponseModel model);

        /// <summary>
        /// Get social information
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel SaveSocialMediaTokenInformation(int id);

        /// <summary>
        /// Delete social media token
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteSocialMediaToken(int id);

        #endregion

        #region Publish Post

        /// <summary>
        /// Post status
        /// </summary>
        /// <param name="model"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        ResponseModel Post(SocialMessageModel model, SocialMediaEnums.SocialNetwork network);

        /// <summary>
        /// Post status
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        void Post(List<SocialMediaMessageModel> model, Page page);

        /// <summary>
        /// Get active social media token of social media
        /// </summary>
        /// <param name="socialMediaId"></param>
        /// <returns></returns>
        SocialMediaToken GetActiveTokenOfSocialMedia(int socialMediaId);

        /// <summary>
        /// Get all available social media message models
        /// </summary>
        /// <returns></returns>
        List<SocialMediaMessageModel> GetAvailableSocialMediaMessageModels();

        #endregion
    }
}