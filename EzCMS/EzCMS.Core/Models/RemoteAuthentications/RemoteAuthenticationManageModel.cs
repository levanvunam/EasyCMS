using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.RemoteAuthentications
{
    public class RemoteAuthenticationManageModel
    {
        #region Constructors

        public RemoteAuthenticationManageModel()
        {
        }

        public RemoteAuthenticationManageModel(RemoteAuthentication remoteAuthentication)
            : this()
        {
            Id = remoteAuthentication.Id;
            Name = remoteAuthentication.Name;
            ServiceUrl = remoteAuthentication.ServiceUrl;
            AuthorizeCode = remoteAuthentication.AuthorizeCode;
            RecordOrder = remoteAuthentication.RecordOrder;
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("RemoteAuthentication_Field_Id")]
        public int? Id { get; set; }

        [LocalizedDisplayName("RemoteAuthentication_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("RemoteAuthentication_Field_ServiceUrl")]
        public string ServiceUrl { get; set; }

        [LocalizedDisplayName("RemoteAuthentication_Field_AuthorizeCode")]
        public string AuthorizeCode { get; set; }

        [LocalizedDisplayName("RemoteAuthentication_Field_Active")]
        public bool Active { get; set; }

        [LocalizedDisplayName("RemoteAuthentication_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion
    }
}
