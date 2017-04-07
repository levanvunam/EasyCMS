using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Plugins
{
    public class PluginInformationModel
    {
        /// <summary>
        /// Is Installed
        /// </summary>
        [LocalizedDisplayName("Plugin_Field_IsInstalled")]
        public bool IsInstalled { get; set; }

        /// <summary>
        /// Plugin Name
        /// </summary>
        [LocalizedDisplayName("Plugin_Field_Name")]
        public string Name { get; set; }

        /// <summary>
        /// Plugin folder
        /// </summary>
        [LocalizedDisplayName("Plugin_Field_Folder")]
        public string Folder { get; set; }

        /// <summary>
        /// Plugin description
        /// </summary>
        [LocalizedDisplayName("Plugin_Field_Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets version information of plugin
        /// </summary>
        [LocalizedDisplayName("Plugin_Field_Version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets author information of plugin
        /// </summary>
        [LocalizedDisplayName("Plugin_Field_Author")]
        public string Author { get; set; }

        /// <summary>
        /// Connection string
        /// </summary>
        [LocalizedDisplayName("Plugin_Field_ConnectionString")]
        public string ConnectionString { get; set; }
    }
}
