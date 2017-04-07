using System.ComponentModel;

namespace EzCMS.Entity.Core.Enums
{
    public class RssFeedEnums
    {
        public enum RssType
        {
            [Description("WordPress")]
            Wordpress = 1,

            [Description("Blogger")]
            GoogleBlogger = 2,

            [Description("Others")]
            Others = 3
        }
    }
}
