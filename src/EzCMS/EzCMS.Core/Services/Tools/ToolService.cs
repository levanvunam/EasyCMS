using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Reflection;
using Ez.Framework.Utilities.Reflection.Enums;
using EzCMS.Core.Models.Tools.CookieManager;
using EzCMS.Core.Models.Tools.SessionManager;
using Newtonsoft.Json.Linq;

namespace EzCMS.Core.Services.Tools
{
    public class ToolService : IToolService
    {
        #region Session Manager

        /// <summary>
        /// Search current sessions.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchSessions(JqSearchIn si, string key)
        {
            var sessions = SearchSessions(key);

            return si.Search(sessions);
        }

        #region Private Methods

        /// <summary>
        /// Search sessions
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private IQueryable<SessionModel> SearchSessions(string key)
        {
            var data = new List<SessionModel>();
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                //Search all sessions
                if (string.IsNullOrEmpty(key))
                {
                    var session = HttpContext.Current.Session.Contents;
                    for (var i = 0; i < session.Count; i++)
                    {
                        data.Add(new SessionModel(session.Keys[i], session[i]));
                    }
                }
                //Search specific session
                else
                {
                    var keyParts = key.Split('.').ToList();
                    object sessionValue = null;
                    if (keyParts.Count() == 1)
                    {
                        sessionValue = HttpContext.Current.Session[key];
                    }
                    else
                    {
                        try
                        {
                            var temp = HttpContext.Current.Session[keyParts[0]];

                            if (temp != null)
                            {
                                keyParts.RemoveAt(0);
                                foreach (var keyPart in keyParts)
                                {
                                    /*
                                     * Check if object is json type
                                     *  If true then convert to dictionary instead of object
                                     */
                                    if (temp is JObject)
                                    {
                                        var jsonObject =
                                            SerializeUtilities.Deserialize<Dictionary<object, object>>(temp.ToString());
                                        temp = jsonObject[keyPart];
                                    }
                                    else
                                    {
                                        temp = temp.GetPropertyValue(keyPart);
                                    }
                                }

                                sessionValue = temp;
                            }
                        }
                        catch
                        {
                        }
                    }
                    if (sessionValue != null)
                    {
                        //Check if object is json type
                        if (sessionValue is JObject)
                        {
                            var session =
                                SerializeUtilities.Deserialize<Dictionary<object, object>>(sessionValue.ToString());
                            data = session.Select(s => new SessionModel(s.Key.ToString(), s.Value)).ToList();
                        }
                        else
                        {
                            var type = sessionValue.GetType();
                            var kind = type.GetKind();

                            if (kind == PropertyKind.Value)
                            {
                                data.Add(new SessionModel(key, sessionValue));
                            }
                            else if (kind == PropertyKind.List)
                            {
                                var i = 0;
                                foreach (var item in (IEnumerable) sessionValue)
                                {
                                    //Adding array name
                                    data.Add(new SessionModel(string.Format("{0}[{1}]", key, i), item)
                                    {
                                        IsComplexType = false
                                    });
                                    i++;
                                }
                            }
                            else if (kind == PropertyKind.Object)
                            {
                                var properties = ReflectionUtilities.GetAllPropertiesOfType(type);
                                data =
                                    properties.Select(
                                        item => new SessionModel(item.Name, sessionValue.GetPropertyValue(item.Name)))
                                        .ToList();
                            }
                        }
                    }
                }
            }

            return data.AsQueryable();
        }

        #endregion

        #endregion

        #region Cookie Manager

        /// <summary>
        /// Search current cookies.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchCookies(JqSearchIn si)
        {
            var cookies = SearchCookies();

            return si.Search(cookies);
        }

        #region Private Methods

        /// <summary>
        /// Search cookies
        /// </summary>
        /// <returns></returns>
        private IQueryable<CookieModel> SearchCookies()
        {
            var data = new List<CookieModel>();
            if (HttpContext.Current != null)
            {
                var cookies = HttpContext.Current.Request.Cookies;
                for (var i = 0; i < cookies.Count; i++)
                {
                    data.Add(new CookieModel(cookies[i]));
                }
            }

            return data.AsQueryable();
        }

        #endregion

        #endregion
    }
}