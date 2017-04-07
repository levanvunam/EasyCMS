using System.Collections.Generic;

namespace EzCMS.Core.Models.EmailTemplates
{
    public class EmailTemplateResponseModel
    {
        public EmailTemplateResponseModel()
        {
            CCList = new List<string>();
            BCCList = new List<string>();
        }

        #region Public Properties

        public string From { get; set; }

        public string FromName { get; set; }

        public string CC { get; set; }

        public List<string> CCList { get; set; }

        public string BCC { get; set; }

        public List<string> BCCList { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        #endregion
    }
}
