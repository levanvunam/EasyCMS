using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.NoticeTypes;
using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Notices.WidgetManageModels
{
    public class NoticeBoardWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public NoticeBoardWidgetManageModel()
        {
        }

        public NoticeBoardWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var noticeTypeService = HostContainer.GetInstance<INoticeTypeService>();
            NoticeTypes = noticeTypeService.GetNoticeTypes();

            ParseParams(parameters);
        }

        #endregion

        #region Parse Params

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * NoticeTypeId
             * * Number
             * * Template 
             */

            //NoticeTypeId
            if (parameters.Length > 1)
            {
                NoticeTypeId = parameters[1].ToInt(0);
            }

            //Number
            if (parameters.Length > 2)
            {
                Number = parameters[2].ToInt(0);
            }

            //Template 
            if (parameters.Length > 3)
            {
                Template = parameters[3];
            }
        }

        #endregion

        #region Public Properties

        [RequiredInteger("Widget_Noticeboard_Field_NoticeTypeId")]
        [LocalizedDisplayName("Widget_Noticeboard_Field_NoticeTypeId")]
        public int NoticeTypeId { get; set; }

        public IEnumerable<SelectListItem> NoticeTypes { get; set; }

        [LocalizedDisplayName("Widget_Noticeboard_Field_Number")]
        public int Number { get; set; }

        #endregion
    }
}
