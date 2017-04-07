using System.Collections.Generic;

namespace EzCMS.Core.Models.Notices.Widgets
{
    public class NoticeboardWidget
    {
        #region Constructors

        public NoticeboardWidget()
        {
            NoticeWidget = new List<NoticeWidget>();
        }

        public NoticeboardWidget(List<NoticeWidget> noticeWidget)
        {
            NoticeWidget = noticeWidget;
        }

        #endregion

        #region Public Properties

        public List<NoticeWidget> NoticeWidget { get; set; }

        #endregion
    }
}
