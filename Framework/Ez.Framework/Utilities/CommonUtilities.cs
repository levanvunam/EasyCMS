using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace Ez.Framework.Utilities
{
    /// <summary>
    /// Common utilities
    /// </summary>
    public static class CommonUtilities
    {
        /// <summary>
        /// Get price including tax
        /// </summary>
        /// <param name="price"></param>
        /// <param name="taxRateInPercent"></param>
        /// <returns></returns>
        public static double GetPriceIncludeTax(this double price, double taxRateInPercent)
        {
            return (price + price * taxRateInPercent / 100).RoundToMoneyFormat();
        }

        /// <summary>
        /// Round money format
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public static double RoundToMoneyFormat(this double price)
        {
            return Math.Round(price, 2);
        }

        #region Html helper

        /// <summary>
        /// Append value to current dictionary
        /// </summary>
        /// <param name="routeValueDictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static RouteValueDictionary Append(this RouteValueDictionary routeValueDictionary, string key,
            string value)
        {

            if (routeValueDictionary.Any(a => a.Key.Equals(key)))
            {
                var currentValue = routeValueDictionary[key];
                routeValueDictionary.Remove(key);
                routeValueDictionary.Add(key, currentValue + " " + value);
            }
            else
            {
                routeValueDictionary.Add(key, value);
            }
            return routeValueDictionary;
        }

        /// <summary>
        /// Append value to current dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDictionary<string, string> Append(this IDictionary<string, string> dictionary, string key,
            string value)
        {

            if (dictionary.Any(a => a.Key.Equals(key)))
            {
                var currentValue = dictionary[key];
                dictionary.Remove(key);
                dictionary.Add(key, currentValue + " " + value);
            }
            else
            {
                dictionary.Add(key, value);
            }
            return dictionary;
        }

        /// <summary>
        /// Replace value to current dictionary
        /// </summary>
        /// <param name="routeValueDictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static RouteValueDictionary Replace(this RouteValueDictionary routeValueDictionary, string key,
            string value)
        {
            if (routeValueDictionary.Any(a => a.Key.Equals(key)))
            {
                routeValueDictionary.Remove(key);
                routeValueDictionary.Add(key, value);
            }
            else
            {
                routeValueDictionary.Add(key, value);
            }
            return routeValueDictionary;
        }

        /// <summary>
        /// Place value to current dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDictionary<string, string> Replace(this IDictionary<string, string> dictionary, string key,
            string value)
        {
            if (dictionary.Any(a => a.Key.Equals(key)))
            {
                dictionary.Remove(key);
                dictionary.Add(key, value);
            }
            else
            {
                dictionary.Add(key, value);
            }
            return dictionary;
        }

        /// <summary>
        /// Fix under score attribute
        /// </summary>
        /// <param name="routeValueDictionary"></param>
        /// <returns></returns>
        public static RouteValueDictionary FixUnderScoreAttribute(this RouteValueDictionary routeValueDictionary)
        {
            foreach (var item in routeValueDictionary)
            {
                if (item.Key.Contains("_"))
                {
                    var currentValue = item.Value.ToString();
                    routeValueDictionary.Remove(item.Key);
                    routeValueDictionary.Append(item.Key.Replace("_", "-"), currentValue);

                    return routeValueDictionary.FixUnderScoreAttribute();
                }
            }
            return routeValueDictionary;
        }

        #endregion
    }
}
