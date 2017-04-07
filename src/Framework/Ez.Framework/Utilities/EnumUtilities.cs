using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Ez.Framework.Utilities
{
    /// <summary>
    /// Enum utilities
    /// </summary>
    public static class EnumUtilities
    {
        /// <summary>
        /// Gets all items for an enum type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAllItems<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static IEnumerable<SelectListItem> GenerateSelectListItems<T>(GenerateEnumType generateType = GenerateEnumType.IntValueAndStringText,
            T selectedValue = default(T)) where T : struct
        {
            IEnumerable<SelectListItem> selectList;
            switch (generateType)
            {
                case GenerateEnumType.IntValueAndStringText:
                    selectList = GetAllItems<T>().Select(e => new SelectListItem
                    {
                        Text = e.ToString(),
                        Value = Convert.ToInt32(e).ToString(CultureInfo.InvariantCulture),
                        Selected = selectedValue.Equals(e)
                    });
                    break;
                case GenerateEnumType.IntValueAndIntText:
                    selectList = GetAllItems<T>().Select(e => new SelectListItem
                    {
                        Text = Convert.ToInt32(e).ToString(CultureInfo.InvariantCulture),
                        Value = Convert.ToInt32(e).ToString(CultureInfo.InvariantCulture),
                        Selected = selectedValue.Equals(e)
                    });
                    break;
                case GenerateEnumType.IntValueAndDescriptionText:
                    selectList = GetAllItems<T>().Select(e => new SelectListItem
                    {
                        Text = e.GetEnumName(),
                        Value = Convert.ToInt32(e).ToString(CultureInfo.InvariantCulture),
                        Selected = selectedValue.Equals(e)
                    });
                    break;
                case GenerateEnumType.StringValueAndDescriptionText:
                    selectList = GetAllItems<T>().Select(e => new SelectListItem
                    {
                        Text = e.GetEnumName(),
                        Value = e.ToString(),
                        Selected = selectedValue.Equals(e)
                    });
                    break;
                case GenerateEnumType.StringValueAndStringText:
                    selectList = GetAllItems<T>().Select(e => new SelectListItem
                    {
                        Text = e.ToString(),
                        Value = e.ToString(),
                        Selected = selectedValue.Equals(e)
                    });
                    break;
                case GenerateEnumType.DescriptionValueAndDescriptionText:
                    selectList = GetAllItems<T>().Select(e => new SelectListItem
                    {
                        Text = e.GetEnumName(),
                        Value = e.GetEnumName(),
                        Selected = selectedValue.Equals(e)
                    });
                    break;
                default:
                    selectList = new List<SelectListItem>();
                    break;
            }
            return selectList;
        }

        #region Generate Select List

        /// <summary>
        /// Gets all items for an enum type as Text = Name, Value = Id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GenerateSelectListExcept<T>(int value) where T : struct
        {
            return GetAllItems<T>().Where(c => Convert.ToInt32(c) != value).Select(e => new SelectListItem
            {
                Text = e.ToString(),
                Value = Convert.ToInt32(e).ToString(CultureInfo.InvariantCulture)
            });
        }
        #endregion

        #region Helpers

        /// <summary>
        /// Determines whether the enum value contains a specific value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// <c>true</c> if value contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// <code>
        /// EnumExample dummy = EnumExample.Combi;
        /// if (dummy.Contains<EnumExample>(EnumExample.ValueA))
        /// {
        /// Console.WriteLine("dummy contains EnumExample.ValueA");
        /// }
        /// </code>
        /// </example>
        public static bool Contains<T>(this Enum value, T request)
        {
            var valueAsInt = Convert.ToInt32(value, CultureInfo.InvariantCulture);
            var requestAsInt = Convert.ToInt32(request, CultureInfo.InvariantCulture);

            return requestAsInt.Equals(valueAsInt & requestAsInt);
        }

        public static T GetValueFromDescription<T>(this string description)
        {
            var type = typeof(T);

            if (!type.IsEnum)
                throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                if (attribute != null)
                {
                    if (attribute.Description.Equals(description))
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name.Equals(description))
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", "description"); // or return default(T);
        }

        public static IEnumerable<Enum> GetValues(Enum enumeration)
        {
            return enumeration.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fieldInfo => (Enum)fieldInfo.GetValue(enumeration)).ToList();
        }

        public static string GetEnumName<T>(this int value)
        {
            try
            {
                var enumValue = (T)Enum.ToObject(typeof(T), value);
                return GetEnumName(enumValue);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetEnumName<T>(this T value)
        {
            try
            {
                var fi = value.GetType().GetField(value.ToString());

                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                return attributes.Length > 0 ? attributes[0].Description : value.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Convert string to enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumString"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string enumString, T defaultValue = default(T)) where T : struct
        {
            T result;
            if (Enum.TryParse(enumString, true, out result))
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Convert string to enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"> </param>
        /// <returns></returns>
        public static T ToEnum<T>(this int enumValue)
        {
            return (T)(object)enumValue;
        }

        #endregion
    }

    public enum GenerateEnumType
    {
        /// <summary>
        /// Load value from enum value and text from enum name
        /// </summary>
        IntValueAndStringText = 1,

        /// <summary>
        /// Load value from enum name and text from enum name
        /// </summary>
        StringValueAndStringText = 2,

        /// <summary>
        /// Load value from enum value and text from enum value
        /// </summary>
        IntValueAndIntText = 3,

        /// <summary>
        /// Load value from enum value and text from enum description, if description not existed return enum name
        /// </summary>
        IntValueAndDescriptionText = 4,

        /// <summary>
        /// Load value from enum name and text from enum description, if description not existed return enum name
        /// </summary>
        StringValueAndDescriptionText = 5,

        /// <summary>
        /// Load value from enum description and text from enum description, if description not existed return enum name
        /// </summary>
        DescriptionValueAndDescriptionText = 6
    }
}
