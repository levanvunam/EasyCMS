namespace EzCMS.Core.Models.EmailLogs
{
    public class SimpleEmailLog
    {
        public string CC { get; set; }

        public string BCC { get; set; }

        public string EmailFrom { get; set; }

        public string MailTo { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
