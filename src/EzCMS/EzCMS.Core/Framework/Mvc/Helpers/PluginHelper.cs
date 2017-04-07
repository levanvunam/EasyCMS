using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Models.Plugins;
using EzCMS.Entity.Core.SiteInitialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EzCMS.Core.Framework.Mvc.Helpers
{
    public static class PluginHelper
    {
        private const string Name = "Name";
        private const string Description = "Description";
        private const string Version = "Version";
        private const string Author = "Author";

        /// <summary>
        /// Parse the plugin description from a string
        /// </summary>
        /// <param name="manifestText">Input string text of the description</param>
        /// <param name="folder"></param>
        /// <returns>Plugin description</returns>
        public static PluginInformationModel ParseFromString(string manifestText, string folder)
        {
            var pluginDescription = new PluginInformationModel
            {
                IsInstalled = false,
                Folder = folder,
                ConnectionString = string.Empty
            };

            using (var reader = new StringReader(manifestText))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var field = line.Split(new[] { ":" }, 2, StringSplitOptions.None);
                    var fieldLength = field.Length;

                    if (fieldLength != 2)
                        continue;

                    for (var i = 0; i < fieldLength; i++)
                    {
                        field[i] = field[i].Trim();
                    }
                    switch (field[0])
                    {
                        case Name:
                            pluginDescription.Name = field[1];
                            break;
                        case Description:
                            pluginDescription.Description = field[1];
                            break;
                        case Version:
                            pluginDescription.Version = field[1];
                            break;
                        case Author:
                            pluginDescription.Author = field[1];
                            break;
                    }
                }
            }

            return pluginDescription;
        }

        /// <summary>
        /// Parse the plugin description from a string
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Plugin description</returns>
        public static PluginInformationModel ParseFromPath(string path)
        {
            if (File.Exists(path))
            {
                string folder = string.Empty;
                var directoryInfo = new FileInfo(path).Directory;
                if (directoryInfo != null)
                {
                    folder = directoryInfo.Name;
                }
                return ParseFromString(File.ReadAllText(path), folder);
            }

            return null;
        }

        /// <summary>
        /// Get all plugins in Application
        /// </summary>
        /// <returns></returns>
        public static List<PluginInformationModel> GetPlugins()
        {
            var plugins = new List<PluginInformationModel>();
            var pluginRegisters = new List<PluginInformationModel>();

            // Get all plugins in the Application

            var pluginFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            foreach (var plugin in Directory.GetDirectories(pluginFolder, "*", SearchOption.TopDirectoryOnly))
            {
                var pluginInformationFile = Path.Combine(plugin, EzCMSContants.PluginInformationFileName);
                var setupInformation = ParseFromPath(pluginInformationFile);

                if (plugin != null)
                    pluginRegisters.Add(setupInformation);
            }

            // Get installed plugins
            var installPlugins = SiteInitializer.GetConfiguration().Plugins;

            // Check plugin is installed or not
            foreach (var plugin in pluginRegisters)
            {
                if (installPlugins.Any(m => m.Name.Equals(plugin.Name)))
                {
                    plugin.IsInstalled = true;
                    plugin.ConnectionString = installPlugins.First(m => m.Name.Equals(plugin.Name)).ConnectionString;
                }
                plugins.Add(plugin);
            }
            return plugins;
        }

        /// <summary>
        /// Load all installed plugin dlls
        /// </summary>
        /// <returns></returns>
        public static List<string> LoadAllInstalledPluginDlls()
        {
            var applicationBinFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                EzCMSContants.BinDirectory);

            //Get all loaded application dll name
            var applicationDlls = Directory.GetFiles(applicationBinFolder, "*.dll", SearchOption.TopDirectoryOnly).Select(Path.GetFileNameWithoutExtension).ToList();

            var pluginDllPaths = new List<string>();
            var installedPlugins = SiteInitializer.GetConfiguration().Plugins;

            var pluginFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, EzCMSContants.PluginFolder);
            if (!Directory.Exists(pluginFolder))
            {
                try
                {
                    Directory.CreateDirectory(pluginFolder);
                }
                catch (Exception)
                {
                    return new List<string>();
                }
            }
            foreach (var plugin in Directory.GetDirectories(pluginFolder, "*", SearchOption.TopDirectoryOnly))
            {
                var pluginInformationFile = Path.Combine(plugin, EzCMSContants.PluginInformationFileName);
                var pluginInformation = ParseFromPath(pluginInformationFile);
                if (pluginInformation != null && installedPlugins.Any(m => m.Name.Equals(pluginInformation.Name)))
                {

                    var binFolder = Path.Combine(plugin, EzCMSContants.BinDirectory);
                    foreach (var dllFile in Directory.GetFiles(binFolder, "*.dll", SearchOption.TopDirectoryOnly))
                    {
                        var fileName = Path.GetFileNameWithoutExtension(dllFile) ?? string.Empty;

                        var isAdded = pluginDllPaths.Select(Path.GetFileNameWithoutExtension).Any(fileNameWithoutExtension => fileNameWithoutExtension.Equals(dllFile));

                        // Add dll file if it has not been added
                        if (!isAdded && !applicationDlls.Any(fileName.Equals))
                        {
                            pluginDllPaths.Add(dllFile);
                        }
                    }
                }
            }
            return pluginDllPaths;
        }
    }
}
