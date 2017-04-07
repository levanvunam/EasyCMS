using Ez.Framework.Models;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.LinkTrackerClicks
{
    public class LinkTrackerClickModel : BaseGridModel
    {
        #region Constructors

        public LinkTrackerClickModel()
        {
        }

        public LinkTrackerClickModel(LinkTrackerClick linkTrackerClick)
            : base(linkTrackerClick)
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
        }

        #endregion

        #region Public Properties

        public int LinkTrackerId { get; set; }

        public string LinkTrackerName { get; set; }

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
