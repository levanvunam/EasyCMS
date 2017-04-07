using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Notices
{
    public class NoticeDetailModel
    {
        public NoticeDetailModel()
        {
        }

        public NoticeDetailModel(Notice notice)
            : this()
        {
            Id = notice.Id;

            Notice = new NoticeManageModel(notice);

            Created = notice.Created;
            CreatedBy = notice.CreatedBy;
            LastUpdate = notice.LastUpdate;
            LastUpdateBy = notice.LastUpdateBy;

        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("Notice_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("Notice_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("Notice_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("Notice_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public NoticeManageModel Notice { get; set; }

        #endregion
    }
}
