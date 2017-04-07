using Ez.Framework.Utilities.Web.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using Match = System.Text.RegularExpressions.Match;

namespace Ez.Framework.Utilities.Web
{
    /// <summary>
    /// Web utilities
    /// </summary>
    public static class WebUtilities
    {
        #region Restart Appdomain

        private static AspNetHostingPermissionLevel? _trustLevel;

        /// <summary>
        /// Finds the trust level of the running application (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns>The current trust level.</returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
                //set minimum
                _trustLevel = AspNetHostingPermissionLevel.None;

                //determine maximum
                foreach (AspNetHostingPermissionLevel trustLevel in new[]
                {
                    AspNetHostingPermissionLevel.Unrestricted,
                    AspNetHostingPermissionLevel.High,
                    AspNetHostingPermissionLevel.Medium,
                    AspNetHostingPermissionLevel.Low,
                    AspNetHostingPermissionLevel.Minimal
                })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                        _trustLevel = trustLevel;
                        break; //we've set the highest permission we can
                    }
                    catch (System.Security.SecurityException)
                    {
                    }
                }
            }
            return _trustLevel.Value;
        }

        public static void RestartAppDomain()
        {
            if (GetTrustLevel() > AspNetHostingPermissionLevel.Medium)
            {
                //full trust
                HttpRuntime.UnloadAppDomain();

                TryWriteGlobalAsax();
            }
            else
            {
                //medium trust
                bool success = TryWriteWebConfig();
                if (!success)
                {
                    success = TryWriteGlobalAsax();
                    if (!success)
                    {
                        success = TryWriteOfflineFile();
                    }
                }

                if (!success)
                {
                    throw new Exception(
                        "Ez needs to be restarted due to web.config change/ global.asax change/ write app_offline.htm file, but was unable to do so." +
                        Environment.NewLine +
                        "To prevent this issue in the future, a change to the web server configuration is required:" +
                        Environment.NewLine +
                        "- run the application in a full trust environment, or" + Environment.NewLine +
                        "- give the application write access to the web.config /global.aspx /root.");
                }

            }
        }

        private static bool TryWriteGlobalAsax()
        {
            try
            {
                File.SetLastWriteTimeUtc(MapPath("~/global.asax"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryWriteWebConfig()
        {
            try
            {
                // In medium trust, "UnloadAppDomain" is not supported. Touch web.config
                // to force an AppDomain restart.
                File.SetLastWriteTimeUtc(MapPath("~/web.config"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryWriteOfflineFile()
        {
            try
            {
                const string fileName = "~/app_offline.htm";
                var path = MapPath(fileName);
                if (!File.Exists(path))
                {
                    using (var sw = new StreamWriter(path))
                    {
                        sw.Write("");
                    }
                }
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Convert relative path to absolute path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HostingEnvironment.MapPath(path);
            }
            //not hosted. For example, run in unit tests
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }

        #endregion

        #region Web Request

        /// <summary>
        /// Get return code of url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpStatusCode GetResponseCode(this string url)
        {
            HttpStatusCode result = default(HttpStatusCode);
            try
            {
                var request = WebRequest.Create(url);
                request.Method = "HEAD";
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response != null)
                    {
                        result = response.StatusCode;
                    }
                }
            }
            catch (WebException exception)
            {
                if (exception.Status == WebExceptionStatus.ProtocolError &&
                    exception.Response != null)
                {
                    var response = (HttpWebResponse)exception.Response;
                    result = response.StatusCode;
                }
            }
            catch (Exception)
            {
                result = HttpStatusCode.InternalServerError;
            }

            return result;
        }

        /// <summary>
        /// Send request to service and getting result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceUrl"></param>
        /// <param name="authorizeCode"></param>
        /// <param name="apiAction"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T SendApiRequest<T>(string serviceUrl, string authorizeCode, string apiAction, HttpMethod method,
            RouteValueDictionary parameters)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(serviceUrl)
            };

            if (!string.IsNullOrEmpty(authorizeCode))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authorizeCode)));
            }

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage;
            if (method == HttpMethod.Post)
            {
                responseMessage = client.PostAsJsonAsync(apiAction, parameters).Result;
            }
            else
            {
                var url = UrlUtilities.CombineQueryString(apiAction, parameters);
                responseMessage = client.GetAsync(url).Result;
            }

            var result = responseMessage.Content.ReadAsAsync<T>().Result;

            return result;
        }

        /// <summary>
        /// Get url request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T GetRequest<T>(string url) where T : class
        {
            using (var client = new WebClient())
            {
                var content = client.DownloadString(url);

                return SerializeUtilities.Deserialize<T>(content);
            }
        }

        /// <summary>
        /// Get url request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetRequest(string url)
        {
            using (var client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }

        /// <summary>
        /// Get url request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string PostRequest(string url, object data)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/json");
                var dataString = SerializeUtilities.Serialize(data);

                return client.UploadString(url, dataString);
            }
        }

        #region Download Url

        /// <summary>
        /// Download page
        /// </summary>
        /// <param name="url"></param>
        public static WebPageInformationModel DownloadPage(this string url)
        {
            using (var client = new WebClient())
            {
                var source = client.DownloadString(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(source);

                // Get title tag
                var title = (from x in doc.DocumentNode.Descendants()
                             where x.Name.ToLower() == "title"
                             select x.InnerText).FirstOrDefault();
                // Get description tag
                var description = (from x in doc.DocumentNode.Descendants()
                                   where x.Name.ToLower() == "meta"
                                         && x.Attributes["name"] != null
                                         && x.Attributes["name"].Value.ToLower() == "description"
                                   select x.Attributes["content"]).FirstOrDefault();

                var model = new WebPageInformationModel
                {
                    Url = url,
                    Title = title,
                    Description = description != null ? description.Value : string.Empty,
                    ImageUrls = new List<string>()
                };

                var images = (from x in doc.DocumentNode.Descendants()
                              where x.Name.ToLower() == "img" && x.Attributes["src"] != null
                              select x.Attributes["src"].Value).ToList();

                if (images.Any())
                {
                    model.ImageUrls = images.Select(i => i.ToAbsoluteUrl()).ToList();
                    model.ImageUrl = model.ImageUrls.First();
                }

                return model;
            }
        }

        #endregion

        #endregion

        #region Parsing Header

        /// <summary>
        /// Check if request need cache or not
        /// </summary>
        /// <param name="context"></param>
        /// <param name="lastUpdate"></param>
        /// <returns></returns>
        public static bool OutputNeedReCache(HttpContext context, DateTime lastUpdate)
        {

            DateTime? ifModifiedSinceTime = GetIfModifiedSinceUTCTime(context);
            DateTime imageLastModifiedTime = lastUpdate;

            //strip milliseconds before comparison
            imageLastModifiedTime = imageLastModifiedTime.AddMilliseconds(-lastUpdate.Millisecond);
            bool clientNeedsLatest = ifModifiedSinceTime == null || (imageLastModifiedTime > ifModifiedSinceTime);

            if (clientNeedsLatest)
            {
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                //context.Response.Cache.SetMaxAge(new TimeSpan(1, 0, 0));

                context.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                context.Response.Cache.SetLastModified(imageLastModifiedTime);

                // Set Last Modified time
                context.Response.Cache.SetLastModified(lastUpdate);
                return true;
            }

            // The requested file has not changed
            context.Response.ClearContent();
            context.Response.StatusCode = (int)HttpStatusCode.NotModified;
            context.Response.SuppressContent = true;

            return false;
        }

        /// <summary>
        /// Get modified since 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static DateTime? GetIfModifiedSinceUTCTime(HttpContext context)
        {
            DateTime ifModifiedSinceTime;
            string ifModifiedSinceHeaderText = context.Request.Headers.Get("If-Modified-Since");

            if (!string.IsNullOrEmpty(ifModifiedSinceHeaderText) && DateTime.TryParse(ifModifiedSinceHeaderText, out ifModifiedSinceTime))
            {
                return ifModifiedSinceTime;
            }

            return null;
        }

        #endregion

        #region Tiny Url

        /// <summary>
        /// Create google tiny url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string CreateGoogleTinyUrl(this string url)
        {
            const string googleTinyUrlApiUrl = "https://www.googleapis.com/urlshortener/v1/url";

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(googleTinyUrlApiUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    url = url.ToAbsoluteUrl();
                    var json = string.Format("{{\"longUrl\":\"{0}\"}}", url);
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var responseStream = httpResponse.GetResponseStream();
                if (responseStream != null)
                {
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        var responseText = streamReader.ReadToEnd();
                        Console.WriteLine(responseText);
                    }
                }
                return url;
            }
            catch (Exception)
            {
                return url;
            }
        }

        /// <summary>
        /// Get tiny url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToTinyUrl(this string url)
        {
            const string tinyUrlApiUrl = "http://tinyurl.com/api-create.php?url=";
            try
            {
                if (url.Length <= 12)
                {
                    return url;
                }

                if (!url.ToLower().StartsWith("http") && !url.ToLower().StartsWith("ftp"))
                {
                    url = "http://" + url;
                }
                var request = WebRequest.Create(tinyUrlApiUrl + url);
                var res = request.GetResponse();
                string text;
                using (var reader = new StreamReader(res.GetResponseStream()))
                {
                    text = reader.ReadToEnd();
                }
                return text;
            }
            catch (Exception)
            {
                return url;
            }
        }

        #endregion

        #region Browser and User agent

        /// <summary>
        /// Get user and browser information from request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static UserAgentModel GetUserAgentInformationFromRequest(this HttpRequest request)
        {
            var userAgentInformation = new UserAgentModel();
            if (request != null)
            {
                userAgentInformation = new UserAgentModel
                {
                    Platform = request.Browser.Platform,
                    OsVersion = GetOsVersion(request.UserAgent),
                    BrowserName = request.Browser.Browser,
                    BrowserType = request.Browser.Type,
                    BrowserVersion = request.Browser.Version,
                    MajorVersion = request.Browser.MajorVersion,
                    MinorVersion = request.Browser.MinorVersion,
                    IsBeta = request.Browser.Beta,
                    IsCrawler = request.Browser.Crawler,
                    IsAOL = request.Browser.AOL,
                    IsWin16 = request.Browser.Win16,
                    IsWin32 = request.Browser.Win32,
                    SupportsFrames = request.Browser.Frames,
                    SupportsTables = request.Browser.Tables,
                    SupportsCookies = request.Browser.Cookies,
                    SupportsVBScript = request.Browser.VBScript,
                    SupportsJavaScript = request.Browser.EcmaScriptVersion.ToString(),
                    SupportsJavaApplets = request.Browser.JavaApplets,
                    JavaScriptVersion = request.Browser["JavaScriptVersion"],
                    RawAgentString = request.UserAgent
                };

                #region IP Address

                //Checking the forward header
                if (!string.IsNullOrEmpty(request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
                {
                    var ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    string[] addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        userAgentInformation.IpAddress = addresses[0];
                    }
                }
                //Check if the request is from Incapsula service
                else if (!string.IsNullOrEmpty(request.ServerVariables["HTTP_INCAP_CLIENT_IP"]))
                {
                    userAgentInformation.IpAddress = request.ServerVariables["HTTP_INCAP_CLIENT_IP"];
                }
                else
                {
                    userAgentInformation.IpAddress = request.UserHostAddress;
                }

                #endregion
            }

            return userAgentInformation;
        }

        /// <summary>
        /// Get user and browser information from context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static UserAgentModel GetUserAgentInformationFromRequest(this HttpContext context)
        {
            if (context != null)
            {
                return context.Request.GetUserAgentInformationFromRequest();
            }

            return new UserAgentModel();
        }

        /// <summary>
        /// Get OS version from user agent
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        private static string GetOsVersion(this string userAgent)
        {
            if (!string.IsNullOrEmpty(userAgent))
            {
                #region Window

                var windowOs = new Dictionary<string, string>
                {
                    //Window
                    {"Windows Phone 10", "Windows Phone 10"},
                    {"Windows Phone OS 7.5", "Windows Phone 7"},
                    {"Windows Phone 8.0", "Windows Phone 8"},
                    {"Windows Phone 8.1", "Windows Phone 8.1"},
                    {"Windows NT 10.0", "Windows 10"},
                    {"Windows NT 6.4", "Windows 10"},
                    {"Windows NT 6.3", "Windows 8.1"},
                    {"Windows NT 6.2", "Windows 8"},
                    {"Windows NT 6.1", "Windows 7"},
                    {"Windows NT 6.0", "Windows Vista"},
                    {"Windows NT 5.2", "Windows Server 2003"},
                    {"Windows NT 5.1", "Windows XP"},
                    {"Windows NT 5.0", "Windows 2000"},
                    {"Windows 2000", "Windows 2000"},
                };

                //Window check
                if (windowOs.Any(o => userAgent.Contains(o.Key)))
                {
                    return windowOs.First(o => userAgent.Contains(o.Key)).Value;
                }

                #endregion

                #region IOs

                Match match = Regex.Match(userAgent, @"(?<version>\d+(_\d+)+) like Mac OS X");
                if (match.Length > 0)
                {
                    string version = match.Groups["version"].ToString().Replace("_", ".");
                    return string.Format("IOs {0}", version);
                }

                #endregion

                #region Other mobile os

                var osRegex =
                    new Regex(
                        @"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino",
                        RegexOptions.IgnoreCase | RegexOptions.Multiline);
                var deviceRegex =
                    new Regex(
                        @"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-",
                        RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string deviceInfo = string.Empty;

                if (osRegex.IsMatch(userAgent))
                {
                    deviceInfo = osRegex.Match(userAgent).Groups[0].Value;
                }
                if (deviceRegex.IsMatch(userAgent.Substring(0, 4)))
                {
                    deviceInfo += deviceRegex.Match(userAgent).Groups[0].Value;
                }
                if (!string.IsNullOrEmpty(deviceInfo))
                {
                    return deviceInfo;
                }

                #endregion
            }

            return string.Empty;
        }

        #endregion
    }
}
