using System.Collections.Generic;

namespace EzCMS.Core.Models.WordConverters
{
    public class HtmlResult
    {
        public string Html { get; set; }

        public Dictionary<string, byte[]> Files { get; set; }
    }
}