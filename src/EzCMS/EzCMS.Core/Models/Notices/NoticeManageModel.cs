using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using EzCMS.Core.Services.NoticeTypes;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Notices
{
    public class NoticeManageModel
    {
        #region Constructors

        public NoticeManageModel()
        {
            var noticeTypeService = HostContainer.GetInstance<INoticeTypeService>();

            NoticeTypes = Id.HasValue
                ? noticeTypeService.GetNoticeTypes(new List<int> { Id.Value })
                : noticeTypeService.GetNoticeTypes();
        }

        public NoticeManageModel(int? noticeTypeId)
            : this()
        {
            if (noticeTypeId.HasValue)
            {
                NoticeTypeId = noticeTypeId.Value;
            }
        }

        public NoticeManageModel(Notice notice)
            : this()
        {
            Id = notice.Id;
            DateStart = notice.DateStart;
            DateEnd = notice.DateEnd;
            Message = notice.Message;
            IsUrgent = notice.IsUrgent;
            NoticeTypeId = notice.NoticeTypeId;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [StringLength(512)]
        [LocalizedDisplayName("Notice_Field_Message")]
        public string Message { get; set; }

        [LocalizedDisplayName("Notice_Field_IsUrgent")]
        public bool IsUrgent { get; set; }

        [RequiredInteger("Notice_Field_NoticeTypeId")]
        [LocalizedDisplayName("Notice_Field_NoticeTypeId")]
        public int NoticeTypeId { get; set; }

        public IEnumerable<SelectListItem> NoticeTypes { get; set; }

        [LocalizedDisplayName("Notice_Field_DateStart")]
        public DateTime? DateStart { get; set; }

        [DateGreaterThan("DateStart")]
        [LocalizedDisplayName("Notice_Field_DateEnd")]
        public DateTime? DateEnd { get; set; }

        #endregion
    }
}
