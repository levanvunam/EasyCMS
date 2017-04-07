using System;

namespace EzCMS.Core.Models.ClientNavigations.Widgets
{
    public class CacheNavigation
    {
        public DateTime TemplateCacheTime { get; set; }

        public DateTime DataCacheTime { get; set; }

        public string Content { get; set; }
    }
}