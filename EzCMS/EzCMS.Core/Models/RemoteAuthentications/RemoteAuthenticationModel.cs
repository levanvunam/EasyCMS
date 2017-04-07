using Ez.Framework.Models;

namespace EzCMS.Core.Models.RemoteAuthentications
{
    public class RemoteAuthenticationModel : BaseGridModel
    {
        #region Public Properties

        public string Name { get; set; }

        public string ServiceUrl { get; set; }

        public string AuthorizeCode { get; set; }

        #endregion
    }
}
