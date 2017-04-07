using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ez.Framework.Core.JqGrid
{
    public class WhereClauseGenerator
    {

        private List<object> _formatObjects;

        public WhereClause Generate(bool search, string filters, Type targetSearchType)
        {
            _formatObjects = new List<object>();

            return new WhereClause
            {
                Clause = search && !String.IsNullOrEmpty(filters)
                    ? ParseFilter(SerializeUtilities.Deserialize<JqGridFilter>(filters), targetSearchType).ToString()
                    : String.Empty,
                FormatObjects = _formatObjects.ToArray()
            };
        }

        private readonly string[] _formatMapping = {
            // 0 = field name,
            // 1 = total no of formats/params so far - 1
            // F = replaced by fmAdd
            "({0} = @{1}{F})",               // "eq" - equal
            "({0} <> @{1}{F})",              // "ne" - not equal
            "({0} < @{1}{F})",               // "lt" - less than
            "({0} <= @{1}{F})",              // "le" - less than or equal to
            "({0} > @{1}{F})",               // "gt" - greater than
            "({0} >= @{1}{F})",              // "ge" - greater than or equal to
            "({0}.StartsWith(@{1}){F})",     // "bw" - begins with
            "(!{0}.StartsWith(@{1}){F})",    // "bn" - does not begin with
            "({0}.EndsWith(@{1}){F})",       // "ew" - ends with
            "(!{0}.EndsWith(@{1}){F})",      // "en" - does not end with
            //"({0}.IndexOf(@{1}, IgnoreCase) >= 0{F})",       // "cn" - contains
            //"(!{0}.IndexOf(@{1}, IgnoreCase) < 0{F})"       // "nc" - does not contain
            "({0}.ToLower().Contains(@{1}.ToLower()){F})",       // "cn" - contains
            "(!{0}.ToLower().Contains(@{1}.ToLower()){F})"       // "nc" - does not contain
            //"({0}.Contains(@{1}){F})",       // "cn" - contains
            //"(!{0}.Contains(@{1}){F})"       // "nc" - does not contain
        };

        private readonly string[] _nullValueFormatMapping = {
            // 0 = field name
            "({0} = NULL)",     // "eq" - equal
            "({0} != NULL)",    // "ne" - not equal
            "(1=0)",            // "lt" - less than
            "(1=1)",            // "le" - less than or equal to
            "({0} != NULL)",    // "gt" - greater than
            "(1=1)",            // "ge" - greater than or equal to
            "({0} != NULL)",    // "bw" - begins with
            "({0} != NULL)",    // "bn" - does not begin with
            "({0} != NULL)",    // "ew" - ends with
            "({0} != NULL)",    // "en" - does not end with
            "({0} != NULL)",    // "cn" - contains
            "({0} != NULL)"     // "nc" - does not contain
        };

        private StringBuilder ParseRule(ICollection<JqGridRule> rules, GroupOp groupOp, Type targetSearchType)
        {
            if (rules == null || rules.Count == 0)
                return null;

            var sb = new StringBuilder();
            bool firstRule = true;
            var props = targetSearchType.GetProperties().ToDictionary(p => p.Name, p => p.PropertyType);

            foreach (var rule in rules)
            {
                if (!firstRule)
                    // skip groupOp before the first rule
                    sb.Append(groupOp);
                else
                    firstRule = false;

                // get the object type of the rule
                Type ruleParseType;
                bool emptyNullable = false;
                try
                {
                    Type ruleType = ruleParseType = props[rule.Field];

                    if (ruleType.IsGenericType && ruleType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        if (rule.Data == "")
                            emptyNullable = true;
                        ruleParseType = Nullable.GetUnderlyingType(ruleType);
                    }

                }
                catch (KeyNotFoundException)
                {
                    throw new ArgumentOutOfRangeException(rule.Field + " is not a property of type "
                                                          + targetSearchType);
                }


                // parse it in as the correct object type
                var fmAdd = "";
                if (ruleParseType == typeof(string))
                {
                    _formatObjects.Add(rule.Data);
                    if (rule.Data == "")
                        fmAdd = " OR {0} = NULL";
                }
                else if (ruleParseType.IsEnum)
                {
                    if (!string.IsNullOrEmpty(rule.Data))
                    {
                        _formatObjects.Add(Enum.Parse(ruleParseType, rule.Data));
                    }
                }
                else
                {
                    if (emptyNullable)
                        _formatObjects.Add(null);
                    else
                    {
                        var parseMethod = ruleParseType.GetMethod("Parse", new[] { typeof(string) });
                        if (parseMethod != null)
                            _formatObjects.Add(parseMethod.Invoke(props[rule.Field], new object[] { rule.Data }));
                        else
                            throw new ArgumentOutOfRangeException(rule.Field +
                                                                  " is not a string and cannot be parsed either!!");
                    }
                }

                string fm = emptyNullable ? _nullValueFormatMapping[(int)rule.Op]
                                : _formatMapping[(int)rule.Op].Replace("{F}", fmAdd);

                sb.AppendFormat(fm, rule.Field, _formatObjects.Count - 1);

            }
            return sb.Length > 0 ? sb : null;
        }

        private void AppendWithBrackets(StringBuilder dest, StringBuilder src)
        {
            if (src == null || src.Length == 0)
                return;

            if (src.Length > 2 && src[0] != '(' && src[src.Length - 1] != ')')
            {
                dest.Append('(');
                dest.Append(src);
                dest.Append(')');
            }
            else
            {
                // verify that no other '(' and ')' exist in the b. so that
                // we have no case like src = "(x < 0) OR (y > 0)"
                for (int i = 1; i < src.Length - 1; i++)
                {
                    if (src[i] != '(' && src[i] != ')') continue;
                    dest.Append('(');
                    dest.Append(src);
                    dest.Append(')');
                    return;
                }
                dest.Append(src);
            }
        }

        private StringBuilder ParseFilter(ICollection<JqGridFilter> groups, GroupOp groupOp, Type targetSearchType)
        {
            if (groups == null || groups.Count == 0)
                return null;

            var sb = new StringBuilder();
            bool firstGroup = true;
            foreach (var group in groups)
            {
                var sbGroup = ParseFilter(group, targetSearchType);
                if (sbGroup == null || sbGroup.Length == 0)
                    continue;

                if (!firstGroup)
                    // skip groupOp before the first group
                    sb.Append(groupOp);
                else
                    firstGroup = false;

                sb.EnsureCapacity(sb.Length + sbGroup.Length + 2);
                AppendWithBrackets(sb, sbGroup);
            }
            return sb;
        }


        private StringBuilder ParseFilter(JqGridFilter filters, Type targetSearchType)
        {

            var parsedRules = ParseRule(filters.Rules, filters.GroupOp, targetSearchType);
            var parsedGroups = ParseFilter(filters.Groups, filters.GroupOp, targetSearchType);

            if (parsedRules != null && parsedRules.Length > 0)
            {
                if (parsedGroups != null && parsedGroups.Length > 0)
                {
                    var groupOpStr = filters.GroupOp.ToString();
                    var sb = new StringBuilder(parsedRules.Length + parsedGroups.Length + groupOpStr.Length + 4);
                    AppendWithBrackets(sb, parsedRules);
                    sb.Append(groupOpStr);
                    AppendWithBrackets(sb, parsedGroups);
                    return sb;
                }
                return parsedRules;
            }
            return parsedGroups ?? new StringBuilder("1 = 1");
        }
    }
}