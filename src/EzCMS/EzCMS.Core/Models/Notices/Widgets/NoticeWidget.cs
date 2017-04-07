using System;

namespace EzCMS.Core.Models.Notices.Widgets
{
    public class NoticeWidget
    {
        #region Public Properties

        public string Message { get; set; }

        public bool IsUrgent { get; set; }

        public int NoticeTypeId { get; set; }

        public string NoticeTypeName { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        #endregion
    }
}
