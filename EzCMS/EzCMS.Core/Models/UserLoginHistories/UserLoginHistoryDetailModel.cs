using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.UserLoginHistories
{
    public class UserLoginHistoryDetailModel
    {
        public UserLoginHistoryDetailModel()
        {
        }

        public UserLoginHistoryDetailModel(UserLoginHistory userLoginHistory)
            : this()
        {
            Id = userLoginHistory.Id;

            UserId = userLoginHistory.UserId;
            Username = userLoginHistory.UserId.HasValue ? (string.IsNullOrEmpty(userLoginHistory.User.Username) ? userLoginHistory.User.Email : userLoginHistory.User.Username) : string.Empty;

            IpAddress = userLoginHistory.IpAddress;
            Platform = userLoginHistory.Platform;
            OsVersion = userLoginHistory.OsVersion;
            BrowserType = userLoginHistory.BrowserType;
            BrowserName = userLoginHistory.BrowserName;
            BrowserVersion = userLoginHistory.BrowserVersion;
            MajorVersion = userLoginHistory.MajorVersion;
            MinorVersion = userLoginHistory.MinorVersion;
            IsBeta = userLoginHistory.IsBeta;
            IsCrawler = userLoginHistory.IsCrawler;
            IsAOL = userLoginHistory.IsAOL;
            IsWin16 = userLoginHistory.IsWin16;
            IsWin32 = userLoginHistory.IsWin32;
            SupportsFrames = userLoginHistory.SupportsFrames;
            SupportsTables = userLoginHistory.SupportsTables;
            SupportsCookies = userLoginHistory.SupportsCookies;
            SupportsVBScript = userLoginHistory.SupportsVBScript;
            SupportsJavaScript = userLoginHistory.SupportsJavaScript;
            SupportsJavaApplets = userLoginHistory.SupportsJavaApplets;
            SupportsActiveXControls = userLoginHistory.SupportsActiveXControls;
            JavaScriptVersion = userLoginHistory.JavaScriptVersion;
            RawAgentString = userLoginHistory.RawAgentString;

            Created = userLoginHistory.Created;
            CreatedBy = userLoginHistory.CreatedBy;
            LastUpdate = userLoginHistory.LastUpdate;
            LastUpdateBy = userLoginHistory.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        public int? UserId { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_User")]
        public string Username { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_IpAddress")]
        public string IpAddress { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_Platform")]
        public string Platform { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_OsVersion")]
        public string OsVersion { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_BrowserType")]
        public string BrowserType { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_BrowserName")]
        public string BrowserName { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_BrowserVersion")]
        public string BrowserVersion { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_MajorVersion")]
        public int MajorVersion { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_MinorVersion")]
        public double MinorVersion { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_IsBeta")]
        public bool IsBeta { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_IsCrawler")]
        public bool IsCrawler { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_IsAOL")]
        public bool IsAOL { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_IsWin16")]
        public bool IsWin16 { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_IsWin32")]
        public bool IsWin32 { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_SupportsFrames")]
        public bool SupportsFrames { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_SupportsTables")]
        public bool SupportsTables { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_SupportsCookies")]
        public bool SupportsCookies { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_SupportsVBScript")]
        public bool SupportsVBScript { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_SupportsJavaScript")]
        public string SupportsJavaScript { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_SupportsJavaApplets")]
        public bool SupportsJavaApplets { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_SupportsActiveXControls")]
        public bool SupportsActiveXControls { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_JavaScriptVersion")]
        public string JavaScriptVersion { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_RawAgentString")]
        public string RawAgentString { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("UserLoginHistory_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        #endregion
    }
}
