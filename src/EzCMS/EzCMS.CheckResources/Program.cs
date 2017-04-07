using Ez.Framework.Core.Navigations.Attributes;
using Ez.Framework.Utilities;
using EzCMS.Core.Core.Resources.Languages;
using EzCMS.Core.Framework.Utilities;
using EzCMS.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace EzCMS.CheckResources
{
    public class Program
    {
        public static List<string> CheckingDirectories = new List<string>
        {
            "/Framework/Ez.Framework",
            "/EzCMS/EzCMS.Core",
            "/EzCMS/EzCMS.Web/Areas",
            "/EzCMS/EzCMS.Web/Views",
            "/EzCMS/EzCMS.Web/Controllers",
        };

        public static List<string> IgnorePaths = new List<string>
        {
            "/bin",
            "/obj",
            "\\bin",
            "\\obj",
            ".dll",
            ".Designer.cs",
            ".resx"
        };

        public static Dictionary<string, string> SearchDictionary = new Dictionary<string, string>
            {
                {"T(\"{0}", "T(\""},
                {"LocalizedDisplayName(\"{0}", "LocalizedDisplayName(\""},
                {"MText(HtmlTag.Text, \"{0}", "MText(HtmlTag.Text, \""},
                {"MText(HtmlTag.Label, \"{0}", "MText(HtmlTag.Label, \""},
                {"TFormat(\"{0}", "TFormat(\""},
            };

        private const string DebugRunFile = "/EzCMS/EzCMS.CheckResources/bin/Debug/EzCMS.CheckResources.exe";
        private const string RunFile = "/EzCMS/EzCMS.CheckResources/bin/Release/EzCMS.CheckResources.exe";

        public static string FilePath = "/EzCMS/EzCMS.CheckResources/Results/RedundantResources.txt";

        static void Main(string[] args)
        {
            #region Register all modules

            EzCMSConfig.RegisterPluginAssemblies();

            #endregion

            var resourceManager = Resources.ResourceManager;

            //Getting current resource keys
            var availableResources = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            var sb = new StringBuilder();
            var redundantKeys = new List<string>();
            var availableKeys = new List<string>();
            var missingKeys = new List<string>();
            foreach (DictionaryEntry item in availableResources)
            {
                redundantKeys.Add(item.Key.ToString());
                availableKeys.Add(item.Key.ToString());
            }

            #region Navigation

            List<string> navigationResources = new List<string>();
            var assemblies = EzCMSUtilities.GetEzCMSAssemblies();

            var actions = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(Controller).IsAssignableFrom(type))
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                .Where(method => method.IsPublic && !method.IsDefined(typeof(NonActionAttribute)) && method.GetCustomAttributes<NavigationAttribute>(true) != null);

            foreach (var action in actions)
            {
                if (action.DeclaringType != null)
                {
                    var attributes = action.GetCustomAttributes<NavigationAttribute>(true);
                    foreach (var attribute in attributes)
                    {
                        if (attribute != null)
                        {

                            navigationResources.Add(attribute.Name);
                        }
                    }
                }
            }

            redundantKeys.RemoveAll(key => navigationResources.Contains(key));
            navigationResources.RemoveAll(key => availableKeys.Contains(key));
            missingKeys.AddRange(navigationResources);

            #endregion

            #region Find unused resources

            var localPath = Assembly.GetExecutingAssembly().Location.Replace("\\", "/").Replace(RunFile, "").Replace(DebugRunFile, "");

            foreach (var checkingDirectory in CheckingDirectories)
            {
                var fullPath = localPath + checkingDirectory;
                var files = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories);

                var validFiles = files.Where(f => IgnorePaths.All(i => !f.Contains(i))).ToList();

                foreach (var file in validFiles)
                {
                    Console.WriteLine("Checking File: {0}", file);
                    var content = File.ReadAllText(file);

                    var removedKeys = new List<string>();
                    foreach (var key in redundantKeys)
                    {
                        if (content.Contains(string.Format("\"{0}\"", key)))
                        {
                            removedKeys.Add(key);
                        }
                    }

                    foreach (var removedKey in removedKeys)
                    {
                        redundantKeys.Remove(removedKey);
                    }
                }
            }

            #endregion

            #region Find missing resources
            foreach (var checkingDirectory in CheckingDirectories)
            {
                var fullPath = localPath + checkingDirectory;
                var files = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories);

                var validFiles = files.Where(f => IgnorePaths.All(i => !f.Contains(i))).ToList();

                foreach (var file in validFiles)
                {
                    Console.WriteLine("Checking File: {0}", file);
                    var content = File.ReadAllText(file);

                    foreach (var item in SearchDictionary)
                    {
                        var index = -1;
                        do
                        {
                            index = content.IndexOf(item.Value, index + 1, StringComparison.CurrentCulture);

                            if (index >= 0)
                            {
                                var endIndex = content.IndexOf("\"", index + item.Value.Length, StringComparison.CurrentCulture);

                                if (endIndex > 0)
                                {
                                    var localizedResourceMethod = content.Substring(index, endIndex - index);
                                    string[] localizedResources;
                                    if (localizedResourceMethod.TryParseExact(item.Key, out localizedResources, false))
                                    {
                                        var resource = localizedResources[0];

                                        if (!availableKeys.Contains(resource))
                                        {
                                            missingKeys.Add(resource);
                                        }
                                    }
                                }
                            }
                        } while (index >= 0);
                    }
                }
            }

            #endregion

            #region Write files

            Console.Clear();
            Console.WriteLine("Redundant Keys:");
            sb.AppendLine("Redundant Keys:");

            // Write all redundant keys
            foreach (var availableKey in redundantKeys.OrderBy(k => k))
            {
                Console.WriteLine(availableKey);
                sb.AppendLine(availableKey);
            }

            Console.WriteLine();
            Console.WriteLine("Missing Keys:");
            sb.AppendLine("\n\nMissing Keys:");

            // Write all missing keys
            foreach (var missingKey in missingKeys.OrderBy(k => k))
            {
                Console.WriteLine(missingKey);
                sb.AppendLine(missingKey);
            }

            #endregion

            File.WriteAllText(localPath + FilePath, sb.ToString());
            Console.ReadLine();
        }
    }
}
