namespace Ez.Framework.Utilities.Mails.Models
{
    public class EmailModel
    {
        public string From { get; set; }

        public string FromName { get; set; }

        public string To { get; set; }

        public string ToName { get; set; }

        public string CC { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Attachment { get; set; }
    }
}