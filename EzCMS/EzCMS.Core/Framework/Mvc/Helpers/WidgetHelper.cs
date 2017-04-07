using Ez.Framework.Core.IoC;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.WidgetTemplates.TemplateBuilder;
using EzCMS.Core.Services.Widgets.WidgetResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EzCMS.Core.Framework.Mvc.Helpers
{
    public static class WidgetHelper
    {
        #region Resolve content

        /// <summary>
        /// Widget depth
        /// </summary>
        private const int MaxDepth = 5;

        /// <summary>
        /// Resolve content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="maxLoop"></param>
        /// <returns></returns>
        public static string ResolveContent(this string content, int maxLoop = 5)
        {
            bool hasWidgets = true;
            while (hasWidgets && maxLoop > 0)
            {
                content = ResolveWidget(content, out hasWidgets);
                maxLoop--;
            }
            return content;
        }

        /// <summary>
        /// Resolve widget
        /// </summary>
        /// <param name="content"></param>
        /// <param name="hasWidgets"></param>
        /// <returns></returns>
        public static string ResolveWidget(this string content, out bool hasWidgets)
        {
            hasWidgets = false;
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }
            var result = new StringBuilder(content);
            var positions = new Stack<int>();
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] == '{')
                {
                    positions.Push(i);
                }
                else if (result[i] == '}')
                {
                    if (positions.Count > 0)
                    {
                        int beginPos = positions.Pop();
                        if (positions.Count < MaxDepth && i > beginPos + 1)
                        {
                            var match = result.ToString(beginPos + 1, i - beginPos - 1);
                            try
                            {
                                var replacement = ProcessWidgetRenderingToken(match, out hasWidgets);
                                result.Remove(beginPos, i - beginPos + 1);
                                result.Insert(beginPos, replacement);

                                // No widget found so return same value as input
                                if (match.Equals(replacement, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    i = beginPos + replacement.Length;
                                }
                                else
                                {
                                    i = beginPos + replacement.Length - 1;
                                }
                            }
                            catch (Exception)
                            {
                                // Do nothing
                            }
                        }
                    }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Render widget
        /// </summary>
        /// <param name="widgetRenderingParamsString"></param>
        /// <param name="hasWidgets"></param>
        /// <returns></returns>
        public static string ProcessWidgetRenderingToken(this string widgetRenderingParamsString, out bool hasWidgets)
        {
            hasWidgets = false;
            // Function key is the first parameter in the rendering param from {} tags
            var widgetRenderingParams = ParseParams(widgetRenderingParamsString);

            var functionKey = widgetRenderingParams.FirstOrDefault();
            if (functionKey == null)
            {
                return "{" + widgetRenderingParamsString + "}";
            }

            if (WorkContext.Widgets.Any(c => c.Widget.Equals(functionKey)))
            {
                IWidgetResolver instance;

                if (TryResolve(functionKey, out instance))
                {
                    try
                    {
                        // Render the widget in html string with found resolver
                        var widgetRenderedHtml = instance.Render(widgetRenderingParams);
                        hasWidgets = true;

                        return !string.IsNullOrEmpty(widgetRenderedHtml)
                                   ? widgetRenderedHtml
                                   : string.Empty;
                    }
                    catch (Exception ex)
                    {
                        return "{!" + widgetRenderingParamsString + "!Error(" + ex.Message.SafeSubstring(200) + ")}";
                    }
                }
            }

            // If there is no resolver found, then return back the widgetRenderingParams
            return "{" + widgetRenderingParamsString + "}";
        }

        #region Private methods

        /// <summary>
        /// Resolve widget
        /// </summary>
        /// <param name="functionKey"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        private static bool TryResolve(string functionKey, out IWidgetResolver instance)
        {
            instance = HostContainer.GetInstances<IWidgetResolver>().FirstOrDefault(t => t.GetSetup().Widget.Equals(functionKey, StringComparison.InvariantCultureIgnoreCase));

            if (instance != null)
            {
                return true;
            }
            return false;
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// Parse widget into parameters
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public static string[] ParseParams(this string widget)
        {
            if (string.IsNullOrEmpty(widget)) return new string[] { };

            return widget.RemoveString("{").RemoveString("}").Split(new[] { EzCMSContants.WidgetSeparator }, StringSplitOptions.None);
        }

        /// <summary>
        /// Adding widget character at the begin / end of input if not exists
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToWidgetFormat(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException();
            }

            if (!input.StartsWith("{"))
            {
                input = string.Format("{{{0}", input);
            }

            if (!input.EndsWith("}"))
            {
                input = string.Format("{0}}}", input);
            }

            return input;
        }

        #endregion

        /// <summary>
        /// Parse widget properties to razor syntax
        /// </summary>
        /// <returns></returns>
        public static string ParseProperties(this string content, string type, bool usingRaw = false, string prefix = EzCMSContants.RazorModel)
        {
            var dynamicType = ReflectionUtilities.GetDynamicType(type);

            return ParseProperties(content, dynamicType, usingRaw, prefix);
        }

        /// <summary>
        /// Parse widget properties to razor syntax
        /// </summary>
        /// <returns></returns>
        public static string ParseProperties(this string content, Type type, bool usingRaw = false, string prefix = EzCMSContants.RazorModel)
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }

            var modelProperties = GetWidgetParsingFromType(type);

            var result = new StringBuilder(content);
            var positions = new Stack<int>();
            for (var i = 0; i < result.Length; i++)
            {
                if (result[i] == '{')
                {
                    positions.Push(i);
                }
                else if (result[i] == '}')
                {
                    if (positions.Count > 0)
                    {
                        var beginPos = positions.Pop();
                        if (i > beginPos + 1)
                        {
                            var match = result.ToString(beginPos + 1, i - beginPos - 1);
                            if ((WorkContext.Widgets == null || !WorkContext.Widgets.Any(c => c.Widget.Equals(match))) && modelProperties.Any(p => p.Name.Equals(match)))
                            {
                                var property = modelProperties.First(p => p.Name.Equals(match));
                                var replacement = string.Format("@{0}.{1}", prefix, match);
                                if (usingRaw || property.Type == WidgetFieldParsingType.Raw)
                                {
                                    replacement = string.Format("@Raw({0}.{1})", prefix, match);
                                }
                                result.Remove(beginPos, i - beginPos + 1);
                                result.Insert(beginPos, replacement);
                                i = beginPos + replacement.Length - 1;
                            }
                        }
                    }
                }
            }
            return result.ToString();
        }

        #region Generate Full Template 

        /// <summary>
        /// Get full template
        /// </summary>
        /// <param name="content"></param>
        /// <param name="style"></param>
        /// <param name="script"></param>
        /// <param name="dataType"></param>
        /// <param name="widgetString"></param>
        /// <returns></returns>
        public static string GetFullTemplate(string content, string style, string script, string dataType, string widgetString)
        {
            var widgets = SerializeUtilities.Deserialize<List<Shortcut>>(widgetString);

            return GetFullTemplate(content, style, script, dataType, widgets);
        }

        /// <summary>
        /// Get full template
        /// </summary>
        /// <param name="content"></param>
        /// <param name="style"></param>
        /// <param name="script"></param>
        /// <param name="dataType"></param>
        /// <param name="shortcuts"></param>
        /// <returns></returns>
        public static string GetFullTemplate(string content, string style, string script, string dataType, List<Shortcut> shortcuts)
        {
            // Generate content from Content / Style / Script
            content = string.Format(@"{0}{1}{2}{3}{4}", style,
                string.IsNullOrEmpty(style) ? string.Empty : Environment.NewLine, content,
                string.IsNullOrEmpty(script) ? string.Empty : Environment.NewLine, script);

            // Replace widgets
            if (shortcuts != null)
            {
                var maxLoop = EzCMSContants.MaxPropertyParsingLoop;
                while (maxLoop > 0)
                {
                    foreach (var shortcut in shortcuts)
                    {
                        content = content.Replace(string.Format("{{{0}}}", shortcut.Name), shortcut.Content);
                    }
                    maxLoop--;
                }
            }

            // Parse properties
            content = content.ParseProperties(dataType);

            return content;
        }

        #endregion

        /// <summary>
        /// Get all properties from type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<WidgetFieldParsingModel> GetWidgetParsingFromType(Type type)
        {
            var properties = type.GetProperties();
            return properties.Select(p => new WidgetFieldParsingModel
            {
                Name = p.Name,
                Type = p.GetCustomAttributes(typeof(UsingRaw), true).FirstOrDefault() != null ? WidgetFieldParsingType.Raw : WidgetFieldParsingType.Normal
            }).ToList();
        }
    }
}
