namespace EzCMS.Entity.Core.Enums
{
    public class PageEnums
    {
        public enum PageStatus
        {
            Online = 1,
            Draft = 2,
            Offline = 3
        }

        public enum PagePosition
        {
            Before = 1,
            After = 2
        }

        public enum PageResponseCode
        {
            Ok = 0,
            RequiredSSL,
            FileTemplateRedirect,
            PageDraft,
            PageOffline,
            Redirect301
        }

        public enum SEOScore
        {
            Good,
            Medium,
            Bad,
            VeryBad
        }
    }
}
