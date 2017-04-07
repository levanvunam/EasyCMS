using Ez.Framework.Utilities;
using EzCMS.Core.Framework.EmbeddedResource;
using EzCMS.Core.Models.Widgets;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EzCMS.Core.Framework.Mvc.Helpers
{
    public class DataInitializeHelper
    {
        private const string ResourceNamespace = "EzCMS.Core.Core.Resources";

        /// <summary>
        /// Get resource content
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetResourceContent(string resourceName, DataSetupResourceType type = DataSetupResourceType.None)
        {
            if (type == DataSetupResourceType.None)
            {
                return EmbeddedResourceHelper.GetString(resourceName, ResourceNamespace);
            }
            return EmbeddedResourceHelper.GetString(string.Format("{0}.{1}", type.GetEnumName(), resourceName), ResourceNamespace);
        }

        /// <summary>
        /// Get plugin resource content
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="pluginResourceNameSpace"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetPluginResourceContent(string resourceName, string pluginResourceNameSpace, DataSetupResourceType type = DataSetupResourceType.None)
        {
            if (type == DataSetupResourceType.None)
            {
                return EmbeddedResourceHelper.GetString(resourceName, ResourceNamespace);
            }
            return EmbeddedResourceHelper.GetString(string.Format("{0}.{1}", type.GetEnumName(), resourceName), pluginResourceNameSpace);
        }

        #region Widgets

        #region Core

        /// <summary>
        /// Get widget template content
        /// </summary>
        /// <param name="widgetSetup"></param>
        /// <returns></returns>
        public static string GetTemplateContent(WidgetSetupModel widgetSetup)
        {
            return
                EmbeddedResourceHelper.GetString(
                    string.Format("{0}.{1}.{2}.cshtml", DataSetupResourceType.WidgetTemplate.GetEnumName(),
                        widgetSetup.Widget, widgetSetup.DefaultTemplate), ResourceNamespace);
        }

        /// <summary>
        /// Get widget template content
        /// </summary>
        /// <param name="widgetSetup"></param>
        /// <returns></returns>
        public static string GetTemplateScript(WidgetSetupModel widgetSetup)
        {
            return
                EmbeddedResourceHelper.GetNullableString(
                    string.Format("{0}.{1}.{2}.Script.cshtml", DataSetupResourceType.WidgetTemplate.GetEnumName(),
                        widgetSetup.Widget, widgetSetup.DefaultTemplate), ResourceNamespace);
        }

        /// <summary>
        /// Get widget template content
        /// </summary>
        /// <param name="widgetSetup"></param>
        /// <returns></returns>
        public static string GetTemplateStyle(WidgetSetupModel widgetSetup)
        {
            return
                EmbeddedResourceHelper.GetNullableString(
                    string.Format("{0}.{1}.{2}.Style.cshtml", DataSetupResourceType.WidgetTemplate.GetEnumName(),
                        widgetSetup.Widget, widgetSetup.DefaultTemplate), ResourceNamespace);
        }

        /// <summary>
        /// Get widget template widgets
        /// </summary>
        /// <param name="widgetSetup"></param>
        /// <returns></returns>
        public static string GetTemplateWidgets(WidgetSetupModel widgetSetup)
        {
            try
            {
                var folder = string.Format("{0}.{1}.Widgets", DataSetupResourceType.WidgetTemplate.GetEnumName(), widgetSetup.Widget);
                var widgets = GetAllResourcesInFolder(folder).Select(r => new WidgetTemplate
                {
                    Name = r.Name.Replace(".cshtml", string.Empty),
                    Content = r.Content
                }).ToList();
                return SerializeUtilities.Serialize(widgets);
            }
            catch (ArgumentException)
            {
                // No folder
                return string.Empty;
            }
        }

        /// <summary>
        /// Get widget template content
        /// </summary>
        /// <param name="widgetSetup"></param>
        /// <returns></returns>
        public static string GetTemplateFullContent(WidgetSetupModel widgetSetup)
        {
            var content = GetTemplateContent(widgetSetup);
            var style = GetTemplateStyle(widgetSetup);
            var script = GetTemplateScript(widgetSetup);
            var widgets = GetTemplateWidgets(widgetSetup);

            return WidgetHelper.GetFullTemplate(content, style, script, widgetSetup.Type.AssemblyQualifiedName, widgets);
        }

        #endregion

        #region Plugin

        /// <summary>
        /// Get plugin widget template content
        /// </summary>
        /// <param name="widgetSetup"></param>
        /// <param name="pluginResourceNameSpace"></param>
        /// <returns></returns>
        public static string GetPluginTemplateContent(WidgetSetupModel widgetSetup, string pluginResourceNameSpace)
        {
            return
                EmbeddedResourceHelper.GetString(
                    string.Format("{0}.{1}.{2}.cshtml", DataSetupResourceType.WidgetTemplate.GetEnumName(),
                        widgetSetup.Widget, widgetSetup.DefaultTemplate), pluginResourceNameSpace);
        }

        /// <summary>
        /// Get plugin widget template content
        /// </summary>
        /// <param name="widgetSetup"></param>
        /// <param name="pluginResourceNameSpace"></param>
        /// <returns></returns>
        public static string GetPluginTemplateScript(WidgetSetupModel widgetSetup, string pluginResourceNameSpace)
        {
            return
                EmbeddedResourceHelper.GetNullableString(
                    string.Format("{0}.{1}.{2}.Script.cshtml", DataSetupResourceType.WidgetTemplate.GetEnumName(),
                        widgetSetup.Widget, widgetSetup.DefaultTemplate), pluginResourceNameSpace);
        }

        /// <summary>
        /// Get plugin widget template content
        /// </summary>
        /// <param name="widgetSetup"></param>
        /// <param name="pluginResourceNameSpace"></param>
        /// <returns></returns>
        public static string GetPluginTemplateStyle(WidgetSetupModel widgetSetup, string pluginResourceNameSpace)
        {
            return
                EmbeddedResourceHelper.GetNullableString(
                    string.Format("{0}.{1}.{2}.Style.cshtml", DataSetupResourceType.WidgetTemplate.GetEnumName(),
                        widgetSetup.Widget, widgetSetup.DefaultTemplate), pluginResourceNameSpace);
        }

        /// <summary>
        /// Get widget template widgets
        /// </summary>
        /// <param name="widgetSetup"></param>
        /// <param name="pluginResourceNameSpace"></param>
        /// <returns></returns>
        public static string GetPluginTemplateWidgets(WidgetSetupModel widgetSetup, string pluginResourceNameSpace)
        {
            try
            {
                var folder = string.Format("{0}.{1}.Widgets", DataSetupResourceType.WidgetTemplate.GetEnumName(), widgetSetup.Widget);
                var widgets = GetAllResourcesInFolder(folder, DataSetupResourceType.None, pluginResourceNameSpace).Select(r => new WidgetTemplate
                {
                    Name = r.Name.Replace(".cshtml", string.Empty),
                    Content = r.Content
                }).ToList();
                return SerializeUtilities.Serialize(widgets);
            }
            catch (ArgumentException)
            {
                // No folder
                return string.Empty;
            }
        }

        /// <summary>
        /// Get plugin widget template content
        /// </summary>
        /// <param name="widgetSetup"></param>
        /// <param name="pluginResourceNameSpace"></param>
        /// <returns></returns>
        public static string GetPluginTemplateFullContent(WidgetSetupModel widgetSetup, string pluginResourceNameSpace)
        {
            var content = GetPluginTemplateContent(widgetSetup, pluginResourceNameSpace);
            var style = GetPluginTemplateStyle(widgetSetup, pluginResourceNameSpace);
            var script = GetPluginTemplateScript(widgetSetup, pluginResourceNameSpace);
            var widgets = GetPluginTemplateWidgets(widgetSetup, pluginResourceNameSpace);

            return WidgetHelper.GetFullTemplate(content, style, script, widgetSetup.Type.AssemblyQualifiedName, widgets);
        }

        #endregion

        #endregion

        #region Folder

        /// <summary>
        /// Get all resources in forlder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="type"></param>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public static List<EmbeddedResource.EmbeddedResource> GetAllResourcesInFolder(string folder, DataSetupResourceType type = DataSetupResourceType.None, string @namespace = ResourceNamespace)
        {
            if (type == DataSetupResourceType.None)
            {
                return EmbeddedResourceHelper.GetEmbeddedResourcesOfFolder(folder, @namespace);
            }

            if (string.IsNullOrEmpty(folder))
            {
                return EmbeddedResourceHelper.GetEmbeddedResourcesOfFolder(type.GetEnumName(), @namespace);
            }

            return EmbeddedResourceHelper.GetEmbeddedResourcesOfFolder(string.Format("{0}.{1}", type.GetEnumName(), folder), @namespace);
        }

        /// <summary>
        /// Get all resources in forlder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="type"></param>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public static List<string> GetAllResourceNamesInFolder(string folder, DataSetupResourceType type = DataSetupResourceType.None, string @namespace = ResourceNamespace)
        {
            if (type == DataSetupResourceType.None)
            {
                return EmbeddedResourceHelper.GetEmbeddedResourceNamesOfFolder(folder, @namespace);
            }

            if (string.IsNullOrEmpty(folder))
            {
                return EmbeddedResourceHelper.GetEmbeddedResourceNamesOfFolder(type.GetEnumName(), @namespace);
            }

            return EmbeddedResourceHelper.GetEmbeddedResourceNamesOfFolder(string.Format("{0}.{1}", type.GetEnumName(), folder), @namespace);
        }

        #endregion
    }

    public enum DataSetupResourceType
    {
        //Description is resource folder

        [Description("WidgetTemplates")]
        WidgetTemplate = 1,

        [Description("EmailTemplates")]
        EmailTemplate,

        [Description("SiteSettings")]
        SiteSetting,

        [Description("SubscriptionTemplates")]
        SubscriptionTemplate,

        [Description("NotificationTemplates")]
        NotificationTemplate,

        [Description("Languages")]
        Languages,

        [Description("Others")]
        Others,

        [Description("")]
        None
    }
}