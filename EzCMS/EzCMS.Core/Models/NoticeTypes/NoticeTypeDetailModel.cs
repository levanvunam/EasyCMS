using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.NoticeTypes
{
    public class NoticeTypeDetailModel
    {
        #region Constructors

        public NoticeTypeDetailModel()
        {
        }

        public NoticeTypeDetailModel(NoticeType noticeType)
            : this()
        {
            Id = noticeType.Id;

            NoticeType = new NoticeTypeManageModel(noticeType);

            Created = noticeType.Created;
            CreatedBy = noticeType.CreatedBy;
            LastUpdate = noticeType.LastUpdate;
            LastUpdateBy = noticeType.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("NoticeType_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("NoticeType_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("NoticeType_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("NoticeType_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public NoticeTypeManageModel NoticeType { get; set; }

        #endregion
    }
}
