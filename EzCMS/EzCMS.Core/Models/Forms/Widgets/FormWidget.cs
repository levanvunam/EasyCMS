using System.Globalization;
using System.Web;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Forms.Widgets
{
    public class FormWidget
    {
        public FormWidget()
        {
        }

        public FormWidget(Form form)
            : this()
        {
            var encryptId = PasswordUtilities.ComplexEncrypt(form.Id.ToString(CultureInfo.InvariantCulture));
            ScriptUrl = UrlUtilities.GenerateUrl(HttpContext.Current.Request.RequestContext, "SiteApi", "ScriptLoader",
                new { area = "", f = encryptId }, true);
            IframeUrl = UrlUtilities.GenerateUrl(HttpContext.Current.Request.RequestContext, "SiteApi", "IframeLoader",
                new { area = "", f = encryptId }, true);
        }

        #region Public Properties

        public string IframeUrl { get; set; }

        public string ScriptUrl { get; set; }

        #endregion
    }
}
