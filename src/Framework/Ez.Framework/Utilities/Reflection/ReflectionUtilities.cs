using Ez.Framework.Utilities.Reflection.Attributes;
using Ez.Framework.Utilities.Reflection.Enums;
using Ez.Framework.Utilities.Reflection.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ez.Framework.Utilities.Reflection
{
    /// <summary>
    /// Reflection utilities
    /// </summary>
    public static class ReflectionUtilities
    {
        #region Ez Assembly Helpers

        /// <summary>
        /// Get assemblies with prefix
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssemblies(string prefix)
        {
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            return currentAssemblies.Where(a => a.FullName.StartsWith(prefix));
        }

        /// <summary>
        /// Get loaded assemblies with prefix
        /// </summary>
        /// <returns></returns>
        public static List<AssemblyName> GetReferencedAssemblies(string prefix)
        {
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblies = currentAssemblies.SelectMany(i => i.GetReferencedAssemblies()).
                Where(i => i.FullName.Contains(prefix)).Distinct().ToList();

            return assemblies;
        }

        /// <summary>
        /// Get loaded assembly by name
        /// </summary>
        /// <returns></returns>
        public static Assembly GetAssembly(string name)
        {
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            return currentAssemblies.FirstOrDefault(i => i.FullName.StartsWith(name));
        }

        /// <summary>
        /// Get dynamic type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetDynamicType(string type)
        {
            var returnType = Type.GetType(type);

            if (returnType == null)
            {
                //Cannot convert type, so maybe type is from dynamic assembly
                returnType = Type.GetType(
                    type,
                    (name) =>
                    {
                        // Returns the assembly of the type by enumerating loaded assemblies
                        // in the app domain            
                        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(z => z.FullName == name.FullName);
                    },
                    null,
                    true);
            }

            return returnType;
        }

        #endregion

        #region General Helpers

        /// <summary>
        /// Get all types that implement an interface
        /// </summary>
        /// <param name="type"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllGenericOfClass(Type type, string prefix)
        {
            return GetAssemblies(prefix)
                .SelectMany(s => s.GetTypes())
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == type)
                .Select(x => x.GetGenericArguments()[0]);
            //.Where( x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
        }

        #region Get implementations

        public static IEnumerable<Type> GetAllImplementTypesOfGenericAbstract(Type type, string prefix)
        {
            return GetAssemblies(prefix)
                    .SelectMany(s => s.GetTypes())
                    .Where(t => t.BaseType != null && t.BaseType.IsGenericType &&
                                t.BaseType.GetGenericTypeDefinition() == type);
        }

        #endregion

        /// <summary>
        /// Get all types that implement an interface
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllImplementTypesOf(this Type interfaceType, string prefix)
        {
            try
            {
                return GetAssemblies(prefix)
                    .SelectMany(s => s.GetTypes())
                    .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                //Display or log the error based on your application.
            }

            return new List<Type>();
        }

        /// <summary>
        /// Get all types that implement an interface
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllImplementTypes(this Type interfaceType, string prefix)
        {
            return GetAllImplementTypesOf(interfaceType, prefix);
        }

        /// <summary>
        /// Get all properties from type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetAllPropertiesOfType(Type type)
        {
            return type.GetProperties().ToList();
        }

        /// <summary>
        /// Get all properties from type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetPropertiesHaveAttribute(Type type, Type attributeType)
        {
            return type.GetProperties().Where(p => p.GetCustomAttribute(attributeType) != null).ToList();
        }

        /// <summary>
        /// Get attribute of member
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this MemberInfo memberInfo) where T : class
        {
            var customAttributes = memberInfo.GetCustomAttributes(typeof(T), false);
            var attribute = customAttributes.FirstOrDefault(a => a is T) as T;
            return attribute;
        }

        /// <summary>
        /// Get type name of type
        /// Fix problem with nullable type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeName(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GetGenericArguments()[0].Name;
            }

            if (type == typeof(Nullable<>))
            {
                return Nullable.GetUnderlyingType(type).Name;
            }

            return type.Name;
        }

        /// <summary>
        /// Attempts to set a named property of an entity to an arbitrary value. The value is set if the property is found.
        /// </summary>
        /// <typeparam name="T">An entity deriving of type EntityObject.</typeparam>
        /// <param name="entityToSet">The instance of the entity whose value will be set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value of the property to set.</param>
        public static void SetProperty<T>(this T entityToSet, string propertyName, object value)
        {
            var targetProperty = entityToSet.GetType().GetProperty(propertyName);
            if (targetProperty != null)
                targetProperty.SetValue(entityToSet, value, null);
        }

        /// <summary>
        /// Get all properties from type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<string> GetAllPropertyNamesOfType(Type type)
        {
            var properties = type.GetProperties();
            return properties.Select(p => p.Name).ToList();
        }

        /// <summary>
        /// Get value of property by name
        /// </summary>
        /// <param name="entity">the entity</param>
        /// <param name="propertyName">the property name</param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(this T entity, string propertyName)
        {
            var targetProperty = entity.GetType().GetProperty(propertyName);
            if (targetProperty != null)
                return targetProperty.GetValue(entity);
            return null;
        }

        #endregion

        #region Property Dropdown Builder

        /// <summary>
        /// Generate template
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GeneratePropertyTemplate(string typeName, string name)
        {
            var type = Type.GetType(typeName);
            if (type != null)
            {
                var kind = type.GetKind();

                if (kind == PropertyKind.List)
                {
                    Type baseType = type.GetGenericArguments()[0];
                    var properties = baseType.GetPropertyListFromType(1, "item");
                    const string template = @"@foreach(var item in {0}){{
{1}}}
";
                    const string propertyTemplate = @"    <div>@item.{0}</div>";
                    var propertyBuilder = new StringBuilder();
                    foreach (var item in properties)
                    {
                        if (item.Kind == PropertyKind.Value)
                        {
                            propertyBuilder.AppendLine(string.Format(propertyTemplate, item.Name));
                        }
                    }
                    return string.Format(template, name, propertyBuilder);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Get property list from type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="maxLoop"></param>
        /// <param name="prefix"></param>
        /// <param name="loadedKind"></param>
        /// <returns></returns>
        public static List<PropertyModel> GetPropertyListFromType(this Type type, int maxLoop, string prefix = "", PropertyKind? loadedKind = null)
        {
            var kind = GetKind(type);
            if (kind == PropertyKind.Value)
            {
                return new List<PropertyModel>
                {
                    new PropertyModel
                    {
                        Name = "Model",
                        Type = type.AssemblyQualifiedName,
                        Kind = PropertyKind.Value,
                        Value = prefix
                    }
                };
            }

            if (kind == PropertyKind.List)
            {
                var property = new PropertyModel
                {
                    Name = "List",
                    Type = type.AssemblyQualifiedName,
                    Kind = PropertyKind.List,
                    Value = string.Format("{0}", prefix)
                };

                try
                {
                    Type baseType = type.GetGenericArguments()[0];
                    if (!baseType.IsValueType && baseType != typeof(string))
                    {
                        property.Children = GetPropertyListFromType(baseType, maxLoop - 1, "item");
                    }
                }
                catch (Exception)
                {
                    //Do nothing
                }

                return new List<PropertyModel>
                {
                    property
                };
            }

            if (maxLoop == 0) return new List<PropertyModel>();
            var properties = GetAllPropertiesOfType(type).Where(p => p.GetAttribute<IgnoreInDropdownAttribute>() == null || !p.GetAttribute<IgnoreInDropdownAttribute>().Ignore);
            var result = new List<PropertyModel>();
            foreach (var property in properties)
            {
                var item = new PropertyModel
                {
                    Name = property.Name,
                    Value = string.Format("@{0}.{1}", prefix, property.Name),
                    Type = property.PropertyType.AssemblyQualifiedName,
                    Children = new List<PropertyModel>(),
                    Kind = GetKind(property.PropertyType)
                };

                switch (item.Kind)
                {
                    case PropertyKind.Value:
                        if (loadedKind == null || loadedKind.Value == PropertyKind.Value)
                        {
                            result.Add(item);
                        }
                        break;
                    case PropertyKind.Object:
                        if (loadedKind == null || loadedKind.Value == PropertyKind.Object)
                        {
                            item.Children = GetPropertyListFromType(property.PropertyType, maxLoop - 1, string.Format("{0}.{1}", prefix, item.Name));
                            result.Add(item);
                        }
                        break;
                    case PropertyKind.List:
                        if (loadedKind == null || loadedKind.Value == PropertyKind.List)
                        {
                            item.Value = string.Format("{0}.{1}", prefix, property.Name);
                            try
                            {
                                Type baseType = property.PropertyType.GetGenericArguments()[0];
                                if (!baseType.IsValueType && baseType != typeof(string))
                                {
                                    item.Children = GetPropertyListFromType(baseType, maxLoop - 1, "item");
                                }
                            }
                            catch (Exception)
                            {
                                //Do nothing
                            }
                            result.Add(item);
                        }
                        break;
                }

            }
            return result;
        }

        /// <summary>
        /// Get kind from type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyKind GetKind(this Type type)
        {
            if (type.IsValueType || type == typeof(string))
            {
                return PropertyKind.Value;
            }
            // List object
            if ((type.IsGenericType &&
                (type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                || type.GetGenericTypeDefinition() == typeof(IList<>)
                || type.GetGenericTypeDefinition() == typeof(List<>)))
                || type.IsArray)
            {
                return PropertyKind.List;
            }
            //Object type
            return PropertyKind.Object;
        }

        #endregion
    }
}
