using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Html;
using EzCMS.Entity.Entities.Models;
using System.Web;

namespace EzCMS.Core.Models.Forms.Render
{
    public class FormLoaderScriptModel : FormRenderModel
    {
        public FormLoaderScriptModel()
        {
        }

        public FormLoaderScriptModel(Form form)
            : base(form)
        {
            AllowAjaxSubmit = form.AllowAjaxSubmit;
            Content = Content.RemoveNewLine();

            PostUrl = UrlUtilities.GenerateUrl(HttpContext.Current.Request.RequestContext, "SiteApi", "SubmitForm",
                new { area = "", f = EncryptId, isAjaxRequest = form.AllowAjaxSubmit }, true);

            CurrentSiteUrl = "~/".ToAbsoluteUrl();
        }

        #region Public Properties

        public string PostUrl { get; set; }

        public bool AllowAjaxSubmit { get; set; }

        public string CurrentSiteUrl { get; set; }

        #endregion
    }
}
