using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Services
{
    public class ServiceDetailModel
    {
        public ServiceDetailModel()
        {
        }

        public ServiceDetailModel(Service service)
            : this()
        {
            Id = service.Id;

            Service = new ServiceManageModel(service);

            Created = service.Created;
            CreatedBy = service.CreatedBy;
            LastUpdate = service.LastUpdate;
            LastUpdateBy = service.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("Service_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("Service_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("Service_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("Service_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public ServiceManageModel Service { get; set; }

        #endregion
    }
}
