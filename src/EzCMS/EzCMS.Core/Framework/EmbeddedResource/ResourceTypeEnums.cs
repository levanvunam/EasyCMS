using System.ComponentModel;

namespace EzCMS.Core.Framework.EmbeddedResource
{
    public enum ResourceTypeEnums
    {
        [Description("Pages")]
        Pages = 1,

        [Description("PageTemplates")]
        PageTemplates = 2,

        [Description("WidgetTemplates")]
        WidgetTemplates = 3,

        [Description("MigrationScripts")]
        MigrationScripts = 4,

        [Description("Styles")]
        Styles = 5,

        [Description("Scripts")]
        Scripts = 6
    }
}