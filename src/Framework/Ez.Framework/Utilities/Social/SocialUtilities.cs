using Ez.Framework.Utilities.Social.Helper;
using Ez.Framework.Utilities.Social.Models;
using Facebook;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Ez.Framework.Utilities.Social
{
    public static class SocialUtilities
    {
        #region Facebook

        #region Setup

        /// <summary>
        /// Get facebook authorize url
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public static string GetFacebookAuthorizeUrl(string clientId, string callbackUrl, List<string> permissions)
        {
            var permissionString = string.Join(",", permissions);
            return string.Format("https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}", clientId, callbackUrl, permissionString);
        }

        /// <summary>
        /// Get facebook access token
        /// </summary>
        /// <param name="key"></param>
        /// <param name="secret"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetFacebookAccessToken(string key, string secret, string callbackUrl, string code)
        {
            var client = new FacebookClient
            {
                AppId = key,
                AppSecret = secret,
            };

            dynamic response = client.Get("/oauth/access_token", new
            {
                client_id = key,
                client_secret = secret,
                code = code,
                redirect_uri = callbackUrl
            });

            return GetFacebookLongLiveToken(key, secret, response.access_token);
        }

        /// <summary>
        /// Get long live token from access token
        /// </summary>
        /// <param name="key"></param>
        /// <param name="secret"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string GetFacebookLongLiveToken(string key, string secret, string accessToken)
        {
            var client = new FacebookClient
            {
                AppId = key,
                AppSecret = secret,
            };

            dynamic response = client.Get("/oauth/access_token", new
            {
                client_id = key,
                client_secret = secret,
                grant_type = "fb_exchange_token",
                fb_exchange_token = accessToken
            });

            return response.access_token;
        }

        /// <summary>
        /// Get facebook access token
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        public static SocialMediaResponseModel GetFacebookAccountInfo(SocialMediaResponseModel model, string callbackUrl)
        {
            model.Verifier = HttpContext.Current.Request["code"];

            #region Get Access Token

            var client = new FacebookClient();

            dynamic response = client.Get("/oauth/access_token", new
            {
                client_id = model.AppId,
                client_secret = model.AppSecret,
                code = model.Verifier,
                redirect_uri = callbackUrl
            });

            model.AccessToken = response.access_token;

            //Default expiry date of Facebook is 60 days
            model.ExpiredDate = DateTime.UtcNow.AddDays(60);

            #endregion

            #region Get account info

            client = new FacebookClient(model.AccessToken);

            dynamic me = client.Get("me", new
            {
                fields = "name,email,first_name,last_name"
            });
            model.Email = me.email;
            model.FullName = StringUtilities.GenerateFullName(me.first_name, me.last_name);

            #endregion

            return model;
        }

        /// <summary>
        /// Get facebook access token
        /// </summary>
        /// <param name="key"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string GetFacebookAppAccessToken(string key, string secret)
        {
            return string.Format("{0}|{1}", key, secret);
        }

        #endregion

        #region Post

        /// <summary>
        /// Post status to Facebook
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        public static object PostFacebookStatus(SocialMediaAuthorizeModel model, SocialMessageModel message)
        {
            var client = new FacebookClient(model.AccessToken);

            if (!string.IsNullOrEmpty(message.ImageUrl))
            {
                return client.Post("/me/feed", new
                {
                    message = message.Message,
                    link = message.Url,
                    picture = message.ImageUrl,
                    name = message.Title,
                    description = message.Description
                });
            }

            return client.Post("/me/feed", new
            {
                message = message.Message,
                link = message.Url,
                name = message.Title,
                description = message.Description
            });
        }

        #endregion

        #region Check Status

        /// <summary>
        /// Check if token is expired
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsFacebookTokenExpired(SocialMediaAuthorizeModel model)
        {
            try
            {
                var client = new FacebookClient(model.AccessToken);
                dynamic result = client.Get("me/friends");
            }
            catch (FacebookOAuthException)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Get Feed

        /// <summary>
        /// Get Facebook posts
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static List<SocialFeedItemModel> GetFacebookPosts(string accessToken)
        {
            try
            {
                var client = new FacebookClient(accessToken);
                dynamic result = client.Get("me/posts?fields=id,caption,created_time,description,link,message,name,object_id,picture,place,story");

                var posts = new List<SocialFeedItemModel>();
                foreach (var item in result.data)
                {
                    var post = new SocialFeedItemModel
                    {
                        Id = GetData<string>(item, "id"),
                        Message = GetData<string>(item, "message"),
                        Caption = GetData<string>(item, "caption"),
                        Description = GetData<string>(item, "description"),
                        Link = GetData<string>(item, "link"),
                        Title = GetData<string>(item, "name"),
                        Picture = GetData<string>(item, "picture")
                    };

                    DateTime created;
                    if (DateTime.TryParse(GetData<string>(item, "created_time"), out created))
                    {
                        post.Created = created;
                    }

                    posts.Add(post);
                }

                return posts;
            }
            catch (Exception)
            {
                return new List<SocialFeedItemModel>();
            }
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static T GetData<T>(dynamic item, string name) where T : class
        {
            try
            {
                return (T)item[name];
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        #endregion

        #endregion

        #region Twitter

        /// <summary>
        /// Get twitter account info
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static SocialMediaResponseModel GetTwitterAccountInfo(SocialMediaResponseModel model)
        {
            var authorizer = new MvcAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = model.AppId,
                    ConsumerSecret = model.AppSecret,
                    AccessToken = model.AccessToken,
                    AccessTokenSecret = model.AccessTokenSecret
                }
            };

            var context = new TwitterContext(authorizer);

            var account = (from a in context.Account
                           where a.Type == AccountType.VerifyCredentials
                           select a).SingleOrDefault();

            if (account != null)
            {
                model.FullName = account.User.ScreenNameResponse;
            }

            return model;
        }

        /// <summary>
        /// Post status to Twitter
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        public static object PostTwitterStatus(SocialMediaAuthorizeModel model, SocialMessageModel message)
        {
            var authorizer = new MvcAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = model.AppId,
                    ConsumerSecret = model.AppSecret,
                    AccessToken = model.AccessToken,
                    AccessTokenSecret = model.AccessTokenSecret
                }
            };
            var context = new TwitterContext(authorizer);

            if (string.IsNullOrEmpty(message.ImageUrl))
            {
                var tweet = Task.Run(() => context.TweetAsync(message.Message));
                return tweet;
            }

            //Post with image
            try
            {
                var webClient = new WebClient();
                byte[] imageBytes = webClient.DownloadData(message.ImageUrl);
                var media = Task.Run(() => context.UploadMediaAsync(imageBytes, MimeMapping.GetMimeMapping(message.ImageUrl)));
                var tweet = Task.Run(() => context.TweetAsync(message.Message, new List<ulong> { media.Result.MediaID }));
                return tweet;
            }
            catch (Exception)
            {
                var tweet = Task.Run(() => context.TweetAsync(message.Message));
                return tweet;
            }
        }

        #region Get Feed

        /// <summary>
        /// Get Twitter posts
        /// </summary>
        /// <param name="model"></param>
        /// <param name="screenName"></param>
        public static List<SocialFeedItemModel> GetTwitterPosts(SocialMediaAuthorizeModel model, string screenName)
        {
            var authorizer = new MvcAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = model.AppId,
                    ConsumerSecret = model.AppSecret,
                    AccessToken = model.AccessToken,
                    AccessTokenSecret = model.AccessTokenSecret
                }
            };
            var context = new TwitterContext(authorizer);

            var tweets = (from tweet in context.Status
                          where tweet.Type == StatusType.User
                          && tweet.ScreenName == screenName
                          select tweet)
                .ToList();

            return tweets.Select(t => new SocialFeedItemModel
            {
                Id = t.ID.ToString(CultureInfo.InvariantCulture),
                Message = t.Text,
                Created = t.CreatedAt
            }).ToList();
        }

        #endregion

        #endregion

        #region LinkedIn

        /// <summary>
        /// Get linked in authorize url
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        public static string GetLinkedInAuthorizeUrl(string clientId, string callbackUrl)
        {
            return LinkedInHelper.GetLinkedInAuthorizeUrl(clientId, callbackUrl);
        }

        /// <summary>
        /// Get LinkedIn account info
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        public static SocialMediaResponseModel GetLinkedInAccountInfo(SocialMediaResponseModel model, string callbackUrl)
        {
            model.Verifier = HttpContext.Current.Request["code"];

            DateTime expiredDate;

            // Get access token and expired date
            model.AccessToken = LinkedInHelper.GetAccessToken(model.AppId, model.AppSecret, model.Verifier, callbackUrl, out expiredDate);

            model.ExpiredDate = expiredDate;

            #region Get User Profile

            var linkedLinkHelper = new LinkedInHelper(model.AccessToken);

            var userInfo = linkedLinkHelper.GetCurrentUser();

            model.FullName = StringUtilities.GenerateFullName(userInfo.FirstName, userInfo.LastName);
            model.Email = userInfo.Email;

            #endregion

            return model;
        }

        /// <summary>
        /// Post status to Twitter
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        public static object PostLinkedInStatus(SocialMediaAuthorizeModel model, SocialMessageModel message)
        {
            var linkedInHelper = new LinkedInHelper(model.AccessToken);

            return linkedInHelper.PostStatus(message);
        }

        /// <summary>
        /// Check if LinkedIn token is expired
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsLinkedInTokenExpired(SocialMediaAuthorizeModel model)
        {
            try
            {
                var linkedInHelper = new LinkedInHelper(model.AccessToken);

                var userInfo = linkedInHelper.GetCurrentUser();
            }
            catch (Exception)
            {
                return true;
            }

            return false;
        }

        #region Get Feed

        /// <summary>
        /// Get LinkedIn feed
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static List<SocialFeedItemModel> GetLinkedInPosts(string accessToken)
        {
            var linkedInHelper = new LinkedInHelper(accessToken);

            //var posts = linkedInHelper.GetCurrentUserFeed();

            return new List<SocialFeedItemModel>();
        }

        #endregion

        #endregion
    }
}