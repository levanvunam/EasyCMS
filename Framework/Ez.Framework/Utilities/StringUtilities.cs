using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using static System.String;

namespace Ez.Framework.Utilities
{
    /// <summary>
    /// String utilities
    /// </summary>
    public static class StringUtilities
    {
        #region Web

        public static string RemoveTags(this string html)
        {
            if (IsNullOrEmpty(html))
            {
                return Empty;
            }

            var result = new char[html.Length];

            var cursor = 0;
            var inside = false;
            foreach (var current in html)
            {
                switch (current)
                {
                    case '<':
                        inside = true;
                        continue;
                    case '>':
                        inside = false;
                        continue;
                }

                if (!inside)
                {
                    result[cursor++] = current;
                }
            }

            return new string(result, 0, cursor);
        }

        public static string StripHtml(this string inputString)
        {
            if (IsNullOrEmpty(inputString)) return Empty;
            return Regex.Replace(inputString, @"<(.|\n)*?>", Empty);
        }

        public static Dictionary<string, string> ToDictionary(this string s)
        {
            var dictionary = new Dictionary<string, string>();
            var keyValuePairs = s.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in keyValuePairs)
            {
                var values = pair.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length != 2)
                    continue;

                var key = values[0];
                var value = values[1];

                dictionary[key.ToLower()] = value;
            }
            return dictionary;
        }

        public static RouteValueDictionary ToRouteValueDictionary(this string s)
        {
            var dictionary = s.ToDictionary();

            var routeDictionary = new RouteValueDictionary();

            foreach (var routeValue in dictionary)
            {
                var key = routeValue.Key;
                routeDictionary.Add(key.EndsWith("-")
                                    ? key.Substring(0, key.Length - 1)
                                    : key, routeValue.Value);
            }

            return routeDictionary;
        }

        #endregion

        #region Common

        public static string Pluralize(this string name, bool toLowercase = false)
        {
            if (IsNullOrEmpty(name)) return Empty;
            var pluralizationService = PluralizationService.CreateService(
                CultureInfo.CurrentCulture);
            return toLowercase ? pluralizationService.Pluralize(name).ToLower() : pluralizationService.Pluralize(name);
        }

        public static string Singularize(this string name, bool toLowercase = false)
        {
            if (IsNullOrEmpty(name)) return Empty;
            var pluralizationService = PluralizationService.CreateService(
                CultureInfo.CurrentCulture);
            return toLowercase ? pluralizationService.Singularize(name).ToLower() : pluralizationService.Singularize(name);
        }

        public static string ToLowercaseNamingConvention(this string s, bool toLowercase = false)
        {
            var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            if (toLowercase)
            {
                return r.Replace(s, " ").ToLower();
            }
            return r.Replace(s, " ");
        }

        public static string CamelFriendly(this string camel)
        {
            if (IsNullOrWhiteSpace(camel))
                return "";

            var sb = new StringBuilder(camel);

            for (var i = camel.Length - 1; i > 0; i--)
            {
                var current = sb[i];
                if ('A' <= current && current <= 'Z')
                {
                    sb.Insert(i, ' ');
                }
            }

            return sb.ToString();
        }

        public static string FormatWith(this string s, params object[] parms)
        {
            return Format(s, parms);
        }

        public static string GetBasePathFromCurrentPath(string folderNameToGet, string currentPath, string baseFolderName)
        {
            if (IsNullOrEmpty(currentPath))
            {
                return null;
            }
            string basePath = null;
            var viewsPartIndex = currentPath.LastIndexOf(@"\" + baseFolderName, StringComparison.OrdinalIgnoreCase);
            if (viewsPartIndex >= 0)
            {
                basePath = currentPath.Substring(0, viewsPartIndex + 1) + folderNameToGet;
            }

            return basePath;
        }

        public static string SafeSubstring(this string input, int length)
        {
            if (length > input.Length)
            {
                return input;
            }

            var endPosition = input.IndexOf(" ", length, StringComparison.Ordinal);
            if (endPosition < 0) endPosition = input.Length;

            return length >= input.Length ? input : input.Substring(0, endPosition) + "...";
        }

        public static string RemoveDiacritics(this string name)
        {
            var stFormD = name.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var t in from t in stFormD
                              let uc = CharUnicodeInfo.GetUnicodeCategory(t)
                              where uc != UnicodeCategory.NonSpacingMark
                              select t)
            {
                sb.Append(t);
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        public static string Slugify(this string input)
        {
            var disallowed = new Regex(@"[/:?#\[\]@!$&'()*+,.;=\s\""\<\>\\\|%]+");

            var cleanedSlug = disallowed.Replace(input, "-").Trim('-', '.');

            var slug = Regex.Replace(cleanedSlug, @"\-{2,}", "-");

            if (slug.Length > 1000)
                slug = slug.Substring(0, 1000).Trim('-', '.');

            slug = slug.RemoveDiacritics();
            return slug;
        }

        public static string RemoveDoubleSpace(this string input)
        {
            while (input.Contains("  "))
            {
                input = input.Replace("  ", " ");
            }
            return input;
        }

        public static string RemoveString(this string input, string remove)
        {
            if (IsNullOrEmpty(remove)) return input;

            while (input.Contains(remove))
            {
                input = input.Replace(remove, Empty);
            }
            return input;
        }

        /// <summary>
        /// Convert a dictionary to aggregated string format like key=value;key2=value2;...;keyn=valuen
        /// </summary>
        /// <param name="dictionary">The input dictionary</param>
        /// <returns>Aggregated string in format key=value;key2=value2;...;keyn=valuen</returns>
        public static string ToAggregatedString(this Dictionary<string, string> dictionary)
        {
            var str = dictionary.Keys.Aggregate("", (current, dictionaryKey) => current + (dictionaryKey + "=" + dictionary[dictionaryKey] + ";"));
            return str;
        }

        public static string ReplaceOnce(this string template, string placeholder, string replacement)
        {
            var loc = template.IndexOf(placeholder, StringComparison.Ordinal);
            if (loc < 0)
            {
                return template;
            }
            return new StringBuilder(template.Substring(0, loc))
                .Append(replacement)
                .Append(template.Substring(loc + placeholder.Length))
                .ToString();
        }

        public static string LastPart(this string input, char separator)
        {
            var pos = input.LastIndexOf(separator);
            if (pos < 0)
            {
                return input;
            }
            if (pos == input.Length - 1)
            {
                return "";
            }
            return input.Substring(pos + 1);
        }


        #endregion

        #region Encode / Decode

        public static byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        /// <summary>
        /// Decode string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decode(this string input)
        {
            return HttpUtility.HtmlDecode(input);
        }

        #endregion

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        #region File & Folder

        /// <summary>
        /// Generate unique avatar file name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="extension"> </param>
        /// <returns></returns>
        public static string GenerateAvatarFileName(this int id, string extension)
        {
            return Format("{0}_{1}.{2}", id, DateTime.UtcNow.ToString("yyyyMMddhhmmss"), extension);
        }

        /// <summary>
        /// Convert path to string
        /// This must match with jquery script
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ToIdString(this string path)
        {
            if (IsNullOrEmpty(path)) return Empty;

            var invalidUrlCharacter = new Regex(@"[^a-z|^_|^\d|^\u4e00-\u9fa5|^/]+",
                                                                          RegexOptions.Compiled | RegexOptions.Singleline |
                                                                          RegexOptions.IgnoreCase);

            return invalidUrlCharacter.Replace(path.Replace("/", "").Replace("_", "").Replace("\\", ""), "");
        }

        /// <summary>
        /// Convert path to string
        /// </summary>
        /// <param name="path"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string ToIdStringByHash(this string path, string prefix = "")
        {
            if (IsNullOrEmpty(path)) return Empty;

            var hashCode = path.GetHashCode() > 0 ? path.GetHashCode() : path.GetHashCode() * -1;
            if (IsNullOrEmpty(prefix))
            {
                return hashCode.ToString(CultureInfo.InvariantCulture);
            }
            return Format("{0}_{1}", prefix, hashCode);
        }

        #endregion

        #region Get Match String

        /// <summary>
        /// Get matching string from list of search items
        /// Generate the final string base on input string with higher match on top
        /// </summary>
        /// <param name="input"></param>
        /// <param name="searchItems"></param>
        /// <returns></returns>
        public static string GetMatchingContent(this string input, List<string> searchItems)
        {
            if (IsNullOrEmpty(input))
            {
                return Empty;
            }

            var contents = new List<MatchingString>();
            foreach (var s in searchItems)
            {
                if (IsNullOrWhiteSpace(s)) continue;
                var position = -1;
                do
                {
                    position = input.IndexOf(s, position + 1, StringComparison.Ordinal);
                    if (position < 0) break;
                    var startString = input.Substring(0, position);
                    var startStringPosition = startString.LastIndexOf(".", StringComparison.Ordinal);
                    if (startStringPosition < 0) startStringPosition = 0;
                    var endStringPosition = input.IndexOf(".", position, StringComparison.Ordinal);
                    if (endStringPosition < 0)
                    {
                        endStringPosition = input.Length - 1;
                    }
                    var stringResult = input.Substring(startStringPosition, endStringPosition - startStringPosition).Trim();
                    var content = contents.SingleOrDefault(c => c.Content.Equals(stringResult));
                    if (content != null)
                    {
                        content.Value = content.Value + 1;
                    }
                    else
                    {
                        content = new MatchingString
                        {
                            Content = stringResult,
                            Value = 1
                        };
                        contents.Add(content);
                    }
                } while (position > 0);
            }
            contents = contents.Distinct().OrderByDescending(c => c.Value).ToList();
            if (contents.Count == 0)
            {
                return input.SafeSubstring(300).Trim();
            }
            if (contents.Count > 3)
            {
                contents = contents.Take(3).ToList();
            }
            return Join("...", contents.Select(c => c.Content)) + "...";
        }
        #endregion

        public static void GenerateName(this string fullName, out string firstName, out string lastName)
        {
            firstName = Empty;
            lastName = Empty;
            if (IsNullOrEmpty(fullName))
                return;
            var nameParts = fullName.Split(' ');
            // Full name more than 1 part
            if (nameParts.Count() >= 2)
            {
                lastName = nameParts.Last();
                firstName = Join(" ", nameParts.Take(nameParts.Count() - 1));
            }
            // Only 1 part
            else
            {
                firstName = fullName;
            }
        }

        public static string GenerateFullName(string firstName, string lastName)
        {
            if (IsNullOrEmpty(firstName) && IsNullOrEmpty(lastName))
            {
                return Empty;
            }

            if (IsNullOrEmpty(firstName))
            {
                return lastName;
            }

            return Format("{0} {1}", firstName, lastName);
        }

        /// <summary>
        /// Generate unique string
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueString()
        {
            var now = DateTime.UtcNow.ToString("U");
            return PasswordUtilities.Base64Encode(now);
        }

        #region Format

        public static string[] ParseExact(this string data, string format)
        {
            return ParseExact(data, format, false);
        }

        public static string[] ParseExact(this string data, string format, bool ignoreCase)
        {
            string[] values;

            if (TryParseExact(data, format, out values, ignoreCase))
                return values;
            throw new ArgumentException("Format not compatible with value.");
        }

        public static bool TryExtract(this string data, string format, out string[] values)
        {
            return TryParseExact(data, format, out values, false);
        }

        public static bool TryParseExact(this string data, string format, out string[] values, bool ignoreCase)
        {
            int tokenCount;
            format = Regex.Escape(format).Replace("\\{", "{");

            for (tokenCount = 0; ; tokenCount++)
            {
                string token = Format("{{{0}}}", tokenCount);
                if (!format.Contains(token)) break;
                format = format.Replace(token,
                    Format("(?'group{0}'.*)", tokenCount));
            }

            RegexOptions options =
                ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;

            Match match = new Regex(format, options).Match(data);

            if (tokenCount != (match.Groups.Count - 1))
            {
                values = new string[] { };
                return false;
            }

            values = new string[tokenCount];
            for (int index = 0; index < tokenCount; index++)
                values[index] = match.Groups[Format("group{0}", index)].Value;

            return true;
        }
        #endregion
    }

    public class StringLengthComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var result = -(x.Length.CompareTo(y.Length));
            if (result == 0)
            {
                result = String.Compare(x, y, StringComparison.Ordinal);
            }
            return result;
        }
    }

    public class MatchingString
    {
        public int Value { get; set; }

        public string Content { get; set; }
    }
}
