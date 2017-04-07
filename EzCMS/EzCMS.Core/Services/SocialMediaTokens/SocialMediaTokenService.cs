using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Social;
using Ez.Framework.Utilities.Social.Enums;
using Ez.Framework.Utilities.Social.Models;
using Ez.Framework.Utilities.Web;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Models.SocialMedia.Widget;
using EzCMS.Core.Models.SocialMedia.Feed;
using EzCMS.Core.Models.SocialMediaTokens;
using EzCMS.Core.Services.SocialMediaLogs;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.SocialMediaTokens
{
    public class SocialMediaTokenService : ServiceHelper, ISocialMediaTokenService
    {
        private readonly ISocialMediaLogService _socialMediaLogService;
        private readonly IRepository<Entity.Entities.Models.SocialMedia> _socialMediaRepository;
        private readonly IRepository<SocialMediaToken> _socialMediaTokenRepository;

        public SocialMediaTokenService(IRepository<SocialMediaToken> socialMediaTokenRepository,
            IRepository<Entity.Entities.Models.SocialMedia> socialMediaRepository,
            ISocialMediaLogService socialMediaLogService)
        {
            _socialMediaTokenRepository = socialMediaTokenRepository;
            _socialMediaRepository = socialMediaRepository;
            _socialMediaLogService = socialMediaLogService;
        }

        #region Check Status

        /// <summary>
        /// Check and update active token status
        /// </summary>
        public void CheckAndUpdateTokenStatus()
        {
            var tokens = GetAllActiveTokens().ToList();

            foreach (var socialMediaToken in tokens)
            {
                var authorizeModel = new SocialMediaAuthorizeModel
                {
                    AppId = socialMediaToken.AppId,
                    AppSecret = socialMediaToken.AppSecret,
                    AccessToken = socialMediaToken.AccessToken,
                    AccessTokenSecret = socialMediaToken.AccessTokenSecret
                };

                var expired = false;
                switch (socialMediaToken.SocialMediaId.ToEnum<SocialMediaEnums.SocialNetwork>())
                {
                    case SocialMediaEnums.SocialNetwork.Facebook:
                        expired = SocialUtilities.IsFacebookTokenExpired(authorizeModel);
                        break;
                    case SocialMediaEnums.SocialNetwork.Twitter:
                        //Currently now the twitter token does not expired
                        break;
                    case SocialMediaEnums.SocialNetwork.LinkedIn:
                        expired = SocialUtilities.IsLinkedInTokenExpired(authorizeModel);
                        break;
                }

                if (expired)
                {
                    socialMediaToken.Status = SocialMediaEnums.TokenStatus.Expired;
                    socialMediaToken.ExpiredDate = null;
                    socialMediaToken.IsDefault = false;
                    Update(socialMediaToken);
                }
            }
        }

        #endregion

        #region Widget

        /// <summary>
        /// Get social media feed by token id
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public SocialFeedWidget GetFeed(int tokenId, int total)
        {
            var token = GetById(tokenId);

            if (token != null && token.Status == SocialMediaEnums.TokenStatus.Active)
            {
                var posts = new List<SocialFeedItemModel>();
                switch (token.SocialMediaId.ToEnum<SocialMediaEnums.SocialNetwork>())
                {
                    case SocialMediaEnums.SocialNetwork.Facebook:
                        posts = SocialUtilities.GetFacebookPosts(token.AccessToken);
                        break;
                    case SocialMediaEnums.SocialNetwork.Twitter:
                        var authorize = new SocialMediaAuthorizeModel
                        {
                            AppId = token.AppId,
                            AppSecret = token.AppSecret,
                            AccessToken = token.AccessToken,
                            AccessTokenSecret = token.AccessTokenSecret
                        };
                        posts = SocialUtilities.GetTwitterPosts(authorize, token.FullName);
                        break;
                    case SocialMediaEnums.SocialNetwork.LinkedIn:
                        posts = SocialUtilities.GetLinkedInPosts(token.AccessToken);
                        break;
                }

                return new SocialFeedWidget
                {
                    Feed = posts
                };
            }

            return null;
        }

        #endregion

        #region Details

        /// <summary>
        /// Get social media token detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SocialMediaTokenDetailModel GetSocialMediaTokenDetailModel(int id)
        {
            var socialMediaToken = GetById(id);

            if (socialMediaToken != null)
            {
                return new SocialMediaTokenDetailModel(socialMediaToken);
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Delete social media token
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteSocialMediaToken(int id)
        {
            var token = GetById(id);

            if (token != null)
            {
                if (token.SocialMediaLogs.Any())
                {
                    _socialMediaLogService.DeleteSocialMediaLogs(token.SocialMediaLogs);
                }

                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("SocialMediaToken_Message_DeleteSuccessfully")
                    : T("SocialMediaToken_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("SocialMediaToken_Message_DeleteSuccessfully")
            };
        }

        /// <summary>
        /// Get active token by social media id
        /// </summary>
        /// <param name="socialMediaId"></param>
        /// <returns></returns>
        public SocialMediaToken GetActiveTokenOfSocialMedia(int socialMediaId)
        {
            //Get default token
            var socialMediaToken = FetchFirst(s =>
                s.SocialMediaId == socialMediaId
                && s.Status == SocialMediaEnums.TokenStatus.Active
                && s.IsDefault);

            //If cannot find default token then get the first active one
            if (socialMediaToken == null)
            {
                socialMediaToken = FetchFirst(s =>
                    s.SocialMediaId == socialMediaId
                    && !string.IsNullOrEmpty(s.AccessToken)
                    && s.Status == SocialMediaEnums.TokenStatus.Active);
            }

            return socialMediaToken;
        }

        /// <summary>
        /// Get active token by social media id
        /// </summary>
        /// <param name="socialMediaId"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetActiveTokens(int? socialMediaId = null)
        {
            return Fetch(
                s =>
                    (!socialMediaId.HasValue || s.SocialMediaId == socialMediaId) &&
                    !string.IsNullOrEmpty(s.AccessToken)
                    && s.Status == SocialMediaEnums.TokenStatus.Active).Select(token => new SelectListItem
                    {
                        Text = token.FullName,
                        Value = SqlFunctions.StringConvert((double) token.Id).Trim(),
                        Selected = socialMediaId.HasValue && socialMediaId == token.Id
                    });
        }

        /// <summary>
        /// Set token as default
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel SetDefaultToken(int id)
        {
            var token = GetById(id);

            if (token != null)
            {
                if (token.Status == SocialMediaEnums.TokenStatus.Active)
                {
                    if (!token.IsDefault)
                    {
                        var defaultToken = GetActiveTokenOfSocialMedia(token.SocialMediaId);

                        if (defaultToken != null)
                        {
                            defaultToken.IsDefault = false;
                            Update(defaultToken);
                        }

                        token.IsDefault = true;
                        Update(token);
                    }

                    return new ResponseModel
                    {
                        Success = true,
                        Message = T("SocialMediaToken_Message_SetDefaultTokenSuccessfully")
                    };
                }

                return new ResponseModel
                {
                    Success = false,
                    Message = T("SocialMediaToken_Message_CannotSetDefaultTokenForPendingAndExpired")
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("SocialMediaToken_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Get active token by social media id
        /// </summary>
        /// <param name="socialMediaId"></param>
        /// <returns></returns>
        public IQueryable<SocialMediaToken> GetAllActiveTokens(int? socialMediaId = null)
        {
            return
                Fetch(
                    s =>
                        (!socialMediaId.HasValue || s.SocialMediaId == socialMediaId) &&
                        !string.IsNullOrEmpty(s.AccessToken) && s.Status == SocialMediaEnums.TokenStatus.Active);
        }

        #region Base

        public IQueryable<SocialMediaToken> GetAll()
        {
            return _socialMediaTokenRepository.GetAll();
        }

        public IQueryable<SocialMediaToken> Fetch(Expression<Func<SocialMediaToken, bool>> expression)
        {
            return _socialMediaTokenRepository.Fetch(expression);
        }

        public SocialMediaToken FetchFirst(Expression<Func<SocialMediaToken, bool>> expression)
        {
            return _socialMediaTokenRepository.FetchFirst(expression);
        }

        public SocialMediaToken GetById(object id)
        {
            return _socialMediaTokenRepository.GetById(id);
        }

        internal ResponseModel Insert(SocialMediaToken socialMediaToken)
        {
            return _socialMediaTokenRepository.Insert(socialMediaToken);
        }

        internal ResponseModel Update(SocialMediaToken socialMediaToken)
        {
            return _socialMediaTokenRepository.Update(socialMediaToken);
        }

        internal ResponseModel Delete(SocialMediaToken socialMediaToken)
        {
            return _socialMediaTokenRepository.Delete(socialMediaToken);
        }

        internal ResponseModel Delete(object id)
        {
            return _socialMediaTokenRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _socialMediaTokenRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the social media tokens.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchSocialMediaTokens(JqSearchIn si)
        {
            var data = GetAll();

            var socialMediaTokens = Maps(data);

            return si.Search(socialMediaTokens);
        }

        /// <summary>
        /// Export social media tokens
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var socialMediaTokens = Maps(data);

            var exportData = si.Export(socialMediaTokens, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <returns></returns>
        public IQueryable<SocialMediaTokenModel> Maps(IQueryable<SocialMediaToken> socialMediaTokens)
        {
            return socialMediaTokens.Select(m => new SocialMediaTokenModel
            {
                Id = m.Id,
                SocialMediaId = m.SocialMediaId,
                SocialMedia = m.SocialMedia.Name,
                IsDefault = m.IsDefault,
                FullName = m.FullName,
                Email = m.Email,
                AppId = m.AppId,
                AppSecret = m.AppSecret,
                Status = m.Status,
                ExpiredDate = m.ExpiredDate,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Setup

        #region Setup Social Media Token

        /// <summary>
        /// Get SocialMediaToken manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SocialMediaTokenManageModel GetSocialMediaTokenManageModel(int? id = null)
        {
            var socialMediaToken = GetById(id);
            if (socialMediaToken != null)
            {
                return new SocialMediaTokenManageModel(socialMediaToken);
            }
            return new SocialMediaTokenManageModel();
        }

        /// <summary>
        /// Save social media token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveSocialMediaToken(SocialMediaTokenManageModel model)
        {
            ResponseModel response;
            var socialMediaToken = GetById(model.Id);
            if (socialMediaToken != null)
            {
                socialMediaToken.SocialMediaId = model.SocialMediaId;
                socialMediaToken.AppId = model.AppId;
                socialMediaToken.AppSecret = model.AppSecret;

                //Unset the Is default and Status to wait for the callback function
                socialMediaToken.IsDefault = false;
                socialMediaToken.Status = SocialMediaEnums.TokenStatus.Pending;

                if (model.SocialMediaId == (int) SocialMediaEnums.SocialNetwork.Twitter)
                {
                    socialMediaToken.AccessToken = model.AccessToken;
                    socialMediaToken.AccessTokenSecret = model.AccessTokenSecret;
                }

                response = Update(socialMediaToken);
                response.SetMessage(response.Success
                    ? T("SocialMediaToken_Message_UpdateSuccessfully")
                    : T("SocialMediaToken_Message_UpdateFailure"));
            }
            else
            {
                socialMediaToken = new SocialMediaToken
                {
                    SocialMediaId = model.SocialMediaId,
                    AppId = model.AppId,
                    AppSecret = model.AppSecret,
                    AccessToken = model.AccessToken,
                    AccessTokenSecret = model.AccessTokenSecret,
                    Status = SocialMediaEnums.TokenStatus.Pending
                };
                response = Insert(socialMediaToken);
                response.SetMessage(response.Success
                    ? T("SocialMediaToken_Message_CreateSuccessfully")
                    : T("SocialMediaToken_Message_CreateFailure"));
            }

            if (response.Success)
            {
                var id = response.Data.ToInt();
                response.Data = GetAuthorizeUrl(id);
            }

            return response;
        }

        #endregion

        #region Authorize

        /// <summary>
        /// Get authorize url
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetAuthorizeUrl(int id)
        {
            var socialMediaToken = GetById(id);

            if (socialMediaToken != null)
            {
                return GetAuthorizeUrl(socialMediaToken);
            }

            return string.Empty;
        }

        /// <summary>
        /// Get authorize url
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string GetAuthorizeUrl(SocialMediaToken model)
        {
            var callbackUrl = UrlUtilities.GenerateUrl(
                HttpContext.Current.Request.RequestContext,
                "SocialMediaTokens",
                "Callback", new
                {
                    id = model.Id
                }, true);

            var socialMedia = _socialMediaRepository.GetById(model.SocialMediaId);
            switch (socialMedia.Id.ToEnum<SocialMediaEnums.SocialNetwork>())
            {
                case SocialMediaEnums.SocialNetwork.Facebook:
                    return SocialUtilities.GetFacebookAuthorizeUrl(model.AppId, callbackUrl, new List<string>
                    {
                        SocialMediaEnums.FacebookPermission.Email.GetEnumName(),
                        SocialMediaEnums.FacebookPermission.PublishActions.GetEnumName(),
                        SocialMediaEnums.FacebookPermission.UserPosts.GetEnumName(),
                        SocialMediaEnums.FacebookPermission.PublicProfile.GetEnumName()
                    });
                case SocialMediaEnums.SocialNetwork.Twitter:
                    //Twitter do not need the authorize url
                    return callbackUrl;
                case SocialMediaEnums.SocialNetwork.LinkedIn:
                    callbackUrl = UrlUtilities.GenerateUrl(
                        HttpContext.Current.Request.RequestContext,
                        "SocialMediaTokens",
                        "LinkedInCallback", null, true);

                    StateManager.SetSession(EzCMSContants.LinkedInCallbackId, model.Id);

                    return SocialUtilities.GetLinkedInAuthorizeUrl(model.AppId, callbackUrl);
            }
            return string.Empty;
        }

        #endregion

        #region Save Social Information

        /// <summary>
        /// Get social information
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel SaveSocialMediaTokenInformation(int id)
        {
            var socialMediaToken = GetById(id);

            if (socialMediaToken != null)
            {
                var model = new SocialMediaResponseModel
                {
                    Id = socialMediaToken.Id,
                    AppId = socialMediaToken.AppId,
                    AppSecret = socialMediaToken.AppSecret,
                    FullName = socialMediaToken.FullName,
                    Email = socialMediaToken.Email,
                    AccessToken = socialMediaToken.AccessToken,
                    AccessTokenSecret = socialMediaToken.AccessTokenSecret,
                    Verifier = socialMediaToken.Verifier
                };

                var callbackUrl = UrlUtilities.GenerateUrl(
                    HttpContext.Current.Request.RequestContext,
                    "SocialMediaTokens",
                    "Callback", new
                    {
                        id
                    }, true);

                try
                {
                    switch (socialMediaToken.SocialMediaId.ToEnum<SocialMediaEnums.SocialNetwork>())
                    {
                        case SocialMediaEnums.SocialNetwork.Facebook:
                            model = SocialUtilities.GetFacebookAccountInfo(model, callbackUrl);
                            break;
                        case SocialMediaEnums.SocialNetwork.Twitter:
                            model = SocialUtilities.GetTwitterAccountInfo(model);
                            break;
                        case SocialMediaEnums.SocialNetwork.LinkedIn:
                            //Specific callback url for LinkedIn
                            callbackUrl = UrlUtilities.GenerateUrl(
                                HttpContext.Current.Request.RequestContext,
                                "SocialMediaTokens",
                                "LinkedInCallback", null, true);

                            StateManager.SetSession(EzCMSContants.LinkedInCallbackId, id);

                            model = SocialUtilities.GetLinkedInAccountInfo(model, callbackUrl);
                            break;
                    }
                    return SaveSocialMediaResponse(model);
                }
                catch (Exception exception)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("SocialMediaToken_Message_InvalidConfigure"),
                        DetailMessage = exception.BuildErrorMessage()
                    };
                }
            }

            return new ResponseModel
            {
                Success = false
            };
        }

        /// <summary>
        /// Save social media response
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveSocialMediaResponse(SocialMediaResponseModel model)
        {
            var socialMediaToken = GetById(model.Id);
            if (socialMediaToken != null)
            {
                socialMediaToken.FullName = model.FullName;
                socialMediaToken.Email = model.Email;
                socialMediaToken.AccessToken = model.AccessToken;
                socialMediaToken.AccessTokenSecret = model.AccessTokenSecret;
                socialMediaToken.Verifier = model.Verifier;
                socialMediaToken.ExpiredDate = model.ExpiredDate;
                socialMediaToken.Status = SocialMediaEnums.TokenStatus.Active;

                //If there are no default token for this social network then assign current token as default one
                if (!Fetch(token => token.IsDefault && token.SocialMediaId == socialMediaToken.SocialMediaId).Any())
                {
                    socialMediaToken.IsDefault = true;
                }

                var response = Update(socialMediaToken);

                return response.SetMessage(response.Success
                    ? T("SocialMediaToken_Message_UpdateSuccessfully")
                    : T("SocialMediaToken_Message_UpdateFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("SocialMediaToken_Message_ObjectNotFound")
            };
        }

        #endregion

        #endregion

        #region Publish Post

        /// <summary>
        /// Get all available social media message models
        /// </summary>
        /// <returns></returns>
        public List<SocialMediaMessageModel> GetAvailableSocialMediaMessageModels()
        {
            var messages = _socialMediaRepository.GetAll().ToList()
                .Select(socialMedia => new SocialMediaMessageModel(socialMedia)).ToList();

            return messages;
        }

        /// <summary>
        /// Get active token by social media id
        /// </summary>
        /// <param name="socialNetwork"></param>
        /// <returns></returns>
        public SocialMediaToken GetActiveTokenOfSocialMedia(SocialMediaEnums.SocialNetwork socialNetwork)
        {
            var socialMediaToken = FetchFirst(token => token.SocialMediaId == (int) socialNetwork && token.IsDefault);
            if (socialMediaToken == null)
            {
                socialMediaToken = FetchFirst(s =>
                    s.SocialMediaId == (int) socialNetwork
                    && s.Status == SocialMediaEnums.TokenStatus.Active);
            }

            return socialMediaToken;
        }

        /// <summary>
        /// Post status
        /// </summary>
        /// <param name="model"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public ResponseModel Post(SocialMessageModel model, SocialMediaEnums.SocialNetwork network)
        {
            try
            {
                var token = GetActiveTokenOfSocialMedia(network);

                if (token != null)
                {
                    object response = null;
                    var authorizeModel = new SocialMediaResponseModel
                    {
                        AppId = token.AppId,
                        AppSecret = token.AppSecret,
                        AccessToken = token.AccessToken,
                        AccessTokenSecret = token.AccessTokenSecret
                    };

                    switch (network)
                    {
                        case SocialMediaEnums.SocialNetwork.Facebook:
                            response = SocialUtilities.PostFacebookStatus(authorizeModel, model);
                            break;
                        case SocialMediaEnums.SocialNetwork.Twitter:
                            response = SocialUtilities.PostTwitterStatus(authorizeModel, model);
                            break;
                        case SocialMediaEnums.SocialNetwork.LinkedIn:
                            response = SocialUtilities.PostLinkedInStatus(authorizeModel, model);
                            break;
                    }

                    var responseResult = string.Empty;
                    if (response != null)
                    {
                        responseResult = SerializeUtilities.Serialize(response);
                    }

                    return new ResponseModel
                    {
                        Success = true,
                        Data = responseResult
                    };
                }

                return new ResponseModel
                {
                    Success = false,
                    Message = T("SocialMediaToken_Message_ObjectNotFound")
                };
            }
            catch (Exception exception)
            {
                return new ResponseModel(exception);
            }
        }

        /// <summary>
        /// Post status
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public void Post(List<SocialMediaMessageModel> messages, Page page)
        {
            var sendingMessages = messages.Where(m => m.PostStatus && m.SocialMediaTokenId.HasValue).ToList();
            if (sendingMessages.Any())
            {
                var pageInformation = page.FriendlyUrl.ToAbsoluteUrl().DownloadPage();
                var socialMessageModel = new SocialMessageModel
                {
                    Title = pageInformation.Title,
                    ImageUrl = pageInformation.ImageUrl,
                    Url = page.FriendlyUrl.ToAbsoluteUrl(),
                    Description = pageInformation.Description
                };

                foreach (var message in sendingMessages)
                {
                    socialMessageModel.Message = message.Message;
                    var response = Post(socialMessageModel, message.SocialNetwork);

                    var log = new SocialMediaLog
                    {
                        SocialMediaId = message.SocialMediaId,
                        SocialMediaTokenId = message.SocialMediaTokenId.Value,
                        PageId = page.Id,
                        PostedContent = message.Message
                    };
                    if (response.Success)
                    {
                        log.PostedResponse = response.Data == null ? string.Empty : response.Data.ToString();
                    }
                    else
                    {
                        log.PostedResponse = response.DetailMessage;
                    }
                    _socialMediaLogService.Insert(log);
                }
            }
        }

        #endregion
    }
}