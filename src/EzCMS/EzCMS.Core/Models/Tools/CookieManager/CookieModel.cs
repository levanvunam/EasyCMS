using System;
using System.Net;
using System.Web;

namespace EzCMS.Core.Models.Tools.CookieManager
{
    public class CookieModel
    {
        public CookieModel()
        {
            
        }

        public CookieModel(HttpCookie cookie)
        {
            Name = WebUtility.HtmlDecode(cookie.Name);
            Value = cookie.Value;
            ExpiredDate = cookie.Expires;
        }

        #region Public Properties

        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime ExpiredDate { get; set; }

        #endregion
    }
}
