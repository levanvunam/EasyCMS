using Ez.Framework.Utilities.Time;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Ez.Framework.Utilities
{
    /// <summary>
    /// Convert utilities
    /// </summary>
    public static class ConvertUtilities
    {
        /// <summary>
        /// Convert string to type
        /// </summary>
        /// <returns></returns>
        public static dynamic ToType(this string input, PropertyInfo property, TimeZoneInfo timezone)
        {
            object value;
            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
            {
                value = input.ToType<DateTime>();
                value = ((DateTime)value).ToUTCTime(timezone);
            }
            else if (property.PropertyType == typeof(TimeSpan) || property.PropertyType == typeof(TimeSpan?))
            {
                value = input.ToType<TimeSpan>();
                value = ((TimeSpan)value).ToUTCTime(timezone);
            }
            else
            {
                var typeConverter = TypeDescriptor.GetConverter(property.PropertyType);
                value = typeConverter.ConvertFromString(input);
            }

            return value;
        }

        public static T ToType<T>(this object value)
        {
            var type = typeof(T);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Nullable type.
                if (value == null)
                {
                    // you may want to do something different here.
                    return default(T);
                }

                // Get the type that was made nullable.
                Type valueType = type.GetGenericArguments()[0];

                // Convert to the value type.
                object result = Convert.ChangeType(value, valueType);

                // Cast the value type to the nullable type.
                return (T)result;
            }
            // Not nullable.
            return (T)Convert.ChangeType(value, typeof(T));
        }

        #region Convert Bool

        public static bool ToBool(this object value, bool defaultValue)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static bool ToBool(this object value)
        {
            return ToBool(value, false);
        }

        #endregion

        #region Convert Date

        public static DateTime ToDate(this object value, string dateFormat)
        {
            return ToDate(value, new DateTime(0), dateFormat);
        }

        public static DateTime? ToNullableDate(this object value, string dateFormat)
        {
            if (value == null || value == DBNull.Value)
                return null;

            try
            {
                return DateTime.ParseExact(value.ToString(), dateFormat, null);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime ToDate(this object value, DateTime defaultValue, string dateFormat)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            try
            {
                return DateTime.ParseExact(value.ToString(), dateFormat, null);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime? ToNullableDate(this object value, DateTime? defaultValue, string dateFormat)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            try
            {
                return DateTime.ParseExact(value.ToString(), dateFormat, null);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime StringToShortDate(string value, string shortDatePattern, DateTime defaultValue, string shortDateFormat)
        {
            try
            {
                var dateInfo = new System.Globalization.DateTimeFormatInfo();

                shortDatePattern = string.IsNullOrEmpty(shortDatePattern) ? shortDateFormat : shortDatePattern;
                dateInfo.ShortDatePattern = shortDatePattern;

                return Convert.ToDateTime(value, dateInfo);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime StringToShortDate(string value, string shortDatePattern, string shortDateFormat)
        {
            return StringToShortDate(value, shortDatePattern, new DateTime(0), shortDateFormat);
        }

        #endregion

        #region Convert Int

        public static int ToInt(this object value, int defaultValue)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int? ToNullableInt(this object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return null;
            }
        }

        public static int ToInt(this object value)
        {
            return ToInt(value, 0);
        }

        #endregion

        #region Convert Long / Float / Double / Decimal

        public static long ToLong(this object value, long defaultValue)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            try
            {
                return Convert.ToInt64(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static float ToFloat(this object value, float defaultValue)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            try
            {
                return Convert.ToSingle(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static double ToDouble(this object value, double defaultValue)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            try
            {
                return Convert.ToDouble(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static decimal ToDecimal(this object value, decimal defaultValue)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string ToString(this object value, string defaultValue)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            try
            {
                return Convert.ToString(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static long ToLong(this object value)
        {
            return ToLong(value, 0);
        }

        public static float ToFloat(this object value)
        {
            return ToFloat(value, 0);
        }

        public static double ToDouble(this object value)
        {
            return ToDouble(value, 0);
        }

        public static decimal ToDecimal(this object value)
        {
            return ToDecimal(value, 0);
        }

        #endregion

        #region Convert Money

        public static string ToMoney(this double? money, bool showEmptyResult = false)
        {
            string content;
            if (money.HasValue)
                content = string.Format("{0:C}", money);
            else
            {
                content = showEmptyResult ? string.Empty : string.Format("{0:C}", 0);
            }

            return content;
        }

        public static string ToMoney(this int money, bool showEmptyResult = false)
        {
            return ToMoney((double)money, showEmptyResult);
        }

        public static string ToMoney(this double money, bool showEmptyResult = false)
        {
            var content = string.Format("{0:C}", money);

            return content;
        }

        #endregion

        /// <summary>
        /// Convert object to string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(this object value)
        {
            return ToString(value, "");
        }

        /// <summary>
        /// Safe text
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SafeText(string value)
        {
            if (value == null)
                return "";

            return value;
        }

        /// <summary>
        /// Compare 2 object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <returns></returns>
        public static bool Compare<T>(T object1, T object2)
        {
            return EqualityComparer<T>.Default.Equals(object1, object2);
        }
    }
}
