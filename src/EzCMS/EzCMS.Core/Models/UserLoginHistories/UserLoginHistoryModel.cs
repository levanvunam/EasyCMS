using Ez.Framework.Models;

namespace EzCMS.Core.Models.UserLoginHistories
{
    public class UserLoginHistoryModel : BaseGridModel
    {
        #region Public Properties

        public int? UserId { get; set; }

        public string Username { get; set; }

        public string IpAddress { get; set; }

        public string Platform { get; set; }

        public string OsVersion { get; set; }

        public string BrowserType { get; set; }

        public string BrowserName { get; set; }

        public string BrowserVersion { get; set; }

        public int MajorVersion { get; set; }

        public double MinorVersion { get; set; }

        public bool IsBeta { get; set; }

        public bool IsCrawler { get; set; }

        public bool IsAOL { get; set; }

        public bool IsWin16 { get; set; }

        public bool IsWin32 { get; set; }

        public bool SupportsFrames { get; set; }

        public bool SupportsTables { get; set; }

        public bool SupportsCookies { get; set; }

        public bool SupportsVBScript { get; set; }

        public string SupportsJavaScript { get; set; }

        public bool SupportsJavaApplets { get; set; }

        public bool SupportsActiveXControls { get; set; }

        public string JavaScriptVersion { get; set; }

        #endregion
    }
}
