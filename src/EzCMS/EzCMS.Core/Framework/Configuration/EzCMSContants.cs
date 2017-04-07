namespace EzCMS.Core.Framework.Configuration
{
    /// <summary>
    /// EzCMS business constants
    /// </summary>
    public class EzCMSContants
    {
        #region Reflection

        public const string EzCMSCoreProject = "EzCMS.Core";

        #endregion

        #region Background Task Name

        public const string EmailTaskName = "Email Sending Task";
        public const string CleanupLogsTaskName = "Cleanup Logs Task";
        public const string NotificationTaskName = "Notification Task";
        public const string SlideInHelpUpdatingTaskName = "Slide In Help Updating Task";
        public const string SubscriptionDirectlyTaskName = "Subscription Directly Task";
        public const string SubscriptionNightlyTaskName = "Subscription Nightly Task";
        public const string AccountExpiresNotificationTaskName = "Account Expires Notification Task";
        public const string EzCMSDailyTaskName = "Ez Daily Task";
        public const string DeactivationExpiredAccountsTaskName = "Deactivation Expired Accounts Task";

        #endregion

        #region Context Key

        #region Applications

        public const string TemplateCaches = ":::TemplateCaches:::";
        public const string AvailableResourceManagers = ":::AvailableResourceManagers:::";
        public const string EzCMSVersion = ":::EzCMSVersion:::";
        public const string SlideInHelpDictionary = ":::SlideInHelpDictionary:::";
        public const string LocalizedResourceDictionary = ":::LocalizedResourceDictionary:::";
        public const string ApplicationWidgets = ":::ApplicationWidgets:::";
        public const string NotFoundHandledUrls = ":::NotFoundHandledUrls:::";
        public const string GoogleAnalyticAccessToken = ":::GoogleAnalyticAccessToken:::";

        #endregion

        #region Sessions

        public const string CurrentUser = ":::CurrentUser:::";
        public const string CurrentCuture = ":::CurrentCuture:::";
        public const string SetupSessionKey = ":::SetupSessionKey:::";
        public const string LinkedInCallbackId = ":::LinkedInCallbackId:::";

        public const string CurrentPageStatusCode = ":::CurrentPageStatusCode:::";
        public const string ActivePageId = "ActivePageId";

        #endregion

        #region Cookies

        public const string CurrentContactInformation = ":::CurrentContactInformation:::";
        public const string CurrentTimezone = ":::CurrentTimezone:::";

        #endregion

        #endregion

        #region Email

        public const string NotificationEmail = "notifications@customercommunity.com.au";

        public const string EasyCMSEmail = "weded9@interactivepartners.com.au";

        public const string LanguageResourceNamespaceFormat = "EzCMS.Core.Core.Resources.Languages.Resources_{0}";
        public const string DefaultLanguageResourceNamespace = "EzCMS.Core.Core.Resources.Languages.Resources";

        #endregion

        #region Razor Constants

        public const string RenderBody = "@RenderBody()";

        public const string RenderBodyWidget = "{RenderBody}";

        public const string RenderPageContent = "@Raw(Model.Content)";

        public const string RenderSection = "@RenderSection";

        public const string DbTemplate = "DbTemplate";

        public const string RazorModel = "Model";

        #endregion

        #region Client Navigation

        public const int OrderMultipleTimes = 100;

        #endregion

        #region Media

        public const string NoImage = "/Images/no-image.png";

        public const string ProtectedDocumentPath = "/ProtectedDocuments";

        public const string NoWidget = "/Images/Widgets/NoWidget.png";

        public const string NoAvatar = "/Images/noavatar.png";

        public const string AvatarFolder = "/Media/uploads/Avatars/";

        public const string UploadFolder = "~/Images/uploads/";

        public const string CssResourceUrl = "/Resources/{0}.css";

        public const string ScriptResourceUrl = "/Resources/{0}.js";

        public const string MediaAssociatePath = "/Media/Uploads/Associates";
        public const string MediaBannerPath = "/Media/Uploads/Banners";
        public const string MediaBodyTemplatePath = "/Media/Uploads/BodyTemplates";
        public const string MediaHotelPath = "/Media/Uploads/Hotels";
        public const string MediaLocationPath = "/Media/Uploads/Locations";
        public const string MediaLocationTypePath = "/Media/Uploads/LocationTypes";
        public const string MediaNewsPath = "/Media/Uploads/News";
        public const string MediaRotatingImagesPath = "/Media/Uploads/RotatingImages";
        public const string MediaServicePath = "/Media/Uploads/Services";
        public const string MediaTestimonialPath = "/Media/Uploads/Testimonials";

        #endregion

        #region Plugins

        public const string BinDirectory = "bin";

        public const string PluginFolder = "Plugins";

        public const string PluginInformationFileName = "Plugin.txt";

        #endregion

        #region Route

        public const string EzCMSPageFriendlyUrlRoute = "FriendlyUrl";

        #endregion

        #region Form Builder

        public const string RemoveElementClass = "remove-on-render";
        public const string AddElementClass = "add-on-render";

        #endregion

        #region Polls

        public const string VotedUserCookie = ":::VotedPoll:::{0}";
        public const int PollCookieExpireDay = 500;

        #endregion

        #region Link Tracker

        public const int LinkTrackerCookieExpireDay = 1;

        #endregion

        #region Widgets

        public const string WidgetImagePathFormat = "/Images/Widgets/{0}.png";

        public const string WidgetSeparator = "_";

        public const int MaxPropertyLoop = 3;

        public const int MaxPropertyParsingLoop = 2;

        #endregion

        #region Google Maps

        public const string GoogleMapDefaultAddress = "Sydney, Australia";
        public const double GoogleMapDefaultLatitude = -33.868356;
        public const double GoogleMapDefaultLongitude = 151.203121;
        public const int GoogleMapDefaultZoom = 13;
        public const int GoogleMapDefaultZoomFrom = 5;
        public const int GoogleMapDefaultZoomTo = 18;

        #endregion

        #region SQL Tool

        public const int DefaultHistoryLength = 20;

        public const int DefaultHistoryStart = 0;

        #endregion

        public const string FileTemplateId = "FileTemplateId";

        public const string EzCMSServiceUrl = "http://help.Ezcms.com/";

        public const string GoogleBlogRssUrlFormat = "https://{0}.blogspot.com/feeds/posts/default?alt=rss";
    }
}
