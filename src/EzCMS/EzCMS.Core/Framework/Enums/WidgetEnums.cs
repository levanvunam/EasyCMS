using System.ComponentModel;

namespace EzCMS.Core.Framework.Enums
{
    public enum WidgetType
    {
        [Description("Pages")]
        Pages,
        [Description("EzCMS Content")]
        EzCMSContent,
        [Description("Site Map")]
        SiteMap,
        [Description("Navigation")]
        Navigation,
        [Description("Client Widgets")]
        Client,
    }

    public enum WidgetFieldParsingType
    {
        Normal,
        Raw
    }
}