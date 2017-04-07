using System.Collections.Generic;

namespace EzCMS.Help.Plugins.Business.Models.WordConverter
{
    public class HtmlToWordRequest
    {
        public string Html { get; set; }
        public Dictionary<string, byte[]> Files { get; set; }
    }
}