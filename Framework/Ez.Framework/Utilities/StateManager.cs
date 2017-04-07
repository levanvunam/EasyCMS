using System;
using System.Linq;
using System.Web;

namespace Ez.Framework.Utilities
{
    /// <summary>
    /// Ez state manager
    /// </summary>
    public static class StateManager
    {
        #region Application

        /// <summary>
        /// Get application by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <param name="setState"></param>
        public static T GetApplication<T>(string name, T defaultValue = default(T), bool setState = false)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Application[name] != null)
                {
                    return (T)HttpContext.Current.Application[name];
                }
            }

            //Set default value to state
            if (setState)
            {
                SetApplication(name, defaultValue);
            }

            return defaultValue;
        }

        /// <summary>
        /// Set application
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetApplication<T>(string name, T value)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Application[name] = value;
            }
        }

        #endregion

        #region Session

        /// <summary>
        /// Get session by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <param name="setState"></param>
        public static T GetSession<T>(string name, T defaultValue = default(T), bool setState = false)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                if (HttpContext.Current.Session[name] != null)
                {
                    return (T)HttpContext.Current.Session[name];
                }
            }

            //Set default value to state
            if (setState)
            {
                SetSession(name, defaultValue);
            }

            return defaultValue;
        }

        /// <summary>
        /// Set session
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetSession<T>(string name, T value)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[name] = value;
            }
        }

        #endregion

        #region Short Session

        /// <summary>
        /// Get context item by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <param name="setState"></param>
        public static T GetContextItem<T>(string name, T defaultValue = default(T), bool setState = false)
        {
            if (HttpContext.Current != null)
            {
                var item = HttpContext.Current.Items[name];

                if (item != null)
                {
                    return (T)item;
                }
            }

            //Set default value to state
            if (setState)
            {
                SetContextItem(name, defaultValue);
            }

            return defaultValue;
        }

        /// <summary>
        /// Set context item
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetContextItem<T>(string name, T value)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[name] = value;
            }
        }

        #endregion

        #region Cookie

        /// <summary>
        /// Get cookie by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <param name="setCookie"></param>
        /// <param name="expiredDate"></param>
        public static T GetCookie<T>(string name, T defaultValue = default(T), bool setCookie = false, DateTime? expiredDate = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException();
            }

            if (HttpContext.Current != null)
            {
                //May be cookie name has been encode, need to encode the input too
                var encodeName = HttpUtility.UrlEncode(name);

                var cookieName =
                    HttpContext.Current.Request.Cookies.AllKeys.FirstOrDefault(
                        k => k.Equals(name, StringComparison.CurrentCultureIgnoreCase) ||
                             k.Equals(encodeName, StringComparison.CurrentCultureIgnoreCase));

                if (cookieName != null)
                {
                    var cookie = HttpContext.Current.Request.Cookies[cookieName];
                    if (cookie != null)
                    {
                        return SerializeUtilities.Deserialize<T>(cookie.Value);
                    }
                }
            }

            //Set default value if cookie is not found
            if (setCookie)
            {
                SetCookie(name, defaultValue, expiredDate);
            }

            return defaultValue;
        }

        /// <summary>
        /// Set cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="expiredDate"></param>
        public static void SetCookie<T>(string name, T value, DateTime? expiredDate = null)
        {
            if (HttpContext.Current != null)
            {
                //Create cookie
                HttpCookie cookie;
                if (typeof(T) == typeof(string))
                {
                    cookie = new HttpCookie(name, value.ToString());
                }
                else
                {
                    var cookieValue = SerializeUtilities.Serialize(value);
                    cookie = new HttpCookie(name, cookieValue);
                }

                //Set expired date
                if (!expiredDate.HasValue)
                {
                    expiredDate = DateTime.UtcNow.AddDays(1);
                }
                cookie.Expires = expiredDate.Value;
                cookie.Path = "/";

                HttpContext.Current.Response.Cookies.Set(cookie);
            }
        }

        #endregion
    }
}