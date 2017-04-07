using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ez.Framework.Utilities.Time
{
    /// <summary>
    /// Timezone utilities
    /// </summary>
    public static class TimeZoneUtilities
    {
        #region Properties

        private static readonly Dictionary<string, string> OlsonWindowsTimes = new Dictionary<string, string>
        {
            {"Africa/Abidjan", "Greenwich Standard Time"},
            {"Africa/Accra", "Greenwich Standard Time"},
            {"Africa/Addis_Ababa", "E. Africa Standard Time"},
            {"Africa/Algiers", "W. Central Africa Standard Time"},
            {"Africa/Antananarivo", "E. Africa Standard Time"},
            {"Africa/Asmera", "E. Africa Standard Time"},
            {"Africa/Bamako", "Greenwich Standard Time"},
            {"Africa/Bangui", "W. Central Africa Standard Time"},
            {"Africa/Banjul", "Greenwich Standard Time"},
            {"Africa/Bissau", "Greenwich Standard Time"},
            {"Africa/Blantyre", "South Africa Standard Time"},
            {"Africa/Brazzaville", "W. Central Africa Standard Time"},
            {"Africa/Bujumbura", "South Africa Standard Time"},
            {"Africa/Cairo", "Egypt Standard Time"},
            {"Africa/Casablanca", "Morocco Standard Time"},
            {"Africa/Comoro", "E. Africa Standard Time"},
            {"Africa/Conakry", "Greenwich Standard Time"},
            {"Africa/Dakar", "Greenwich Standard Time"},
            {"Africa/Dar_es_Salaam", "E. Africa Standard Time"},
            {"Africa/Djibouti", "E. Africa Standard Time"},
            {"Africa/Douala", "W. Central Africa Standard Time"},
            {"Africa/El_Aaiun", "Greenwich Standard Time"},
            {"Africa/Freetown", "Greenwich Standard Time"},
            {"Africa/Gaborone", "South Africa Standard Time"},
            {"Africa/Harare", "South Africa Standard Time"},
            {"Africa/Johannesburg", "South Africa Standard Time"},
            {"Africa/Juba", "Greenwich Standard Time"},
            {"Africa/Kampala", "E. Africa Standard Time"},
            {"Africa/Khartoum", "E. Africa Standard Time"},
            {"Africa/Kigali", "South Africa Standard Time"},
            {"Africa/Kinshasa", "W. Central Africa Standard Time"},
            {"Africa/Lagos", "W. Central Africa Standard Time"},
            {"Africa/Lome", "Greenwich Standard Time"},
            {"Africa/Libreville", "W. Central Africa Standard Time"},
            {"Africa/Luanda", "W. Central Africa Standard Time"},
            {"Africa/Lubumbashi", "South Africa Standard Time"},
            {"Africa/Lusaka", "South Africa Standard Time"},
            {"Africa/Malabo", "W. Central Africa Standard Time"},
            {"Africa/Maputo", "South Africa Standard Time"},
            {"Africa/Maseru", "South Africa Standard Time"},
            {"Africa/Mayotte", "E. Africa Standard Time"},
            {"Africa/Mbabane", "South Africa Standard Time"},
            {"Africa/Mogadishu", "E. Africa Standard Time"},
            {"Africa/Monrovia", "Greenwich Standard Time"},
            {"Africa/Nairobi", "E. Africa Standard Time"},
            {"Africa/Ndjamena", "W. Central Africa Standard Time"},
            {"Africa/Niamey", "W. Central Africa Standard Time"},
            {"Africa/Nouakchott", "Greenwich Standard Time"},
            {"Africa/Ouagadougou", "Greenwich Standard Time"},
            {"Africa/Porto-Novo", "W. Central Africa Standard Time"},
            {"Africa/Sao_Tome", "Greenwich Standard Time"},
            {"Africa/Syowa", "E. Africa Standard Time"},
            {"Africa/Tripoli", "South Africa Standard Time"},
            {"Africa/Tunis", "W. Central Africa Standard Time"},
            {"Africa/Windhoek", "Namibia Standard Time"},
            {"America/Anchorage", "Alaskan Standard Time"},
            {"America/Anguilla", "SA Western Standard Time"},
            {"America/Antigua", "SA Western Standard Time"},
            {"America/Araguaina", "SA Eastern Standard Time"},
            {"America/Argentina/La_Rioja", "Argentina Standard Time"},
            {"America/Argentina/Rio_Gallegos", "Argentina Standard Time"},
            {"America/Argentina/Salta", "Argentina Standard Time"},
            {"America/Argentina/San_Juan", "Argentina Standard Time"},
            {"America/Argentina/San_Luis", "Argentina Standard Time"},
            {"America/Argentina/Tucuman", "Argentina Standard Time"},
            {"America/Argentina/Ushuaia", "Argentina Standard Time"},
            {"America/Aruba", "SA Eastern Standard Time"},
            {"America/Asuncion", "Paraguay Standard Time"},
            {"America/Bahia", "Bahia Standard Time"},
            {"America/Bahia_Banderas", "Central Standard Time (Mexico)"},
            {"America/Barbados", "SA Eastern Standard Time"},
            {"America/Belem", "SA Eastern Standard Time"},
            {"America/Belize", "Central America Standard Time"},
            {"America/Bermuda", "Atlantic Standard Time"},
            {"America/Blanc-Sablon", "SA Western Standard Time"},
            {"America/Boa_Vista", "SA Western Standard Time"},
            {"America/Bogota", "SA Pacific Standard Time"},
            {"America/Boise", "Mountain Standard Time"},
            {"America/Buenos_Aires", "Argentina Standard Time"},
            {"America/Cambridge_Bay", "Mountain Standard Time"},
            {"America/Campo_Grande", "Central Brazilian Standard Time"},
            {"America/Cancun", "Central Standard Time (Mexico)"},
            {"America/Caracas", "Venezuela Standard Time"},
            {"America/Catamarca", "Argentina Standard Time"},
            {"America/Cayenne", "SA Eastern Standard Time"},
            {"America/Cayman", "SA Pacific Standard Time"},
            {"America/Chicago", "Central Standard Time"},
            {"America/Chihuahua", "Mountain Standard Time (Mexico)"},
            {"America/Coral_Harbour", "SA Pacific Standard Time"},
            {"America/Cordoba", "Argentina Standard Time"},
            {"America/Costa_Rica", "Central America Standard Time"},
            {"America/Cuiaba", "Central Brazilian Standard Time"},
            {"America/Curacao", "SA Western Standard Time"},
            {"America/Danmarkshavn", "UTC"},
            {"America/Dawson", "Pacific Standard Time"},
            {"America/Dawson_Creek", "US Mountain Standard Time"},
            {"America/Denver", "Mountain Standard Time"},
            {"America/Detroit", "Eastern Standard Time"},
            {"America/Dominica", "SA Western Standard Time"},
            {"America/Edmonton", "Mountain Standard Time"},
            {"America/Eirunepe", "SA Western Standard Time"},
            {"America/El_Salvador", "Central America Standard Time"},
            {"America/Fortaleza", "SA Eastern Standard Time"},
            {"America/Glace_Bay", "Atlantic Standard Time"},
            {"America/Godthab", "Greenland Standard Time"},
            {"America/Goose_Bay", "Atlantic Standard Time"},
            {"America/Grand_Turk", "Eastern Standard Time"},
            {"America/Grenada", "SA Western Standard Time"},
            {"America/Guadeloupe", "SA Western Standard Time"},
            {"America/Guatemala", "Central America Standard Time"},
            {"America/Guayaquil", "SA Pacific Standard Time"},
            {"America/Guyana", "SA Western Standard Time"},
            {"America/Halifax", "Atlantic Standard Time"},
            {"America/Hermosillo", "US Mountain Standard Time"},
            {"America/Indiana/Knox", "Central Standard Time"},
            {"America/Indiana/Louisville", "Central Standard Time"},
            {"America/Indiana/Marengo", "US Eastern Standard Time"},
            {"America/Indiana/Monticello", "Central Standard Time"},
            {"America/Indiana/Petersburg", "Central Standard Time"},
            {"America/Indiana/Tell_City", "Central Standard Time"},
            {"America/Indiana/Vevay", "US Eastern Standard Time"},
            {"America/Indiana/Vincennes", "Central Standard Time"},
            {"America/Indiana/Winamac", "Central Standard Time"},
            {"America/Indianapolis", "US Eastern Standard Time"},
            {"America/Inuvik", "Mountain Standard Time"},
            {"America/Iqaluit", "Eastern Standard Time"},
            {"America/Jamaica", "SA Pacific Standard Time"},
            {"America/Jujuy", "Argentina Standard Time"},
            {"America/Juneau", "Alaskan Standard Time"},
            {"America/La_Paz", "SA Western Standard Time"},
            {"America/Lima", "SA Pacific Standard Time"},
            {"America/Los_Angeles", "Pacific Standard Time"},
            {"America/Maceio", "SA Eastern Standard Time"},
            {"America/Managua", "Central America Standard Time"},
            {"America/Manaus", "SA Western Standard Time"},
            {"America/Marigot", "SA Western Standard Time"},
            {"America/Martinique", "SA Western Standard Time"},
            {"America/Matamoros", "Central Standard Time"},
            {"America/Mazatlan", "Mountain Standard Time (Mexico)"},
            {"America/Menominee", "Central Standard Time"},
            {"America/Mendoza", "Argentina Standard Time"},
            {"America/Merida", "Central Standard Time (Mexico)"},
            {"America/Mexico_City", "Central Standard Time (Mexico)"},
            {"America/Moncton", "Atlantic Standard Time"},
            {"America/Monterrey", "Central Standard Time (Mexico)"},
            {"America/Montevideo", "Montevideo Standard Time"},
            {"America/Montreal", "Eastern Standard Time"},
            {"America/Montserrat", "SA Western Standard Time"},
            {"America/Nassau", "Eastern Standard Time"},
            {"America/New_York", "Eastern Standard Time"},
            {"America/Nipigon", "Eastern Standard Time"},
            {"America/Nome", "Alaskan Standard Time"},
            {"America/Noronha", "UTC-02"},
            {"America/North_Dakota/Beulah", "Central Standard Time"},
            {"America/North_Dakota/Center", "Central Standard Time"},
            {"America/North_Dakota/New_Salem", "Central Standard Time"},
            {"America/Ojinaga", "Mountain Standard Time"},
            {"America/Panama", "SA Pacific Standard Time"},
            {"America/Pangnirtung", "Eastern Standard Time"},
            {"America/Paramaribo", "SA Eastern Standard Time"},
            {"America/Phoenix", "US Mountain Standard Time"},
            {"America/Port_of_Spain", "SA Western Standard Time"},
            {"America/Port-au-Prince", "SA Pacific Standard Time"},
            {"America/Porto_Velho", "SA Western Standard Time"},
            {"America/Puerto_Rico", "SA Western Standard Time"},
            {"America/Rainy_River", "Central Standard Time"},
            {"America/Rankin_Inlet", "Central Standard Time"},
            {"America/Recife", "SA Eastern Standard Time"},
            {"America/Regina", "Canada Central Standard Time"},
            {"America/Resolute", "Central Standard Time"},
            {"America/Rio_Branco", "SA Western Standard Time"},
            {"America/Santa_Isabel", "Pacific Standard Time (Mexico)"},
            {"America/Santarem", "SA Eastern Standard Time"},
            {"America/Santiago", "Pacific SA Standard Time"},
            {"America/Santo_Domingo", "SA Western Standard Time"},
            {"America/Sao_Paulo", "E. South America Standard Time"},
            {"America/Scoresbysund", "Azores Standard Time"},
            {"America/Shiprock", "Mountain Standard Time"},
            {"America/Sitka", "Alaskan Standard Time"},
            {"America/St_Barthelemy", "SA Eastern Standard Time"},
            {"America/St_Johns", "Newfoundland Standard Time"},
            {"America/St_Kitts", "SA Western Standard Time"},
            {"America/St_Lucia", "SA Western Standard Time"},
            {"America/St_Thomas", "SA Western Standard Time"},
            {"America/St_Vincent", "SA Western Standard Time"},
            {"America/Swift_Current", "Canada Central Standard Time"},
            {"America/Tegucigalpa", "Central America Standard Time"},
            {"America/Thule", "Atlantic Standard Time"},
            {"America/Thunder_Bay", "Eastern Standard Time"},
            {"America/Tijuana", "Pacific Standard Time"},
            {"America/Toronto", "Eastern Standard Time"},
            {"America/Tortola", "SA Western Standard Time"},
            {"America/Vancouver", "Pacific Standard Time"},
            {"America/Whitehorse", "Pacific Standard Time"},
            {"America/Winnipeg", "Central Standard Time"},
            {"America/Yakutat", "Alaskan Standard Time"},
            {"America/Yellowknife", "Mountain Standard Time"},
            {"Antarctica/Ashgabat", "West Asia Standard Time"},
            {"Antarctica/Davis", "SE Asia Standard Time"},
            {"Antarctica/DumontDUrville", "West Pacific Standard Time"},
            {"Antarctica/Dushanbe", "West Asia Standard Time"},
            {"Antarctica/Mawson", "West Asia Standard Time"},
            {"Antarctica/McMurdo", "New Zealand Standard Time"},
            {"Antarctica/Palmer", "Pacific SA Standard Time"},
            {"Antarctica/Rothera", "SA Eastern Standard Time"},
            {"Antarctica/Samarkand", "West Asia Standard Time"},
            {"Antarctica/South_Pole", "New Zealand Standard Time"},
            {"Antarctica/Tashkent", "West Asia Standard Time"},
            {"Antarctica/Vostok", "Central Asia Standard Time"},
            {"Arctic/Longyearbyen", "W. Europe Standard Time"},
            {"Asia/Aden", "Arab Standard Time"},
            {"Asia/Almaty", "Central Asia Standard Time"},
            {"Asia/Amman", "Jordan Standard Time"},
            {"Asia/Anadyr", "Magadan Standard Time"},
            {"Asia/Aqtau", "West Asia Standard Time"},
            {"Asia/Aqtobe", "West Asia Standard Time"},
            {"Asia/Baghdad", "Arabic Standard Time"},
            {"Asia/Bahrain", "Arab Standard Time"},
            {"Asia/Baku", "Azerbaijan Standard Time"},
            {"Asia/Bangkok", "SE Asia Standard Time"},
            {"Asia/Beirut", "Middle East Standard Time"},
            {"Asia/Bishkek", "Central Asia Standard Time"},
            {"Asia/Brunei", "Singapore Standard Time"},
            {"Asia/Calcutta", "India Standard Time"},
            {"Asia/Chongqing", "China Standard Time"},
            {"Asia/Choibalsan", "Ulaanbaatar Standard Time"},
            {"Asia/Colombo", "Sri Lanka Standard Time"},
            {"Asia/Damascus", "Syria Standard Time"},
            {"Asia/Dhaka", "Bangladesh Standard Time"},
            {"Asia/Dili", "Tokyo Standard Time"},
            {"Asia/Dubai", "Arabian Standard Time"},
            {"Asia/Gaza", "Egypt Standard Time"},
            {"Asia/Harbin", "China Standard Time"},
            {"Asia/Hebron", "Egypt Standard Time"},
            {"Asia/Hong_Kong", "China Standard Time"},
            {"Asia/Hovd", "SE Asia Standard Time"},
            {"Asia/Irkutsk", "North Asia East Standard Time"},
            {"Asia/Jakarta", "SE Asia Standard Time"},
            {"Asia/Jayapura", "Tokyo Standard Time"},
            {"Asia/Jerusalem", "Israel Standard Time"},
            {"Asia/Kabul", "Afghanistan Standard Time"},
            {"Asia/Kamchatka", "Magadan Standard Time"},
            {"Asia/Karachi", "Pakistan Standard Time"},
            {"Asia/Kashgar", "China Standard Time"},
            {"Asia/Katmandu", "Nepal Standard Time"},
            {"Asia/Kolkata", "India Standard Time"},
            {"Asia/Krasnoyarsk", "North Asia Standard Time"},
            {"Asia/Kuala_Lumpur", "Singapore Standard Time"},
            {"Asia/Kuching", "Singapore Standard Time"},
            {"Asia/Manila", "Singapore Standard Time"},
            {"Asia/Kuwait", "Arab Standard Time"},
            {"Asia/Macau", "China Standard Time"},
            {"Asia/Magadan", "Magadan Standard Time"},
            {"Asia/Makassar", "Singapore Standard Time"},
            {"Asia/Muscat", "Arabian Standard Time"},
            {"Asia/Nicosia", "E. Europe Standard Time"},
            {"Asia/Novosibirsk", "N. Central Asia Standard Time"},
            {"Asia/Novokuznetsk", "N. Central Asia Standard Time"},
            {"Asia/Omsk", "N. Central Asia Standard Time"},
            {"Asia/Oral", "West Asia Standard Time"},
            {"Asia/Phnom_Penh", "SE Asia Standard Time"},
            {"Asia/Pontianak", "SE Asia Standard Time"},
            {"Asia/Pyongyang", "Korea Standard Time"},
            {"Asia/Qatar", "Arab Standard Time"},
            {"Asia/Qyzylorda", "Central Asia StandardNovosibirsk Time"},
            {"Asia/Rangoon", "Myanmar Standard Time"},
            {"Asia/Riyadh", "Arab Standard Time"},
            {"Asia/Saigon", "SE Asia Standard Time"},
            {"Asia/Sakhalin", "Vladivostok Standard Time"},
            {"Asia/Seoul", "Korea Standard Time"},
            {"Asia/Shanghai", "China Standard Time"},
            {"Asia/Singapore", "Singapore Standard Time"},
            {"Asia/Taipei", "Taipei Standard Time"},
            {"Asia/Tashkent", "West Asia Standard Time"},
            {"Asia/Tbilisi", "Georgian Standard Time"},
            {"Asia/Tehran", "Iran Standard Time"},
            {"Asia/Thimphu", "Bangladesh Standard Time"},
            {"Asia/Tokyo", "Tokyo Standard Time"},
            {"Asia/Ulaanbaatar", "Ulaanbaatar Standard Time"},
            {"Asia/Urumqi", "China Standard Time"},
            {"Asia/Vientiane", "SE Asia Standard Time"},
            {"Asia/Vladivostok", "Vladivostok Standard Time"},
            {"Asia/Yakutsk", "Yakutsk Standard Time"},
            {"Asia/Yekaterinburg", "Ekaterinburg Standard Time"},
            {"Asia/Yerevan", "Armenian Standard Time"},
            {"Atlantic/Azores", "Azores Standard Time"},
            {"Atlantic/Cape_Verde", "Cape Verde Standard Time"},
            {"Atlantic/Reykjavik", "Greenwich Standard Time"},
            {"Atlantic/South_Georgia", "UTC-02"},
            {"Atlantic/St_Helena", "Greenwich Standard Time"},
            {"Australia/Adelaide", "Cen. Australia Standard Time"},
            {"Australia/Brisbane", "E. Australia Standard Time"},
            {"Australia/Broken_Hill", "Cen. Australia Standard Time"},
            {"Australia/Casey", "W. Australia Standard Time"},
            {"Australia/Currie", "Tasmania Standard Time"},
            {"Australia/Darwin", "AUS Central Standard Time"},
            {"Australia/Hobart", "Tasmania Standard Time"},
            {"Australia/Lindeman", "E. Australia Standard Time"},
            {"Australia/Melbourne", "AUS Eastern Standard Time"},
            {"Australia/Perth", "W. Australia Standard Time"},
            {"Australia/Sydney", "AUS Eastern Standard Time"},
            {"CST6CDT", "Central Standard Time"},
            {"EST5EDT", "Central Standard Time"},
            {"Etc/GMT-1", "W. Central Africa Standard Time"},
            {"Etc/GMT-2", "South Africa Standard Time"},
            {"Etc/GMT-3", "E. Africa Standard Time"},
            {"Etc/GMT-4", "Arabian Standard Time"},
            {"Etc/GMT-5", "West Asia Standard Time"},
            {"Etc/GMT-6", "Central Asia Standard Time"},
            {"Etc/GMT-7", "SE Asia Standard Time"},
            {"Etc/GMT-8", "Singapore Standard Time"},
            {"Etc/GMT-9", "Tokyo Standard Time"},
            {"Etc/GMT-10", "West Pacific Standard Time"},
            {"Etc/GMT-12", "UTC+12"},
            {"Etc/GMT-11", "Central Pacific Standard Time"},
            {"Etc/GMT", "UTC"},
            {"Etc/GMT+1", "Cape Verde Standard Time"},
            {"Etc/GMT+2", "UTC-02"},
            {"Etc/GMT+3", "SA Eastern Standard TimeBogota"},
            {"Etc/GMT+4", "SA Western Standard Time"},
            {"Etc/GMT+5", "SA Pacific Standard Time"},
            {"Etc/GMT+6", "Central America Standard Time"},
            {"Etc/GMT+7", "US Mountain Standard Time"},
            {"Etc/GMT+10", "Hawaiian Standard Time"},
            {"Etc/GMT+11", "UTC-11"},
            {"Etc/GMT+12", "Dateline Standard Time"},
            {"Etc/GMT-13", "Tonga Standard Time"},
            {"Europe/Amsterdam", "W. Europe Standard Time"},
            {"Europe/Oslo", "W. Europe Standard Time"},
            {"Europe/San_Marino", "W. Europe Standard Time"},
            {"Europe/Vatican", "W. Europe Standard Time"},
            {"Europe/Stockholm", "W. Europe Standard Time"},
            {"Europe/Athens", "GTB Standard Time"},
            {"Europe/Belgrade", "Central Europe Standard Time"},
            {"Europe/Berlin", "W. Europe Standard Time"},
            {"Europe/Gibraltar", "W. Europe Standard Time"},
            {"Europe/Rome", "W. Europe Standard Time"},
            {"Europe/Vaduz", "W. Europe Standard Time"},
            {"Europe/Luxembourg", "W. Europe Standard Time"},
            {"Europe/Monaco", "W. Europe Standard Time"},
            {"Europe/Malta", "W. Europe Standard Time"},
            {"Europe/Andorra", "W. Europe Standard Time"},
            {"Europe/Vienna", "W. Europe Standard Time"},
            {"Europe/Zurich", "W. Europe Standard Time"},
            {"Europe/Bratislava", "Central Europe Standard Time"},
            {"Europe/Brussels", "Romance Standard Time"},
            {"Europe/Bucharest", "GTB Standard Time"},
            {"Europe/Budapest", "Central Europe Standard Time"},
            {"Europe/Canary", "GMT Standard Time"},
            {"Europe/Ceuta", "Romance Standard Time"},
            {"Europe/Chisinau", "GTB Standard Time"},
            {"Europe/Copenhagen", "Romance Standard Time"},
            {"Europe/Dublin", "GMT Standard Time"},
            {"Europe/Faeroe", "GMT Standard Time"},
            {"Europe/Jersey", "GMT Standard Time"},
            {"Europe/Madeira", "GMT Standard Time"},
            {"Europe/Madrid", "Romance Standard Time"},
            {"Europe/Guernsey", "GMT Standard Time"},
            {"Europe/Helsinki", "FLE Standard Time"},
            {"Europe/Isle_of_Man", "GMT Standard Time"},
            {"Europe/Istanbul", "GTB Standard Time"},
            {"Europe/Kaliningrad", "Kaliningrad Standard Time"},
            {"Europe/Kiev", "FLE Standard Time"},
            {"Europe/Ljubljana", "Central Europe Standard Time"},
            {"Europe/Lisbon", "GMT Standard Time"},
            {"Europe/London", "GMT Standard Time"},
            {"Europe/Minsk", "Kaliningrad Standard Time"},
            {"Europe/Mariehamn", "FLE Standard Time"},
            {"Europe/Moscow", "Russian Standard Time"},
            {"Europe/Paris", "Romance Standard Time"},
            {"Europe/Podgorica", "Central European Standard Time"},
            {"Europe/Prague", "Central European Standard Time"},
            {"Europe/Riga", "FLE Standard Time"},
            {"Europe/Sarajevo", "Central European Standard Time"},
            {"Europe/Sofia", "FLE Standard Time"},
            {"Europe/Tallinn", "FLE Standard Time"},
            {"Europe/Tirane", "Central Europe Standard Time"},
            {"Europe/Warsaw", "Central European Standard Time"},
            {"Europe/Samara", "Russian Standard Time"},
            {"Europe/Volgograd", "Russian Standard Time"},
            {"Europe/Simferopol", "FLE Standard Time"},
            {"Europe/Skopje", "Central European Standard Time"},
            {"Europe/Vilnius", "FLE Standard Time"},
            {"Europe/Uzhgorod", "FLE Standard Time"},
            {"Europe/Zaporozhye", "FLE Standard Time"},
            {"Europe/Zagreb", "Central European Standard Time"},
            {"Indian/Chagos", "Central Asia Standard Time"},
            {"Indian/Christmas", "SE Asia Standard Time"},
            {"Indian/Cocos", "Myanmar Standard Time"},
            {"Indian/Maldives", "West Asia Standard Time"},
            {"Indian/Kerguelen", "West Asia Standard Time"},
            {"Indian/Mauritius", "Mauritius Standard Time"},
            {"Indian/Reunion", "Mauritius Standard Time"},
            {"Indian/Mahe", "Mauritius Standard Time"},
            {"Pacific/Apia", "Samoa Standard Time"},
            {"Pacific/Auckland", "New Zealand Standard Time"},
            {"Pacific/Efate", "Central Pacific Standard Time"},
            {"Pacific/Fakaofo", "Hawaiian Standard Time"},
            {"Pacific/Fiji", "Fiji Standard Time"},
            {"Pacific/Galapagos", "Central America Standard Time"},
            {"Pacific/Guadalcanal", "Central Pacific Standard Time"},
            {"Pacific/Guam", "West Pacific Standard Time"},
            {"Pacific/Honolulu", "Hawaiian Standard Time"},
            {"Pacific/Johnston", "Hawaiian Standard Time"},
            {"Pacific/Kosrae", "Central Pacific Standard Time"},
            {"Pacific/Noumea", "Central Pacific Standard Time"},
            {"Pacific/Macquarie", "Central Pacific Standard Time"},
            {"Pacific/Palau", "Tokyo Standard Time"},
            {"Pacific/Pago_Pago", "UTC-11"},
            {"Pacific/Niue", "UTC-11"},
            {"Pacific/Midway", "UTC-11"},
            {"Pacific/Ponape", "Central Pacific Standard Time"},
            {"Pacific/Port_Moresby", "West Pacific Standard Time"},
            {"Pacific/Truk", "West Pacific Standard Time"},
            {"Pacific/Saipan", "West Pacific Standard Time"},
            {"Pacific/Rarotonga", "Hawaiian Standard Time"},
            {"Pacific/Tahiti", "Hawaiian Standard Time"},
            {"Pacific/Tongatapu", "Tonga Standard Time"},
            {"Pacific/Enderbury", "Tonga Standard Time"},
            {"Pacific/Funafuti", "UTC+12"},
            {"Pacific/Kwajalein", "UTC+12"},
            {"Pacific/Majuro", "UTC+12"},
            {"Pacific/Nauru", "UTC+12"},
            {"Pacific/Tarawa", "UTC+12"},
            {"Pacific/Wake", "UTC+12"},
            {"Pacific/Wallis", "UTC+12"},
            {"PST8PDT", "Pacific Standard Time"},
            {"MST7MDT", "Mountain Standard Time"},
        };
        #endregion

        /// <summary>
        /// Converts an Olson time zone ID to a Windows time zone ID.
        /// </summary>
        /// <param name="olsonTimeZoneId">An Olson time zone ID. See http://unicode.org/repos/cldr-tmp/trunk/diff/supplemental/zone_tzid.html. </param>
        /// <returns>
        /// The TimeZoneInfo corresponding to the Olson time zone ID, 
        /// or null if you passed in an invalid Olson time zone ID.
        /// </returns>
        /// <remarks>
        /// See http://unicode.org/repos/cldr-tmp/trunk/diff/supplemental/zone_tzid.html
        /// </remarks>
        public static TimeZoneInfo OlsonTimeZoneToTimeZoneInfo(this string olsonTimeZoneId)
        {
            string windowsTimeZoneId;
            var windowsTimeZone = default(TimeZoneInfo);
            if (olsonTimeZoneId != null && OlsonWindowsTimes.TryGetValue(olsonTimeZoneId, out windowsTimeZoneId))
            {
                try
                {
                    windowsTimeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
                }
                catch (TimeZoneNotFoundException)
                {
                }
                catch (InvalidTimeZoneException)
                {
                }
            }
            return windowsTimeZone;
        }

        /// <summary>
        /// Converts an Olson time zone ID to a Windows time zone ID.
        /// </summary>
        /// <param name="timeZoneInfo">An Olson time zone ID. See http://unicode.org/repos/cldr-tmp/trunk/diff/supplemental/zone_tzid.html. </param>
        /// <returns>
        /// The TimeZoneInfo corresponding to the Olson time zone ID, 
        /// or null if you passed in an invalid Olson time zone ID.
        /// </returns>
        /// <remarks>
        /// See http://unicode.org/repos/cldr-tmp/trunk/diff/supplemental/zone_tzid.html
        /// </remarks>
        public static string TimeZoneInfoToOlsonTimeZone(this string timeZoneInfo)
        {
            timeZoneInfo = HttpUtility.UrlDecode(timeZoneInfo);

            string olsonTimeZoneId;

            if (OlsonWindowsTimes.ContainsValue(timeZoneInfo))
            {
                olsonTimeZoneId = OlsonWindowsTimes.First(t => t.Value.Equals(timeZoneInfo)).Key;
            }
            else
            {
                olsonTimeZoneId = OlsonWindowsTimes.First(t => t.Value.Equals(TimeZoneInfo.Utc.Id)).Key;
            }

            return olsonTimeZoneId;
        }

        #region To Local

        /// <summary>
        /// Convert utc time to local time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static DateTime? ToUserDateTime(this DateTime? dateTime, TimeZoneInfo timeZone)
        {
            if (dateTime.HasValue)
            {
                dateTime = DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
                return TimeZoneInfo.ConvertTimeFromUtc(dateTime.Value, timeZone);
            }
            return null;
        }

        /// <summary>
        /// Convert utc time to local time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static DateTime ToUserDateTime(this DateTime dateTime, TimeZoneInfo timeZone)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }

        /// <summary>
        /// Convert utc time to local time
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static TimeSpan? ToUserTime(this TimeSpan? time, TimeZoneInfo timeZone)
        {
            if (time.HasValue)
            {
                var dateTime = new DateTime(time.Value.Ticks);
                var localDateTime = ToUserDateTime(dateTime, timeZone);
                return localDateTime - new DateTime(0);
            }
            return null;
        }

        /// <summary>
        /// Convert utc time to local time
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static TimeSpan ToUserTime(this TimeSpan time, TimeZoneInfo timeZone)
        {
            var dateTime = new DateTime(time.Ticks);
            var localDateTime = ToUserDateTime(dateTime, timeZone);
            return localDateTime - new DateTime(0);
        }

        #endregion

        #region To UTC

        /// <summary>
        /// Convert local time to utc
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static DateTime? ToUTCTime(this DateTime? dateTime, TimeZoneInfo timeZone)
        {
            if (dateTime.HasValue)
            {
                dateTime = DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Unspecified);
                return TimeZoneInfo.ConvertTimeToUtc(dateTime.Value, timeZone);
            }
            return null;
        }

        /// <summary>
        /// Convert local time to utc
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static DateTime ToUTCTime(this DateTime dateTime, TimeZoneInfo timeZone)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
        }

        /// <summary>
        /// Convert local time to utc
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static TimeSpan? ToUTCTime(this TimeSpan? time, TimeZoneInfo timeZone)
        {
            if (time.HasValue)
            {
                var dateTime = new DateTime(time.Value.Ticks);
                var utcTime = ToUTCTime(dateTime, timeZone);
                return utcTime - new DateTime(0);
            }
            return null;
        }

        /// <summary>
        /// Convert local time to utc
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static TimeSpan ToUTCTime(this TimeSpan time, TimeZoneInfo timeZone)
        {
            var dateTime = new DateTime(time.Ticks);
            var utcTime = ToUTCTime(dateTime, timeZone);
            return utcTime - new DateTime(0);
        }

        #endregion

        /// <summary>
        /// Get time zone select list
        /// </summary>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetTimeZones(string timeZoneId = "")
        {
            return TimeZoneInfo.GetSystemTimeZones().Select(t => new SelectListItem
            {
                Text = t.DisplayName,
                Value = t.Id,
                Selected = t.Id.Equals(timeZoneId)
            });
        }
    }
}
