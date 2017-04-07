namespace Ez.Framework.Utilities.Mails.Models
{
    public class EmailSetting
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public bool EnableSsl { get; set; }

        public int Timeout { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public bool UseDefaultCredentials { get; set; }
    }
}