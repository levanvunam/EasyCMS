using Ez.Framework.Utilities.Social.Models;
using Ez.Framework.Utilities.Social.Models.LinkedIn;
using Ez.Framework.Utilities.Web;
using System;
using System.Collections.Generic;

namespace Ez.Framework.Utilities.Social.Helper
{
    public class LinkedInHelper
    {
        private const string URLBase = "https://api.linkedin.com/v1";
        private const string AuthorizationUrl = "https://www.linkedin.com/uas/oauth2/authorization?client_id={0}&redirect_uri={1}&response_type=code&state={2}&scope=r_basicprofile%20r_emailaddress%20w_share";
        const string LinkedInAccessTokenUrl = "https://www.linkedin.com/uas/oauth2/accessToken?grant_type=authorization_code&code={0}&redirect_uri={1}&client_id={2}&client_secret={3}";
        public string AccessToken { get; set; }

        public LinkedInHelper(string accessToken)
        {
            AccessToken = accessToken;
        }

        /// <summary>
        /// Get authorize url
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        public static string GetLinkedInAuthorizeUrl(string clientId, string callbackUrl)
        {
            return string.Format(AuthorizationUrl, clientId, callbackUrl, PasswordUtilities.GetRandomString());
        }

        /// <summary>
        /// Get access token
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="secretKey"></param>
        /// <param name="code"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="expiredDate"></param>
        /// <returns></returns>
        public static string GetAccessToken(string clientId, string secretKey, string code, string callbackUrl, out DateTime expiredDate)
        {
            var accessTokenUrl = GetLinkedInAccessTokenUrl(clientId, secretKey, code, callbackUrl);
            var response = SerializeUtilities.Deserialize<LinkedInAccessToken>(WebUtilities.GetRequest(accessTokenUrl));
            expiredDate = DateTime.UtcNow.AddSeconds(response.expires_in);
            return response.access_token;
        }

        private static string GetLinkedInAccessTokenUrl(string clientId, string secretKey, string code, string callbackUrl)
        {
            return string.Format(LinkedInAccessTokenUrl, code, callbackUrl, clientId, secretKey);
        }

        #region Helper

        /// <summary>
        /// Get response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        private T GetResponse<T>(string path) where T : class
        {
            var url = string.Format("{0}/{1}?oauth2_access_token={2}", URLBase, path, AccessToken);

            return SerializeUtilities.DeserializeXml<T>(WebUtilities.GetRequest(url));
        }

        /// <summary>
        /// Get response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private T PostRequest<T>(string path, object data) where T : class
        {
            var url = string.Format("{0}/{1}?oauth2_access_token={2}", URLBase, path, AccessToken);

            return SerializeUtilities.DeserializeXml<T>(WebUtilities.PostRequest(url, data));
        }

        #endregion

        #region People Information

        /// <summary>
        /// Get current user
        /// </summary>
        /// <returns></returns>
        public Person GetCurrentUser()
        {
            return GetResponse<Person>("people/~:(id,first-name,last-name,headline,email_address)");
        }

        /// <summary>
        /// Get person by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Person GetPersonById(string id)
        {
            return GetResponse<Person>("people/id=" + id);
        }

        /// <summary>
        /// Get person by public profile url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Person GetPersonByPublicProfileUrl(string url)
        {
            return GetResponse<Person>("people::(url=" + url + ")");
        }

        /// <summary>
        /// Search people by keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public PeopleSearchResult SearchPeopleByKeyWord(string keyword)
        {
            return GetResponse<PeopleSearchResult>("people-search?keywords=" + keyword);
        }

        /// <summary>
        /// Get people by first name
        /// </summary>
        /// <param name="firstName"></param>
        /// <returns></returns>
        public PeopleSearchResult GetPeopleByFirstName(string firstName)
        {
            return GetResponse<PeopleSearchResult>("people-search?first-name=" + firstName);
        }

        /// <summary>
        /// Get people by last name
        /// </summary>
        /// <param name="lastname"></param>
        /// <returns></returns>
        public PeopleSearchResult GetPeopleByLastName(string lastname)
        {
            return GetResponse<PeopleSearchResult>("people-search?last-name=" + lastname);
        }

        /// <summary>
        /// Get people by school name
        /// </summary>
        /// <param name="schoolName"></param>
        /// <returns></returns>
        public PeopleSearchResult GetPeopleBySchoolName(string schoolName)
        {
            return GetResponse<PeopleSearchResult>("people-search?school-name=" + schoolName);
        }

        #endregion

        #region Company information

        /// <summary>
        /// Get company
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Company GetCompany(int id)
        {
            return GetResponse<Company>("companies/" + id);
        }

        /// <summary>
        /// Get company by universal name
        /// </summary>
        /// <param name="universalName"></param>
        /// <returns></returns>
        public Company GetCompanyByUniversalName(string universalName)
        {
            return GetResponse<Company>("companies/universal-name=" + universalName);
        }

        /// <summary>
        /// Get companies by email domain
        /// </summary>
        /// <param name="emailDomain"></param>
        /// <returns></returns>
        public CompanyCollection GetCompaniesByEmailDomain(string emailDomain)
        {
            return GetResponse<CompanyCollection>("companies?email-domain=" + emailDomain);
        }

        /// <summary>
        /// Get companies by idand Universal name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="universalName"></param>
        /// <returns></returns>
        public CompanyCollection GetCompaniesByIdAndUniversalName(string id, string universalName)
        {
            return GetResponse<CompanyCollection>("companies::(" + id + ",universal-name=" + universalName + ")");
        }

        #endregion

        #region Get Feed


        /// <summary>
        /// Get current user feed
        /// </summary>
        /// <returns></returns>
        public dynamic GetCurrentUserFeed()
        {
            return GetResponse<dynamic>("people/~/updates");
        }

        #endregion

        #region Post

        /// <summary>
        /// Post linked in status
        /// </summary>
        /// <param name="message"></param>
        public object PostStatus(SocialMessageModel message)
        {
            var postContent = new Dictionary<string, object>
            {
                {"title", message.Title},
                {"description", message.Description},
                {"submitted-url", message.Url}
            };

            if (!string.IsNullOrEmpty(message.ImageUrl))
            {
                postContent.Add("submitted-image-url", message.ImageUrl);
            }

            var postResult = PostRequest<LinkedInShareResponse>("people/~/shares", new
            {
                comment = message.Message,
                content = postContent,
                visibility = new
                {
                    code = "anyone"
                }
            });

            //var key = postResult.UpdateKey;

            //return PostRequest<LinkedInPostResultResponse>("people/~/network/updates", new
            //{
            //    key = key
            //});

            return postResult;
        }

        #endregion
    }
}
