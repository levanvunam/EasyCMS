using Ez.Framework.Configurations;
using System.Collections.Generic;
using System.Linq;

namespace Ez.Framework.Core.Mvc.Helpers
{
    public static class DictionaryHelper
    {
        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> first,
            IDictionary<TKey, TValue> second)
        {
            if (second == null)
                return;

            if (first == null)
                first = new Dictionary<TKey, TValue>();

            foreach (var item in second.Where(item => !first.ContainsKey(item.Key)))
            {
                first.Add(item.Key, item.Value);
            }
        }

        public static string BuildKey(string language, string textKey)
        {
            return string.Format("{0}{1}{2}", language, FrameworkConstants.LocalizeResourceSeperator, textKey);
        }
    }
}
