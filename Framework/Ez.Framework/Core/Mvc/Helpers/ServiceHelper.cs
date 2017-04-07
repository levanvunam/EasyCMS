using Ez.Framework.Configurations;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Services;
using Ez.Framework.Utilities.Time;
using System;

namespace Ez.Framework.Core.Mvc.Helpers
{
    /// <summary>
    /// Base service helper
    /// </summary>
    public abstract class ServiceHelper
    {
        protected readonly ILocalizedResourceService LocalizedResourceService;

        protected ServiceHelper()
        {
            LocalizedResourceService = HostContainer.GetInstance<ILocalizedResourceService>();
        }

        /// <summary>
        /// Get localize resource by key, if not found then insert and set default value
        /// </summary>
        /// <param name="textKey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string T(string textKey, string defaultValue = null)
        {
            return LocalizedResourceService.T(textKey, defaultValue);
        }

        /// <summary>
        /// Get localize resource by key, then format the result
        /// </summary>
        /// <param name="textKey"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string TFormat(string textKey, params object[] parameters)
        {
            return LocalizedResourceService.TFormat(textKey, parameters);
        }

        /// <summary>
        /// Get date range
        /// </summary>
        /// <param name="dateRange"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        public void GetDateRange(CommonEnums.DateRange? dateRange, out DateTime dateFrom, out DateTime dateTo)
        {
            DateTime now = DateTime.UtcNow;
            switch (dateRange)
            {
                case CommonEnums.DateRange.Today:
                    dateFrom = now.ToBeginDate();
                    dateTo = now.ToEndDate();
                    break;
                case CommonEnums.DateRange.Yesterday:
                    var yesterday = now.AddDays(-1);
                    dateFrom = yesterday.ToBeginDate();
                    dateTo = yesterday.ToEndDate();
                    break;
                case CommonEnums.DateRange.CurrentWeek:
                    dateFrom = now.StartOfWeek();
                    dateTo = now;
                    break;
                case CommonEnums.DateRange.CurrentMonth:
                    dateFrom = new DateTime(now.Year, now.Month, 1);
                    dateTo = now;
                    break;
                case CommonEnums.DateRange.CurrentYearToDate:
                    dateFrom = new DateTime(now.Year, 1, 1);
                    dateTo = now;
                    break;
                case CommonEnums.DateRange.PreviousWeek:
                    dateFrom = now.StartOfWeek().AddDays(-7);
                    dateTo = dateFrom.AddDays(7);
                    break;
                case CommonEnums.DateRange.PreviousMonth:
                    dateFrom = new DateTime(now.Year, now.Month - 1, 1);
                    dateTo = new DateTime(now.Year, now.Month, 1);
                    break;
                case CommonEnums.DateRange.PreviousYear:
                    dateFrom = new DateTime(now.Year - 1, 1, 1);
                    dateTo = new DateTime(now.Year, 1, 1);
                    break;
                default:
                    dateFrom = DateTime.MinValue;
                    dateTo = DateTime.MaxValue;
                    break;
            }
        }

        /// <summary>
        /// Build the unique key from language and text key
        /// </summary>
        /// <param name="language"></param>
        /// <param name="textKey"></param>
        /// <returns></returns>
        public static string BuildKey(string language, string textKey)
        {
            return string.Format("{0}{1}{2}", language, FrameworkConstants.LocalizeResourceSeperator, textKey);
        }
    }
}
