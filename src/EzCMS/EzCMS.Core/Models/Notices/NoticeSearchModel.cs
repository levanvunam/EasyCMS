using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.NoticeTypes;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Notices
{
    public class NoticeSearchModel
    {
        #region Constructors

        public NoticeSearchModel()
        {
            var noticetypeService = HostContainer.GetInstance<INoticeTypeService>();
            NoticeTypes = noticetypeService.GetNoticeTypes();
        }

        #endregion

        #region Public Properties

        public string Keyword { get; set; }

        public int? NoticeTypeId { get; set; }

        public IEnumerable<SelectListItem> NoticeTypes { get; set; }

        #endregion
    }
}
