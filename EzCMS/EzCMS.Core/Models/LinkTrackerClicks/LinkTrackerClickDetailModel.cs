using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.LinkTrackerClicks
{
    public class LinkTrackerClickDetailModel
    {
        public LinkTrackerClickDetailModel()
        {
        }

        public LinkTrackerClickDetailModel(LinkTrackerClick linkTrackerClick)
            : this()
        {
            Id = linkTrackerClick.Id;

            LinkTrackerId = linkTrackerClick.LinkTrackerId;
            LinkTrackerName = linkTrackerClick.LinkTracker.Name;

            IpAddress = linkTrackerClick.IpAddress;
            Platform = linkTrackerClick.Platform;
            OsVersion = linkTrackerClick.OsVersion;
            BrowserType = linkTrackerClick.BrowserType;
            BrowserName = linkTrackerClick.BrowserName;
            BrowserVersion = linkTrackerClick.BrowserVersion;
            MajorVersion = linkTrackerClick.MajorVersion;
            MinorVersion = linkTrackerClick.MinorVersion;
            IsBeta = linkTrackerClick.IsBeta;
            IsCrawler = linkTrackerClick.IsCrawler;
            IsAOL = linkTrackerClick.IsAOL;
            IsWin16 = linkTrackerClick.IsWin16;
            IsWin32 = linkTrackerClick.IsWin32;
            SupportsFrames = linkTrackerClick.SupportsFrames;
            SupportsTables = linkTrackerClick.SupportsTables;
            SupportsCookies = linkTrackerClick.SupportsCookies;
            SupportsVBScript = linkTrackerClick.SupportsVBScript;
            SupportsJavaScript = linkTrackerClick.SupportsJavaScript;
            SupportsJavaApplets = linkTrackerClick.SupportsJavaApplets;
            SupportsActiveXControls = linkTrackerClick.SupportsActiveXControls;
            JavaScriptVersion = linkTrackerClick.JavaScriptVersion;
            RawAgentString = linkTrackerClick.RawAgentString;

            Created = linkTrackerClick.Created;
            CreatedBy = linkTrackerClick.CreatedBy;
            LastUpdate = linkTrackerClick.LastUpdate;
            LastUpdateBy = linkTrackerClick.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        public int LinkTrackerId { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_LinkTrackerName")]
        public string LinkTrackerName { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_IpAddress")]
        public string IpAddress { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_Platform")]
        public string Platform { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_OsVersion")]
        public string OsVersion { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_BrowserType")]
        public string BrowserType { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_BrowserName")]
        public string BrowserName { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_BrowserVersion")]
        public string BrowserVersion { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_MajorVersion")]
        public int MajorVersion { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_MinorVersion")]
        public double MinorVersion { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_IsBeta")]
        public bool IsBeta { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_IsCrawler")]
        public bool IsCrawler { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_IsAOL")]
        public bool IsAOL { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_IsWin16")]
        public bool IsWin16 { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_IsWin32")]
        public bool IsWin32 { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_SupportsFrames")]
        public bool SupportsFrames { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_SupportsTables")]
        public bool SupportsTables { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_SupportsCookies")]
        public bool SupportsCookies { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_SupportsVBScript")]
        public bool SupportsVBScript { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_SupportsJavaScript")]
        public string SupportsJavaScript { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_SupportsJavaApplets")]
        public bool SupportsJavaApplets { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_SupportsActiveXControls")]
        public bool SupportsActiveXControls { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_JavaScriptVersion")]
        public string JavaScriptVersion { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_JavaScriptVersion")]
        public string RawAgentString { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("LinkTrackerClick_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        #endregion
    }
}
