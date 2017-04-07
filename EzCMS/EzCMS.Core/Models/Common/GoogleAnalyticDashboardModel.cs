namespace EzCMS.Core.Models.Common
{
    public class GoogleAnalyticDashboardModel
    {
        public string AccessToken { get; set; }

        public bool IsSetup { get; set; }

        public bool IsSetupInformationValid { get; set; }

        public string Message { get; set; }
    }
}
